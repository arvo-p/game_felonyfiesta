public static class ItemDropFootprints{
	public static List<ItemDrop> ammo = new List<ItemDrop>();
	public static List<ItemDrop> guns = new List<ItemDrop>();

	public static void Init(){
		ammo.Add(new ItemDrop(Resources.Item._bullet, ItemDropEffects.Ammo, ItemDrop.Type.Bullets, 64));
		ammo.Add(new ItemDrop(Resources.Item._smallbullets, ItemDropEffects.Ammo, ItemDrop.Type.Smallbullets, 64));
		ammo.Add(new ItemDrop(Resources.Item._rockets, ItemDropEffects.Ammo, ItemDrop.Type.Rockets, 5));
		ammo.Add(new ItemDrop(Resources.Item._rocket, ItemDropEffects.Ammo, ItemDrop.Type.Rockets, 1));
		ammo.Add(new ItemDrop(Resources.Item._shells, ItemDropEffects.Ammo, ItemDrop.Type.Shells, 12));

		guns.Add(new ItemDrop(Resources.Item._pistol, ItemDropEffects.Weapon, ItemDrop.Type.Weapon, (int)Weapon.WeaponType.Pistol));
		guns.Add(new ItemDrop(Resources.Item._assaultgun, ItemDropEffects.Weapon, ItemDrop.Type.Weapon, (int)Weapon.WeaponType.AssaultRifle));
		guns.Add(new ItemDrop(Resources.Item._shotgun, ItemDropEffects.Weapon, ItemDrop.Type.Weapon, (int)Weapon.WeaponType.Shotgun));
		guns.Add(new ItemDrop(Resources.Item._rocketrifle, ItemDropEffects.Weapon, ItemDrop.Type.Weapon, (int)Weapon.WeaponType.RocketLauncher));
		guns.Add(new ItemDrop(Resources.Item._biofoton, ItemDropEffects.Weapon, ItemDrop.Type.Weapon, (int)Weapon.WeaponType.LightningGun));
		guns.Add(new ItemDrop(Resources.Item._bypasser, ItemDropEffects.Weapon, ItemDrop.Type.Weapon, (int)Weapon.WeaponType.Bypasser));
	}
	
	public static ItemDrop SelectRandom(){
     	Random r = new Random();
		int index = r.Next(0, ammo.Count());
		return ammo[index];
	}

	public static ItemDrop SelectRandomGun(){
     	Random r = new Random();
		int index = r.Next(0, guns.Count());
		return guns[index];
	}
}
