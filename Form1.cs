using System;
using System.Numerics;
using Raylib_cs;
using game;

public class Form1 {
	private static Color themeColor = new Color(180, 80, 255, 255);
	private static float animationTimer = 0;
	
	public static void UpdateAndDraw() {
		if (Game.activeState == Game.State.Menu) {
			DrawMenu();
		}
		else if (Game.activeState == Game.State.Paused) {
			DrawPause();
		}
		else if (Game.activeState == Game.State.GameOver) {
			DrawGameOver();
		}
	}

	private static bool DrawButton(string text, int x, int y, int width, int height) {
		Rectangle bounds = new Rectangle(x, y, width, height);
		bool isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), bounds);
		bool isClicked = isHovered && Raylib.IsMouseButtonPressed(MouseButton.Left);

		Color bgColor = isHovered ? themeColor : new Color(60, 20, 20, 150);
		Color textColor = isHovered ? Color.Black : Color.White;

		Raylib.DrawRectangleRec(bounds, bgColor);
		Raylib.DrawRectangleLinesEx(bounds, 2, themeColor);
		
		int textWidth = Raylib.MeasureText(text, 20);
		Raylib.DrawText(text, x + (width - textWidth) / 2, y + (height - 20) / 2, 20, textColor);

		return isClicked;
	}

	private static Texture2D? imgParallaxBack = null;
	private static Texture2D? imgParallaxFore = null;

	private static void DrawMenu() {
		animationTimer += 0.05f;
		
		Raylib.ClearBackground(new Color(15, 15, 25, 255));
		
		if (imgParallaxBack == null) {
			imgParallaxBack = Sprite.GetImage(Resources.UI.Parallax.Background);
			imgParallaxFore = Sprite.GetImage(Resources.UI.Parallax.Foreground);
		}

		Vector2 mousePos = Raylib.GetMousePosition();
		float mousePctX = mousePos.X / Game.windowWidth - 0.5f;
		float mousePctY = mousePos.Y / Game.windowHeight - 0.5f;

		if (imgParallaxBack.HasValue) {
			int backOffset = 40;
			int x = -backOffset + (int)(mousePctX * -backOffset);
			int y = -backOffset + (int)(mousePctY * -backOffset);
			Rectangle dest = new Rectangle(x, y, Game.windowWidth + backOffset * 2, Game.windowHeight + backOffset * 2);
			Raylib.DrawTexturePro(imgParallaxBack.Value, new Rectangle(0, 0, imgParallaxBack.Value.Width, imgParallaxBack.Value.Height), dest, Vector2.Zero, 0, Color.White);
		}

		Color gridColor = new Color((int)themeColor.R, (int)themeColor.G, (int)themeColor.B, 20);
		for (int i = 0; i < Game.windowWidth; i += 80) Raylib.DrawLine(i, 0, i, Game.windowHeight, gridColor);
		for (int i = 0; i < Game.windowHeight; i += 80) Raylib.DrawLine(0, i, Game.windowWidth, i, gridColor);

		if (imgParallaxFore.HasValue) {
			int foreOffset = 80;
			int x = -foreOffset + (int)(mousePctX * -foreOffset);
			int y = -foreOffset + (int)(mousePctY * -foreOffset);
			Rectangle dest = new Rectangle(x, y, Game.windowWidth + foreOffset * 2, Game.windowHeight + foreOffset * 2);
			Raylib.DrawTexturePro(imgParallaxFore.Value, new Rectangle(0, 0, imgParallaxFore.Value.Width, imgParallaxFore.Value.Height), dest, Vector2.Zero, 0, Color.White);
		}
		
		// Draw Menu Container Box
		int menuW = 700;
		int menuH = 600;
		int menuX = (Game.windowWidth - menuW) / 2;
		int menuY = (Game.windowHeight - menuH) / 2;
		Rectangle menuRect = new Rectangle(menuX, menuY, menuW, menuH);
		
		Raylib.DrawRectangleRec(menuRect, new Color(15, 15, 25, 140));
		Raylib.DrawRectangleLinesEx(menuRect, 1, themeColor);

		string title = "FELONY FIESTA";
		int titleWidth = Raylib.MeasureText(title, 40);
		int titleX = menuX + (menuW - titleWidth) / 2;
		int titleY = menuY + 50 + (int)(Math.Sin(animationTimer) * 8);

		Raylib.DrawText(title, titleX, titleY, 40, Color.White);

		if (DrawButton("SINGLE-PLAYER", menuX + 200, menuY + 230, 300, 60)) {
			Game.isTurnBased = false;
			Game.New(1);
			Game.activeState = Game.State.Playing;
		}

		if (DrawButton("TURN-BASED", menuX + 200, menuY + 310, 300, 60)) {
			// Temp skip player select
			Game.isTurnBased = true;
			Game.turnManager.Reset();
			Game.New(2);
			Game.activeState = Game.State.Playing;
		}

		if (DrawButton("LOAD", menuX + 200, menuY + 390, 300, 60)) {
			Game.New(1);
			SaveSystem.Load("savegame.json");
			Game.activeState = Game.State.Playing;
		}

		if (DrawButton("EXIT", menuX + 200, menuY + 470, 300, 60)) {
			// signal exit somehow, e.g. CloseWindow... well, we can't easily break the while loop from here unless we set a flag.
			// we'll just force close
			System.Environment.Exit(0);
		}
	}

	private static void DrawPause() {
		Raylib.DrawRectangle(0, 0, Game.windowWidth, Game.windowHeight, new Color(0, 0, 0, 180));

		int menuW = 500;
		int menuH = 400;
		int menuX = (Game.windowWidth - menuW) / 2;
		int menuY = (Game.windowHeight - menuH) / 2;
		
		Raylib.DrawRectangle(menuX, menuY, menuW, menuH, new Color(15, 15, 25, 140));
		Raylib.DrawRectangleLinesEx(new Rectangle(menuX, menuY, menuW, menuH), 2, themeColor);

		Raylib.DrawText("PAUSED", menuX + 170, menuY + 40, 40, themeColor);

		if (DrawButton("RESUME", menuX + 100, menuY + 140, 300, 60)) {
			Game.activeState = Game.State.Playing;
		}
		
		if (DrawButton("SAVE and QUIT", menuX + 100, menuY + 220, 300, 60)) {
			SaveSystem.Save("savegame.json");
			Game.End();
			Game.activeState = Game.State.Menu;
		}

		if (DrawButton("QUIT", menuX + 100, menuY + 300, 300, 60)) {
			Game.End();
			Game.activeState = Game.State.Menu;
		}
	}

	private static void DrawGameOver() {
		Raylib.DrawRectangle(0, 0, Game.windowWidth, Game.windowHeight, new Color(0, 0, 0, 200));

		int menuW = 600;
		int menuH = 500;
		int menuX = (Game.windowWidth - menuW) / 2;
		int menuY = (Game.windowHeight - menuH) / 2;

		Raylib.DrawRectangle(menuX, menuY, menuW, menuH, new Color(25, 15, 15, 140));
		Raylib.DrawRectangleLinesEx(new Rectangle(menuX, menuY, menuW, menuH), 2, Color.Red);

		Raylib.DrawText("SQUAD WIPED", menuX + 160, menuY + 20, 40, Color.Red);
		Raylib.DrawText("FINAL SCORES", menuX + 180, menuY + 130, 30, Color.White);

		if (Game.env != null) {
			for (int i = 0; i < Game.env.players.Count; i++) {
				string scoreText = $"PLAYER {i + 1}: {Game.env.players[i].score}";
				Raylib.DrawText(scoreText, menuX + 150, menuY + 200 + i * 50, 30, Color.Yellow);
			}
		}

		if (DrawButton("BACK TO MENU", menuX + 150, menuY + 400, 300, 60)) {
			Game.End();
			Game.activeState = Game.State.Menu;
		}
	}
}



