using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code.Utils;

namespace YoooTool.Code.Slk
{

    public static class SlkParseUtil
    {
        #region StringParse2Somehting
        public static int Parse2Int( string str, int defaultValue = 0)
        {
            int r;
            if (int.TryParse(str, out r)) return r;
            return defaultValue;
        }
        public static float Parse2Float(string str, float defaultValue = 0)
        {
            float r;
            if (float.TryParse(str, out r)) return r;
            return defaultValue;
        }
        public static double Parse2Double(string str, double defaultValue = 0)
        {
            double r;
            if (double.TryParse(str, out r)) return r;
            return defaultValue;
        }
        public static bool Parse2Bool(string str, bool defaultValue = false)
        {
            bool r;
            if (bool.TryParse(str, out r)) return r;
            return defaultValue;
        }
        #endregion

        public static string GetIdRefObjectJass<T>(string id) where  T: SlkDataObject
        {
            T slkDataObject = SlkManager.Instance.GetSlkData<T>(id) as T;
            if (slkDataObject == null)
            {
                return "InVaild Id Ref : " + id;
            }
            return slkDataObject.GetJass();
        }

        /// <summary>
        /// careful about ilist's object must is SlkDataObject
        /// </summary>
        /// <param name="slkList"></param>
        /// <returns></returns>
        public static string SlkList2IdList(IList slkList)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (SlkDataObject slkObj in slkList)
            {
                if (!isFirst)
                {
                    sb.Append(SplitChar);
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(slkObj.Id);
            }
            return sb.ToString();
        }

        public static string IdList2Config(List<string> idList)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (var s in idList)
            {
                if (!isFirst)
                {
                    sb.Append(SplitChar);
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(s);
            }
            return sb.ToString();
        }

        public static List<string> Config2IdList(string data)
        {
            List<string> list = new List<string>();
            string[] srr = data.Split(SplitChar);
            foreach (var s in srr)
            {
                list.Add(s);
            }
            return list;
        }

        public static List<T> Config2SlkList<T>(string data) where T:SlkDataObject
        {
            List<T> list = new List<T>();
            string[] srr = data.Split(SplitChar);
            foreach (var s in srr)
            {
                T slkObj = SlkManager.Instance.GetSlkData<T>(s) as T;
                if(slkObj!=null)
                    list.Add(slkObj);
            }
            return list;
        }

        public const char SplitChar = ';';

        public static string IdPool2Config(RandomWeightPool<string> idPool)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (var pair in idPool.GetMapCopy())
            {
                if (!isFirst)
                {
                    sb.Append(SplitChar);
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(string.Format("{0}|{1}", pair.Key, pair.Value));
            }
            return sb.ToString();
        }

        public static RandomWeightPool<string> Config2IdPool(string data)
        {
            RandomWeightPool<string> pool = new RandomWeightPool<string>();
            string[] srr = data.Split(SplitChar);
            foreach (var s in srr)
            {
                string[] pair = s.Split('|');
                pool.SetItemWeight(pair[0], float.Parse(pair[1]));
            }
            return pool;
        }

        /// <summary>
        /// 运行时等所有SLK数据载入后，可以把ID引用变成真的对象引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idPool"></param>
        /// <returns></returns>
        public static RandomWeightPool<T> ParseIdPool<T>(RandomWeightPool<string> idPool) where T : SlkDataObject
        {
            RandomWeightPool<T> pool = new RandomWeightPool<T>();
            foreach (var pair in idPool.GetMapCopy())
            {
                //TODO
            }
            return pool;
        }

    }

}
