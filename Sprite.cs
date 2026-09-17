using System.Numerics;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raylib_cs;

public class Sprite{
	List<Texture2D> frames = new List<Texture2D>();
	public string[]? frames_src;
	int index = 0;
	int length = 0;
	int slothFactor = 1;

	bool _isInfiniteLoop = true;
	bool isAnimationTriggered = false;
	bool _isAnimationFinished = false;
	int restingframe_idx = 0;

	public bool isInfiniteLoop{get => _isInfiniteLoop;}
	public bool isAnimationFinished{get => _isAnimationFinished;}

	public Texture2D frame{
		get{
			if(length == 0) return frames[0];

			if(isInfiniteLoop == true){
				index = (index+1)%(length*slothFactor);
				return frames[index/slothFactor];
			}
			
			if(isAnimationTriggered == false) return frames[restingframe_idx];
			else if((++index) >= length*slothFactor-1){
				isAnimationTriggered = false;
				_isAnimationFinished = true;
			}

			return frames[index/slothFactor];
		}
	}

	public Sprite(string[] filepaths){
		this.frames_src = filepaths;
		foreach(string pre_filepath in filepaths){
			string filepath = Resources.root + "/" + pre_filepath;
			frames.Add(GetImage(pre_filepath));
		}

		length = frames.Count;
		if(length < 8) slothFactor = 1;
		else if(length <= 5) slothFactor = 1; 
	}

	public Sprite(string pre_filepath){
		this.frames_src = new string[]{pre_filepath};
		frames.Add(GetImage(pre_filepath));
		length = 1;
	}

	public Sprite(string[] filepaths, int restingframe_idx, int slothFactor){
		this.frames_src = filepaths;
		this._isInfiniteLoop = false;
		this.restingframe_idx = restingframe_idx;

		foreach(string pre_filepath in filepaths){
			frames.Add(GetImage(pre_filepath));
		}

		length = frames.Count;
		this.slothFactor = slothFactor;
		if(restingframe_idx == -1) this.restingframe_idx = length-1;
	}

	private static Dictionary<string, Texture2D> _imageLibrary = new Dictionary<string, Texture2D>();
	
	public static Texture2D GetImage(string filepath){
		// When called from external places or directly from Sprite, we assume we might need to prepend Resources.root if it doesn't have it
		string fullPath = filepath.StartsWith(Resources.root) ? filepath : Resources.root + "/" + filepath;
		
		if (!_imageLibrary.ContainsKey(fullPath))
			_imageLibrary[fullPath] = Raylib.LoadTexture(fullPath);

		return _imageLibrary[fullPath];
	}

	public Sprite(string[] filepaths, int restingframe_idx, int slothFactor, bool infinite){
		this.frames_src = filepaths;
		this._isInfiniteLoop = infinite;
		this.restingframe_idx = restingframe_idx;
		this.slothFactor = slothFactor;

		foreach(string pre_filepath in filepaths)
			frames.Add(GetImage(pre_filepath));

		length = frames.Count;
		if(restingframe_idx == -1) this.restingframe_idx = length-1;
	}

	public void SetFrame(int c){
    	if(c == -1) index = frames.Count();
		else index = c;
	}

	public void Trigger(){
		_isAnimationFinished = false;
		index = 0;
		isAnimationTriggered = true;
	}
	
	public void MatchSlothFactor(Sprite s){
     	this.slothFactor = s.slothFactor;
	}

	public Sprite Clone(){
		return (Sprite)this.MemberwiseClone();
	}

	public void Trigger(Action NextFunction){
		Trigger();
		while(!isAnimationFinished);
		NextFunction();
	}
}


