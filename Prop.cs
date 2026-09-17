using System.Numerics;
using Raylib_cs;
public class Prop{
	public Environment env = null!;
	public Sprite sprite;
	public Rectangle r;
	public float rotation;
	public int layer;

	public Texture2D Texture2D{get => sprite.frame;}

	public Prop(string[] resources, Rectangle r, float rot){
		this.env = Game.env;
		this.rotation = rot;
		this.sprite = new Sprite(resources,0,6,true);
		this.r = r;
	}
	
	/*public void UpdatePosition(float x, float y, float rot){
		r.X = x;
		r.Y = y;
		this.rotation = rot;
	}*/
}


