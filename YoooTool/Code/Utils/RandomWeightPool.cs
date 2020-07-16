using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoooTool.Code.Utils
{

    public class RandomWeightPool<T>
    {
        /// <summary>
        /// Just A Copy
        /// </summary>
        /// <returns></returns>
        public Dictionary<T, float> GetMapCopy()
        {
            Dictionary<T, float> copy = new Dictionary<T, float>(ObjectWeightMap);
            return copy;
        }
        public bool IsEmpty()
        {
            return ObjectWeightMap.Count == 0;
        }

        protected Dictionary<T, float> ObjectWeightMap = new Dictionary<T, float>();

        protected Random SeedRandom;

        protected float TotalWeight { get; set; }

        public RandomWeightPool(int seed = 0)
        {
            SeedRandom = new Random(seed);
        }
        public RandomWeightPool<T> SetItemWeight(T item, float weight)
        {
            if (!ObjectWeightMap.ContainsKey(item))
            {
                TotalWeight += weight;
                ObjectWeightMap.Add(item, weight);
                return this;
            }
            TotalWeight += weight - ObjectWeightMap[item];
            ObjectWeightMap[item] = weight;
            return this;
        }

        public T GetItem()
        {
            float random = (float)SeedRandom.NextDouble() * TotalWeight;
            float last = 0;
            foreach (var pair in ObjectWeightMap)
            {
                if (random >= last && random < pair.Value + last)
                {
                    return pair.Key;
                }
                last = pair.Value;
            }
            return default(T);
        }
    }
}
