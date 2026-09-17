using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Raylib_cs;

public class Map{
	
	public int[,] map;
	public int[,] secondLayer;
	public int[,] collision;

	public Vector2 worldsize;
	public Vector2 mapsize;

	public List<Building> buildings = new List<Building>();
	public Texture2D[] tileMap;
	public int tileDimension;
	public int tileRenderDimension;

	public enum Tiles{
		Empty=-1,
		Building=4
	}

	public Map(string[] filepathMap, string filepathTileset){
		int[,] buildingsLayer;

		if(File.Exists(filepathMap[0]) == false) throw new Exception("Map file inexistant");
		if(File.Exists(filepathTileset) == false) throw new Exception("Tileset inexistant");

		var dimension = GetMapDimension(filepathMap[0]);
		Console.WriteLine($"Size of map = {dimension.Item2}:{dimension.Item1}");
		this.mapsize = new Vector2(dimension.Item2, dimension.Item1);

		map = CreateMapArray(filepathMap[0],dimension);
		secondLayer = CreateMapArray(filepathMap[1],dimension);
		buildingsLayer = CreateMapArray(filepathMap[3],dimension);
		collision = CreateMapArray(filepathMap[2],dimension);

		tileDimension = 128;
		tileRenderDimension = 64;
		tileMap = ExtractTiles(filepathTileset, tileDimension);

		CreateBuildings(buildingsLayer);
		
		this.worldsize = new Vector2(dimension.height * tileRenderDimension, dimension.width * tileRenderDimension);
	}

	void CreateBuildings(int[,] map){
		int rows = map.GetLength(0); 
		int cols = map.GetLength(1); 
		bool[,] visited = new bool[rows, cols];
		for(int y = 0; y < rows; y++){
            for(int x = 0; x < cols; x++){
                if(map[y, x] == 1 && !visited[y, x]){
					int width = 0;
					while (x + width < cols && map[y, x + width] == 1) width++;

					int height = 0;
					while (y + height < rows && map[y + height, x] == 1) height++;

					for (int ry = y; ry < y + height; ry++)
					for (int rx = x; rx < x + width; rx++)
					visited[ry, rx] = true;

					var bs = BuildingsFootprint.PlaceBuildings(new Rectangle(y,x,width,height));
					if(bs != null) buildings.AddRange(bs);
                }
            }
        }
	}

	public (int x, int y) GetTileFromCoordinates(float x, float y){
		int c = (int)Math.Floor(x/tileRenderDimension);
		if(c >= collision.GetLength(0) || c < 0) return (-1,-1);
		
		int r = (int)Math.Floor(y/tileRenderDimension);
		if(r >= collision.GetLength(1) || r < 0) return (-1,-1);

		return (c, r);
	}

	public (int x, int y) GetTileFromCoordinates(Vector2 dot){
		return GetTileFromCoordinates(dot.X, dot.Y);
	}

	Texture2D[] ExtractTiles(string filepath, int tileSize){
		Texture2D tileset = Raylib.LoadTexture(filepath);

		int columns = tileset.Width / tileSize;
		int rows = tileset.Height / tileSize;
		
		Texture2D[] tileMapArr = new Texture2D[rows*columns];

		for(int r=0; r<rows; r++) {
            for(int c=0;c<columns; c++) {
			    tileMapArr[ (r*columns)+c ] = ExtractTile(tileset, c, r, tileSize);
            }
        }
		
		return tileMapArr;
	}

	Texture2D ExtractTile(Texture2D tileset, int column, int row, int tileSize){
		int x = column * tileSize;
		int y = row * tileSize;

        // In Raylib we don't necessarily need to create a new texture per tile
        // But to keep it close to original logic, we can load a cropped image.
        Image img = Raylib.LoadImageFromTexture(tileset);
        Raylib.ImageCrop(ref img, new Rectangle(x, y, tileSize, tileSize));
        Texture2D tile = Raylib.LoadTextureFromImage(img);
        Raylib.UnloadImage(img);

		return tile;
	}

	(int width, int height) GetMapDimension(string path){
		int countLine=0;
		int countCommas = 0;

		const Int32 BufferSize = 1024;
		string? line;
		
		var fileStream = File.OpenRead(path);
		var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize);
		
		while ((line = streamReader.ReadLine()) != null){
			countLine++;
			if(countLine != 1) continue;
			countCommas = line.Count(f => f == ',')+1;
		}
		
		return (countLine, countCommas);
	}

	int[,] CreateMapArray(string path, (int, int) dimension){
		int[,] newArray = new int[dimension.Item2, dimension.Item1];

		const Int32 BufferSize = 1024;
		string? line;
		
		var fileStream = File.OpenRead(path);
		var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize);
		
		int j = 0;
		while ((line = streamReader.ReadLine()) != null){
			int i = 0;
			foreach(var strNum in line.Split(',')){
				newArray[i,j] = Int32.Parse(strNum);
				i++;
			}
			j++;
		}
		
		return newArray;
	}
}
