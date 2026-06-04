public class WeaponFootprints{
	public List<Weapon> list = new List<Weapon>();
	
	public WeaponFootprints(){
		list.AddRange(
			new Weapon(40){
			   sprite=new Sprite(Resources.Weapon._weapon1,0,4),
			   dimensions=new Size(20,20),
			   phase = 1000,
			   damage=45,
			   ammoType=ItemDrop.Type.Smallbullets,
			   icon=new Sprite(Resources.UI.ASE_KIRVES1), 
			   type=Weapon.Type.Melee,
			},

			new Weapon(40){
			   sprite=new Sprite(Resources.Weapon._weapon2,0,4),
			   dimensions=new Size(20,20),
			   phase = 1000,
			   damage=35,
			   ammoType=ItemDrop.Type.Smallbullets,
			   icon=new Sprite(Resources.UI.ASE_PISTOOLI1),
			   type=Weapon.Type.Gun,
			   reloadSoundPath = Resources.Sounds.Guns._reload_gun,
			   fireSoundPath = Resources.Sounds.Guns._shot_pistol,
			},

			new Weapon(40){
			   sprite=new Sprite(Resources.Weapon._weapon3,0,3),
			   dimensions=new Size(20,20),
			   phase = 200,
			   damage=35,
			   ammoType=ItemDrop.Type.Bullets,
			   icon=new Sprite(Resources.UI.ASE_KONSU1),
			   type=Weapon.Type.Gun,
			   reloadSoundPath = Resources.Sounds.Guns._reload_assaultriffle,
			   fireSoundPath = Resources.Sounds.Guns._shot_assaultriffle,
			},

			new Weapon(40){
			   sprite=new Sprite(Resources.Weapon._weapon4,0,2),
			   dimensions=new Size(20,20),
			   phase = 1000,
			   damage=26,
			   ammoType=ItemDrop.Type.Bullets,
			   icon=new Sprite(Resources.UI.ASE_SHOTGUN),
			   type=Weapon.Type.Gun,
			   reloadSoundPath = Resources.Sounds.Guns._reload_shotgun,
			   fireSoundPath = Resources.Sounds.Guns._shot_shotgun,
			},

			new Weapon(40){
			   sprite=new Sprite(Resources.Weapon._weapon5,0,4),
			   dimensions=new Size(20,20),
			   phase = 1000,
			   damage= 50,
			   ammoType=ItemDrop.Type.Rockets,
			   icon=new Sprite(Resources.UI.ASE_SINKO1),
			   type=Weapon.Type.Gun,
			   reloadSoundPath = Resources.Sounds.Guns._reload_assaultriffle,
			   fireSoundPath = Resources.Sounds.Guns._shot_assaultriffle,
			},

			new Weapon(40){
			   sprite=new Sprite(Resources.Weapon._weapon6,0,2),
			   dimensions=new Size(20,20),
			   phase = 1000,
			   damage=35,
			   ammoType=ItemDrop.Type.Bullets,
			   icon=new Sprite(Resources.UI.ASE_SALAMA1),
			   type=Weapon.Type.Gun,
			   reloadSoundPath = Resources.Sounds.Guns._reload_assaultriffle,
			   fireSoundPath = Resources.Sounds.Guns._shot_assaultriffle,
			},

			new Weapon(40){
			   sprite=new Sprite(Resources.Weapon._weapon8,0,4),
			   dimensions=new Size(20,20),
			   phase = 1000,
			   damage=35,
			   ammoType=ItemDrop.Type.Bullets,
			   icon=new Sprite(Resources.UI.ASE_BYPASSER1),
			   type=Weapon.Type.Gun,
			   reloadSoundPath = Resources.Sounds.Guns._reload_assaultriffle,
			   fireSoundPath = Resources.Sounds.Guns._shot_bypasser,
			}
		);
	}
}
