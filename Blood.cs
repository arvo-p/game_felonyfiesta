using System.Numerics;
using Raylib_cs;
public class Blood : Prop{

	public Blood(Rectangle r) : base(Resources.Environments._blood, r, 0){
		env.props.Add(this);
	}

	public static void SprayBlood(Vector2 pos, Vector2 dir){
		Random rnd = new Random();
		int count = rnd.Next(8, 15);
		for(int i=0;i<count;i++){
			float distance = i * 8f;
			float size = (float)rnd.Next(32, 32+16);
			var bloodDrop = new Blood(new Rectangle(pos.X + dir.X*distance, pos.Y + dir.Y*distance, size, size));
			Game.env.props.Add(bloodDrop);
		}
			//Game.env.props.Add(new Blood(new Vector2(pos.X+Tools.RandomFloat(0,3), pos.Y+Tools.RandomFloat(0, 3))));
	}
}


