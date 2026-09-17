using System.Numerics;
using Raylib_cs;
public static class Tools{
	public static bool IsLineIntersectingRect(Vector2 p1, Vector2 p2, Rectangle r){
		if (Raylib.CheckCollisionPointRec(p1, r) || Raylib.CheckCollisionPointRec(p2, r)) return true;

		return LineIntersectsLine(p1, p2, new Vector2(r.X, r.Y), new Vector2((r.X + r.Width), r.Y)) ||
			   LineIntersectsLine(p1, p2, new Vector2(r.X, (r.Y + r.Height)), new Vector2((r.X + r.Width), (r.Y + r.Height))) ||
			   LineIntersectsLine(p1, p2, new Vector2(r.X, r.Y), new Vector2(r.X, (r.Y + r.Height))) ||
			   LineIntersectsLine(p1, p2, new Vector2((r.X + r.Width), r.Y), new Vector2((r.X + r.Width), (r.Y + r.Height)));
	}

	public static float RandomFloat(float min, float max){
		float diff = max-min;
		return (Random.Shared.NextSingle() * (float)diff)+(float)min;
	}

	public static void Shuffle<T>(this IList<T> list){
		int n = list.Count;
		while (n > 1) {
			n--;
			int k = Game.rand.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static bool IsColliding(Rectangle a, Rectangle b){
		if (a.X + a.Width  < b.X) return false; 
		if (a.X > b.X + b.Width)  return false;
		if (a.Y + a.Height < b.Y) return false; 
		if (a.Y > b.Y + b.Height) return false;

		return true;
	}

	public static bool LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2){
		float d = (a2.X - a1.X) * (b2.Y - b1.Y) - (a2.Y - a1.Y) * (b2.X - b1.X);
		if (d == 0) return false; // Parallel lines

		float u = ((b1.X - a1.X) * (b2.Y - b1.Y) - (b1.Y - a1.Y) * (b2.X - b1.X)) / d;
		float v = ((b1.X - a1.X) * (a2.Y - a1.Y) - (b1.Y - a1.Y) * (a2.X - a1.X)) / d;

		return (u >= 0 && u <= 1) && (v >= 0 && v <= 1);
	}
	
	public static float GetDistance(Vector2 p1, Vector2 p2){
		return (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
	}
	
	public static float GetDistanceSquared(Vector2 p1, Vector2 p2){
		return (float)(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
	}
	
	public static float GetAngleDifference(float angle1, float angle2){
		float diff = (angle2 - angle1 + 180) % 360 - 180;
		return diff < -180 ? diff + 360 : diff;
	}

	public static bool IsCircleColliding(Vector2 center1, float radius1, Vector2 center2, float radius2){
		float combinedradius = radius1 + radius2;
		
		float dx = center1.X - center2.X;
		float dy = center1.Y - center2.Y;
		float distanceSquared = (dx * dx) + (dy * dy);

		return distanceSquared < (combinedradius * combinedradius);
	}

	public static Vector2 Scalar2Vect_Speed(float rot, float scalarspeed){
		double rot_radians = (rot)*0.0174533; 
		Vector2 speed = new Vector2(
			(float)(scalarspeed*Math.Cos(rot_radians)),
			(float)(scalarspeed*Math.Sin(rot_radians))
		);
		return speed;
	}

	public static bool IsCircleColliding(CollisionCircle obj1, CollisionCircle obj2){
		return IsCircleColliding(obj1.center, obj1.radius, obj2.center, obj2.radius); 
	}

	public static Vector2 SwapPointF(Vector2 p){
		float holder = p.X;
		p.X = -p.Y;
		p.Y = holder;

		return p;
	}

	// not used
	public static Vector2 RotateVector(Vector2 vector, float rotationDegrees){
		double radians = rotationDegrees * (Math.PI / 180.0);

		float cos = (float)Math.Cos(radians);
		float sin = (float)Math.Sin(radians);

		float newX = vector.X * cos - vector.Y * sin;
		float newY = vector.X * sin + vector.Y * cos;

		return new Vector2(newX, newY);
	}
}




