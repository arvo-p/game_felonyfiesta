using System.Numerics;
using Raylib_cs;
public class Camera{
	
	public Object? follow;
	public Rectangle r{get => follow?.r ?? new Rectangle(0, 0, 0, 0);}

	public Camera(){}

	public void Follow(Object f){
		this.follow = f;
	}

	public Vector2 WorldToScreen(Vector2 worldPos)
	{
		float offsetX = (Game.windowWidth / 2) - r.X;
		float offsetY = (Game.windowHeight / 2) - r.Y;
		return new Vector2(worldPos.X + offsetX, worldPos.Y + offsetY);
	}
}



