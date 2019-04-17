using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace YoooTool.Code
{

    public class RandomPool<T>
    {
        protected Dictionary<T,float> ObjectWeightMap = new Dictionary<T, float>();

        protected Random SeedRandom;

        protected float TotalWeight { get; set; }

        public RandomPool(int seed = 0)
        {
            SeedRandom = new Random(seed);
        }
        public void SetItemWeight(T item, float weight)
        {
            if (!ObjectWeightMap.ContainsKey(item))
            {
                TotalWeight += weight;
                ObjectWeightMap.Add(item,weight);
                return;
            }
            TotalWeight += weight - ObjectWeightMap[item];
            ObjectWeightMap[item] = weight;
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

    public class PoolConstant
    {
        
    }


    public enum RoomType
    {
        Interation,
        EnemyStrike,
        KeepAlive,
    }

    public class InteractiveObject
    {
        public string Desc;

    }

    /// <summary>
    /// interactive
    /// </summary>
    public class Room_Interactive
    {
        
    }
    /// <summary>
    /// certain enemy battle
    /// </summary>
    public class Room_EnemyStrike
    {

    }
    /// <summary>
    /// keep alive in time
    /// </summary>
    public class Room_KeepAlive
    {
        public float Time;
        
    }

    public class Level_Room
    {

    }


    
    public class LevelCfg 
    {
        
    }
}
