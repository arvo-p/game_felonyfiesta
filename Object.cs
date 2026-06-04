public class Object{
	protected Environment env = null!;
	
	public bool isSolid = true;
	public bool isVisible = true;

	public RectangleF r;
	public float X{get=>r.X;}
	public float Y{get=>r.Y;}
	public float Width{get=>r.Width;}
	public float Height{get=>r.Height;}
	public float speed;
	public int mass=0;
	protected float friction=0.7f;

	public List<Prop>? props = null;
	protected bool inverted_vectors=false;

	public PointF velRepulsion = new PointF(0,0);
	
	float _rotation;
	public float rotation{
		set{
			if(value < 0) _rotation =(value+360)%360;
			else _rotation = value%360;
		}
		get => _rotation;
	}

	PointF? _center=null;
	public PointF center{
		get{
			if(_center == null) PositionUpdated();
			return _center!.Value;
		}
	}

	public List<CollisionCircle> hitboxes = new List<CollisionCircle>();

	public Sprite _sprite = null!;
	public Sprite _spriteNext = null!;

	public virtual Sprite sprite{get => _sprite;}
    public Image image{get => sprite.frame;}

	public Object(){
	}
	
	public virtual void Update(){
	}

	public void Collision(Object obj){
	}

	public PointF movement = new PointF(0, 0);
	public virtual void UpdateRoutine(){
		Update();
	
		movement = Tools.Scalar2Vect_Speed(rotation, speed); 
		if(this.inverted_vectors) movement = Tools.SwapPointF(movement);
		
		movement.X += velRepulsion.X;
		movement.Y += velRepulsion.Y;

		if (Math.Abs(movement.X) > 0.1f || Math.Abs(movement.Y) > 0.1f) {
			env.Move(this, movement);
		}

		speed *= friction;
		velRepulsion.X *= friction;
		velRepulsion.Y *= friction;
	}

	public virtual void IsHit(float damage, float rotation){

	}

	public void PositionUpdated(){
		_center = new PointF(r.X+r.Width/2,r.Y+r.Height/2);
	}

	public void SetCollisionCircles(){
		float radius = r.Height>r.Width?r.Width/2:r.Height/2;
		float totalLength = r.Height>r.Width?r.Height:r.Width;
		float remainingLength = totalLength;
		
		int numCircles = (int)Math.Ceiling(r.Height/r.Width);
		Console.WriteLine("Creating " + numCircles + "collision circles for " + this.GetType().Name);

		for(int i=0;i<numCircles;i++){
			float offset = (i==numCircles-1)?totalLength-radius*2:(totalLength-remainingLength); 
			offset += -(totalLength/2) + radius;
			
			CollisionCircle newCC = new CollisionCircle(offset, radius, this);
			remainingLength -= radius*2;
			hitboxes.Add(newCC);
		}
	}

	public void TransferForce(PointF incomingVelocity, int incomingMass){
		float targetMass = this.mass > 0 ? this.mass : 50; // Default mass if not set
		float massRatio = (float)incomingMass / targetMass;
		
		// Clamp the ratio to prevent extreme physics glitches
		massRatio = Math.Clamp(massRatio, 0.1f, 5.0f);

		velRepulsion.X += incomingVelocity.X * massRatio;
		velRepulsion.Y += incomingVelocity.Y * massRatio;
	}
}
