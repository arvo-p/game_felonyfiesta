using System.Numerics;
using Raylib_cs;
public class CollisionCircle{

	private float _offset;
	public float offset{get => _offset;}

	float _radius;
	public float radius{get => _radius; set => _radius = value;}
	
	private Vector2 previousCenter;
	private Vector2 _center;
	public Vector2 center{
		set{
			previousCenter = _center;
			_center = value;
		}
		get => _center;
	}

	public Object parent;
	public Vector2 pcenter{get => parent.center;}
	public Rectangle pr{get => parent.r;}
	public float protation{get => parent.rotation;}

	public CollisionCircle(float offset, float radius, Object parent){
		this._offset = offset;
		this._radius = radius;
		this.parent = parent;
		
		_center = new Vector2(parent.r.X+radius,parent.r.Y+offset+parent.r.Height/2);
	}
	
	public static void UpdateCenters(List<CollisionCircle> l, Vector2 mov){
		foreach(var cc in l){
			cc._center = new Vector2(cc._center.X+mov.X,cc._center.Y+mov.Y);
		}
	}

	public void RollBackCenter(){
		
		_center = previousCenter;

		/*center = new Vector2(
				(float)(Math.Sin(-rot_radians)*offset) + pcenter.X,
				(float)(Math.Cos(rot_radians)*offset) + pcenter.Y);*/
	}
}


