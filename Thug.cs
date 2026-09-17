using System.Numerics;
using Raylib_cs;
using System.Media;
public class Thug : Enemy{
	public Thug(){
		Init();
	}

	public Thug(Vector2 pos){
		r.X = (pos).X; r.Y = (pos).Y;
		Init();
	}

	protected override void LoadSprites(){
		soundsDeath = new List<NativeAudioPlayer>();
		foreach(var src in Resources.Sounds._thugdeath)
			soundsDeath.Add(new NativeAudioPlayer("Resources/"+src));
		walk = new Sprite(Resources.Thug._walk);
		shoot = new Sprite(Resources.Thug._shoot, 0, 4);
		stand = new Sprite(Resources.Thug._stand);
		dead = new Sprite(Resources.Thug._death, -1, 4);
	}
}




