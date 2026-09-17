using System.Numerics;
using Raylib_cs;
public class Building{
    public float X, Y, Width, Height, Height3D;
	public int rot=0;

	public Sprite? roof;
	public Sprite? Lwall{get => _wall[(0+rot)%_wall.Count] != null? _wall[(0+rot)%_wall.Count]:null;}
	public Sprite? Uwall{get => _wall[(1+rot)%_wall.Count] != null? _wall[(1+rot)%_wall.Count]:null;}
	public Sprite? Rwall{get => _wall[(0+rot)%_wall.Count] != null? _wall[(2+rot)%_wall.Count]:Lwall;}
	public Sprite? Bwall{get => _wall[(1+rot)%_wall.Count] != null? _wall[(3+rot)%_wall.Count]:Uwall;}

	public List<Sprite> _wall = new List<Sprite>();
	

	public Building(string[] wall, string? newroof, float w, float h, float h3D){
		if(newroof != null)
			roof = new Sprite((string)newroof);

		foreach(var wll in wall) _wall.Add(new Sprite(wll));
		
		this.Width = w; //padding
		this.Height = h; //padding
		this.Height3D = h3D;
	}

    public Building(float x, float y, Building modelBuilding){
		this.X = x+10; //padding
		this.Y = y+10; //padding
		
		this._wall = modelBuilding._wall;
		this.roof = modelBuilding.roof;
		this.Width = modelBuilding.Width-20;
		this.Height = modelBuilding.Height-20;
		this.Height3D = modelBuilding.Height3D;
		this.rot = 1;
	}

	#region ICloneable Members
	public object Clone(float x, float y){
		this.X = x;
		this.Y = y;
		return this.MemberwiseClone();
	}

	public object Clone(float x, float y, int rotate){
		// TO - DO
		this.X = x;
		this.Y = y;
		return this.MemberwiseClone();
	}
	#endregion
}

