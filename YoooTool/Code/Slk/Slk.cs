using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code.Utils;

namespace YoooTool.Code.Slk
{

    public class SlkParseUtil
    {
        //TODO 专门定义用来存储SLK的LIST类？

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

    public interface ISlkSerialize
    {
        string Slk_Serialize();
        void Slk_DeSerialize(object data);
    }

    public interface IExport2Jass
    {
        string GetJass();
    }


    /// <summary>
    /// 避免Index相同！
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SlkPropertyAttribute : Attribute
    {
        public int Index { get; private set; }
        
        public SlkPropertyAttribute( int index)
        {
            Index = index;
        }
    }

    public abstract class SlkDataObject :ISlkSerialize
    {
        /// <summary>
        /// 索引ID
        /// </summary>
        [SlkProperty(-1)]
        public string Id { get; set; }
        
        public static string GetProperty2Csv(Type t)
        {
            StringBuilder sb = new StringBuilder();
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public ).ToList();
            //无标签的无效
            properties.RemoveAll((info => info.GetCustomAttribute<SlkPropertyAttribute>() == null));

            properties.Sort((p1, p2) =>
            {
                var p1Index = p1.GetCustomAttribute<SlkPropertyAttribute>();
                var p2Index = p2.GetCustomAttribute<SlkPropertyAttribute>();
                var p1i = p1Index != null ? p1Index.Index : int.MaxValue;
                var p2i = p2Index != null ? p2Index.Index : int.MaxValue;
                return p1i - p2i;
            });
            
            bool isFirst = true;
            foreach (var fieldInfo in properties)
            {
                if (!isFirst)
                {
                    sb.Append(",");
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(string.Format("{0}", fieldInfo.Name));
            }
            return sb.ToString();
        }
        /// <summary>
        /// SLK Serialize主要使用的方法
        /// </summary>
        /// <returns></returns>
        public string GetProperty2Csv()
        {
            StringBuilder sb = new StringBuilder();
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //无标签的无效
            properties.RemoveAll((info => info.GetCustomAttribute<SlkPropertyAttribute>() == null));
            properties.Sort((p1, p2) =>
            {
                var p1Index = p1.GetCustomAttribute<SlkPropertyAttribute>();
                var p2Index = p2.GetCustomAttribute<SlkPropertyAttribute>();
                var p1i = p1Index != null ? p1Index.Index : int.MaxValue;
                var p2i = p2Index != null ? p2Index.Index : int.MaxValue;
                return p1i-p2i;
            });
            bool isFirst = true;
            foreach (var fieldInfo in properties)
            {
                if (!isFirst)
                {
                    sb.Append(",");
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(string.Format("{0}",GetPropertyValueConfig(fieldInfo,this)));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 决定转换
        /// </summary>
        /// <param name="info"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetPropertyValueConfig(PropertyInfo info,object obj)
        {
            var type = info.PropertyType;
            var value = info.GetValue(obj);
            if (type.IsClass)
            {
                if (type == typeof(List<string>))//ID 池
                {
                    return SlkParseUtil.IdList2Config((List<string>)value);
                }
                if (type == typeof(RandomWeightPool<string>))//ID 池
                {
                    return SlkParseUtil.IdPool2Config((RandomWeightPool<string>)value);
                }
                else if (type.IsSubclassOf(typeof(SlkDataObject)))
                {
                    //--引用SLKData對象就返回ID--
                    return ((SlkDataObject) value).Id;
                }
            }
            return value.ToString();
        }


        public abstract string Slk_Serialize();

        public abstract void Slk_DeSerialize(object data);
    }

    /// <summary>
    /// 交互对象
    /// </summary>
    public class SLK_Interactive : SlkDataObject
    {
        [SlkProperty(1)]
        public string WeUnitTypeId { get; set; }
        [SlkProperty(2)]
        public string Desc { get; set; }

        public override string Slk_Serialize()
        {
            return GetProperty2Csv();
        }

        public override void Slk_DeSerialize(object data)
        {
            string[] srr = (string[])data;
            if (srr != null)
            {
                Id = srr[0];
                WeUnitTypeId = srr[1];
                Desc = srr[2];
            }
        }
    }

    public class SLK_Unit : SlkDataObject
    {
        [SlkProperty(1)]
        public string WeUnitTypeId { get; set; }
        [SlkProperty(2)]
        public string Desc { get; set; }

        public override string Slk_Serialize()
        {
            return GetProperty2Csv();
        }

        public override void Slk_DeSerialize(object data)
        {
            string[] srr = (string[]) data;
            if (srr != null)
            {
                Id = srr[0];
                WeUnitTypeId = srr[1];
                Desc = srr[2];
            }
        }
    }

    public class SLK_EnemyGroup : SlkDataObject
    {
        [SlkProperty(1)]
        public List<string> EnemyList { get; set; }

        public override string Slk_Serialize()
        {
            return GetProperty2Csv();
        }

        public override void Slk_DeSerialize(object data)
        {
            string[] srr = (string[])data;
            if (srr != null)
            {
                Id = srr[0];
                //拆分content
                EnemyList = SlkParseUtil.Config2IdList(srr[1]);
            }
        }
    }

    public class SLK_EnemySpawnner :SlkDataObject
    {
        [SlkProperty(1)]
        public float LastTime { get; set; }
        [SlkProperty(2)]
        public float Interval { get; set; }
        [SlkProperty(3)]
        public RandomWeightPool<string> EnemyIdPool { get; set; }

        public override string Slk_Serialize()
        {
            return GetProperty2Csv();
        }

        public override void Slk_DeSerialize(object data)
        {
            string[] srr = (string[])data;
            if (srr != null)
            {
                Id = srr[0];
                LastTime = float.Parse(srr[1]);
                Interval = float.Parse(srr[2]);
                EnemyIdPool = SlkParseUtil.Config2IdPool(srr[3]);
            }
        }
    }

    public class SlkData_Handler<T> : ISlkSerialize where T: SlkDataObject 
    {
        protected Dictionary<string,T> IdMap = new Dictionary<string, T>();

        public T GetData(string id)
        {
            T t = null;
            if (IdMap.TryGetValue(id, out t))
            {
                return t;
            }
            return null;
        }

        public bool AddData(T data)
        {
            if (data == null) return false;
            if (IdMap.ContainsKey(data.Id))
            {
                return false;
            }
            IdMap.Add(data.Id,data);
            return true;
        }

        #region ISlkSerialize
        //JSON XML CSV ??

        public string Slk_Serialize()
        {
            StringBuilder sb = new StringBuilder();
            var title = SlkDataObject.GetProperty2Csv(typeof(T));
            sb.AppendLine(title);
            foreach (var pair in IdMap)
            {
                sb.AppendLine(pair.Value.Slk_Serialize());
            }
            return sb.ToString();
        }
        public void Slk_DeSerialize(object data)
        {
            IdMap.Clear();
            CsvStreamReader reader = CsvStreamReader.CreateReader(data.ToString(),Encoding.UTF8);
            bool isTitleLine = true;
            for (int i = 1; i <= reader.RowCount; i++)
            {
                if (isTitleLine)
                {
                    isTitleLine = false;
                    continue;
                }
                string[] srr = new string[reader.ColCount];
                for (int j = 1; j <= reader.ColCount; j++)
                {
                    srr[j-1] = reader[i,j];
                }
                SlkDataObject unit = new SLK_Unit();
                unit.Slk_DeSerialize(srr);
                AddData((T) unit);
            }
        }
        #endregion
        
    }


    public class SLK_Room : SlkDataObject, IExport2Jass
    {
        //alive room = enemySpawnner
        //battle room = enemyGroup
        //interactive room = 

        public SlkDataObject GetSlkData()
        {
            //by key and id to get slk data
            return null;
        }
        
        /// <summary>
        /// 表示类型 /现在先不需要 直接整合到ID中
        /// </summary>
        [SlkProperty(1)]
        public string Key { get; set; }
        /// <summary>
        /// 引用的一个配置 解析的时候根据ID得到具体类型的配置
        /// SLK_EnemyGroup / SLK_EnemySpawnner
        /// </summary>
        [SlkProperty(2)]
        public string ConfigId { get; set; }
        
        [SlkProperty(3)]
        public string Desc { get; set; }

        public override string Slk_Serialize()
        {
            return GetProperty2Csv();
        }

        public override void Slk_DeSerialize(object data)
        {
            string[] srr = (string[])data;
            if (srr != null)
            {
                Id = srr[0];
                Key = srr[1];
                ConfigId = srr[2];
                Desc = srr[3];
            }
        }

        public string GetJass()
        {
            
            return null;
        }
    }
    



    public class Level
    {
        //list of rooms

        public List<string> RoomList = new List<string>();

        public void Export2Jass()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < RoomList.Count; i++)
            {
                SLK_Room room = null;//TODO GET SLK DATA
                string roomJass = room.GetJass();
                sb.AppendLine(string.Format("set dataArr[{0}] = \"{1}\"", i, ""));
            }
            sb.AppendLine(string.Format("RecordConfig({0})",RoomList.Count));
        }
    }

    public class Slk
    {
        public static SlkData_Handler<SLK_Unit> UnitTab { get; set; } = new SlkData_Handler<SLK_Unit>();
        public static SlkData_Handler<SLK_EnemySpawnner> SpawnnerTab { get; set; } = new SlkData_Handler<SLK_EnemySpawnner>();
        public static SlkData_Handler<SLK_EnemyGroup> EnemyGroupTab { get; set; } = new SlkData_Handler<SLK_EnemyGroup>();
        public static SlkData_Handler<SLK_Room> RoomTab { get; set; } = new SlkData_Handler<SLK_Room>();

        public void Init()
        {
            string folder = "";
            UnitTab.Slk_DeSerialize(folder + "SLK_Unit.csv");
            SpawnnerTab.Slk_DeSerialize(folder + "SLK_Spawnner.csv");
            EnemyGroupTab.Slk_DeSerialize(folder + "SLK_EnemyGroup.csv");
            RoomTab.Slk_DeSerialize(folder + "SLK_Room.csv");
        }

        public T GetSlkData<T>(string id) where T:SlkDataObject
        {
            if (typeof(T) == typeof(SLK_Unit))
            {
                return UnitTab.GetData(id) as T;
            }else if (typeof(T) == typeof(SLK_EnemySpawnner))
            {
                return SpawnnerTab.GetData(id) as T;
            }
            else if (typeof(T) == typeof(SLK_EnemyGroup))
            {
                return EnemyGroupTab.GetData(id) as T;
            }
            return null;
        }


        public void Test()
        {
            SlkData_Handler<SLK_Unit> unitTab = new SlkData_Handler<SLK_Unit>();
            unitTab.AddData(new SLK_Unit() {Id = "Unit_1", WeUnitTypeId = "'e000'",Desc = "small elf"});
            File.WriteAllText("SLK_Unit.csv", unitTab.Slk_Serialize());
            
            SlkData_Handler<SLK_EnemySpawnner> spawnnerTab = new SlkData_Handler<SLK_EnemySpawnner>();
            spawnnerTab.AddData(new SLK_EnemySpawnner()
            {
                EnemyIdPool = new RandomWeightPool<string>()
                    .SetItemWeight("enemy_1", 10)
                    .SetItemWeight("enemy_2", 10)
                    .SetItemWeight("bigEnemy_1", 5)
                    .SetItemWeight("bigEnemy_2", 5)
                ,
                Id = "EnemySpawnner_1",
                Interval = 2,
                LastTime = 30,
            });
            File.WriteAllText("SLK_EnemySpawnner.csv", spawnnerTab.Slk_Serialize());

            SlkData_Handler<SLK_EnemyGroup> enemyGroupTab = new SlkData_Handler<SLK_EnemyGroup>();
            enemyGroupTab.AddData(new SLK_EnemyGroup()
            {
                Id = "EnemyGroup_1",
                EnemyList = new List<string>()
                {
                    "enemy_1",
                    "enemy_1",
                    "enemy_2",
                    "enemy_2",
                }
            });
            File.WriteAllText("SLK_EnemyGroup.csv", enemyGroupTab.Slk_Serialize());

            SlkData_Handler<SLK_Room> roomTab = new SlkData_Handler<SLK_Room>();
            roomTab.AddData(new SLK_Room()
            {
                Id = "Room_1",
                Key = "B",
                ConfigId = "EnemyGroup_1",
                Desc = "Battle"
            });
            File.WriteAllText("SLK_Room.csv", roomTab.Slk_Serialize());

            return;
            SlkData_Handler<SLK_Unit> newTab = new SlkData_Handler<SLK_Unit>();
            newTab.Slk_DeSerialize("SLK_Unit.csv");
            Console.WriteLine("====================");
            Console.WriteLine(newTab.GetData("1").Desc);
        }
    }
}
