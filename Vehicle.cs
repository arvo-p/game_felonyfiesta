public class Vehicle : Entity{

	Keyboard? mykeyboard;
	List<Entity> passengers = new List<Entity>();
	bool owner = false;

	public float stretchFactor = 1.014f;
	public float squashFactor = 0.986f;
	public int isAccelerating = 0;
	public int isTurning = 0;

	public Prop shadow;

	public Vehicle(){
		this.env = Game.env;
		this.mass = 1000;
		this.friction = 0.94f;
		this.maxspeed = 23;
		this.inverted_vectors = true;
		
		_sprite = new Sprite(Resources.Vehicle._car);
		r.Location = new Point(600, 600);
		r.Size = new Size((int)(105-10), (int)(200-10));
		mass = 900;
		SetCollisionCircles();
		setHealth(100);

		props = new List<Prop>();
		shadow = new Prop(Resources.Vehicle._carshadow, new RectangleF(0, 0, r.Width, r.Height), this.rotation);
	}

	private void HandleInput(){
		if(mykeyboard == null) return;
		mykeyboard.ReadKeys();

		if(mykeyboard.GetKeyOnce(Keys.E)){
			LeaveCar();
			return;
		}
		
		if(owner == false) return;
        if(isDead == true) return;

		isTurning = 0;
		if(Math.Abs(speed) > 1){
			float speedFactor = Math.Abs(speed) / (maxspeed);
			float deltarot=0;
			if(mykeyboard.GetKey(Keys.Q)){
				isTurning = -1;
				deltarot = -2/((1-speedFactor*0.5f));
			}
			if(mykeyboard.GetKey(Keys.D)){
				isTurning = 1;
				deltarot = 2/((1-speedFactor*0.5f));
			}

			if(deltarot != 0){
				if(speed < 0) deltarot *= -1;
				env.Rotate(this, deltarot);
			}
		}
		
		isAccelerating = 0;
		if(mykeyboard.GetKey(Keys.Z)){
			if(speed < maxspeed - 0.3f) isAccelerating = 1;
			speed += 1.6f;
		}
		if(mykeyboard.GetKey(Keys.S)){
			if(speed > -maxspeed/2 + 0.3f) isAccelerating = -1;
			speed -= 1.6f;
		}

		speed = Math.Clamp(speed, -maxspeed/2, maxspeed);
	}

	public void MountPassenger(Entity entity, Keyboard? mykeyboard){
		if(passengers.Count() > 4) return;
		if(mykeyboard != null){
			this.mykeyboard = mykeyboard;
			this.owner = true;
		}
		this.passengers.Add(entity);
		Game.camera.Follow(this);
	}

	private void LeaveCar(){
		Entity passenger = passengers[0];
		if(passenger is Player p){
			p.isKeyboardOn = true;
			p.inside = null;
		}
		env.UpdatePosition(passenger, new PointF(r.X-64-passenger.X, r.Y-64-passenger.Y));
		Game.camera.Follow(passenger);
		
		env.All.Add(passenger);
		passengers.Remove(passenger);
		this.owner = false;
		this.mykeyboard = null;
	}

	int countSmoke = 0;
	public void ShockDamage(){
   		if(this.speed <= 6) return;
		Random rand = new Random();
        health -= (int)((speed * mass) / 900f);
		
	   float x = (float)(rand.NextDouble() * 40);
	   float y = (float)(rand.NextDouble() * 30) + 20;
        
		if(countSmoke++ < 5)
			props.Add(new Prop(Resources.Environments._smoke, new RectangleF(x, y, 64, 64), this.rotation+90));
	}

	public override void Update(){
		if(mykeyboard != null) HandleInput();
	}
}
