using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

public class Draw {
	Environment env;
	int windowWidth;
	int windowHeight;

	// 3D
	private Vector2 sun = new Vector2(2.8f, 3.0f);
	private float perspectiveFactor = 0.2f;

	private void DrawRotated(Texture2D Texture2D, Rectangle r, float rotationDegrees) {
		Rectangle source = new Rectangle(0, 0, Texture2D.Width, Texture2D.Height);
		Vector2 origin = new Vector2(r.Width / 2f, r.Height / 2f);
		Rectangle dest = new Rectangle(r.X + origin.X, r.Y + origin.Y, r.Width, r.Height);
		Raylib.DrawTexturePro(Texture2D, source, dest, origin, rotationDegrees, Color.White);
	}

	private void DrawEntity(Entity ent) {
		DrawRotated(ent.Texture2D, ent.r, ent.rotation);
		if (ent.props != null)
			foreach (var p in ent.props)
				DrawRotated(p.Texture2D, p.r, p.rotation); 
	}

	Sprite ammoPanel = new Sprite(Resources.UI._ammopanel);
	private void PrintAmmo() {
		if (Game.camera.follow is not Player p) return;

		Rectangle rPanel = new Rectangle(Game.windowWidth - ammoPanel.frame.Width * 1.4f - 5, 10, ammoPanel.frame.Width * 1.4f, ammoPanel.frame.Height * 1.4f);
		Raylib.DrawTexturePro(ammoPanel.frame, new Rectangle(0, 0, ammoPanel.frame.Width, ammoPanel.frame.Height), rPanel, Vector2.Zero, 0, Color.White);

		string[] texts = {
			$"{p.inventory.bullets}",
			$"{p.inventory.shells}",
			$"{p.inventory.smallbullets}",
			$"{p.inventory.grenades}",
			$"{p.inventory.rockets}",
			$"{p.inventory.batteries}"
		};
		
		if (p.inventory != null) {
			for (int i = 0; i < texts.Length; i++) {
				Raylib.DrawText(texts[i], (int)(rPanel.X + 25 - 17 * (texts[i].Length - 1) / 2 + 86 * (i % 3)), (int)(rPanel.Y + 45 + 45 * (i / 3)), 20, Color.Black);
			}
		}
		
		if (p.selectedWeapon != null) {
			if (p.selectedWeapon.icon != null) {
				Rectangle dest = new Rectangle(rPanel.X + rPanel.Width - 105, 20, 65, 65);
				Raylib.DrawTexturePro(p.selectedWeapon.icon.frame, new Rectangle(0, 0, p.selectedWeapon.icon.frame.Width, p.selectedWeapon.icon.frame.Height), dest, Vector2.Zero, 0, Color.White);
			}
			string text = $"-";
			if (p.selectedWeapon.type != Weapon.Type.Melee)
				text = $"{p.selectedWeapon.currentClip}";
			Raylib.DrawText(text, (int)(rPanel.X + 335 - 17 * (text.Length - 1) / 2), (int)(rPanel.Y + 93), 20, Color.Black);
		}
	}
	
	private void DrawWall(Texture2D texture, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
		Rlgl.SetTexture(texture.Id);
		Rlgl.Begin(DrawMode.Quads);
		Rlgl.Color4ub(255, 255, 255, 255);
		// Counter-clockwise
		Rlgl.TexCoord2f(0.0f, 0.0f); Rlgl.Vertex2f(p1.X, p1.Y);
		Rlgl.TexCoord2f(0.0f, 1.0f); Rlgl.Vertex2f(p4.X, p4.Y);
		Rlgl.TexCoord2f(1.0f, 1.0f); Rlgl.Vertex2f(p3.X, p3.Y);
		Rlgl.TexCoord2f(1.0f, 0.0f); Rlgl.Vertex2f(p2.X, p2.Y);
		// Clockwise
		Rlgl.TexCoord2f(0.0f, 0.0f); Rlgl.Vertex2f(p1.X, p1.Y);
		Rlgl.TexCoord2f(1.0f, 0.0f); Rlgl.Vertex2f(p2.X, p2.Y);
		Rlgl.TexCoord2f(1.0f, 1.0f); Rlgl.Vertex2f(p3.X, p3.Y);
		Rlgl.TexCoord2f(0.0f, 1.0f); Rlgl.Vertex2f(p4.X, p4.Y);
		Rlgl.End();
		Rlgl.SetTexture(0);
	}

	private void DrawBuildings(List<Building> buildings) {
		Rectangle camera = Game.camera.r;
		int screenCX = windowWidth / 2;
		int screenCY = windowHeight / 2;
		
		Rectangle viewport = new Rectangle(camera.X - screenCX - 300, camera.Y - screenCY - 300, windowWidth + 500, windowHeight + 500);

		var visibleBuildings = buildings.Where(b => Tools.IsColliding(viewport, new Rectangle(b.X, b.Y, b.Width, b.Height))).ToList();

		var sortedBuildings = visibleBuildings.OrderByDescending(b => {
			float screenX = b.X - camera.X + screenCX;
			float screenY = b.Y - camera.Y + screenCY;
			double dx = screenX - screenCX;
			double dy = screenY - screenCY;
			return (dx * dx) + (dy * dy); 
    	}).ToList();
		
		foreach (var b in sortedBuildings) {
			Vector2 worldBase = new Vector2(b.X, b.Y);
			Vector2 distFromCam = new Vector2(b.X - Game.camera.r.X, b.Y - Game.camera.r.Y);
			Vector2 offset = new Vector2(
					distFromCam.X * perspectiveFactor * (b.Height3D / 60f),
				    distFromCam.Y * perspectiveFactor * (b.Height3D / 60f)
			);

			Rectangle baseRect = new Rectangle(worldBase.X, worldBase.Y, b.Width, b.Height);
			Rectangle roofRect = new Rectangle(worldBase.X + offset.X, worldBase.Y + offset.Y, b.Width, b.Height);
			Vector2 shadowOffset = new Vector2(b.Height3D * sun.X, b.Height3D * sun.Y);

			Vector2 p1 = new Vector2(worldBase.X, worldBase.Y);
			Vector2 p2 = new Vector2(worldBase.X + b.Width, worldBase.Y);
			Vector2 p3 = new Vector2(worldBase.X + b.Width + shadowOffset.X, worldBase.Y + shadowOffset.Y);
			Vector2 p4 = new Vector2(worldBase.X + shadowOffset.X, worldBase.Y + shadowOffset.Y);

			Color shadowColor = new Color(0, 0, 0, 110);
			Raylib.DrawTriangle(p1, p4, p2, shadowColor);
			Raylib.DrawTriangle(p1, p2, p4, shadowColor);
			Raylib.DrawTriangle(p4, p3, p2, shadowColor);
			Raylib.DrawTriangle(p4, p2, p3, shadowColor);

			// Wall corners
			Vector2 b_tl = new Vector2(baseRect.X, baseRect.Y);
			Vector2 b_tr = new Vector2(baseRect.X + baseRect.Width, baseRect.Y);
			Vector2 b_bl = new Vector2(baseRect.X, baseRect.Y + baseRect.Height);
			Vector2 b_br = new Vector2(baseRect.X + baseRect.Width, baseRect.Y + baseRect.Height);

			Vector2 r_tl = new Vector2(roofRect.X, roofRect.Y);
			Vector2 r_tr = new Vector2(roofRect.X + roofRect.Width, roofRect.Y);
			Vector2 r_bl = new Vector2(roofRect.X, roofRect.Y + roofRect.Height);
			Vector2 r_br = new Vector2(roofRect.X + roofRect.Width, roofRect.Y + roofRect.Height);

			if (offset.X > 0 && b.Lwall != null) {
				DrawWall(b.Lwall.frame, r_tl, r_bl, b_bl, b_tl);
			}
			if (offset.Y > 0 && b.Uwall != null) {
				DrawWall(b.Uwall.frame, r_tl, r_tr, b_tr, b_tl);
			}
			if (offset.X < 0 && b.Rwall != null) {
				DrawWall(b.Rwall.frame, r_tr, r_br, b_br, b_tr);
				Color dark = new Color(0, 0, 0, 80);
				Raylib.DrawTriangle(r_tr, b_tr, r_br, dark);
				Raylib.DrawTriangle(r_tr, r_br, b_tr, dark);
				Raylib.DrawTriangle(r_br, b_tr, b_br, dark);
				Raylib.DrawTriangle(r_br, b_br, b_tr, dark);
			}
			if (offset.Y < 0 && b.Bwall != null) {
				DrawWall(b.Bwall.frame, r_bl, r_br, b_br, b_bl);
				Color dark = new Color(0, 0, 0, 80);
				Raylib.DrawTriangle(r_bl, b_bl, r_br, dark);
				Raylib.DrawTriangle(r_bl, r_br, b_bl, dark);
				Raylib.DrawTriangle(r_br, b_bl, b_br, dark);
				Raylib.DrawTriangle(r_br, b_br, b_bl, dark);
			}
	
			// Drawing roofs and walls
			float ledgeThickness = 2 + (b.Height3D / 10);
			Rectangle innerRoof = new Rectangle(
				roofRect.X + ledgeThickness, 
				roofRect.Y + ledgeThickness, 
				roofRect.Width - (ledgeThickness * 2), 
				roofRect.Height - (ledgeThickness * 2)
			);
	
			Raylib.DrawRectangleRec(roofRect, Color.Gray);
			
			if (b.roof != null) {
				float viewPct = 0.7f; 
				float srcW = b.roof.frame.Width * viewPct;
				float srcH = b.roof.frame.Height * viewPct;

				float maxShiftX = b.roof.frame.Width - srcW + 0.1f;
				float maxShiftY = b.roof.frame.Height - srcH + 0.1f;

				float pctX = Math.Clamp((roofRect.X / windowWidth), 0, 1);
				float pctY = Math.Clamp((roofRect.Y / windowHeight), 0, 1);

				Rectangle manualSrcRect = new Rectangle(pctX * maxShiftX, pctY * maxShiftY, srcW, srcH);
				Raylib.DrawTexturePro(b.roof.frame, manualSrcRect, innerRoof, Vector2.Zero, 0, Color.White);
			}

			Raylib.DrawLineEx(new Vector2(innerRoof.X, innerRoof.Y), new Vector2(innerRoof.X + innerRoof.Width, innerRoof.Y), 2, new Color(0, 0, 0, 120));
			Raylib.DrawLineEx(new Vector2(innerRoof.X, innerRoof.Y), new Vector2(innerRoof.X, innerRoof.Y + innerRoof.Height), 2, new Color(0, 0, 0, 120));
		}
	}

	private void DrawVehicle(Vehicle ent) {
		float fakeHeight = ent.speed * 0.16f;
		float offset = 0;
		if (ent.isTurning != 0) offset = ent.isTurning == 1 ? -8 : 6;
		
		Rectangle r = ent.r;
		if (ent.isAccelerating == 1 && ent.speed > 0.2) {
			r.Height *= ent.stretchFactor;
			r.Width *= ent.squashFactor;
		}
		if ((ent.isAccelerating == -1 && ent.speed > 0.2) || (ent.isAccelerating == 1 && ent.speed < -0.2)) {
			r.Width *= ent.stretchFactor;
			r.Height *= ent.squashFactor;
		}
		
		Rectangle shadowRect = new Rectangle(ent.r.X - offset - fakeHeight, ent.r.Y - fakeHeight, ent.r.Width, ent.r.Height);
		DrawRotated(ent.shadow.Texture2D, shadowRect, ent.rotation);
		DrawRotated(ent.Texture2D, r, ent.rotation);
		
		if (ent.props != null)
			foreach (var p in ent.props)
				DrawRotated(p.Texture2D, p.r, p.rotation);
	}

	DateTime startTime = DateTime.Now;
	private void DrawPlayer(Player player) {
		DrawRotated(player.Texture2D, player.r, player.rotation);

		Weapon? weapon = player.selectedWeapon;
		Vector2 weaponMovementVect = new Vector2(player.r.X, player.r.Y);
		if (weapon == null) return;
		
		if (player.speed > 0.2) {
			float weaponMovement = (float)Math.Sin((DateTime.Now - startTime).TotalSeconds * 20) * 4f;
			var add = Tools.Scalar2Vect_Speed(player.rotation, weaponMovement);	
			weaponMovementVect.X += add.X;
			weaponMovementVect.Y += add.Y;
		}
		DrawRotated(weapon.sprite.frame, new Rectangle(weaponMovementVect.X, weaponMovementVect.Y, player.Width, player.Height), player.rotation);
	}
	
	private void DrawCrosshair(Crosshair c) {
		Rectangle dest = new Rectangle(c.r.X, c.r.Y, c.r.Width, c.r.Height);
		Raylib.DrawTexturePro(c.Texture2D, new Rectangle(0, 0, c.Texture2D.Width, c.Texture2D.Height), dest, Vector2.Zero, 0, Color.White);
	}
	
	private void DrawItem(ItemDrop item) {
		float floatHeight = item.floatY;
		Vector2 ground = new Vector2(item.X, item.Y + item.Height);
		Vector2 offset = new Vector2((ground.X - Game.camera.r.X) * perspectiveFactor * 0.5f, (ground.Y - Game.camera.r.Y) * perspectiveFactor * 0.5f);
		Vector2 shadow = new Vector2(ground.X + offset.X + (floatHeight * sun.X * 0.2f), ground.Y + offset.Y + (floatHeight * sun.Y * 0.2f));

		float shadowScale = Math.Max(0.5f, 1.0f - (floatHeight / 100f));
		int shadowAlpha = (int)Math.Max(0, 110 - (floatHeight * 2));

		float sW = item.Width * shadowScale;
		float sH = (item.Height / 2f) * shadowScale;
		
		Raylib.DrawEllipse((int)shadow.X, (int)shadow.Y, sW / 2, sH / 2, new Color(0, 0, 0, shadowAlpha));
		
		Rectangle dest = new Rectangle(item.X, item.Y + item.floatY, item.Width, item.Height);
		Raylib.DrawTexturePro(item.Texture2D, new Rectangle(0, 0, item.Texture2D.Width, item.Texture2D.Height), dest, Vector2.Zero, 0, Color.White);
	}

	private void DrawMap(Map map) {
		int startX = (int)Math.Max(0, (Game.camera.r.X - windowWidth) / map.tileRenderDimension);
		int startY = (int)Math.Max(0, (Game.camera.r.Y - windowHeight) / map.tileRenderDimension);
		int endX = (int)Math.Min(map.map.GetLength(0), (Game.camera.r.X + windowWidth) / map.tileRenderDimension + 1);
		int endY = (int)Math.Min(map.map.GetLength(1), (Game.camera.r.Y + windowHeight) / map.tileRenderDimension + 1);

		for (int i = startX; i < endX; i++) {
			for (int j = startY; j < endY; j++) {
				int tile1 = map.map[i, j];
				Rectangle dest = new Rectangle(i * map.tileRenderDimension, j * map.tileRenderDimension, map.tileRenderDimension, map.tileRenderDimension);
				
				if (tile1 != -1) {
					Raylib.DrawTexturePro(map.tileMap[tile1], new Rectangle(0, 0, map.tileMap[tile1].Width, map.tileMap[tile1].Height), dest, Vector2.Zero, 0, Color.White);
				}
				
				int tile2 = map.secondLayer[i, j];
				if (tile2 != -1) {
					Raylib.DrawTexturePro(map.tileMap[tile2], new Rectangle(0, 0, map.tileMap[tile2].Width, map.tileMap[tile2].Height), dest, Vector2.Zero, 0, Color.White);
				}
			}
		}
	}

	public void PrintMinimap() {
		if (Game.camera.follow == null) return;

		int size = 200;
		int padding = 20;
		Rectangle miniRect = new Rectangle(padding, padding, size, size);

		Raylib.DrawRectangleRec(miniRect, new Color(0, 0, 0, 180));
		
		Raylib.BeginScissorMode((int)miniRect.X, (int)miniRect.Y, (int)miniRect.Width, (int)miniRect.Height);
		
		Camera2D miniCam = new Camera2D();
		miniCam.Offset = new Vector2(padding + size / 2, padding + size / 2);
		miniCam.Target = Game.camera.follow.center;
		miniCam.Rotation = 0f;
		miniCam.Zoom = 0.15f;
		
		Raylib.BeginMode2D(miniCam);
		
		float scale = miniCam.Zoom;
		Vector2 playerPos = miniCam.Target;
		int td = env.map.tileRenderDimension;
		
		int viewDist = (int)(size / scale / 2) + td;
		int startX = (int)((playerPos.X - viewDist) / td);
		int endX = (int)((playerPos.X + viewDist) / td);
		int startY = (int)((playerPos.Y - viewDist) / td);
		int endY = (int)((playerPos.Y + viewDist) / td);

		for (int i = startX; i <= endX; i++) {
			if (i < 0 || i >= env.map.map.GetLength(0)) continue;
			for (int j = startY; j <= endY; j++) {
				if (j < 0 || j >= env.map.map.GetLength(1)) continue;
				int t1 = env.map.map[i, j];
				Rectangle dest = new Rectangle(i * td, j * td, td, td);
				if (t1 != -1) Raylib.DrawTexturePro(env.map.tileMap[t1], new Rectangle(0, 0, env.map.tileMap[t1].Width, env.map.tileMap[t1].Height), dest, Vector2.Zero, 0, Color.White);
				int t2 = env.map.secondLayer[i, j];
				if (t2 != -1) Raylib.DrawTexturePro(env.map.tileMap[t2], new Rectangle(0, 0, env.map.tileMap[t2].Width, env.map.tileMap[t2].Height), dest, Vector2.Zero, 0, Color.White);
			}
		}

		// Draw Buildings
		foreach (var b in env.map.buildings) {
			Raylib.DrawRectangle((int)b.X, (int)b.Y, (int)b.Width, (int)b.Height, Color.Gray);
		}

		// Draw Entities
		foreach (var npc in env.All.Entities.NPCs) {
			if (!npc.isDead) Raylib.DrawCircle((int)npc.r.X + 32, (int)npc.r.Y + 32, 32, Color.Red);
		}
		foreach (var v in env.All.Entities.Vehicles) {
			Raylib.DrawRectangle((int)v.r.X, (int)v.r.Y, 120, 120, Color.Blue);
		}

		// Player indicator
		Raylib.DrawCircle((int)playerPos.X, (int)playerPos.Y, 32, Color.White);
		
		Raylib.EndMode2D();
		Raylib.EndScissorMode();

		Raylib.DrawRectangleLinesEx(miniRect, 2, new Color((int)Color.SkyBlue.R, (int)Color.SkyBlue.G, (int)Color.SkyBlue.B, 255));
	}

	private void PrintHealth() {
		if (Game.camera.follow is not Player p) return;

		int width = 200;
		int height = 25;
		int padding = 20;
		Rectangle healthRect = new Rectangle(padding, padding + 200 + 10, width, height);

		Raylib.DrawRectangleRec(healthRect, new Color(0, 0, 0, 180));

		float healthPercent = (float)p.health / p.maxhealth;
		if (healthPercent < 0) healthPercent = 0;
		Raylib.DrawRectangle((int)healthRect.X, (int)healthRect.Y, (int)(width * healthPercent), height, Color.Red);
		Raylib.DrawRectangleLinesEx(healthRect, 2, Color.SkyBlue);

		string healthText = $"{p.health} / {p.maxhealth}";
		Raylib.DrawText(healthText, (int)(healthRect.X + 5), (int)(healthRect.Y + 2), 20, Color.White);
	}

	public void Update() {
		float offsetX = (windowWidth / 2f) - Game.camera.r.X;
    	float offsetY = (windowHeight / 2f) - Game.camera.r.Y;
		
		Camera2D cam = new Camera2D();
		cam.Offset = new Vector2(offsetX, offsetY);
		cam.Target = new Vector2(0, 0); // Offset handles the translation
		cam.Rotation = 0f;
		cam.Zoom = 1.0f;

		Raylib.BeginMode2D(cam);
		
		DrawMap(env.map);

		foreach (var prp in env.props) {
			Raylib.DrawTexturePro(prp.Texture2D, new Rectangle(0, 0, prp.Texture2D.Width, prp.Texture2D.Height), prp.r, Vector2.Zero, 0, Color.White);
		}
		
		foreach (var obj in env.All.Objects) DrawRotated(obj.Texture2D, obj.r, obj.rotation);
		
		if (env.crosshair.isOn) DrawCrosshair(env.crosshair);

		foreach (var obj in env.All.Entities.NPCs) DrawEntity(obj);
		foreach (var obj in env.All.Entities.Vehicles) DrawVehicle(obj);
		foreach (var obj in env.All.Entities.Players) DrawPlayer(obj);
		foreach (var obj in env.All.Items) DrawItem(obj);
		
		DrawBuildings(env.map.buildings);
		
		Raylib.EndMode2D();
		
		// UI
		PrintAmmo();
		PrintMinimap();
		PrintHealth();
		
		if (Game.isTurnBased) Game.turnManager.Draw(); // Assuming TurnManager.Draw is updated to Raylib too
		if (env.isDialoguePlaying) {
			Rectangle dest = new Rectangle(30, Game.windowHeight - 230, 200, 200);
         	Raylib.DrawTexturePro(env.dialogue.nowhead.frame, new Rectangle(0, 0, env.dialogue.nowhead.frame.Width, env.dialogue.nowhead.frame.Height), dest, Vector2.Zero, 0, Color.White);
		}
	}

	public Draw(Environment e, int windowWidth, int windowHeight) {
		this.env = e;
		this.windowWidth = windowWidth;
		this.windowHeight = windowHeight;
	}
}


