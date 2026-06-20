using System.Windows.Forms;
using game;

public static class Game{
	public static Environment env = null!;
	public static Draw draw = null!;
	public static Camera camera = null!;
	public static int windowWidth;
	public static int windowHeight;
	public enum State { Menu, Playing, Paused, GameOver }
	public static State activeState = State.Menu;
	public static bool isTurnBased = false;

	public static Random rand = null!;
	public static TurnManager turnManager = new TurnManager();
	private static Keyboard? escKeyboard;
	
	private static System.Windows.Forms.Timer gametimer = new System.Windows.Forms.Timer();

	public static void Init(Form window, int pwindowWidth, int pwindowHeight){
		windowWidth = pwindowWidth;
		windowHeight = pwindowHeight;
		rand = new Random(5556);
		escKeyboard = new Keyboard(new Keys[] { Keys.Escape });

		Resources.Font.Load();
		BuildingsFootprint.Init();
		ItemDropFootprints.Init();
		DialogueFootprints.Init();

		Loop(window);
	}

	public static void End(){
     	camera = null;
		draw = null;
		env = null;
	}

	public static void New(int playerCount = 1){

		camera = new Camera();
		new Environment(playerCount);
		draw = new Draw(env, windowWidth, windowHeight);
	}

	public static void Loop(Form window){
		gametimer.Interval = 18;
		gametimer.Tick += (s, e) => {
			escKeyboard.ReadKeys();
			if (activeState == State.Playing && escKeyboard.GetKeyOnce(Keys.Escape)) {
				activeState = State.Paused;
				if (window is Form1 f) f.ShowPauseMenu(true);
			}
			else if (activeState == State.Paused && escKeyboard.GetKeyOnce(Keys.Escape)) {
				activeState = State.Playing;
				if (window is Form1 f) f.ShowPauseMenu(false);
			}

			if (activeState == State.Playing && env != null) {
				if (isTurnBased) turnManager.Update();
				env.Update();
			}
			window.Invalidate();
		};
		gametimer.Start();
	}
}
