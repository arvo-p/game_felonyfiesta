using System.Media;

public class Dialogue{
	public string text;
	NativeAudioPlayer audio; 
	NativeAudioPlayer ring = null;
	public List<Sprite> heads = new List<Sprite>();
	public bool isPlaying = true;

	short[] head_anim = {0, 2, 0, 2, 1};
	short counterHeadanim=0;
	public Sprite nowhead{
		get{
         	return heads[head_anim[(counterHeadanim++/5)%head_anim.Length]];
		}
	}

 	public Dialogue(string text, string[] heads, string audio_src){
		if(ring==null)
         	ring = new NativeAudioPlayer("Resources/Sounds/ringtone.wav");

		foreach(var head in heads)
			this.heads.Add(new Sprite(head)); 
		this.text = text;
		audio = new NativeAudioPlayer(System.IO.Path.GetFullPath("Resources/" + audio_src));
	}
	
	public async void Play(){
		isPlaying = true;
        await ring.PlayAsync();
    	await audio.PlayAsync();
		isPlaying = false;
	}
}
