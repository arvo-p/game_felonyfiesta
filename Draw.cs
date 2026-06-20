using System.Drawing;
using System.Drawing.Drawing2D;

public class Draw{
	Environment env;
	int windowWidth;
	int windowHeight;

	// 3D
	private PointF sun = new PointF(2.8f, 3.0f);
	private float perspectiveFactor = 0.2f; 
	
	private Matrix cameraMatrix = new Matrix(); 
	private Matrix m = new Matrix();
	private Matrix w = new Matrix();

	private GraphicsState DrawRotatedBegin(PaintEventArgs e, Image image, RectangleF r, float rotation){
		var state = e.Graphics.Save();
		e.Graphics.TranslateTransform((float)(r.Width)/2+r.X, (float)r.Height / 2+r.Y);
		e.Graphics.RotateTransform(rotation);
		e.Graphics.DrawImage(image, -r.Width/2, -r.Height/2, r.Width, r.Height);
		return state;
	}

	private void DrawRotated(PaintEventArgs e, Image image, RectangleF r, float rotation){e.Graphics.Restore(DrawRotatedBegin(e, image, r, rotation));}

	private void DrawEntity(PaintEventArgs e, Entity ent){
		var state = DrawRotatedBegin(e, ent.image, ent.r, ent.rotation);
		if(ent.props != null)
			foreach(var p in ent.props)
				e.Graphics.DrawImage(p.image, -p.r.Width/2+p.r.X, -p.r.Height/2+p.r.Y, p.r.Width, p.r.Height);
		e.Graphics.Restore(state);
	}

	Sprite ammoPanel = new Sprite(Resources.UI._ammopanel);
	private void PrintAmmo(PaintEventArgs e){
		if(Game.camera.follow is not Player p) return;
		Graphics g = e.Graphics;

		RectangleF rPanel = new RectangleF(Game.windowWidth - ammoPanel.frame.Width * 1.4f - 5, 10, ammoPanel.frame.Width * 1.4f, ammoPanel.frame.Height * 1.4f);
		g.DrawImage(ammoPanel.frame, rPanel.X, rPanel.Y, rPanel.Width, rPanel.Height);

		using(Font font = new Font(Resources.Font._pfc.Families[0], 11, FontStyle.Bold))
			using(Brush brush = new SolidBrush(Color.Black)){
				string[] texts = {
					$"{p.inventory.bullets}",
					$"{p.inventory.shells}",
					$"{p.inventory.smallbullets}",
					$"{p.inventory.grenades}",
					$"{p.inventory.rockets}",
					$"{p.inventory.batteries}"
				};
				if(p.inventory != null){
					for(int i=0;i<texts.Count();i++)
						g.DrawString(texts[i], font, brush, rPanel.X + 25 - 17*(texts[i].Length-1)/2+86*(i%3), rPanel.Y + 45 + 45*(i/3));
				}
				if(p.selectedWeapon != null){
					if(p.selectedWeapon.icon != null)
					   g.DrawImage(p.selectedWeapon.icon.frame, rPanel.X + rPanel.Width - 105, 20, 65, 65);
					string text = $"-";
					if(p.selectedWeapon.type != Weapon.Type.Melee)
						text = $"{p.selectedWeapon.currentClip}";
					g.DrawString(text, font, brush, rPanel.X + 335 - 17*(text.Length-1)/2, rPanel.Y + 93);
				}
			}
	}
	
	private void DrawBuildings(PaintEventArgs e, List<Building> buildings){
		Graphics g = e.Graphics;
		RectangleF camera = Game.camera.r;
	
		int screenCX = windowWidth / 2;
		int screenCY = windowHeight / 2;
		
		RectangleF viewport = new RectangleF(camera.X - screenCX - 300, 
											 camera.Y - screenCY - 300, 
											 windowWidth + 500, 
											 windowHeight + 500);

		var visibleBuildings = buildings.Where(b => Tools.IsColliding(viewport, new RectangleF(b.X, b.Y, b.Width, b.Height))).ToList();

		var sortedBuildings = visibleBuildings.OrderByDescending(b => {
			float screenX = b.X - camera.X + screenCX;
			float screenY = b.Y - camera.Y + screenCY;
			double dx = screenX - screenCX;
			double dy = screenY - screenCY;
			return (dx * dx) + (dy * dy); 
    	}).ToList();
		
		foreach (var b in sortedBuildings){
			PointF worldBase = new PointF(b.X, b.Y);
			PointF distFromCam = new PointF(b.X - Game.camera.r.X, b.Y - Game.camera.r.Y);
			PointF offset = new PointF(
					distFromCam.X * perspectiveFactor * (b.Height3D / 60f),
				    distFromCam.Y * perspectiveFactor * (b.Height3D / 60f)
			);

			RectangleF baseRect = new RectangleF(worldBase.X, worldBase.Y , b.Width, b.Height);
			RectangleF roofRect = new RectangleF(worldBase.X + offset.X, worldBase.Y + offset.Y, b.Width, b.Height);
			PointF shadowOffset = new PointF(b.Height3D * sun.X, b.Height3D * sun.Y);

			PointF[] shadowPoints = {
				new PointF(worldBase.X, worldBase.Y),
				new PointF(worldBase.X + b.Width, worldBase.Y),
				new PointF(worldBase.X + b.Width + shadowOffset.X, worldBase.Y + shadowOffset.Y),
				new PointF(worldBase.X + shadowOffset.X, worldBase.Y + shadowOffset.Y)
			};

			using (Brush shadowBrush = new SolidBrush(Color.FromArgb(110, 0, 0, 0))){
				g.FillPolygon(shadowBrush, shadowPoints);
			}
			
			PointF[] leftWallPoints = {
				new PointF(roofRect.Left, roofRect.Top),      
				new PointF(roofRect.Left, roofRect.Bottom), 
				new PointF(baseRect.Left, baseRect.Bottom),
				new PointF(baseRect.Left, baseRect.Top)
			};
			PointF[] frontWallPoints = {
				new PointF(roofRect.Left, roofRect.Top),
				new PointF(roofRect.Right, roofRect.Top),
				new PointF(baseRect.Right, baseRect.Top),
				new PointF(baseRect.Left, baseRect.Top)
			};
			PointF[] rightWallPoints = {
				new PointF(roofRect.Right, roofRect.Top), 
				new PointF(roofRect.Right, roofRect.Bottom),
				new PointF(baseRect.Right, baseRect.Bottom),
				new PointF(baseRect.Right, baseRect.Top)
			};
			PointF[] botWallPoints = {
				new PointF(roofRect.Left, roofRect.Bottom), 
				new PointF(roofRect.Right, roofRect.Bottom),
				new PointF(baseRect.Right, baseRect.Bottom),
				new PointF(baseRect.Left, baseRect.Bottom)
			};
		
			RectangleF srcRect;
			using (Brush shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0))){
				if(roofRect.Left > baseRect.Left){
					srcRect = new RectangleF(0, 0, b.Lwall.frame.Width, b.Lwall.frame.Height);
					g.DrawImage(b.Lwall.frame, new PointF[]{leftWallPoints[0], leftWallPoints[1], leftWallPoints[3]}, srcRect, GraphicsUnit.Pixel);
				}
				if(roofRect.Top > baseRect.Top){
					srcRect = new RectangleF(0, 0, b.Uwall.frame.Width, b.Uwall.frame.Height);
					g.DrawImage(b.Uwall.frame, new PointF[]{frontWallPoints[0], frontWallPoints[1], frontWallPoints[3]}, srcRect, GraphicsUnit.Pixel);
				}
				if(roofRect.Right < baseRect.Right){
					srcRect = new RectangleF(0, 0, b.Rwall.frame.Width, b.Rwall.frame.Height);
					g.DrawImage(b.Rwall.frame, new PointF[]{rightWallPoints[0], rightWallPoints[1], rightWallPoints[3]}, srcRect, GraphicsUnit.Pixel);
					g.FillPolygon(shadowBrush, rightWallPoints);
				}
				if(roofRect.Bottom < baseRect.Bottom){
					srcRect = new RectangleF(0, 0, b.Lwall.frame.Width, b.Lwall.frame.Height);
					g.DrawImage(b.Lwall.frame, new PointF[]{botWallPoints[0], botWallPoints[1], botWallPoints[3]}, srcRect, GraphicsUnit.Pixel);
					g.FillPolygon(shadowBrush, botWallPoints);
				}
			}

			float ledgeThickness = 5f;
			ledgeThickness = 2 + (b.Height3D / 10);
			RectangleF innerRoof = new RectangleF(
				roofRect.X + ledgeThickness, 
				roofRect.Y + ledgeThickness, 
				roofRect.Width - (ledgeThickness * 2), 
				roofRect.Height - (ledgeThickness * 2)
			);
	
			g.FillRectangle(Brushes.Gray, roofRect);
			
			if(b.roof != null){
				float viewPct = 0.7f; 
				float srcW = b.roof.frame.Width * viewPct;
				float srcH = b.roof.frame.Height * viewPct;

				float maxShiftX = b.roof.frame.Width - srcW + 0.1f;
				float maxShiftY = b.roof.frame.Height - srcH + 0.1f;

				float pctX = Math.Clamp((roofRect.X / windowWidth), 0, 1);
				float pctY = Math.Clamp((roofRect.Y / windowHeight), 0, 1);

				RectangleF manualSrcRect = new RectangleF(
					pctX * maxShiftX, 
					pctY * maxShiftY, 
					srcW, 
					srcH
				);

				g.DrawImage(b.roof.frame, innerRoof, manualSrcRect, GraphicsUnit.Pixel);
			}

			using(Pen darkPen = new Pen(Color.FromArgb(120, 0, 0, 0), 2)){
				g.DrawLine(darkPen, innerRoof.Left, innerRoof.Top, innerRoof.Right, innerRoof.Top);
				g.DrawLine(darkPen, innerRoof.Left, innerRoof.Top, innerRoof.Left, innerRoof.Bottom);
			}
		}
	}

	private void DrawVehicle(PaintEventArgs e, Vehicle ent){
		var initialState = e.Graphics.Save();
		
		RectangleF r = ent.r;
		float fakeHeight = ent.speed * 0.16f;
		float offset = 0;

		if(ent.isTurning != 0) offset = ent.isTurning==1?-8:6;
		if(ent.isAccelerating==1&&ent.speed>0.2){
			r.Height *= ent.stretchFactor;
			r.Width *= ent.squashFactor;
		}
		if((ent.isAccelerating==-1&&ent.speed>0.2)||(ent.isAccelerating==1&&ent.speed<(-0.2))){
			r.Width *= ent.stretchFactor;
			r.Height *= ent.squashFactor;
		}
		
		var shadowState = e.Graphics.Save();
		w.Reset();
		w.Translate(r.Width/2f+r.X, r.Height/2f+r.Y);
		w.Rotate(ent.rotation);
		e.Graphics.MultiplyTransform(w); 
		e.Graphics.DrawImage(ent.shadow.image, -r.Width/2+offset+fakeHeight, -r.Height/2+fakeHeight, ent.r.Width, ent.r.Height);
		e.Graphics.Restore(shadowState);

		m.Reset();
		m.Translate(r.Width/2f+r.X, r.Height/2f + r.Y);
		m.Rotate(ent.rotation);
		e.Graphics.MultiplyTransform(m); 
		e.Graphics.DrawImage(ent.image, -r.Width/2, -r.Height/2, r.Width, r.Height);
		
		if(ent.props != null)
			foreach(var p in ent.props)
				e.Graphics.DrawImage(p.image, -p.r.Width/2+p.r.X, -p.r.Height/2+p.r.Y, p.r.Width, p.r.Height);

		e.Graphics.Restore(initialState);
	}

	DateTime startTime = DateTime.Now;
	private void DrawPlayer(PaintEventArgs e, Player player){
		Graphics g = e.Graphics;
		DrawRotated(e, player.image, player.r, player.rotation);

		Weapon? weapon = player.selectedWeapon;
		PointF weaponMovementVect = player.r.Location;
		if(weapon == null) return;
		if(player.speed > 0.2){
			float weaponMovement = (float)Math.Sin((DateTime.Now-startTime).TotalSeconds * 20) * 4f;
			var add = Tools.Scalar2Vect_Speed(player.rotation, weaponMovement);	
			weaponMovementVect.X += add.X;
			weaponMovementVect.Y += add.Y;
		}
		DrawRotated(e, weapon.sprite.frame, new RectangleF(weaponMovementVect.X, weaponMovementVect.Y, player.Width, player.Height), player.rotation);
	}
	
	private void DrawCrosshair(PaintEventArgs e, Crosshair c){
		e.Graphics.DrawImage(c.image, c.r.X, c.r.Y, c.r.Width, c.r.Height);
	}
	
	private void DrawItem(PaintEventArgs e, ItemDrop item){
		float floatHeight = item.floatY;
		PointF ground = new PointF(item.X, item.Y+item.Height);
		PointF offset = new PointF((ground.X - Game.camera.r.X) * perspectiveFactor * 0.5f, (ground.Y - Game.camera.r.Y) * perspectiveFactor * 0.5f);
		PointF shadow = new PointF(ground.X + offset.X + (floatHeight * sun.X * 0.2f), ground.Y + offset.Y + (floatHeight * sun.Y * 0.2f));

		float shadowScale = Math.Max(0.5f, 1.0f - (floatHeight / 100f));
		int shadowAlpha = (int)Math.Max(0, 110 - (floatHeight * 2));

		float sW = item.Width * shadowScale;
		float sH = (item.Height / 2f) * shadowScale;
		
		GraphicsPath shadowPath = new GraphicsPath();
		shadowPath.AddEllipse(shadow.X - (sW / 2), shadow.Y - (sH / 2), sW, sH);

		using (PathGradientBrush pgb = new PathGradientBrush(shadowPath)) {
			pgb.CenterColor = Color.FromArgb(shadowAlpha, 0, 0, 0);
			pgb.SurroundColors = new Color[] { Color.FromArgb(0, 0, 0, 0) };
			pgb.CenterPoint = new PointF(shadow.X, shadow.Y);
			e.Graphics.FillPath(pgb, shadowPath);
		}

		e.Graphics.DrawImage(item.image, item.X, item.Y+item.floatY, item.Width, item.Height);
	}

	private void DrawMap(PaintEventArgs e, Map map){
		int currentI = (int)Math.Floor(Game.camera.r.X/(Game.windowWidth));
		int currentJ = (int)Math.Floor(Game.camera.r.Y/(Game.windowHeight));
		
		for(int i=currentI-1; i<currentI+2;i++){
			if(i < 0 || i > map.gmap.GetLength(0) - 1) continue;
			for(int j=currentJ-1; j<currentJ+2;j++){
				if(j < 0 || j > map.gmap.GetLength(1) - 1) continue;
				e.Graphics.DrawImageUnscaled(map.gmap[i,j], i*Game.windowWidth, j*Game.windowHeight); 
			}
		}
	}

	PointF start, end;
	public void DebugSetLine(PointF start, PointF end){
		this.start = start;
		this.end = end;
	}

	private void PrintMinimap(PaintEventArgs e) {
		if (Game.camera.follow == null) return;
		Graphics g = e.Graphics;

		int size = 200;
		int padding = 20;
		RectangleF miniRect = new RectangleF(padding, padding, size, size);

		// Background and Border
		g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)), miniRect);
		
		var state = g.Save();
		g.SetClip(miniRect);

		float scale = 0.15f; // Zoom level
		PointF playerPos = Game.camera.follow.center;

		// Transform to minimap space
		g.TranslateTransform(padding + size / 2, padding + size / 2);
		g.ScaleTransform(scale, scale);
		g.TranslateTransform(-playerPos.X, -playerPos.Y);

		// Draw Tiles
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
				if (t1 != -1) g.DrawImage(env.map.tileMap[t1], i * td, j * td, td + 1, td + 1);
				int t2 = env.map.secondLayer[i, j];
				if (t2 != -1) g.DrawImage(env.map.tileMap[t2], i * td, j * td, td + 1, td + 1);
			}
		}

		// Draw Buildings
		foreach (var b in env.map.buildings) {
			g.FillRectangle(Brushes.Gray, b.X, b.Y, b.Width, b.Height);
		}

		// Draw Entities
		foreach (var npc in env.All.Entities.NPCs) if (!npc.isDead) g.FillEllipse(Brushes.Red, npc.r.X, npc.r.Y, 64, 64);
		foreach (var v in env.All.Entities.Vehicles) g.FillRectangle(Brushes.Blue, v.r.X, v.r.Y, 120, 120);

		// Player indicator
		g.FillEllipse(Brushes.White, playerPos.X - 32, playerPos.Y - 32, 64, 64);

		g.Restore(state);
		g.DrawRectangle(new Pen(Color.Cyan, 2), padding, padding, size, size);
	}

	private void PrintHealth(PaintEventArgs e) {
		if (Game.camera.follow is not Player p) return;
		Graphics g = e.Graphics;

		int width = 200;
		int height = 25;
		int padding = 20;
		RectangleF healthRect = new RectangleF(padding, padding + 200 + 10, width, height);

		g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)), healthRect);

		float healthPercent = (float)p.health / p.maxhealth;
		if (healthPercent < 0) healthPercent = 0;
		g.FillRectangle(Brushes.Crimson, healthRect.X, healthRect.Y, width * healthPercent, height);

		g.DrawRectangle(new Pen(Color.Cyan, 2), healthRect.X, healthRect.Y, width, height);

		using (Font font = new Font(Resources.Font._pfc.Families[0], 10, FontStyle.Bold))
		using (Brush brush = new SolidBrush(Color.White)) {
			string healthText = $"{p.health} / {p.maxhealth}";
			g.DrawString(healthText, font, brush, healthRect.X + 5, healthRect.Y + 2);
		}
	}

	public void Update(PaintEventArgs e){
		e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
		e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
	
		float offsetX = (windowWidth / 2) - Game.camera.r.X;
    	float offsetY = (windowHeight / 2) - Game.camera.r.Y;
		
		cameraMatrix.Reset();
		cameraMatrix.Translate(offsetX, offsetY);
		e.Graphics.Transform = cameraMatrix;
		
		DrawMap(e, env.map);

		foreach(var prp in env.props) e.Graphics.DrawImage(prp.image, prp.r.X, prp.r.Y, prp.r.Width, prp.r.Height);
		foreach(var obj in env.All.Objects) DrawRotated(e, obj.image, obj.r, obj.rotation);
		
		if(env.crosshair.isOn) DrawCrosshair(e, env.crosshair);

		foreach(var obj in env.All.Entities.NPCs) DrawEntity(e, obj);
		foreach(var obj in env.All.Entities.Vehicles) DrawVehicle(e, obj);
		foreach(var obj in env.All.Entities.Players) DrawPlayer(e, obj);
		foreach(var obj in env.All.Items) DrawItem(e, obj);
		
		DrawBuildings(e, env.map.buildings);
		
		cameraMatrix.Reset();
		e.Graphics.Transform = cameraMatrix;
		// UI
		
		PrintAmmo(e);
		PrintMinimap(e);
		PrintHealth(e);
		if (Game.isTurnBased) Game.turnManager.Draw(e.Graphics);
		if(env.isDialoguePlaying){
         	e.Graphics.DrawImage(env.dialogue.nowhead.frame, 30, Game.windowHeight-230, 200, 200);
		}

		/*
		 * DEBUG collision
		Pen myPen = new Pen(Color.Red);
        myPen.Width = 8;
		foreach(var ent in env.entities){
			foreach(var hit in ent.hitboxes){
				e.Graphics.DrawEllipse(myPen, hit.center.X-hit.radius, hit.center.Y-hit.radius, hit.radius*2, hit.radius*2); 
				float diffHeight = hit.offset;
        		e.Graphics.DrawLine(myPen, ent.r.X+ent.r.Width/2, ent.r.Y+ent.r.Height/2, ent.r.X+ent.r.Width/2, diffHeight+ent.r.Y+ent.r.Height/2);
			}
		}*/
	}

	public Draw(Environment e, int windowWidth, int windowHeight){
		this.env = e;
		this.windowWidth = windowWidth;
		this.windowHeight = windowHeight;
	}
}
