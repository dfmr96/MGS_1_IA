using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Randoms
{
    public enum ItemType
    {
        ration,
        ammo,
        granade
    }
    public class ItemsRandom : MonoBehaviour
    {
        public List<RarityInfo> infos;
        Dictionary<ItemType, float> _items;
        
        
        private void Awake()
        {
            _items = new Dictionary<ItemType, float>();
            for (int i = 0; i < infos.Count; i++)
            {
                _items[infos[i].type] = infos[i].weight;
            }
        }
        
        public void GetRandomItem()
        {
            //TO DO
            ItemType item = MyRandoms.Roulette(_items);
        }
    }
}