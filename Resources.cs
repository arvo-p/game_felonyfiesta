using System.Drawing.Text;

public class Resources{
	public static string root = "Resources";
	
	public static class Font{
		public static PrivateFontCollection _pfc = new PrivateFontCollection();
		public static void Load(){
			_pfc.AddFontFile(root+"/Font/bank gothic medium bt.ttf");
		}
	}

	public static class Player{
		public static string[] _stand = {
			"Player/Player_stand.png"
		};
		public static string[] _walk = {
			"Player/Player_walk1.png",
			"Player/Player_walk2.png",
			"Player/Player_walk3.png",
			"Player/Player_walk4.png",
			"Player/Player_walk5.png",
			"Player/Player_walk6.png",
			"Player/Player_walk7.png",
			"Player/Player_walk8.png",
			"Player/Player_walk9.png",
			"Player/Player_walk10.png",
			"Player/Player_walk11.png",
			"Player/Player_walk12.png"
		};
		public static string[] _fire = {
			"Player/Player_fire1.png",
			"Player/Player_fire2.png",
			"Player/Player_fire3.png",
			"Player/Player_fire4.png"
		};
		public static string[] _death = {
			"Player/Player_death1.png",
			"Player/Player_death2.png",
			"Player/Player_death3.png",
			"Player/Player_death4.png",
			"Player/Player_death5.png",
			"Player/Player_death6.png",
			"Player/Player_death7.png"
		};
		public static string[] _firebig = {
			"Player/Player_firebig1.png",
			"Player/Player_firebig2.png",
			"Player/Player_firebig3.png"
		};
		public static string[] _meleethrow = {
			"Player/Player_meleethrow1.png",
			"Player/Player_meleethrow2.png",
			"Player/Player_meleethrow3.png",
			"Player/Player_meleethrow4.png",
			"Player/Player_meleethrow5.png"
		};
	}

	public static class Weapon{
		public static string[] _weapon1 = {
			"Player/Weapon1_1.png",
			"Player/Weapon1_2.png",
			"Player/Weapon1_3.png",
			"Player/Weapon1_4.png",
			"Player/Weapon1_5.png"
		};

		public static string[] _weapon2 = {
			"Player/Weapon2_1.png",
			"Player/Weapon2_2.png",
			"Player/Weapon2_3.png",
			"Player/Weapon2_4.png"
		};

		public static string[] _weapon3 = {
			"Player/Weapon3_1.png",
			"Player/Weapon3_2.png",
			"Player/Weapon3_3.png",
			"Player/Weapon3_4.png"
		};

		public static string[] _weapon4 = {
			"Player/Weapon4_1.png",
			"Player/Weapon4_2.png",
			"Player/Weapon4_3.png",
			"Player/Weapon4_4.png"
		};

		public static string[] _weapon5 = {
			"Player/Weapon5_1.png",
			"Player/Weapon5_2.png",
			"Player/Weapon5_3.png",
			"Player/Weapon5_4.png"
		};

		public static string[] _weapon6 = {
			"Player/Weapon6_1.png",
			"Player/Weapon6_2.png",
			"Player/Weapon6_3.png",
			"Player/Weapon6_4.png"
		};
		public static string[] _weapon8 = {
			"Player/Weapon8_bypasser1.png",
			"Player/Weapon8_bypasser2.png"
		};
	}

	public static class Environments{
		public static string[] _smoke = {
			"Environments/smoke1.png",
			"Environments/smoke2.png",
			"Environments/smoke3.png",
			"Environments/smoke4.png"
		};
		public static string[] _blood = {
			"Environments/blood.png"
		};
	}
	
    public static class Heads{
     	public static string[] _chillguy = {
         	"Heads/chillguy0.png",
         	"Heads/chillguy1.png",
         	"Heads/chillguy2.png",
		};
	}
	
	public static class Dialogues{
        public static string D1 = "Sounds/dialogue/1.wav"; 
	}	

	public static class UI{
		public static string[] _ammopanel = {
			"UI/ammunition.png"
		};
		public static string[] _crosshair1 = {
			"UI/Crosshair.png"
		};
		public static string[] _crosshair2 = {
			"UI/Crosshair1.png"
		};

		public static string[] ASE_BYPASSER1 = {
         	"UI/ASE_BYPASSER1.png"
		};
		public static string[] ASE_KIRVES1 = {
         	"UI/ASE_KIRVES1.png"
		};
		public static string[] ASE_KONSU1 = {
         	"UI/ASE_KONSU1.png"
		};
		public static string[] ASE_KRANU1 = {
         	"UI/ASE_KRANU1.png"
		};
		public static string[] ASE_PISTOOLI1 = {
         	"UI/ASE_PISTOOLI1.png"
		};
		public static string[] ASE_SALAMA1 = {
         	"UI/ASE_SALAMA1.png"
		};
		public static string[] ASE_SINKO1 = {
         	"UI/ASE_SINKO1.png"
		};
		public static string[] ASE_SHOTGUN = {
			"UI/Weaponicon_shotgun.png"
	 	};

		public static class Parallax {
			public static string Background = "Parallax/background.png";
			public static string Foreground = "Parallax/foreground.png";
		}
	}

	public static class Item{
		public static string _bullet = "Items/Item_bullets.png";
		public static string _rockets = "Items/Item_rockets.png";
		public static string _rocket = "Items/Item_rocket.png";
		public static string _shells = "Items/Item_shells.png";
		public static string _smallbullets= "Items/Item_smallbullets.png";
		public static string _battery = "Items/Item_battery.png";
		public static string _biofoton = "Items/Item_Biofoton.png";
		public static string _bypasser = "Items/Item_Bypasser.png";
		public static string _assaultgun = "Items/Weapon_assaultgun.png";
		public static string _rocketrifle = "Items/Weapon_rocketrifle.png";
		public static string _shockfist = "Items/Weapon_shockfist.png";
		public static string _shotgun = "Items/Weapon_shotgun.png";
		public static string _pistol = "Items/Weapon_pistol.png";
		public static string _grenade = "Items/Grenade.png";
	}
	
	public static class Sounds{
     	public static string[] _thugdeath = {
         	"Sounds/thugdeath1.wav",
			"Sounds/thugdeath2.wav"
		};
		public static string _main_theme = "Sounds/theme_music.wav";
		public static class Guns{
			public static string _reload_assaultriffle = "Sounds/guns/reload_assaultriffle.wav";
			public static string _reload_gun = "Sounds/guns/reload_gun.wav";
			public static string _reload_shotgun = "Sounds/guns/reload_shotgun.wav";
			public static string _shot_assaultriffle = "Sounds/guns/shot_assaultriffle.wav";
			public static string _shot_bypasser = "Sounds/guns/shot_bypasser.wav";
			public static string _shot_pistol = "Sounds/guns/shot_pistol.wav";
			public static string _shot_shotgun = "Sounds/guns/shot_shotgun.wav";
		}
	}

	public static class Thug{
		public static string[] _walk = {
			"Thug/Biothug_walk1.png",
			"Thug/Biothug_walk2.png",
			"Thug/Biothug_walk3.png", 
			"Thug/Biothug_walk4.png", 
			"Thug/Biothug_walk5.png",
			"Thug/Biothug_walk6.png",
			"Thug/Biothug_walk7.png",
			"Thug/Biothug_walk8.png",
			"Thug/Biothug_walk9.png",
			"Thug/Biothug_walk10.png",
			"Thug/Biothug_walk11.png"
		};
		public static string[] _stand = {
			"Thug/Biothug_stand.png"
		};
		public static string[] _shoot = {
			"Thug/Biothug_shoot1.png",
			"Thug/Biothug_shoot2.png",
			"Thug/Biothug_shoot3.png"
		};
		public static string[] _death = {
			"Thug/Biothug_death1.png",
			"Thug/Biothug_death2.png",
			"Thug/Biothug_death3.png",
			"Thug/Biothug_death4.png",
			"Thug/Biothug_death5.png",
			"Thug/Biothug_death6.png"
		};
	}

	public static class Merc{
		public static string[] _walk = {
			"Merc/Mercwalk1.png",
			"Merc/Mercwalk2.png",
			"Merc/Mercwalk3.png",
			"Merc/Mercwalk4.png",
			"Merc/Mercwalk5.png",
			"Merc/Mercwalk6.png",
			"Merc/Mercwalk7.png",
			"Merc/Mercwalk8.png"
		};
		public static string[] _stand = {
			"Merc/Mercstand1.png"
		};
		public static string[] _shoot = {
			"Merc/Mercattack1.png",
			"Merc/Mercattack2.png",
			"Merc/Mercattack3.png"
		};
		public static string[] _death = {
			"Merc/Mercdeath1.png",
			"Merc/Mercdeath2.png",
			"Merc/Mercdeath3.png",
			"Merc/Mercdeath4.png"
		};
	}

	public static class Doctor{
		public static string[] _walk = {
			"Doctor/MadFatDoctor_walk1.png",
			"Doctor/MadFatDoctor_walk2.png",
			"Doctor/MadFatDoctor_walk3.png",
			"Doctor/MadFatDoctor_walk4.png",
			"Doctor/MadFatDoctor_walk5.png",
			"Doctor/MadFatDoctor_walk6.png",
			"Doctor/MadFatDoctor_walk7.png",
			"Doctor/MadFatDoctor_walk8.png",
			"Doctor/MadFatDoctor_walk9.png",
			"Doctor/MadFatDoctor_walk10.png"
		};
		public static string[] _stand = {
			"Doctor/MadFatDoctor_stand.png"
		};
		public static string[] _fire = {
			"Doctor/MadFatDoctor_fire1.png",
			"Doctor/MadFatDoctor_fire2.png",
			"Doctor/MadFatDoctor_fire3.png"
		};
		public static string[] _death = {
			"Doctor/MadFatDoctor_death1.png",
			"Doctor/MadFatDoctor_death2.png",
			"Doctor/MadFatDoctor_death3.png",
			"Doctor/MadFatDoctor_death4.png",
			"Doctor/MadFatDoctor_death5.png"
		};
	}

	public static class Vehicle{
		public static string[] _carshadow = {
			"Vehicle/Shadow.png"
		};
		public static string[] _car = {
			"Vehicle/Car.png"
		};
		public static string[] _audi = {
			"Vehicle/Audi.png"
		};
		public static string[] _blackviper = {
			"Vehicle/Black_viper.png"
		};
		public static string[] _taxi = {
			"Vehicle/taxi.png"
		};
	}
	
	public static class Building{
		public static string[] _mcgurkWall = {
			"Tiles/Buildings/mcgurk_wall1.png",
			"Tiles/Buildings/mcgurk_wall2.png",
			"Tiles/Buildings/mcgurk_wall3.png"
		};
		public static string[] _mcgurkRoof = {
			"Tiles/Buildings/mcgurk_roof.png"
		};

		public static string[] _hotelWall = {
			"Tiles/Buildings/hotel_wall1.png",
			"Tiles/Buildings/hotel_wall2.png",
			"Tiles/Buildings/hotel_wall3.png"
		};
		public static string[] _hotelRoof = {
			"Tiles/Buildings/hotel_roof.png"
		};

		public static string[] _museumWall = {
			"Tiles/Buildings/museum_wall1.png",
			"Tiles/Buildings/museum_wall2.png",
			"Tiles/Buildings/museum_wall3.png"
		};
		public static string[] _museumRoof = {
			"Tiles/Buildings/museum_roof.png"
		};

		public static string[] _retailWall = {
			"Tiles/Buildings/retail_wall1.png",
			"Tiles/Buildings/retail_wall2.png",
			"Tiles/Buildings/retail_wall3.png"
		};
		public static string[] _retailRoof = {
			"Tiles/Buildings/retail_roof.png"
		};

		public static string[] _retail2Wall = {
			"Tiles/Buildings/retail2_wall1.png",
			"Tiles/Buildings/retail2_wall2.png",
			"Tiles/Buildings/retail2_wall3.png"
		};
		public static string[] _retail2Roof = {
			"Tiles/Buildings/retail_roof.png"
		};

		public static string[] _retail3Wall = {
			"Tiles/Buildings/retail3_wall1.png",
			"Tiles/Buildings/retail3_wall2.png",
			"Tiles/Buildings/retail3_wall3.png"
		};
		public static string[] _retail3Roof = {
			"Tiles/Buildings/retail_roof.png"
		};
	}
}
