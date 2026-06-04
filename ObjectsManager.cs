
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectsManager : IEnumerable<Object>
{
    public List<Object> Objects = new List<Object>();
	public List<ItemDrop> Items = new List<ItemDrop>();
    public EntityManager Entities = new EntityManager();

    public IEnumerator<Object> GetEnumerator()
    {
        foreach (var obj in Objects) yield return obj;
		foreach (var ite in Items) yield return ite;
        foreach (var ent in Entities) yield return ent;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Objects.Count + Entities.Count + Items.Count;
    
    public Object GetByIndex(int index){
        if (index < Objects.Count) return Objects[index];
		int itemIndex = index - Objects.Count;

        if (itemIndex < Items.Count) return Items[itemIndex];
		int entityIndex = itemIndex - Items.Count;

        return Entities[entityIndex];
    }

	public void Add(Object obj){
		if (obj is ItemDrop item){
			Items.Add(item);
			return;
		}
		if (obj is Entity entity){
			Entities.Add(entity);
			return;
        }

        Objects.Add(obj);
		return;
    }

	public void Remove(Object obj){
		if (obj is ItemDrop item){
			Items.Remove(item);
		}else if(obj is Entity entity){
			Entities.Remove(entity);
        }else{
            Objects.Remove(obj);
        }
    }

	public Object this[int index]{
        get{
            if (index < Objects.Count) return Objects[index];

            int itemIndex = index - Objects.Count;
            if (itemIndex < Items.Count) return Items[itemIndex];
			
			int entityIndex = itemIndex - Items.Count;
			if (entityIndex < Entities.Count) return Entities[entityIndex];

            throw new System.IndexOutOfRangeException("Entity index out of range.");
        }
    }
}
