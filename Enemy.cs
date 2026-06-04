
using System.Media;
public class Enemy : Entity{
	public Sprite walk = null!;
	public Sprite shoot = null!;  
	public Sprite stand = null!;
	public Sprite dead = null!;
 	
	protected List<NativeAudioPlayer> soundsDeath;

	protected Player local_player = null!;

	protected Pathfinding pathf = new Pathfinding();
	protected List<Point>? path = null;

	public bool isActionInProgress = false;
	internal float aiming_rotation;
	protected float aiming_error = 0;

	public float accuracy = 0.6f;
	public float tAttack = 1;
	public float tStun = 1;
	
	public bool hasPosTarget=false;
	public PointF posTarget = new PointF();
 
	protected List<PointF>? currentPath = null;
	DateTime dtRefreshPath=DateTime.Now;

	private int damage = 5;
	private float shootCooldown = 0;
	
	public Entity? GetNearestPlayer(){
		Player? nearest = null;
		float minDist = float.MaxValue;
		foreach(var p in env.players){
			if(p.isDead) continue;
			float d = Tools.GetDistanceSquared(this.center, p.center);
			if(d < minDist){
				minDist = d;
				nearest = p;
			}
		}
		if(nearest == null) return null;
		return nearest.inside == null ? nearest : nearest.inside;
	}

	public void UpdateAI(){
		Entity? target = GetNearestPlayer();
		if(target == null) return;

		if((DateTime.Now - dtRefreshPath).TotalSeconds > 1){
			dtRefreshPath = DateTime.Now;
			currentPath = pathf.FindPath((int)this.X, (int)this.Y, (int)target.X, (int)target.Y); 
		}

		PointF nextPoint = target.center; 
		if(currentPath != null && currentPath.Count > 0){
			nextPoint = currentPath[0];
		}
		
		PointF difference = new PointF(this.center.Y-nextPoint.Y,this.center.X-nextPoint.X);
		aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;

		if(currentPath != null && currentPath.Count > 0 && Math.Abs(this.X - nextPoint.X) < 40 && Math.Abs(this.Y-nextPoint.Y)<40){
			currentPath.RemoveAt(0);
		}
	}

	public void FaceTarget(){
		Entity? target = GetNearestPlayer();
		if(target == null) return;
		PointF difference = new PointF(this.center.Y-target.center.Y,this.center.X-target.center.X);
		aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;
		this.rotation = aiming_rotation;
	}

	public void ShootAction(){
		if(_sprite != shoot) _sprite = shoot;
		isActionInProgress = true;

		aiming_error = (float)((Game.rand.NextDouble() * 2 - 1) * 150 * (1.0f - accuracy));
		tAttack = 1;
		
		_sprite.Trigger();

		float shotAngle = aiming_rotation + aiming_error;
		Object? victim = HitscanCheck(this.center, 400, shotAngle);   
		if(victim != null && victim is Entity ent) ent.IsHit(damage, shotAngle, this);

		speed = 0;
	}

	public void Action(){
		UpdateAI();
		Entity? target = GetNearestPlayer();
		if(target == null) return;

		if(isActionInProgress) return;

		if(this.rotation-aiming_rotation > 180){
			this.rotation -= 180;
			speed *= -1;
		}

		if(isStunned == true){
			tStun+=-0.1f;
			if(tStun < 0) isStunned = false;
			else return;
		}
		
		float distance = Tools.GetDistanceSquared(target.r.Location, this.r.Location);
		if(distance > 90000){
			if(_sprite != walk) _sprite = walk;
			speed = Math.Clamp(speed + 1, -3, 3);
		}else{
			if(shootCooldown <= 0){
				ShootAction();
				shootCooldown = 1f; // Seconds between shots
			}
		}
	}

	public override void DropItem(){
		var rand = new Random();
		double roll = rand.NextDouble();
		if(roll < 0.4) { 
			env.All.Add(new ItemDrop(this.X, this.Y, ItemDropFootprints.SelectRandom()));
		} else if(roll < 0.6) { 
			env.All.Add(new ItemDrop(this.X, this.Y, ItemDropFootprints.SelectRandomGun()));
		}
	}

	public override void IsHit(float damage, float rotation){
		float radians = (float)(Math.PI/180)*rotation;

		if(isDead) return;
		_sprite = stand;
		
		health += -(int)damage;
		speed = -5;
		isStunned = true;
		tStun = 1;

		Blood.SprayBlood(r.Location, new PointF((float)Math.Cos(radians),(float)Math.Sin(radians)));
	}

	private Sprite UpdateSprite(){
		if(isDead) return dead;
		if(isDown) return dead;
		if(isActionInProgress) return shoot;
		if(Math.Abs(speed) > 0.2f) return walk;
		return stand;
	}
	
	DateTime? dtDead;
	public override void Update(){
		if(isDead){
			if(doUpdateSprite||dtDead==null){
				doUpdateSprite = false;
				_sprite = UpdateSprite();
				_sprite.Trigger();
				
                dtDead = DateTime.Now;
				
				if(soundsDeath != null)
			   	    soundsDeath[Game.rand.Next(0,soundsDeath.Count)].Play(); 
			}else if((DateTime.Now - dtDead.Value).TotalSeconds >= 10){
             	env.All.Remove(this);
			}
			return;
		}
		
		if(isDown){
			if(doUpdateSprite){
				doUpdateSprite = false;
				_sprite = UpdateSprite();
				_sprite.SetFrame(-1);
			}
			TryGetUp();
			return;
		}

		if(isActionInProgress){
			tAttack+=-0.052f;
			if(tAttack <= 0) isActionInProgress = false;
			else aiming_rotation += aiming_error;
		}

		if(shootCooldown > 0) shootCooldown -= 0.018f; // Roughly matches 18ms timer interval

		if(Game.isTurnBased){
			// Only update AI/Rotation if we are currently moving or it's our turn
			// (TurnManager handles the AP-based triggers)
			if(speed > 0.2f || Game.turnManager.IsActiveEnemy(this)){
				UpdateAI();
				float diff = Tools.GetAngleDifference(this.rotation, aiming_rotation);
				this.rotation += diff * 0.15f; 
			}
		}else{
			Action();
			this.rotation = aiming_rotation;
		}

		_sprite = UpdateSprite();
	}
	
	protected void Init(){
		this.env = Game.env;

		this._friction_turnbased = 0.82f; // Slightly less friction for smoother gliding

		r.Size = new Size(64, 64);
		setHealth(100);
		LoadSprites();
		_sprite = stand;

		mass = 90;

		PositionUpdated();
		SetCollisionCircles();
	}
	
	protected virtual void LoadSprites(){} 

	public Enemy(){

	}
}
