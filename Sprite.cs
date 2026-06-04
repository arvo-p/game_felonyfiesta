public class Sprite{
	List<Image> frames = new List<Image>();
	public string[]? frames_src;
	int index=0;
	int length=0;
	int slothFactor=1;

	bool _isInfiniteLoop = true;
	bool isAnimationTriggered = false;
	bool _isAnimationFinished = false;
	int restingframe_idx = 0;

	public bool isInfiniteLoop{get => _isInfiniteLoop;}
	public bool isAnimationFinished{get => _isAnimationFinished;}

	public Image frame{
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
			Image i = Image.FromStream(new MemoryStream(File.ReadAllBytes(filepath)));
			frames.Add(i);
		}

		length = frames.Count;
		if(length < 8) slothFactor = 1;
		else if(length <= 5) slothFactor = 1; 
	}

	public Sprite(string pre_filepath){
		this.frames_src = new string[]{pre_filepath};
		string filepath = Resources.root + "/" + pre_filepath;
		Image i = Image.FromStream(new MemoryStream(File.ReadAllBytes(filepath)));
		frames.Add(i);
		length = 1;
	}

	public Sprite(string[] filepaths, int restingframe_idx, int slothFactor){
		this.frames_src = filepaths;
		this._isInfiniteLoop = false;
		this.restingframe_idx = restingframe_idx;

		foreach(string pre_filepath in filepaths){
			string filepath = Resources.root + "/" + pre_filepath;
			Image i = Image.FromStream(new MemoryStream(File.ReadAllBytes(filepath)));
			frames.Add(i);
		}

		length = frames.Count;
		this.slothFactor = slothFactor;
		if(restingframe_idx == -1) this.restingframe_idx = length-1;
	}

	private static Dictionary<string, Image> _imageLibrary = new Dictionary<string, Image>();
	
	public static Image GetImage(string filepath){
		filepath = Resources.root + "/" + filepath;
		
		if (!_imageLibrary.ContainsKey(filepath))
			_imageLibrary[filepath] = Image.FromStream(new MemoryStream(File.ReadAllBytes(filepath)));

		return _imageLibrary[filepath];
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

	public void Trigger(Action NextFunction){
		Trigger();
		while(!isAnimationFinished);
		NextFunction();
	}
}
