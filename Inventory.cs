using System.Numerics;
using Raylib_cs;
public class Inventory{
	Player? owner=null;

	public int grenades = 10;
	public int bullets = 128;
	public int batteries = 30;
	public int rockets = 5;
	public int shells = 12;
	public int smallbullets = 0;

	public void AddAmmo(int quantity, ItemDrop.Type type){
    	switch(type){
         	case ItemDrop.Type.Grenades:
				grenades += quantity;
				break;
         	case ItemDrop.Type.Bullets:
				bullets += quantity;
				break;
         	case ItemDrop.Type.Batteries:
				batteries += quantity;
				break;
         	case ItemDrop.Type.Rockets:
				rockets += quantity;
				break;
         	case ItemDrop.Type.Shells:
				shells += quantity;
				break;
         	case ItemDrop.Type.Smallbullets:
				smallbullets += quantity;
				break;
		}
		
		if(owner == null) return;
		if(owner.selectedWeapon == null) return;
		if(owner.selectedWeapon.currentClip == 0) owner.selectedWeapon.Reload();
	}

	public Inventory(Player p){
		owner = p;
	}
}

