public class Entity : Object{
	
	public bool isAttacking = false;
	public bool isDead = false;
	public bool isDown = false;
	protected bool isStunned = false;
	protected bool doUpdateSprite = false;
	
	protected float maxspeed;

	// If entity is _inside_ a vehicle
	public Entity? inside = null;
	
	int _health;
	public int maxhealth;
	public int health{
		get => _health;
		set{
			if(value < 0){
				_health=0;
				if(isDead == false) doUpdateSprite = true;
				Die();
				DropItem();
			}
			else if(value > maxhealth) _health=maxhealth;
			else _health = value;
		}
	}

	protected DateTime isDownTime = DateTime.Now;

	public virtual void TryGetUp(){
		if((DateTime.Now - isDownTime).TotalSeconds >= 5)
			PutDown(false);
	}

	public virtual void PutDown(bool state){
     	doUpdateSprite = true;
		isDown = state;
		if(state) isDownTime = DateTime.Now;
	}

	public Entity? lastAttacker = null;

	public virtual void Die(){
		isDead = true;
		if (lastAttacker is Player p) {
			p.score += 5;
		}
		else if (lastAttacker is Vehicle v && v.passengers.Count > 0 && v.passengers[0] is Player p2) {
			p2.score += 5;
		}
	}
	
	public virtual void DropItem(){
	}

	public Object? HitscanCheck(PointF start, float range, float? angleOverride = null){
		float angle;
		if(angleOverride != null) angle = angleOverride.Value * 0.0174533f;
		else angle = this.rotation*0.0174533f;

		PointF targetPoint = start;
		for (float d = 0; d < range; d += 16){
			targetPoint.X = start.X + (float)Math.Cos(angle) * d;
			targetPoint.Y = start.Y + (float)Math.Sin(angle) * d;

			var r = env.map.GetTileFromCoordinates(targetPoint);
			if(r.Item1 == -1 || env.map.collision[r.Item1, r.Item2] == 1){
				break;  
			}
		}

		float closestDistance = float.MaxValue;
		Object? closestHit = null;

		foreach(var obj in env.All){
			if(obj == this) continue;
			if(obj is Entity ent) if(ent.isDead) continue;
			if(Tools.IsLineIntersectingRect(start, targetPoint, obj.r)){
				float dist = Tools.GetDistance(start, obj.center);
				if(dist < closestDistance){
					closestDistance = dist; 
					closestHit = obj;
				}
			}
		}

		Game.draw.DebugSetLine(start, targetPoint);
		if(closestHit != null){
			return closestHit;
		}

		return null;
	}

	public Entity(){
	}

	protected void setHealth(int health){
		this.maxhealth = health;
		this._health = health;
	}

	public override void IsHit(float damage, float rotation){
	}

	public override void IsHit(float damage, float rotation, Entity? attacker){
		lastAttacker = attacker;
		IsHit(damage, rotation);
	}
}
