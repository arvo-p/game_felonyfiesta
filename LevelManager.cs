using System.Numerics;
using Raylib_cs;
public class LevelManager{
	Environment env;
	internal List<Enemy> managedEnemies = new List<Enemy>();
	
	internal int waveMaxEnemies = 30;
	internal int waveRemaining = 30; //quantity of enemies to generate before the level finishes
	internal int waveSimultaneousMax = 5; //maximum quantity of enemies at the same time
	
	internal bool isLevelFinished = false;
	DateTime? dtRest;

	public LevelManager(){
		env = Game.env;
	}
	
	public bool isDialoguePlaying = false;
	public Dialogue dia;
	internal int dialogue_pointer = 0;
    private void NextDialogue(){
		int len = DialogueFootprints.Dialogues.Count;
		if(dialogue_pointer > len - 1) return; // no more dialogues
		dia = DialogueFootprints.Dialogues[(dialogue_pointer++)];
		isDialoguePlaying = true;
		dia.Play();
	}

	private void NextLevel(){
		waveMaxEnemies += 10;
		waveSimultaneousMax += 5;
		waveRemaining = waveMaxEnemies;
		isLevelFinished = false;
	}

    bool isGameStarting = true;
	public void Update(){
		if(isGameStarting){
			NextDialogue();
			isGameStarting = false;
		}
 		if(isDialoguePlaying && dia.isPlaying == false) isDialoguePlaying = false;
		if(isLevelFinished == true){
			if(!dtRest.HasValue) dtRest = DateTime.Now;
			else if((DateTime.Now - dtRest.Value).TotalSeconds >= 10){
				if(dia.isPlaying == false && isDialoguePlaying)
					NextLevel();              
				else if(!isDialoguePlaying) NextDialogue();
			}
			return;
		}
		
		foreach(var e in managedEnemies.ToList())
			if(e.isDead == true) managedEnemies.Remove(e);

		
		int countGenerateEnemies = waveSimultaneousMax - managedEnemies.Count();
		if(countGenerateEnemies > 0){
			if(countGenerateEnemies > waveRemaining){
				countGenerateEnemies = waveRemaining;
				waveRemaining = 0;
			}else waveRemaining -= countGenerateEnemies;
			
			for(int i=0;i<countGenerateEnemies;i++){
				Enemy e = RandomEnemy();
				//Enemy e = new Thug(SpawnCoordinates(env.p.new Vector2(r.X, r.Y),300));
				
				managedEnemies.Add(e);
				env.All.Add(e);
			}
		}

		isLevelFinished = (managedEnemies.Count() == 0);
	}
	
	private Enemy RandomEnemy(){
		Random r = new Random();
		int choice = r.Next(0, 3);
		if(env.players.Count == 0) return new Thug();

		Vector2 playerLoc = new Vector2(env.players[0].r.X, env.players[0].r.Y);
		if(choice == 0) return new Thug(SpawnCoordinates(playerLoc,300));
		if(choice == 1) return new Merc(SpawnCoordinates(playerLoc,300));
		if(choice == 2) return new Doctor(SpawnCoordinates(playerLoc,300));
		return new Thug();
	}
	
	public Vector2 SpawnCoordinates(Vector2 center, int spawnRadius){
		Random rand = new Random();
		int mapWidth = (int)env.map.worldsize.X;
		int mapHeight = (int)env.map.worldsize.Y;
		int tileSize = env.map.tileRenderDimension;
		int entitySize = 64; 
		int maxAttempts = 30;
		
		for(int i = 0; i < maxAttempts; i++){
			Vector2 quadrant = new Vector2(rand.Next(0, 2) == 0? -1:1, rand.Next(0, 2) == 0? -1:1);
			int randomX = (int)(center.X + quadrant.X*(Game.windowWidth/2+spawnRadius) + rand.Next(-spawnRadius, spawnRadius + 1)); 
			int randomY = (int)(center.Y + quadrant.Y*(Game.windowHeight/2+spawnRadius) + rand.Next(-spawnRadius, spawnRadius + 1)); 
			
			// Ensure the entire entity footprint is within world bounds
			if(randomX >= 0 && randomX + entitySize < mapWidth && randomY >= 0 && randomY + entitySize < mapHeight){
				int[] checkX = {randomX, randomX + entitySize - 1};
				int[] checkY = {randomY, randomY + entitySize - 1};

				bool isClear = true;
				foreach(var x in checkX){
					foreach(var y in checkY){
						int tx = x / tileSize;
						int ty = y / tileSize;
						
						// Safety check against the collision array bounds
						if(tx >= 0 && tx < env.map.collision.GetLength(0) && ty >= 0 && ty < env.map.collision.GetLength(1)){
							if(env.map.collision[tx, ty] != -1) isClear = false;
						}else{
							isClear = false;
						}
					}
				}

				if(isClear) return new Vector2(randomX, randomY);
			}
		}
		
		return new Vector2(0, 0);
	}
}





