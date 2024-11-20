using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Randoms
{
    public class ItemSpawner : MonoBehaviour
    {
        public static ItemSpawner Instance;
        public GameObject healthPrefab;
        public GameObject ammoPrefab;
        public GameObject grenadePrefab;
        public Dictionary<GameObject, float> _items;
        public PlayerController playerController;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this; 
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            playerController = Constants.Player;
        }

        public void SpawnRandomItem(Vector3 position)
        {
            _items = new Dictionary<GameObject, float>
            {
                { healthPrefab, Constants.Player.playerModel.MaxHealth - Constants.Player.Health},  
                { ammoPrefab, 5},     
                { grenadePrefab, 10} 
            };
            GameObject selectedItem = MyRandoms.Roulette(_items);

            if (selectedItem != null)
            {
                Instantiate(selectedItem, position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("No item selected for spawn.");
            }
        }
    }
}