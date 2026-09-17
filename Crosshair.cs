using System.Numerics;
using Raylib_cs;
public class Crosshair{
	public bool isOn = false;
	public Rectangle r;
	
	bool hasTarget=false;
	public bool isLockedOnTarget=false;

	Vector2 target;
	
	Sprite onTarget;
	Sprite defaultC;

	Sprite sprite;

	public Texture2D Texture2D{get => sprite.frame;}
	
	public Crosshair(string[] crosshair1, string[] crosshair2){
		r.Width = 70; r.Height = 70;
		r.X = (new Vector2(20, 20)).X; r.Y = (new Vector2(20, 20)).Y;

		defaultC = new Sprite(crosshair1);
		onTarget = new Sprite(crosshair2);
		
		sprite = defaultC;
	}

	public void Update(){
		if(hasTarget == false || isLockedOnTarget == true) return;
		float lerpFactor = 0.15f;

		Vector2 diff = new Vector2(target.X - r.X, target.Y - r.Y);
		r.Y += diff.Y*lerpFactor;
		r.X += diff.X*lerpFactor;

		if(Math.Abs(diff.Y) < 4 && Math.Abs(diff.X) < 4){
			sprite = onTarget;
			hasTarget = false;
			isLockedOnTarget = true;
		}
	}

	public void NewTarget(float x, float y){
		hasTarget = true;
		isLockedOnTarget = false;
		sprite = defaultC; 
		target = new Vector2(x-r.Width/2, y-r.Height/2);
	}

	public void SetPosition(float x, float y){
		r.X = x-r.Width/2;
		r.Y = y-r.Height/2;
	}
}





