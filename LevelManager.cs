public class LevelManager{
	Environment env;
	List<Enemy> managedEnemies = new List<Enemy>();
	
	int waveMaxEnemies = 20;
	int waveRemaining = 6; //quantity of enemies to generate before the level finishes
	int waveSimultaneousMax = 3; //maximum quantity of enemies at the same time
	
	bool isLevelFinished = false;
	DateTime? dtRest;

	public LevelManager(){
		env = Game.env;
		NextDialogue();
	}
	
	public bool isDialoguePlaying = false;
	public Dialogue dia;
	int dialogue_pointer = 0;
    private void NextDialogue(){
		int len = DialogueFootprints.Dialogues.Count;
		if(dialogue_pointer > len - 1) return; // no more dialogues
		dia = DialogueFootprints.Dialogues[(dialogue_pointer++)];
		isDialoguePlaying = true;
		dia.Play();
	}

	private void NextLevel(){
		waveMaxEnemies += 10;
		waveSimultaneousMax += 5;
		waveRemaining = waveMaxEnemies;
		isLevelFinished = false;
	}

	public void Update(){
 		if(isDialoguePlaying && dia.isPlaying == false) isDialoguePlaying = false;
		if(isLevelFinished == true){
			if(!dtRest.HasValue) dtRest = DateTime.Now;
			else if((DateTime.Now - dtRest.Value).TotalSeconds >= 10){
				if(dia.isPlaying == false && isDialoguePlaying)
					NextLevel();              
				else if(!isDialoguePlaying) NextDialogue();
			}
			return;
		}
		
		foreach(var e in managedEnemies.ToList())
			if(e.isDead == true) managedEnemies.Remove(e);

		
		int countGenerateEnemies = waveSimultaneousMax - managedEnemies.Count();
		if(countGenerateEnemies > 0){
			if(countGenerateEnemies > waveRemaining){
				countGenerateEnemies = waveRemaining;
				waveRemaining = 0;
			}else waveRemaining -= countGenerateEnemies;
			
			for(int i=0;i<countGenerateEnemies;i++){
				Enemy e = RandomEnemy();
				//Enemy e = new Thug(SpawnCoordinates(env.p.r.Location,300));
				
				managedEnemies.Add(e);
				env.All.Add(e);
			}
		}

		isLevelFinished = (managedEnemies.Count() == 0);
	}
	
	private Enemy RandomEnemy(){
		Random r = new Random();
		int choice = r.Next(0, 3);

		if(choice == 0) return new Thug(SpawnCoordinates(env.p.r.Location,300));
		if(choice == 1) return new Merc(SpawnCoordinates(env.p.r.Location,300));
		if(choice == 2) return new Doctor(SpawnCoordinates(env.p.r.Location,300));
		return null;
	}
	
	public PointF SpawnCoordinates(PointF center, int spawnRadius){
		Random rand = new Random();
		int mapWidth = env.map.worldsize.Width;
		int mapHeight = env.map.worldsize.Height;
		int tileSize = env.map.tileRenderDimension;
		int entitySize = 64; //Value is fixed for now
		int maxAttempts = 30;
		
		for(int i=0;i<maxAttempts;i++){
			Point quadrant = new Point(rand.Next(0, 2) == 0? -1:1, rand.Next(0, 2) == 0? -1:1);
			int randomX = (int)center.X + quadrant.X*(Game.windowWidth/2+spawnRadius) + rand.Next(-spawnRadius, spawnRadius + 1); 
			int randomY = (int)center.Y + quadrant.Y*(Game.windowHeight/2+spawnRadius) + rand.Next(-spawnRadius, spawnRadius + 1); 
			if(randomX >= 0 && randomX < mapWidth && randomY >= 0 && randomY <= mapHeight){
				int[] checkX = {randomX, randomX + entitySize - 1};
				int[] checkY = {randomY, randomY + entitySize - 1};

				bool isClear = true;
				foreach(var x in checkX) foreach(var y in checkY)
					if(env.map.collision[x/tileSize, y/tileSize] != -1) isClear = false;

				if(isClear) return new PointF(randomX, randomY);
			}
		}
		
		return new PointF(0, 0);
	}
}
