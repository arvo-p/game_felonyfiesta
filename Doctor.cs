public class Doctor : Enemy{
 	public Doctor(){
    	Init();
	}

	public Doctor(PointF pos){
		r.Location = pos;
		Init();
	}

	protected override void LoadSprites(){
		soundsDeath = new List<NativeAudioPlayer>();
		foreach(var src in Resources.Sounds._thugdeath)
			soundsDeath.Add(new NativeAudioPlayer("Resources/"+src));

		walk = new Sprite(Resources.Doctor._walk);
		shoot = new Sprite(Resources.Doctor._fire, 0, 4);
		stand = new Sprite(Resources.Doctor._stand);
		dead = new Sprite(Resources.Doctor._death, -1, 4);
	}
}
