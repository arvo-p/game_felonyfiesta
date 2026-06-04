using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;

public static class SaveSystem{
	public class SaveData{
		public bool IsTurnBased{ get; set; }
		public LevelData Level{ get; set; } = new();
		public List<PlayerData> Players{ get; set; } = new();
		public List<NPCData> NPCs{ get; set; } = new();
		public List<VehicleData> Vehicles{ get; set; } = new();
		public List<ItemData> Items{ get; set; } = new();
	}

	public class LevelData{
		public int WaveMaxEnemies{ get; set; }
		public int WaveRemaining{ get; set; }
		public int WaveSimultaneousMax{ get; set; }
		public bool IsLevelFinished{ get; set; }
		public int DialoguePointer{ get; set; }
	}

	public class PlayerData{
		public float X{ get; set; }
		public float Y{ get; set; }
		public float Rotation{ get; set; }
		public float Speed{ get; set; }
		public int Health{ get; set; }
		public int SelectedWeaponIndex{ get; set; }
		public InventoryData Inventory{ get; set; } = new();
	}

	public class InventoryData{
		public int Grenades{ get; set; }
		public int Bullets{ get; set; }
		public int Batteries{ get; set; }
		public int Rockets{ get; set; }
		public int Shells{ get; set; }
		public int SmallBullets{ get; set; }
	}

	public class NPCData{
		public string Type{ get; set; } = "";
		public float X{ get; set; }
		public float Y{ get; set; }
		public float Width{ get; set; }
		public float Height{ get; set; }
		public float Rotation{ get; set; }
		public float Speed{ get; set; }
		public int Health{ get; set; }
	}

	public class VehicleData{
		public float X{ get; set; }
		public float Y{ get; set; }
		public float Width{ get; set; }
		public float Height{ get; set; }
		public float Rotation{ get; set; }
		public float Speed{ get; set; }
		public int Health{ get; set; }
	}

	public class ItemData{
		public string Type{ get; set; } = "";
		public int Quantity{ get; set; }
		public float X{ get; set; }
		public float Y{ get; set; }
	}

    public static void Save(string filePath){
        var env = Game.env;
        var lm = env.levelManager;
        if(env.players.Count == 0) return;

        var data = new SaveData();
		data.IsTurnBased = Game.isTurnBased;

        // Level Data
        data.Level.WaveMaxEnemies = lm.waveMaxEnemies;
        data.Level.WaveRemaining = lm.waveRemaining;
        data.Level.WaveSimultaneousMax = lm.waveSimultaneousMax;
        data.Level.IsLevelFinished = lm.isLevelFinished;
        data.Level.DialoguePointer = lm.dialogue_pointer;

        // Players Data
        foreach(var p in env.players){
            data.Players.Add(new PlayerData{
                X = p.r.X,
                Y = p.r.Y,
                Rotation = p.rotation,
                Speed = p.speed,
                Health = p.health,
                SelectedWeaponIndex = p.idxSelectedWeapon,
                Inventory = new InventoryData{
                    Grenades = p.inventory.grenades,
                    Bullets = p.inventory.bullets,
                    Batteries = p.inventory.batteries,
                    Rockets = p.inventory.rockets,
                    Shells = p.inventory.shells,
                    SmallBullets = p.inventory.smallbullets
                }
            });
        }

        // Entities
        foreach(var npc in env.All.Entities.NPCs){
            data.NPCs.Add(new NPCData{
                Type = npc.GetType().Name,
                X = npc.r.X,
                Y = npc.r.Y,
                Width = npc.r.Width,
                Height = npc.r.Height,
                Rotation = npc.rotation,
                Speed = npc.speed,
                Health = npc.health
            });
        }

        foreach(var vehicle in env.All.Entities.Vehicles){
            data.Vehicles.Add(new VehicleData
           {
                X = vehicle.r.X,
                Y = vehicle.r.Y,
                Width = vehicle.r.Width,
                Height = vehicle.r.Height,
                Rotation = vehicle.rotation,
                Speed = vehicle.speed,
                Health = vehicle.health
            });
        }

        foreach(var item in env.All.Items){
            data.Items.Add(new ItemData
           {
                Type = item.type.ToString(),
                Quantity = item.quantity,
                X = item.r.X,
                Y = item.r.Y
            });
        }

        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions{ WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public static void Load(string filePath){
		if(!File.Exists(filePath)) return;

		string json = File.ReadAllText(filePath);
		var data = JsonSerializer.Deserialize<SaveData>(json);
		if(data == null) return;

		Game.isTurnBased = data.IsTurnBased;
		var env = Game.env;
		var lm = env.levelManager;
		
		env.All.Objects.Clear();
		env.All.Items.Clear();
		env.All.Entities.NPCs.Clear();
		env.All.Entities.Vehicles.Clear();
		env.All.Entities.Players.Clear();
		env.players.Clear();
		
		lm.managedEnemies.Clear();

		lm.waveMaxEnemies = data.Level.WaveMaxEnemies;
		lm.waveRemaining = data.Level.WaveRemaining;
		lm.waveSimultaneousMax = data.Level.WaveSimultaneousMax;
		lm.isLevelFinished = data.Level.IsLevelFinished;
		lm.dialogue_pointer = data.Level.DialoguePointer;

        foreach(var pData in data.Players){
            Player p = new Player(env.crosshair);
            p.r.X = pData.X;
            p.r.Y = pData.Y;
            p.rotation = pData.Rotation;
            p.speed = pData.Speed;
            p.health = pData.Health;
            p.isDead = p.health <= 0;
            p.idxSelectedWeapon = (short)pData.SelectedWeaponIndex;
            p.inventory.grenades = pData.Inventory.Grenades;
            p.inventory.bullets = pData.Inventory.Bullets;
            p.inventory.batteries = pData.Inventory.Batteries;
            p.inventory.rockets = pData.Inventory.Rockets;
            p.inventory.shells = pData.Inventory.Shells;
            p.inventory.smallbullets = pData.Inventory.SmallBullets;
            p.PositionUpdated();
            p.hitboxes.Clear();
            p.SetCollisionCircles();
            
            env.players.Add(p);
            env.All.Add(p);
        }

		if(env.players.Count > 0) Game.camera.Follow(env.players[0]);

		foreach(var npcData in data.NPCs){
			Enemy? npc = null;
			PointF pos = new PointF(npcData.X, npcData.Y);
			if(npcData.Type == "Thug") npc = new Thug(pos);
			else if(npcData.Type == "Merc") npc = new Merc(pos);
			else if(npcData.Type == "Doctor") npc = new Doctor(pos);

            if(npc != null){
				npc.rotation = npcData.Rotation;
				npc.speed = npcData.Speed;
				npc.health = npcData.Health;
				npc.r.Width = npcData.Width;
				npc.r.Height = npcData.Height;
				npc.PositionUpdated();
				env.All.Add(npc);
				lm.managedEnemies.Add(npc);
            }
		}

        Game.turnManager.Reset();
        if (Game.isTurnBased) Game.turnManager.EndEnemiesTurn();

            // Restore Vehicles
            foreach(var vData in data.Vehicles){
				Vehicle v = new Vehicle();
				v.r.X = vData.X;
				v.r.Y = vData.Y;
				v.r.Width = vData.Width;
				v.r.Height = vData.Height;
				v.rotation = vData.Rotation;
				v.speed = vData.Speed;
				v.health = vData.Health;
				v.PositionUpdated();
				env.All.Add(v);
            }

            // Restore Items
            foreach(var iData in data.Items){
				if(Enum.TryParse(iData.Type, out ItemDrop.Type type)){
					ItemDrop? model = null;
					if(type == ItemDrop.Type.Weapon){
						// For weapons, we match by the 'quantity'(which is the enum value)
						model = ItemDropFootprints.guns.FirstOrDefault(m => m.quantity == iData.Quantity);
					}
					else{
						model = ItemDropFootprints.ammo.FirstOrDefault(m => m.type == type);
					}

					if(model != null){
						// ItemDrop constructor for cloning takes(x, y, model) and adds 8 to x and y
						// So we pass iData.X - 8 to compensate
						ItemDrop item = new ItemDrop(iData.X - 8, iData.Y - 8, model);
						item.quantity = iData.Quantity;
						env.All.Add(item);
					}
				}
            }
    }
}
