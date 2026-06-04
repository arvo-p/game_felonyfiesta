using System; 

public class ItemDrop : Object{
	private Func<Player, int, Type, bool>? Effect = null;
	
	public enum Type{
        Grenades,
	    Bullets,
		Batteries,
		Rockets,
		Shells,
		Smallbullets,
		Weapon,
	}	

	public int quantity = 0;
	public Type type;

	public ItemDrop(string srcSprite, Func<Player, int, Type, bool> f, Type type, int quantity){
		_sprite = new Sprite(srcSprite);
		this.Effect = f;
		r.Width = 48;
		r.Height = 48;
		this.type = type;
		this.quantity = quantity;
	}

	public ItemDrop(string[] srcSprites, Func<Player, int, Type, bool> f, Type type, int quantity){
		_sprite = new Sprite(srcSprites);
		this.Effect = f;
		r.Width = 48;
		r.Height = 48;
		this.type = type;
		this.quantity = quantity;
	}
	
	public bool TakeMe(Player p){
		if(Effect == null) return true; //todo: just store in inventory if it's null?
		return Effect.Invoke(p, quantity, type);
	}

	private DateTime startTime = DateTime.Now;
	public float floatY = 0;
	float float_speed = 3;
	float float_amplitude = 0.7f;

	public override void Update(){
		floatY = floatY+(float)Math.Sin((DateTime.Now-startTime).TotalSeconds * float_speed) * float_amplitude;
	}

	public ItemDrop(float x, float y, ItemDrop model){
		r.X = x + 8;
		r.Y = y + 8;
		r.Width = 48;
		r.Height = 48;
		this._sprite = model._sprite;
		this.Effect = model.Effect;
		this.type = model.type;
		this.quantity = model.quantity;
		SetCollisionCircles();
	}
}
