using System.Numerics;
using Raylib_cs;
public static class BuildingsFootprint{
	public static List<Building> list = new List<Building>(); 
	public static void Init(){
		list.Add(new Building(Resources.Building._mcgurkWall,Resources.Building._mcgurkRoof[0],256,256,130)); 
		list.Add(new Building(Resources.Building._hotelWall,Resources.Building._hotelRoof[0],256+64,256+64,200)); 
		list.Add(new Building(Resources.Building._museumWall,Resources.Building._museumRoof[0],256+128,256+64,170)); 
		list.Add(new Building(Resources.Building._retailWall,Resources.Building._retailRoof[0],256,256,130)); 
		list.Add(new Building(Resources.Building._retail2Wall,Resources.Building._retail2Roof[0],256,256,150)); 
	}

	public static List<Building>? PlaceBuildings(Rectangle r){
		int tileRenderDimension = 64; // should match map's tileRender dimension, TO-DO
		
		List<Building> ret = new List<Building>();
		if(r.Width < 0 || r.Height < 0) return null;
		r.X *= tileRenderDimension;
		r.Y *= tileRenderDimension;
		r.Width *= tileRenderDimension;
		r.Height *= tileRenderDimension;

		Tools.Shuffle(list);
		foreach(var model in list){
			if(r.Width == model.Height && r.Height == model.Width){
				ret.Add(new Building(r.X, r.Y, model));
				break;
			}
		}

		return ret;
	}
}

