using System.Numerics;
using Raylib_cs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EntityManager : IEnumerable<Entity>
{
    public List<Entity> NPCs = new List<Entity>();
    public List<Vehicle> Vehicles = new List<Vehicle>();
    public List<Player> Players = new List<Player>();

    public IEnumerator<Entity> GetEnumerator()
    {
        foreach (var npc in NPCs) yield return npc;
        foreach (var vehicle in Vehicles) yield return vehicle;
        foreach (var player in Players) yield return player;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => NPCs.Count + Vehicles.Count + Players.Count;
    
    public Entity GetByIndex(int index){
        if (index < NPCs.Count) return NPCs[index];
		if (index < NPCs.Count + Vehicles.Count) return Vehicles[index - NPCs.Count];
        return Players[index - NPCs.Count - Vehicles.Count];
    }

	public void Remove(Entity entity){
        if(entity is Player player) Players.Remove(player);
        else if(entity is Vehicle vehicle) Vehicles.Remove(vehicle);
		else NPCs.Remove(entity);
    }

	public void Add(Entity entity){
        if(entity is Player player) Players.Add(player);
        else if(entity is Vehicle vehicle) Vehicles.Add(vehicle);
		else NPCs.Add(entity);
    }
	
	public Entity this[int index]{
        get{
            if(index < NPCs.Count) return NPCs[index];
            int vehicleIndex = index - NPCs.Count;
            if(vehicleIndex < Vehicles.Count) return Vehicles[vehicleIndex];
			int playerIndex = index - NPCs.Count - Vehicles.Count;
			if(playerIndex < Players.Count) return Players[playerIndex];

            throw new System.IndexOutOfRangeException("Entity index out of range.");
        }
    }
}

