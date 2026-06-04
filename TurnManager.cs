using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

public class TurnManager {
    public enum TurnState {
		PlayerTurn,
        EnemyTurn,
        Transition
    }

	public enum PlayerTurnState{
     	Selection,
		Moving,
		Aiming,
	}

    public TurnState currentState = TurnState.PlayerTurn;
	public PlayerTurnState playerState = PlayerTurnState.Selection;

    private int _playerAP, enemyAP = 100;

    public int playerAP {
        get => _playerAP;
        set { if (value > 0) _playerAP = value; else _playerAP = 0; }
    }
    
    public float movementAmplitude = 0;

    private List<Enemy> enemyQueue = new List<Enemy>();
    private List<Player> playerQueue = new List<Player>();

    private Enemy? currentActiveEnemy = null;
    public Player? currentActivePlayer = null;

    public void Reset() {
        currentState = TurnState.PlayerTurn;
        playerState = PlayerTurnState.Selection;
        playerAP = 100;
        enemyAP = 100;
        movementAmplitude = 0;
        enemyQueue.Clear();
        playerQueue.Clear();
        currentActiveEnemy = null;
        currentActivePlayer = null;
    }

    public void Update() {
        if (Game.activeState != Game.State.Playing || !Game.isTurnBased)
            return;

        switch (currentState) {
		   	case TurnState.PlayerTurn:
				UpdatePlayerTurn();
				break;

            case TurnState.EnemyTurn:
                UpdateEnemyTurn();
                break;
        }
    }

    public void EndPlayersTurn() {
        currentState = TurnState.EnemyTurn;
        enemyQueue = Game.env.All.Entities.NPCs.OfType<Enemy>()
                         .Where(n => !n.isDead)
                         .ToList();
        NextEnemy();
    }

    public void EndEnemiesTurn() {
        currentState = TurnState.PlayerTurn;
        playerQueue = Game.env.players.Where(n => !n.isDead).ToList();
        NextPlayer();
    }

    private void NextEnemy() {
        if (enemyQueue.Count == 0) {
			EndEnemiesTurn();
            return;
        }

        currentActiveEnemy = enemyQueue[0];
        enemyQueue.RemoveAt(0);
        enemyAP = 100;

        // Only follow if relatively close to a player
        Entity? target = currentActiveEnemy.GetNearestPlayer();

        if (target != null &&
            Tools.GetDistanceSquared(currentActiveEnemy.center, target.center) <
                1000000) {
            Game.camera.Follow(currentActiveEnemy);
        }
    }

    public void NextPlayer() {
        if (playerQueue.Count == 0) {
			EndPlayersTurn();
            return;
        }

        currentActivePlayer = playerQueue[0];
        playerQueue.RemoveAt(0);
        playerAP = 100;
		playerState = PlayerTurnState.Selection; 
        movementAmplitude = 0;
		Game.camera.Follow(currentActivePlayer); 

    }

    public bool IsActiveEnemy(Enemy e) {
        return currentState == TurnState.EnemyTurn &&
               ReferenceEquals(currentActiveEnemy, e);
    }

	private void UpdatePlayerTurn(){
        if (currentActivePlayer == null || currentActivePlayer.isDead) {
            NextPlayer();
            return;
        }
		
		switch(playerState){
            case PlayerTurnState.Selection:
				break;
            case PlayerTurnState.Aiming:
                break;
            case PlayerTurnState.Moving:
                if(Game.env.players.Count > 0){
                    Player p = currentActivePlayer;

                    if (Math.Abs(p.speed) < 0.2f && !p.isAttacking &&
                        Math.Abs(p.velRepulsion.X) < 0.1f &&
                        Math.Abs(p.velRepulsion.Y) < 0.1f) {
                        if (playerAP <= 0)
                            NextPlayer();
                        else
                            playerState = PlayerTurnState.Selection;
                    }
                }
				break;
		}
	}

    private void UpdateEnemyTurn() {
        if (currentActiveEnemy == null || currentActiveEnemy.isDead) {
            NextEnemy();
            return;
        }

        Entity? target = currentActiveEnemy.GetNearestPlayer();

        if(target == null){
            enemyAP = 0;
            NextEnemy();
            return;
        }

        // Use distance to nearest player for instant turn decision
        if (Tools.GetDistanceSquared(currentActiveEnemy.center, target.center) >
            1440000) {

            // Process entire turn instantly
            while (enemyAP > 0 && !currentActiveEnemy.isDead) {
                PerformEnemyAction();
                currentActiveEnemy.UpdateRoutine();
                if (enemyAP < 20)
                    break;
            }

            enemyAP = 0;
            NextEnemy();

            return;
        }

        // Chained movement logic for nearby enemies
        if (!currentActiveEnemy.isActionInProgress) {
            // If they have AP and have slowed down, trigger the next move
            // immediately
            if (enemyAP >= 20 && Math.Abs(currentActiveEnemy.speed) < 12.0f) {
                PerformEnemyAction();
            }

            // Only end turn when they are out of AP AND have physically stopped
            else if (enemyAP < 20 &&
                     Math.Abs(currentActiveEnemy.speed) < 0.5f) {
                NextEnemy();
            }
        }
    }

    private void PerformEnemyAction() {
        if (currentActiveEnemy == null)
            return;

        currentActiveEnemy.UpdateAI();
        Entity? target = currentActiveEnemy.GetNearestPlayer();

        if (target == null)
            return;

        float dist =
            Tools.GetDistance(currentActiveEnemy.center, target.center);

        if (dist < 400 && enemyAP >= 30) {
            currentActiveEnemy.FaceTarget();

            currentActiveEnemy.ShootAction();

            enemyAP -= 30;

        }

        else if (enemyAP >= 20) {
            // Instead of snapping rotation, we let Enemy.Update lerp to
            // aiming_rotation

            currentActiveEnemy.speed = 18;

            enemyAP -= 20;

        }

        else {
            enemyAP = 0;
        }
    }

    public void Draw(Graphics g) {
		if(currentState != TurnState.PlayerTurn) return;

        if(Game.activeState != Game.State.Playing || !Game.isTurnBased) return;
		if(playerState == PlayerTurnState.Selection)
		   DrawMovementArrow(g);
		else if (playerState == PlayerTurnState.Aiming)
			DrawAimingText(g);

        DrawAPBar(g);
    }

    private void DrawAimingText(Graphics g) {
        PointF screenPos =
            Game.camera.WorldToScreen(currentActivePlayer.center);

        g.DrawString("AIMING MODE - [Q/D] CHANGE TARGET - [ENTER] FIRE",
                     SystemFonts.DefaultFont, Brushes.Orange, screenPos.X - 120,
                     screenPos.Y - 60);
    }

    private void DrawMovementArrow(Graphics g) {
		if(currentActivePlayer==null) return;

        Player p = currentActivePlayer;
        PointF pPos = p.center;
        PointF screenPos = Game.camera.WorldToScreen(pPos);
        float rad = p.rotation * (MathF.PI / 180f);
        float length = movementAmplitude * 5;
        using (Pen pen = new Pen(Color.Lime, 4)) {
            pen.CustomEndCap = new AdjustableArrowCap(5, 5);

            g.DrawLine(pen, screenPos.X, screenPos.Y,
                       screenPos.X + MathF.Cos(rad) * length,
                       screenPos.Y + MathF.Sin(rad) * length);
        }

        int cost = (int)(movementAmplitude * 2);

        g.DrawString($"COST: {cost} AP", SystemFonts.DefaultFont, Brushes.Lime,
                     screenPos.X + 20, screenPos.Y - 40);
    }

    private void DrawAPBar(Graphics g) {
        int barW = 200;
        int barH = 20;
        int x = (Game.windowWidth - barW) / 2;
        int y = Game.windowHeight - 50;
        g.FillRectangle(Brushes.Black, x, y, barW, barH);
        g.FillRectangle(Brushes.DeepSkyBlue, x, y, (barW * playerAP) / 100,
                        barH);

        g.DrawRectangle(Pens.White, x, y, barW, barH);
        g.DrawString($"AP: {playerAP} / 100", SystemFonts.DefaultFont,
                     Brushes.White, x + barW + 10, y);
    }
}
