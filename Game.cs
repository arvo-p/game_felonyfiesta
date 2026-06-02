using System.Windows.Forms;

public static class Game{
	public static Environment env = null!;
	public static Draw draw = null!;
	public static Camera camera = null!;
	public static int windowWidth;
	public static int windowHeight;
	
	public static Random rand = null!;

	private static System.Windows.Forms.Timer gametimer = new System.Windows.Forms.Timer();

	public static void Init(Form window, int pwindowWidth, int pwindowHeight){
		windowWidth = pwindowWidth;
		windowHeight = pwindowHeight;
		rand = new Random(5556);

		Resources.Font.Load();
		BuildingsFootprint.Init();
		ItemDropFootprints.Init();
		DialogueFootprints.Init();

		camera = new Camera();
		new Environment();
		draw = new Draw(env, windowWidth, windowHeight);

		gametimer.Interval = 18;
		gametimer.Tick += (s, e) => {
			env.Update();
			window.Invalidate();
		};
		gametimer.Start();
	}
}
