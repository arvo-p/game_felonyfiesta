public class Camera{
	
	public Object? follow;
	public RectangleF r{get => follow?.r ?? RectangleF.Empty;}

	public Camera(){}

	public void Follow(Object f){
		this.follow = f;
	}

	public PointF WorldToScreen(PointF worldPos)
	{
		float offsetX = (Game.windowWidth / 2) - r.X;
		float offsetY = (Game.windowHeight / 2) - r.Y;
		return new PointF(worldPos.X + offsetX, worldPos.Y + offsetY);
	}
}
