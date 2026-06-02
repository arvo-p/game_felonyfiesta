public class Merc : Enemy{
 	public Merc(){
    	Init();
	}

	public Merc(PointF pos){
		r.Location = pos;
		Init();
	}

	protected override void LoadSprites(){
		walk = new Sprite(Resources.Merc._walk);
		shoot = new Sprite(Resources.Merc._shoot, 0, 3);
		stand = new Sprite(Resources.Merc._stand);
		dead = new Sprite(Resources.Merc._death, -1, 4);
	}
}
