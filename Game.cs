using System.Numerics;
using Raylib_cs;
using System;
using Raylib_cs;

public static class Game {
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

	public static void Init(int pwindowWidth, int pwindowHeight) {
		windowWidth = pwindowWidth;
		windowHeight = pwindowHeight;
		rand = new Random(5556);
		escKeyboard = new Keyboard(new KeyboardKey[] { KeyboardKey.Escape });

		// Resources.Font.Load(); // Removed winforms font load
		BuildingsFootprint.Init();
		ItemDropFootprints.Init();
		DialogueFootprints.Init();
	}

	public static void End() {
     	camera = null;
		draw = null;
		env = null;
	}

	public static void New(int playerCount = 1) {
		camera = new Camera();
		new Environment(playerCount);
		draw = new Draw(env, windowWidth, windowHeight);
	}

	public static void Loop() {
        escKeyboard.ReadKeys();
        if (activeState == State.Playing && escKeyboard.GetKeyOnce(KeyboardKey.Escape)) {
            activeState = State.Paused;
        }
        else if (activeState == State.Paused && escKeyboard.GetKeyOnce(KeyboardKey.Escape)) {
            activeState = State.Playing;
        }

        if (activeState == State.Playing && env != null) {
            if (isTurnBased) turnManager.Update();
            env.Update();
        }
        
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.RayWhite);
        
        if (activeState == State.Playing || activeState == State.Paused || activeState == State.GameOver) {
            if (draw != null) draw.Update();
        }
		
		Form1.UpdateAndDraw();
        
        Raylib.EndDrawing();
	}
}



