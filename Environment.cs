using System.Drawing.Drawing2D;

public class Environment{

	public WeaponFootprints weaponFootprints = new WeaponFootprints();
	public Map map;
	public List<Player> players = new List<Player>();
	public Crosshair crosshair;
	public LevelManager levelManager;

	public ObjectsManager All = new ObjectsManager();
	public List<Prop> props = new List<Prop>();

	public bool isDialoguePlaying{get=>levelManager.isDialoguePlaying;}  
	public Dialogue dialogue{get=>levelManager.dia;}

	public Environment(int playerCount = 1){
		Game.env = this;
		map = new Map(new string[]{"Resources/Tiles/test_Background.csv","Resources/Tiles/test_Decorations.csv",
				"Resources/Tiles/test_Collisions.csv","Resources/Tiles/test_Buildings.csv"}, "Resources/Tiles/tilesheet.png");

		crosshair = new Crosshair(Resources.UI._crosshair1,Resources.UI._crosshair2);
		levelManager = new LevelManager();
		
		// Initial spawn point for the squad
		PointF baseSpawn = levelManager.SpawnCoordinates(new PointF(2000, 2000), 1000);

		for(int i = 0; i < playerCount; i++){
			Player p = new Player(crosshair);

			PointF pSpawn = levelManager.SpawnCoordinates(baseSpawn, 150);
			p.r.Location = pSpawn;
			p.PositionUpdated();
			p.SetCollisionCircles();

			players.Add(p);
			All.Add(p);
		}

		if(players.Count > 0) Game.camera.Follow(players[0]);

		All.Add(new Vehicle());
	}
	
	public void Update(){
		levelManager.Update();
		
		foreach(var obj in All.ToList()) obj.UpdateRoutine();
		if(props.Count() > 200) props.RemoveAt(0);

		if (players.Count > 0 && players.All(p => p.isDead)) {
			if (Game.activeState == Game.State.Playing) {
				Game.activeState = Game.State.GameOver;
			}
		}
	}

	private const float DegToRad = MathF.PI / 180f;

	private int ResolveRectangleTileCollision(RectangleF rect){
		// Check the four corners with a small inner padding to avoid edge snagging
		const float padding = 8f;
		PointF[] pointsToCheck = {
			new PointF(rect.Left + padding, rect.Top + padding),
			new PointF(rect.Left + padding, rect.Bottom - padding),
			new PointF(rect.Right - padding, rect.Top + padding),
			new PointF(rect.Right - padding, rect.Bottom - padding)
		};
		
		foreach(var point in pointsToCheck){
			var tile = map.GetTileFromCoordinates(point);
			if(tile.x == -1 || map.collision[tile.x, tile.y] == 1) return 1;
		}	

		return 0;
	}

	public PointF CheckRectangleTileCollision(CollisionCircle cc, PointF movement){
		float size = cc.radius * 2;
		
		// Separate X and Y checks allow for sliding along walls
		RectangleF futureX = new RectangleF(cc.center.X - cc.radius + movement.X, cc.center.Y - cc.radius, size, size);
		if(ResolveRectangleTileCollision(futureX) == 1) movement.X = 0;

		RectangleF futureY = new RectangleF(cc.center.X - cc.radius, cc.center.Y - cc.radius + movement.Y, size, size);
		if(ResolveRectangleTileCollision(futureY) == 1) movement.Y = 0; 
		
		return movement;
	}

	private bool ResolveCircleTileCollision(CollisionCircle cc, int tilex, int tiley, int mode){
		return ResolveCircleTileCollision(cc.center, cc.radius, tilex, tiley, cc.parent, mode);
	}

	private bool ResolveCircleTileCollision(PointF center, float radius, int tilex, int tiley, Object parent, int mode){
		int tileSize = map.tileRenderDimension;
		
		PointF closest = new PointF(
			Math.Clamp(center.X, tileSize * tilex, tileSize * (tilex + 1)), 
			Math.Clamp(center.Y, tileSize * tiley, tileSize * (tiley + 1))
		);
		
		PointF offset = new PointF(center.X - closest.X, center.Y - closest.Y);
		float distSq = offset.X * offset.X + offset.Y * offset.Y;

		// Handle cases where center is exactly on the edge/corner
		if(distSq == 0) {
			offset = new PointF(0, -1); 
			distSq = 1;
		}

		if(distSq < radius * radius){
			if(parent == null) return true;
			
			var overlapInfo = GetCirclesCollisionOverlap(offset, distSq, radius, 0); 
			PointF pushOut = new PointF(overlapInfo.direction.X * (overlapInfo.overlap + 0.2f), overlapInfo.direction.Y * (overlapInfo.overlap + 0.2f));
			
			/* 
			 * Mode 0 handles translation
			 * Mode 1 handles rotation
			 */
			if(mode == 0){ 
				/*
				 * WallCollisionEvents is called before any correction
				 * to the entity's position or velocity.
				 */
				WallCollisionEvents(parent); 
				parent.speed *= 0.85f; // Gentle slowdown instead of aggressive bounce
				UpdatePosition(parent, pushOut);
			}
			else if(mode == 1){ 
				float localX = closest.X - center.X;
				float localY = closest.Y - center.Y;
				// 2D Cross product (torque): r x F
				float torque = localX * pushOut.Y - localY * pushOut.X;

				float rotationStep = (overlapInfo.overlap / radius) * DegToRad;
				parent.rotation += (torque > 0) ? rotationStep : -rotationStep;
				
				UpdatePosition(parent, pushOut);
			}

			return true;
		}

		return false;
	}

	public bool CheckCircleTileCollision(CollisionCircle cc, PointF mov, int mode){
		float tileSize = map.tileRenderDimension;
		
		// Determine tile range to check
		int startX = (int)((cc.center.X + mov.X - cc.radius) / tileSize);
		int startY = (int)((cc.center.Y + mov.Y - cc.radius) / tileSize);
		int endX = (int)((cc.center.X + mov.X + cc.radius) / tileSize);
		int endY = (int)((cc.center.Y + mov.Y + cc.radius) / tileSize);
		
		int mapW = map.collision.GetLength(0);
		int mapH = map.collision.GetLength(1);

		for(int x = startX; x <= endX; x++){
			if(x < 0 || x >= mapW) continue;
			for(int y = startY; y <= endY; y++){
				if(y < 0 || y >= mapH) continue;
				if(map.collision[x, y] == 1 && ResolveCircleTileCollision(cc, x, y, mode)) return true;
			}
		}

		return false;
	}

	private (float overlap, PointF direction, float distance) GetCirclesCollisionOverlap(PointF direction, float distSq, float r1, float r2){
		float distance = MathF.Sqrt(distSq); 
		
		if (distance == 0){
			direction = new PointF(1, 0);
			distance = 1;
		}
		
		direction.X /= distance;
		direction.Y /= distance;
		
		float overlap = (r1 + r2) - distance;
		return (overlap, direction, distance);
	}
 
	private (float overlap, PointF direction) GetCirclesCollisionOverlap(CollisionCircle h1, CollisionCircle h2){
		PointF diff = new PointF(h1.center.X - h2.center.X, h1.center.Y - h2.center.Y);
		float distSq = diff.X * diff.X + diff.Y * diff.Y; 
		var r = GetCirclesCollisionOverlap(diff, distSq, h1.radius, h2.radius);
		return (r.overlap, r.direction);
	}

  	private void RunOverEntity(Vehicle vehicle, Entity entity){
         entity.PutDown(true);
         entity.IsHit((int)(Math.Abs(vehicle.speed) * 2f), vehicle.rotation, vehicle);
    
         float radians = (vehicle.rotation+45) * DegToRad;
         PointF direction = new PointF(MathF.Cos(radians), MathF.Sin(radians));
         entity.TransferForce(new PointF(direction.X * vehicle.speed * 0.5f, direction.Y * vehicle.speed * 0.5f), vehicle.mass);
         Blood.SprayBlood(entity.center, direction);
    
         vehicle.speed *= 0.9f;
     }
	
	public void UpdatePosition(Object target, PointF movement){
		target.r.X += movement.X;
		target.r.Y += movement.Y;

		target.PositionUpdated();
		CollisionCircle.UpdateCenters(target.hitboxes, movement); 
	}

	public Object? IsObjectColliding(Object sourceObject, float padding, Func<Object, CollisionCircle, CollisionCircle, int, int>? onCollision, int mode) {
		foreach (var other in All.ToList()) {
			if (ReferenceEquals(other, sourceObject)) continue;

			// Skip entities that shouldn't block movement (dead or run over by vehicles)
			if (other is Entity entity){
				if (entity.isDead & entity is not Vehicle) continue;
				if (sourceObject is Vehicle vehi) if(entity.isDown) continue;
			}

			// Ignore objects further than 300 units (300^2 = 90000)
			float dx = sourceObject.X - other.X;
			float dy = sourceObject.Y - other.Y;
			if (dx * dx + dy * dy >= 90000f) continue;

			foreach (var hitbox1 in sourceObject.hitboxes){
				foreach (var hitbox2 in other.hitboxes){
					if(!Tools.IsCircleColliding(hitbox1.center, hitbox1.radius + padding, hitbox2.center, hitbox2.radius)){
						continue;
					}

					if(other is Entity ent && sourceObject is Vehicle veh){
						if(Math.Abs(veh.speed) > 12){
							RunOverEntity((Vehicle)sourceObject, ent);	
						}
					}
					
					// Process item pickups without blocking movement
					if (other is ItemDrop item){
						if (sourceObject is Player player) player.TakeItem(item);
						continue;
					}

					onCollision?.Invoke(sourceObject, hitbox1, hitbox2, mode);
					return other;
				}
			}
		}

		return null;
	}

	private int AdjustCirclesOverlap(Object obj, CollisionCircle h1, CollisionCircle h2, int mode){
		var r = GetCirclesCollisionOverlap(h1, h2);
		PointF push = new PointF(r.direction.X * (r.overlap + 0.01f), r.direction.Y * (r.overlap + 0.01f));

		if(mode == 1){ // rotation torque calculation
			float localX = h1.center.X - obj.X;
			float localY = h1.center.Y - obj.Y;
			float torque = localX * push.Y - localY * push.X;
			
			float rotationStep = (r.overlap / h1.radius) * DegToRad;
			if(torque > 0) h1.parent.rotation += rotationStep;
			else h1.parent.rotation -= rotationStep;
		}
		
		UpdatePosition(obj, push);

		// Maybe try using object's TransferForce function instead, this would
		// take mass into account.
		if(h2.parent is not Vehicle)
			Move(h2.parent, new PointF(-push.X, -push.Y)); 
		else
			Move(h1.parent, new PointF(push.X, push.Y));
		
		return 0;
	}

	public void Rotate(Object obj, float addRotation){
		int hitboxCount = obj.hitboxes.Count;
		float baseRotation = obj.rotation;
		float newRotation = baseRotation + addRotation;
		
		if(hitboxCount == 1){
			obj.rotation = newRotation;
			return;
		}

		List<PointF> centers = new List<PointF>();
		for(int i = 0; i < hitboxCount; i++){
			CollisionCircle hitbox = obj.hitboxes[i];
			float diffHeight = hitbox.offset;
			double rot_radians = (obj.rotation) * DegToRad; 
			
			PointF rotatedcoords = new PointF(
				((float)(Math.Sin(-rot_radians) * diffHeight) + hitbox.pcenter.X),
				((float)(Math.Cos(rot_radians) * diffHeight) + hitbox.pcenter.Y)
			);
		
			if(CheckCircleTileCollision(hitbox, new PointF(rotatedcoords.X - hitbox.center.X, rotatedcoords.Y - hitbox.center.Y), 1)) return; 
			centers.Add(rotatedcoords);
		}
		
		if(IsObjectColliding(obj, 0, AdjustCirclesOverlap, 1) != null) return;
		obj.rotation = newRotation;
		for(int i = 0; i < hitboxCount; i++) obj.hitboxes[i].center = centers[i];
	}
	
	public void WallCollisionEvents(Object obj){
    	if(obj is Vehicle v) v.ShockDamage();
	}

	public void Move(Object obj, PointF movement){
		int countHitboxes = obj.hitboxes.Count;

		if(countHitboxes == 1) {
			movement = CheckRectangleTileCollision(obj.hitboxes[0], movement);
		} else {
			// Separate X and Y checks allow for sliding along walls
			PointF moveX = new PointF(movement.X, 0);
			foreach(var cc in obj.hitboxes) {
				if(CheckCircleTileCollision(cc, moveX, 0)) {
					movement.X = 0;
					break;
				}
			}

			PointF moveY = new PointF(0, movement.Y);
			foreach(var cc in obj.hitboxes) {
				if(CheckCircleTileCollision(cc, moveY, 0)) {
					movement.Y = 0;
					break;
				}
			}
		}
		
		if(movement.X == 0 && movement.Y == 0) return;
		
		if(obj.isSolid == true) IsObjectColliding(obj, 0, AdjustCirclesOverlap, 0);

		obj.r.Y += movement.Y;
		obj.r.X += movement.X;
		obj.PositionUpdated();
		CollisionCircle.UpdateCenters(obj.hitboxes, movement); 

		return;
	}
}

