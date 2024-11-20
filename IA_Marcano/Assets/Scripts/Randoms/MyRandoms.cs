using System;
using System.Collections.Generic;

namespace DefaultNamespace.Randoms
{
    public class MyRandoms
    {
        public static T Roulette<T>(Dictionary<T, float> items)
        {
            float total = 0;
            foreach (var item in items)
            {
                total += item.Value;
            }
            float random = UnityEngine.Random.Range(0, total);
            foreach (var item in items)
            {
                random -= item.Value;
                if (random <= 0)
                {
                    return item.Key;
                }
            }
            //default(T)
            return default;
        }
        
        public static void Shuffle<T>(List<T> items, Action<T, T> onSwap = null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                //0
                int r = UnityEngine.Random.Range(i, items.Count);
                if (onSwap != null)
                {
                    onSwap(items[i], items[r]);
                }
                T aux = items[r];
                items[r] = items[i];
                items[i] = aux;
            }
        }
    }
}