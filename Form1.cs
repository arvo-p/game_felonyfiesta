using System.Drawing;
using System.Media;
using System.Drawing.Drawing2D;

namespace game;

public partial class Form1 : Form{
	private const short windowWidth = 1024+256;
	private const short windowHeight = 512+256;

	private List<Control> menuControls = new List<Control>();
	private List<Control> pauseControls = new List<Control>();
	private List<Control> gameOverControls = new List<Control>();
	private List<Control> playerSelectControls = new List<Control>();
	private Image? imgParallaxBack;
	private Image? imgParallaxFore;
	private Color themeColor = Color.FromArgb(180, 80, 255); // Electric Violet
	private float animationTimer = 0;
    
	public SoundPlayer maintheme = new SoundPlayer(@"Resources/"+Resources.Sounds._main_theme);
    public Form1(){
		Resources.Font.Load();
		InitializeUI();
		LoadParallaxImages();
		CreateMenuUI();
		Debug();
		Game.Init(this, windowWidth, windowHeight);
		maintheme.Play();
	}

	private void LoadParallaxImages(){
		try {
			imgParallaxBack = Sprite.GetImage(Resources.UI.Parallax.Background);
			imgParallaxFore = Sprite.GetImage(Resources.UI.Parallax.Foreground);
		} catch (Exception ex) {
			Console.WriteLine("Parallax load error: " + ex.Message);
		}
	}

	private void CreateMenuUI(){
		int menuW = 800;
		int menuH = 700;
		int menuX = (int)((windowWidth - menuW) / 2);
		int menuY = (int)((windowHeight - menuH) / 2);

		Button btnNewGame = CreateStyledButton("SINGLE-PLAYER", menuX + 250, menuY + 230);
		btnNewGame.Click += (s, e) => StartGame(false);
		this.Controls.Add(btnNewGame);
		menuControls.Add(btnNewGame);

		Button btnNewGameTurn = CreateStyledButton("TURN-BASED", menuX + 250, menuY + 310);
		btnNewGameTurn.Click += (s, e) => StartGame(true);
		this.Controls.Add(btnNewGameTurn);
		menuControls.Add(btnNewGameTurn);

		Button btnLoad = CreateStyledButton("LOAD", menuX + 250, menuY + 390);
		btnLoad.Click += (s, e) => LoadGame();
		this.Controls.Add(btnLoad);
		menuControls.Add(btnLoad);

		Button btnExit = CreateStyledButton("EXIT", menuX + 250, menuY + 470);
		btnExit.Click += (s, e) => Application.Exit();
		this.Controls.Add(btnExit);
		menuControls.Add(btnExit);

		foreach(var ctrl in menuControls) ctrl.BringToFront();

		CreatePauseUI();
		CreatePlayerSelectUI();
		CreateGameOverUI();
	}

	private void CreateGameOverUI(){
		int menuW = 600;
		int menuH = 500;
		int menuX = (windowWidth - menuW) / 2;
		int menuY = (windowHeight - menuH) / 2;

		Label lblGameOver = new Label();
		lblGameOver.Text = "SQUAD WIPED";
		if (Resources.Font._pfc.Families.Length > 0)
			lblGameOver.Font = new Font(Resources.Font._pfc.Families[0], 42, FontStyle.Bold);
		else
			lblGameOver.Font = new Font("Arial", 42, FontStyle.Bold);

		lblGameOver.ForeColor = Color.Red;
		lblGameOver.BackColor = Color.Transparent;
		lblGameOver.AutoSize = false;
		lblGameOver.Size = new Size(menuW, 100);
		lblGameOver.TextAlign = ContentAlignment.MiddleCenter;
		lblGameOver.Location = new Point(menuX, menuY + 20);
		lblGameOver.Visible = false;
		this.Controls.Add(lblGameOver);
		gameOverControls.Add(lblGameOver);

		Button btnBackToMenu = CreateStyledButton("BACK TO MENU", menuX + 150, menuY + 400);
		btnBackToMenu.Click += (s, e) => QuitToMenu();
		btnBackToMenu.Visible = false;
		this.Controls.Add(btnBackToMenu);
		gameOverControls.Add(btnBackToMenu);
	}

	public void ShowGameOverUI(bool show){
		foreach(var ctrl in gameOverControls) {
			ctrl.Visible = show;
			ctrl.Enabled = show;
			if(show) ctrl.BringToFront();
		}
	}

	private void CreatePlayerSelectUI(){
		int menuW = 800;
		int menuH = 400;
		int menuX = (windowWidth - menuW) / 2 + 5;
		int menuY = (windowHeight - menuH) / 2;

		Label lblAsk = new Label();
		lblAsk.Text = "SELECT SQUAD SIZE";
		if(Resources.Font._pfc.Families.Length > 0){
			lblAsk.Font = new Font(Resources.Font._pfc.Families[0], 28, FontStyle.Bold);
		}else{
			lblAsk.Font = new Font("Arial", 28, FontStyle.Bold);
		}
		lblAsk.ForeColor = themeColor;
		lblAsk.BackColor = Color.Transparent;
		lblAsk.AutoSize = false;
		lblAsk.Size = new Size(menuW, 80);
		lblAsk.TextAlign = ContentAlignment.MiddleCenter;
		lblAsk.Location = new Point(menuX, menuY);
		lblAsk.Visible = false;
		this.Controls.Add(lblAsk);
		playerSelectControls.Add(lblAsk);

		for(int i = 1; i <= 3; i++){
			int count = i;
			Button btn = new Button();
			btn.Text = i.ToString() + (i == 1 ? " PLAYER" : " PLAYERS");
			btn.Size = new Size(180, 180);
			btn.Location = new Point(menuX + 100 + (i-1) * 220, menuY + 120);
			btn.FlatStyle = FlatStyle.Flat;
			btn.FlatAppearance.BorderSize = 3;
			btn.FlatAppearance.BorderColor = themeColor;
			btn.ForeColor = Color.White;
			btn.BackColor = Color.FromArgb(80, 20, 20, 30);
			btn.Visible = false;
			
			if(Resources.Font._pfc.Families.Length > 0){
				btn.Font = new Font(Resources.Font._pfc.Families[0], 14, FontStyle.Bold);
			}else{
				btn.Font = new Font("Arial", 14, FontStyle.Bold);
			}

			int originalY = btn.Location.Y;
			btn.MouseEnter += (s, e) => { 
				btn.BackColor = themeColor; 
				btn.ForeColor = Color.Black; 
				btn.Location = new Point(btn.Location.X, originalY - 15); 
			};
			btn.MouseLeave += (s, e) => { 
				btn.BackColor = Color.FromArgb(80, 20, 20, 30); 
				btn.ForeColor = Color.White; 
				btn.Location = new Point(btn.Location.X, originalY); 
			};
			
			btn.Click += (s, e) => {
				foreach(var c in playerSelectControls){
					c.Visible = false;
					c.Enabled = false;
				}

        		Game.turnManager.Reset();
				Game.activeState = Game.State.Playing;
				maintheme.Stop();
				Game.New(count);
			};

			this.Controls.Add(btn);
			playerSelectControls.Add(btn);
		}
	}

	private void CreatePauseUI(){
		int menuW = 500;
		int menuH = 400;
		int menuX = (windowWidth - menuW) / 2;
		int menuY = (windowHeight - menuH) / 2;

		Label lblPaused = new Label();
		lblPaused.Text = "PAUSED";
		if (Resources.Font._pfc.Families.Length > 0)
			lblPaused.Font = new Font(Resources.Font._pfc.Families[0], 32, FontStyle.Bold);
		else
			lblPaused.Font = new Font("Arial", 32, FontStyle.Bold);

		lblPaused.ForeColor = themeColor;
		lblPaused.BackColor = Color.Transparent;
		lblPaused.AutoSize = false;
		lblPaused.Size = new Size(menuW, 80);
		lblPaused.TextAlign = ContentAlignment.MiddleCenter;
		lblPaused.Location = new Point(menuX, menuY + 40);
		lblPaused.Visible = false;
		this.Controls.Add(lblPaused);
		pauseControls.Add(lblPaused);

		Button btnResume = CreateStyledButton("RESUME", menuX + 100, menuY + 140);
		btnResume.Click += (s, e) => ShowPauseMenu(false);
		btnResume.Visible = false;
		this.Controls.Add(btnResume);
		pauseControls.Add(btnResume);

		Button btnSaveQuit = CreateStyledButton("SAVE and QUIT", menuX + 100, menuY + 220);
		btnSaveQuit.Click += (s, e) => SaveAndQuit();
		btnSaveQuit.Visible = false;
		this.Controls.Add(btnSaveQuit);
		pauseControls.Add(btnSaveQuit);

		Button btnQuit = CreateStyledButton("QUIT", menuX + 100, menuY + 300);
		btnQuit.Click += (s, e) => QuitToMenu();
		btnQuit.Visible = false;
		this.Controls.Add(btnQuit);
		pauseControls.Add(btnQuit);
	}

	public void ShowPauseMenu(bool show){
		if (show) Game.activeState = Game.State.Paused;
		else Game.activeState = Game.State.Playing;

		foreach(var ctrl in pauseControls) {
			ctrl.Visible = show;
			ctrl.Enabled = show;
			if(show) ctrl.BringToFront();
		}
	}

	private void SaveAndQuit(){
		SaveSystem.Save("savegame.json");
		QuitToMenu();
	}

	private void QuitToMenu(){
		ShowPauseMenu(false);
		ShowGameOverUI(false);
		Game.activeState = Game.State.Menu;
		Game.End();
		foreach(var ctrl in menuControls) {
			ctrl.Visible = true;
			ctrl.Enabled = true;
			ctrl.BringToFront();
		}
	}

	private Button CreateStyledButton(string text, int x, int y){
		Button btn = new Button();
		btn.Text = text;
		btn.Size = new Size(300, 60);
		btn.Location = new Point(x, y);
		btn.FlatStyle = FlatStyle.Flat;
		btn.FlatAppearance.BorderSize = 2;
		btn.FlatAppearance.BorderColor = themeColor;
		btn.ForeColor = Color.White;
		btn.BackColor = Color.FromArgb(60, 20, 20, 30); // Semi-transparent button background

		if (Resources.Font._pfc.Families.Length > 0)
			btn.Font = new Font(Resources.Font._pfc.Families[0], 13, FontStyle.Bold);
		else
			btn.Font = new Font("Arial", 13, FontStyle.Bold);
		
		btn.MouseEnter += (s, e) => { btn.BackColor = themeColor; btn.ForeColor = Color.Black; };
		btn.MouseLeave += (s, e) => { btn.BackColor = Color.FromArgb(60, 20, 20, 30); btn.ForeColor = Color.White; };
		
		return btn;
	}

	private void StartGame(bool turnBased){
		Game.isTurnBased = turnBased;
		
		foreach(var ctrl in menuControls){
			ctrl.Visible = false;
			ctrl.Enabled = false;
		}

		if(turnBased){
			foreach(var ctrl in playerSelectControls){
				ctrl.Visible = true;
				ctrl.Enabled = true;
				ctrl.BringToFront();
			}
		}else{
			maintheme.Stop();
			Game.New(1);
			Game.activeState = Game.State.Playing;
		}
	}

	private void LoadGame(){
		maintheme.Stop();
		Game.New();
		SaveSystem.Load("savegame.json");
		foreach(var ctrl in menuControls){
			ctrl.Visible = false;
			ctrl.Enabled = false;
		}
		Game.activeState = Game.State.Playing;
	}

	private void Debug(){
		Console.WriteLine("New console");
	}

	private void InitializeUI(){
		BackColor = Color.White;
		ClientSize = new Size(windowWidth, windowHeight);
		MaximizeBox = false;
		KeyPreview = false;
		MinimizeBox = false;
		DoubleBuffered = true;
		Name = "Petty Goober";
		Text = "";

		this.SetStyle(ControlStyles.AllPaintingInWmPaint, true); 
	    this.SetStyle(ControlStyles.UserPaint, true);           
    	this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		this.UpdateStyles();
	}

	private void SetFullscreen(bool fullscreen){
		if (fullscreen) {
			WindowState = FormWindowState.Maximized;
			FormBorderStyle = FormBorderStyle.None;
			ClientSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
		}
		else {
			WindowState = FormWindowState.Normal;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			ClientSize = new Size(windowWidth, windowHeight);
		}
		CenterToScreen();
    }

	protected override CreateParams CreateParams {
		get {
			CreateParams cp = base.CreateParams;
			cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
			return cp;
		}
	}

	protected override void OnPaintBackground(PaintEventArgs e){
	}
	protected override void OnPaint(PaintEventArgs e){
       	e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
		if (Game.activeState == Game.State.Playing || Game.activeState == Game.State.Paused || Game.activeState == Game.State.GameOver){
			Game.draw?.Update(e);

			if (Game.activeState == Game.State.Paused) {
				using (SolidBrush b = new SolidBrush(Color.FromArgb(180, 0, 0, 0))) {
					e.Graphics.FillRectangle(b, this.ClientRectangle);
				}

				int menuW = 500;
				int menuH = 400;
				int menuX = (windowWidth - menuW) / 2;
				int menuY = (windowHeight - menuH) / 2;
				Rectangle menuRect = new Rectangle(menuX, menuY, menuW, menuH);
				using (SolidBrush b = new SolidBrush(Color.FromArgb(140, 15, 15, 25)))
				{
					e.Graphics.FillRectangle(b, menuRect);
				}
				using (Pen p = new Pen(themeColor, 1))
				{
					e.Graphics.DrawRectangle(p, menuRect);
				}
			}
			else if (Game.activeState == Game.State.GameOver) {
				ShowGameOverUI(true);
				DrawLeaderboard(e.Graphics);
			}
		}
		else {
			animationTimer += 0.05f;

			// Parallax Logic
			Point mousePos = this.PointToClient(Cursor.Position);
			float mousePctX = (float)mousePos.X / windowWidth - 0.5f;
			float mousePctY = (float)mousePos.Y / windowHeight - 0.5f;

			// Background layer
			if (imgParallaxBack != null) {
				int backOffset = 40;
				int x = -backOffset + (int)(mousePctX * -backOffset);
				int y = -backOffset + (int)(mousePctY * -backOffset);
				e.Graphics.DrawImage(imgParallaxBack, x, y, windowWidth + backOffset * 2, windowHeight + backOffset * 2);
			}

			// Subtle grid
			using (Pen p = new Pen(Color.FromArgb(20, themeColor), 1)) {
				for (int i = 0; i < windowWidth; i += 80) e.Graphics.DrawLine(p, i, 0, i, windowHeight);
				for (int i = 0; i < windowHeight; i += 80) e.Graphics.DrawLine(p, 0, i, windowWidth, i);
			}

			// Foreground layer
			if (imgParallaxFore != null) {
				int foreOffset = 80;
				int x = -foreOffset + (int)(mousePctX * -foreOffset);
				int y = -foreOffset + (int)(mousePctY * -foreOffset);
				e.Graphics.DrawImage(imgParallaxFore, x, y, windowWidth + foreOffset * 2, windowHeight + foreOffset * 2);
			}

			// Draw Menu Container Box
			int menuW = 700;
			int menuH = 600;
			int menuX = (windowWidth - menuW) / 2;
			int menuY = (windowHeight - menuH) / 2;
			Rectangle menuRect = new Rectangle(menuX, menuY, menuW, menuH);
			using (SolidBrush b = new SolidBrush(Color.FromArgb(140, 15, 15, 25))) {
				e.Graphics.FillRectangle(b, menuRect);
			}
			using (Pen p = new Pen(themeColor, 1)) {
				e.Graphics.DrawRectangle(p, menuRect);
			}

			string title = "FELONY FIESTA";
			using (Font titleFont = new Font(Resources.Font._pfc.Families[0], 42, FontStyle.Bold)) {
				SizeF size = e.Graphics.MeasureString(title, titleFont);
				float titleX = menuX + (menuW - size.Width) / 2;
				float titleY = menuY + 50 + (float)Math.Sin(animationTimer) * 8;

				// Outer Glow
				for (int i = 10; i > 0; i--) {
					using (Brush b = new SolidBrush(Color.FromArgb(15 - i, themeColor))) {
						e.Graphics.DrawString(title, titleFont, b, titleX, titleY + i);
						e.Graphics.DrawString(title, titleFont, b, titleX, titleY - i);
						e.Graphics.DrawString(title, titleFont, b, titleX + i, titleY);
						e.Graphics.DrawString(title, titleFont, b, titleX - i, titleY);
					}
				}

				// Main Text
				e.Graphics.DrawString(title, titleFont, Brushes.White, titleX, titleY);
				
				// Pulse Effect
				int alphaPulse = (int)(30 + Math.Sin(animationTimer * 2) * 20);
				using (Brush b = new SolidBrush(Color.FromArgb(alphaPulse, themeColor))) {
					e.Graphics.DrawString(title, titleFont, b, titleX, titleY);
				}
			}
		}
	}

	private void DrawLeaderboard(Graphics g) {
		using (SolidBrush b = new SolidBrush(Color.FromArgb(200, 0, 0, 0))) {
			g.FillRectangle(b, this.ClientRectangle);
		}

		int menuW = 600;
		int menuH = 500;
		int menuX = (windowWidth - menuW) / 2;
		int menuY = (windowHeight - menuH) / 2;
		Rectangle menuRect = new Rectangle(menuX, menuY, menuW, menuH);

		using (SolidBrush b = new SolidBrush(Color.FromArgb(140, 25, 15, 15))) {
			g.FillRectangle(b, menuRect);
		}
		using (Pen p = new Pen(Color.Red, 2)) {
			g.DrawRectangle(p, menuRect);
		}

		using (Font scoreFont = new Font("Arial", 24, FontStyle.Bold)) {
			g.DrawString("FINAL SCORES", scoreFont, Brushes.White, menuX + 180, menuY + 130);

			if (Game.env != null) {
				for (int i = 0; i < Game.env.players.Count; i++) {
					string scoreText = $"PLAYER {i + 1}: {Game.env.players[i].score}";
					g.DrawString(scoreText, scoreFont, Brushes.Yellow, menuX + 150, menuY + 200 + i * 50);
				}
			}
		}
	}
}
