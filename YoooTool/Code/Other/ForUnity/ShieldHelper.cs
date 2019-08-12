using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoooTool.Code.Other.ForUnity
{
    public class Unit
    {
        public float Shield = 100;
        public float ShieldMax = 100;


    }

    public class ShieldHelper
    {
        public Dictionary<string,ShieldBuff> ShieldMap = new Dictionary<string, ShieldBuff>();
        
        //--优先级序列--
        public List<ShieldBuff> ShieldList = new List<ShieldBuff>();
        public void Add(string key,float value)
        {
            var buff = new ShieldBuff()
            {
                Key = key,
                Value = value,
            };
            ShieldMap.Add(key,buff);
            ShieldList.Add(buff);
        }
        public void Remove(string key)
        {
            ShieldBuff buff = null;
            if (ShieldMap.ContainsKey(key))
            {
                buff = ShieldMap[key];
            }
            else
            {
                return;
            }
            ShieldMap.Remove(key);
            ShieldList.Remove(buff);
        }

        /// <summary>
        /// 清数值而已
        /// </summary>
        public void Clear()
        {
            foreach (var shieldBuff in ShieldList)
            {
                shieldBuff.Value = 0;
            }
        }

        public void Update(string key, float value)
        {
            ShieldBuff buff = null;
            if (!ShieldMap.ContainsKey(key))
            {
                Add(key,value);
            }
            buff = ShieldMap[key];
            buff.Value = value;
        }

        

        public float CalDmg(float dmg)
        {
            foreach (var shieldBuff in ShieldList)
            {
                if (shieldBuff.Value >= dmg)
                {
                    shieldBuff.Value -= dmg;
                    dmg = 0;
                    break;
                }
                else
                {
                    dmg -= shieldBuff.Value;
                    shieldBuff.Value = 0;
                }
            }
            return dmg;
        }

        public class ShieldBuff
        {
            //public int Priority;
            public string Key;
            public float Value;
            public bool IsVaild()
            {
                return Value > 0;
            }
        }
    }
}
