
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

	protected bool isActionInProgress = false;
	protected float aiming_rotation;
	protected float aiming_error = 0;

	public float accuracy = 0.8f;
	public float tAttack = 1;
	public float tStun = 1;
	
	public bool hasPosTarget=false;
	public PointF posTarget = new PointF();
 
	protected List<PointF>? currentPath = null;
	DateTime dtRefreshPath=DateTime.Now;

	private int damage = 10;
	
	public void Action(){
		Entity target = local_player.inside==null?local_player:local_player.inside;

		if((dtRefreshPath-DateTime.Now).TotalSeconds > 1){
			dtRefreshPath = DateTime.Now;
			currentPath = pathf.FindPath((int)this.X, (int)this.Y, (int)target.X, (int)target.Y); 
		}

		PointF nextPoint = new PointF(target.X,target.Y); 
		if(currentPath != null && currentPath.Count > 0){
			nextPoint = currentPath[0];
		}
		
		PointF difference = new PointF(this.Y-nextPoint.Y,this.X-nextPoint.X);
		aiming_rotation = ((float)Math.Atan2(difference.X, difference.Y)*180f)/3.14f+180;

		if(this.rotation-aiming_rotation > 180){
			this.rotation -= 180;
			speed *= -1;
		}

		if(isStunned == true){
			tStun+=-0.1f;
			if(tStun < 0) isStunned = false;
			else return;
		}
		
		if(isActionInProgress){
			tAttack+=-0.052f;
			if(tAttack <= 0) isActionInProgress = false;
			else {
				aiming_rotation += aiming_error;
				return;
			}
		}
		
		float distance = Tools.GetDistanceSquared(target.r.Location, this.r.Location);
		if(distance > 90000){
			if(_sprite != walk) _sprite = walk;
			speed = Math.Clamp(speed + 1, -3, 3);
			if(currentPath != null && currentPath.Count > 0 && Math.Abs(this.X - nextPoint.X) < 40 && Math.Abs(this.Y-nextPoint.Y)<40){
				currentPath.RemoveAt(0);
			}
		}else{
			if(_sprite != shoot) _sprite = shoot;
			isActionInProgress = true;

			aiming_error = (float)((Game.rand.NextDouble() * 2 - 1) * 30 * (1.0f - accuracy));
			aiming_rotation += aiming_error;

			tAttack = 1;
			
			_sprite.Trigger();

			Object? victim = HitscanCheck(this.center, 400);   
			if(victim != null && victim is Entity ent) ent.IsHit(damage, aiming_rotation);

			speed = 0;
		}
	}

	public override void DropItem(){
		var rand = new Random();
		if(rand.Next(0, 6)==0)
			env.All.Add(new ItemDrop(this.X, this.Y, ItemDropFootprints.SelectRandom()));
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
	
	public void ActionFormation(){
		// TO-DO
		// Maybe later
	}

	public override void Update(){
		if(isDead){
			if(doUpdateSprite){
				doUpdateSprite = false;
				_sprite = UpdateSprite();
				_sprite.Trigger();
				
				if(soundsDeath != null)
			   	    soundsDeath[Game.rand.Next(0,soundsDeath.Count)].Play(); 
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

		_sprite = UpdateSprite();

 		Action();

		this.rotation = aiming_rotation;
	}
	
	protected void Init(){
		this.env = Game.env;
		this.local_player = env.p;

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
