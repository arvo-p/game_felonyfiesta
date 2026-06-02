using System.Media;
public class Thug : Enemy{
	public Thug(){
		Init();
	}

	public Thug(PointF pos){
		r.Location = pos;
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
