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
        public static object ParseByType(Type type, string str)
        {
            if (type == typeof(int))
            {
                return Parse2Int(str);
            }else if (type == typeof(float))
            {
                return Parse2Float(str);
            }else if (type == typeof(double))
            {
                return Parse2Double(str);
            }
            else if(type == typeof(bool))
            {
                return Parse2Bool(str);
            }
            
            else if (type == typeof(List<string>))
            {
                return Config2IdList(str);
            }
            else if (type == typeof(RandomWeightPool<string>))
            {
                return Config2IdPool(str);
            }
            return null;
        }

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

        public static string IdList2Config(List<string> idList,bool merge = true)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            if (merge)
            {
                //转成 x*number的形式
                Dictionary<string, int> countMap = new Dictionary<string, int>();
                foreach (var s in idList)
                {
                    if (countMap.ContainsKey(s))
                    {
                        countMap[s]++;
                    }
                    else
                    {
                        countMap.Add(s, 1);
                    }
                }
                foreach (var pair in countMap)
                {
                    if (!isFirst)
                    {
                        sb.Append(SplitChar);
                    }
                    else
                    {
                        isFirst = false;
                    }

                    if (pair.Value > 1)
                    {
                        sb.Append(string.Format("{0}*{1}", pair.Key, pair.Value));
                    }
                    else
                    {
                        //缺省形式
                        sb.Append(pair.Key);
                    }
                }
            }
            else
            {
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
            }
            return sb.ToString();
        }

        public static List<string> Config2IdList(string data)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(data))
            {
                string[] srr = data.Split(SplitChar);
                foreach (var s in srr)
                {
                    string[] exp = s.Split(MergeChar);
                    int count = 1;
                    string id = s;
                    if (exp.Length > 1)
                    {
                        id = exp[0];
                        count = int.Parse(exp[1]);
                    }
                    for (int i = 0; i < count; i++)
                    {
                        list.Add(id);
                    }
                }
            }
            return list;
        }

        public static List<T> Config2SlkList<T>(string data) where T:SlkDataObject
        {
            List<T> list = new List<T>();
            var idList = Config2IdList(data);
            foreach (var id in idList)
            {
                T slkObj = SlkManager.Instance.GetSlkData<T>(id) as T;
                if (slkObj != null)
                    list.Add(slkObj);
            }
            return list;
        }

        public const char SplitChar = ';';
        public const char MergeChar = '*';

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
            if (string.IsNullOrEmpty(data)) return pool;
            string[] srr = data.Split(SplitChar);
            foreach (var s in srr)
            {
                string[] pair = s.Split('|');
                if(pair.Length>1)
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
