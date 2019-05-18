using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code.Utils;

namespace YoooTool.Code.Slk
{

    public interface ISlkSerialize
    {
        string Slk_Serialize();
        void Slk_DeSerialize(object data);
        void Slk_LateDeSerialize(object data);
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

    #region SLK_DataObject Classes


    /// <summary>
    /// 交互对象
    /// </summary>
    public class SLK_Interact : SlkDataObject
    {
        [SlkProperty(1)]
        public string WeUnitTypeId { get; set; }
        
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
            }
        }
        public override string GetJass()
        {
            return WeUnitTypeId;
        }
    }

    public class SLK_Unit : SlkDataObject
    {
        [SlkProperty(1)]
        public string WeUnitTypeId { get; set; }
        [SlkProperty(2)]
        public int CombatPower { get; set; }
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
                CombatPower = SlkParseUtil.Parse2Int(srr[2]);
            }
        }

        public override string GetJass()
        {
            return WeUnitTypeId;
        }
    }

    public class SLK_EnemyGroup : SlkDataObject
    {
        [SlkProperty(1)]
        public int Level { get; set; }
        [SlkProperty(2)]
        public int Index { get; set; }
        [SlkProperty(3)]
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
                Level = SlkParseUtil.Parse2Int(srr[1]);
                Index = SlkParseUtil.Parse2Int(srr[2]);
                //拆分content
                EnemyList = SlkParseUtil.Config2IdList(srr[3]);
            }
        }

        public override string GetJass()
        {
            string jass = string.Format("{0}@{1}", Level, Index);
            return String.Format("{0}#{1}", "EnemyGroup", jass);
        }
    }

    public class SLK_EnemySpawnner : SlkDataObject
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
                LastTime = SlkParseUtil.Parse2Float(srr[1]);
                Interval = SlkParseUtil.Parse2Float(srr[2]);
                EnemyIdPool = SlkParseUtil.Config2IdPool(srr[3]);
            }
        }

        public override string GetJass()
        {
            //对应WE解析所需
            string jass = string.Format("{0}@{1}@{2}", LastTime, Interval,Id );//SlkParseUtil.IdPool2Config(EnemyIdPool)
            return String.Format("{0}#{1}", "EnemySpawn", jass);
            //return Id;
        }
    }


    public enum RuleType
    {
        Battle,
        RandomBattle,
        Alive,
        Interact,
        RandomInteract,
    }
    public class SLK_RoomRule : SlkDataObject
    {
        [SlkProperty(1)]
        public RuleType Type { get; set; }
        [SlkProperty(2)]
        public string Parameters { get; set; }
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
                RuleType pType;
                Enum.TryParse(srr[1], out pType);
                Type = pType;
                Parameters = srr[2];
            }
            //throw new Exception("Must Override This");
        }
        public override string GetJass()
        {
            return string.Format("{0}#{1}", Type, Parameters);
        }
    }
    public class SLK_Room : SlkDataObject
    {
        /// <summary>
        /// 引用的一个配置 解析的时候根据ID得到具体类型的配置
        /// SLK_EnemyGroup / SLK_EnemySpawnner
        /// </summary>
        [SlkProperty(1)]
        public string ConfigId { get; set; }
        
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
                ConfigId = srr[1];
            }
        }

        public override string GetJass()
        {
            //直接就是引用的
            return SlkParseUtil.GetIdRefObjectJass<SLK_RoomRule>(ConfigId);
        }
    }
    public class SLK_Level : SlkDataObject
    {
        /// <summary>
        /// 是否随机打乱顺序
        /// </summary>
        [SlkProperty(1)]
        public bool IsRandom { get; set; }
        /// <summary>
        /// 包含的房间列表
        /// </summary>
        [SlkProperty(2)]
        public List<string> RefRooms { get; set; }

        [SlkProperty(3)]
        public List<SLK_Room> RefRooms_Ins { get; set; }
        /* TODO 将来实现
        /// <summary>
        /// 通过所需要的最小房间数量
        /// </summary>
        public int MinRoomCount { get; set; }
        /// <summary>
        /// 任务房间-必须通过的房间
        /// </summary>
        public string MissionRoom { get; set; }
        /// <summary>
        /// 奖励等级
        /// </summary>
        public int RewardLevel { get; set; }
        */
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
                IsRandom = SlkParseUtil.Parse2Bool(srr[1]);
                RefRooms = SlkParseUtil.Config2IdList(srr[2]);
            }
        }

        public override void Slk_LateDeSerialize(object data)
        {
            string[] srr = (string[])data;
            if (srr != null)
            {
                Console.WriteLine("----------------");
                RefRooms_Ins = SlkParseUtil.Config2SlkList<SLK_Room>(srr[3]);
                foreach (var refRoomsIn in RefRooms_Ins)
                {
                    Console.WriteLine(refRoomsIn.Id);
                }
            }
        }

        public override string GetJass()
        {
            //没人会引用Level 不需要实现了
            return "";
        }

        public string GetJassConfig()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < RefRooms.Count; i++)
            {
                sb.AppendLine(string.Format("set DungeonLevel_dataArr[{0}] = \"{1}\"", i + 1, SlkParseUtil.GetIdRefObjectJass<SLK_Room>(RefRooms[i])));
            }
            sb.AppendLine(string.Format("call RecordConfig({0},{1})", RefRooms.Count, this.IsRandom.ToString().ToLower()));
            return sb.ToString();
        }
    }
    #endregion
    public class ExportHelper
    {
        public void ExportAllLevel2Jass(bool split = false)
        {
            var levels = SlkManager.Instance.LevelTab.GetAllData();
            if (split)
            {
                foreach (var slkLevel in levels)
                {
                    File.WriteAllText("LevelCfg/" + slkLevel.Id + ".jass", slkLevel.GetJassConfig());
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var slkLevel in levels)
                {
                    sb.AppendLine(string.Format("//------{0}------",slkLevel.Id));
                    sb.AppendLine(slkLevel.GetJassConfig());
                }
                File.WriteAllText("LevelCfg/" + "AllLevel" + ".jass", sb.ToString());
            }
        }
        
        public void ExportEnemySpawnner2Jass()
        {
            //导出Spawnner 的jass
            StringBuilder sb = new StringBuilder();
            var list = SlkManager.Instance.EnemySpawnnerTab.GetAllData();
            for (int i = 0; i < list.Count; i++)
            {
                var spawnner = list[i];
                var poolName = spawnner.Id;
                var lastTime = spawnner.LastTime.ToString("f2");
                var interval = spawnner.Interval.ToString("f2");
                sb.AppendLine(string.Format("//--ConfigBegin--{0}--", poolName));
                foreach (var pair in spawnner.EnemyIdPool.GetMapCopy())
                {
                    var data = SlkParseUtil.GetIdRefObjectJass<SLK_Unit>(pair.Key);
                    var weight = pair.Value.ToString("f2");
                    sb.AppendLine(string.Format("call WeightPoolLib_RegistPool_Int(\"{0}\",{1},{2})", poolName, data, weight));
                }
                sb.AppendLine(string.Format("call RecordSpawnnerCfg(\"{0}\",{1},{2})", poolName, lastTime, interval));
                sb.AppendLine(string.Format("//--ConfigEnd--{0}--", poolName));
            }
            File.WriteAllText("EnemySpawnner.jass", sb.ToString());
        }

        public void ExportEnemyGroup2Jass()
        {
            //导出EnemyGroup
            //EnemyGroup需要重新审视..实际JASS引用的是2个坐标，再存一个ID映射2个坐标？
            StringBuilder sb = new StringBuilder();
            var list = SlkManager.Instance.EnemyGroupTab.GetAllData();
            for (int i = 0; i < list.Count; i++)
            {
                var data = list[i];
                for (int j = 0; j < data.EnemyList.Count; j++)
                {
                    var enemy = SlkParseUtil.GetIdRefObjectJass<SLK_Unit>(data.EnemyList[j]);
                    sb.AppendLine(string.Format("set dataArr[{0}] = {1}", j + 1, enemy));
                }
                sb.AppendLine(string.Format("set dataLength = {0}", data.EnemyList.Count));
                sb.AppendLine(string.Format("call RecordCurrentToLevel({0})", data.Level));
            }
            File.WriteAllText("EnemyGroup.jass", sb.ToString());
        }
    }

    public interface ISlkData_Handler
    {
        string Handler_Serialize();
        void Handler_DeSerialize(object data = null);
        /// <summary>
        /// 处理其他实例引用
        /// </summary>
        void Handler_LateDeSerialize();

    }
    public class SlkData_Handler<T> : ISlkData_Handler where T: SlkDataObject
    {
        //reflect call
        public void RegistToMap(Dictionary<string, SlkDataObject> map)
        {
            //add type as key ext? typeof(T).Name +
            foreach (var pair in IdMap)
            {
                string fkey = SlkManager.SlkIdSearchFix<T>(pair.Key);
                map.Add(fkey,pair.Value);
            }
        }

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

        public List<T> GetAllData()
        {
            return IdMap.Values.ToList();
        }

        #region Serialize

        public string Handler_Serialize()
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

        private CsvStreamReader reader;
        public void Handler_DeSerialize(object data = null)
        {
            if (data == null)
            {
                data = GetType().GetGenericArguments()[0].GetType().Name + ".csv";
            }
            Console.WriteLine(data);
            IdMap.Clear();
            reader = CsvStreamReader.CreateReader(data.ToString(),Encoding.UTF8);
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
                T t = SlkDataObjectFactory<T>.Create();
                t.Slk_DeSerialize(srr);
                AddData(t);
            }
        }

        public void Handler_LateDeSerialize()
        {
            if (reader == null)
            {
                return;
            }
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
                    srr[j - 1] = reader[i, j];
                }
                var slk = GetAllData()[i - 2];
                slk.Slk_LateDeSerialize(srr);
            }
        }
        #endregion

    }

    public class SlkManager
    {
        public event Action OnDeserialize;
        protected virtual void ExcuteOnDeserialize()
        {
            OnDeserialize?.Invoke();
        }
        public  SlkData_Handler<SLK_Interact> InteractTab { get; set; } = new SlkData_Handler<SLK_Interact>();
        public  SlkData_Handler<SLK_Unit> UnitTab { get; set; } = new SlkData_Handler<SLK_Unit>();
        public  SlkData_Handler<SLK_EnemySpawnner> EnemySpawnnerTab { get; set; } = new SlkData_Handler<SLK_EnemySpawnner>();
        public  SlkData_Handler<SLK_EnemyGroup> EnemyGroupTab { get; set; } = new SlkData_Handler<SLK_EnemyGroup>();
        public SlkData_Handler<SLK_RoomRule> RoomRuleTab { get; set; } = new SlkData_Handler<SLK_RoomRule>();
        public  SlkData_Handler<SLK_Room> RoomTab { get; set; } = new SlkData_Handler<SLK_Room>();
        public SlkData_Handler<SLK_Level> LevelTab { get; set; } = new SlkData_Handler<SLK_Level>();
        
        public static SlkManager Instance { get; private set; }

        public static SlkManager CreateInstance()
        {
            if (Instance == null)
            {
                Instance = new SlkManager();
                Instance.Init();
            }
            return Instance;
        }

        public void Init()
        {
            //文件名和SLK类名相同
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            foreach (var propertyInfo in properties)
            {
                string file = propertyInfo.PropertyType.GetGenericArguments()[0].Name + ".csv";
                Console.WriteLine("Handler_DeSerialize : " + file);
                var des = propertyInfo.PropertyType.GetMethod("Handler_DeSerialize", BindingFlags.Public | BindingFlags.Instance);
                des.Invoke(propertyInfo.GetValue(this), new object[] { file });
            }
            SearchMapInit();
            ExcuteOnDeserialize();
            foreach (var propertyInfo in properties)
            {
                var des = propertyInfo.PropertyType.GetMethod("Handler_LateDeSerialize", BindingFlags.Public | BindingFlags.Instance);
                des.Invoke(propertyInfo.GetValue(this), new object[] {  });
                Console.WriteLine("Handler_LateDeSerialize : " + propertyInfo.Name);
            }
        }
        #region Search Map

        public static string SlkIdSearchFix<T>(string id) where T : SlkDataObject
        {
            return typeof(T).Name + "@" + id;
        }

        public static Type SlkIdSearchFix2Type(string fixId)
        {
            int end = fixId.IndexOf("@", StringComparison.Ordinal);
            string typeName = fixId.Substring(0, end);
            return Assembly.GetAssembly(typeof(SlkDataObject)).GetType(typeof(SlkDataObject).Namespace + "." + typeName);
        }
        //ID会自动添加类型作为前缀
        protected Dictionary<string, SlkDataObject> SlkIdSearchMap = new Dictionary<string, SlkDataObject>();

        void SearchMapInit()
        {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            foreach (var propertyInfo in properties)
            {
                //GetGenericTypeDefinition
                var reg = propertyInfo.PropertyType.GetMethod("RegistToMap", BindingFlags.Public | BindingFlags.Instance);
                reg.Invoke(propertyInfo.GetValue(this), new object[] { SlkIdSearchMap });
            }
        }


        #endregion

        public SlkDataObject GetSlkData<T>(string cfgId) where T : SlkDataObject
        {
            SlkDataObject data;
            if (!typeof(T).IsSubclassOf(typeof(SlkDataObject)))
            {
                //说明传入的就是 SlkDataObject -此时丢失了参考-
                Console.WriteLine(string.Format("[Error] Not Find Id: {0} / {1}",cfgId, "Can't Find SlkDataObject as T"));
                var list = SlkIdSearchMap.Keys.Where(s => s.Contains(cfgId)).ToList();
                string wantList = "";
                foreach (var ids in list)
                {
                    wantList += string.Format("[{0}]", SlkIdSearchFix2Type(ids)?.Name);
                }
                Console.WriteLine(string.Format("[{0}] You May Want {1}", "Suggest",wantList));
                //尝试搜索？
                return null;
            }
            SlkIdSearchMap.TryGetValue(SlkIdSearchFix<T>(cfgId), out data);
            if (data == null)
            {
                Console.WriteLine(string.Format("[Error] Not Find Id: {0} / {1}",cfgId,typeof(T)));
            }
            return data;
        }

        //--给UI来操作--
        public void ModifyId()
        {
            //修改一个SLK的ID，并且更新引用相关？
        }

        public void SaveOut()
        {
            
        }


        public void OutPutTest()
        {
            SlkData_Handler<SLK_Interact> interTab = new SlkData_Handler<SLK_Interact>();
            interTab.AddData(new SLK_Interact() { Id = "Interact_1", WeUnitTypeId = "'e000'" });
            interTab.AddData(new SLK_Interact() { Id = "Interact_2", WeUnitTypeId = "'e001'"});
            File.WriteAllText("SLK_Interact.csv", interTab.Handler_Serialize());
            return;
            SlkData_Handler<SLK_Unit> unitTab = new SlkData_Handler<SLK_Unit>();
            unitTab.AddData(new SLK_Unit() { Id = "enemy_1", WeUnitTypeId = "'e000'",});
            unitTab.AddData(new SLK_Unit() { Id = "enemy_2", WeUnitTypeId = "'e001'",});
            unitTab.AddData(new SLK_Unit() { Id = "bigEnemy_1", WeUnitTypeId = "'e002'", });
            unitTab.AddData(new SLK_Unit() { Id = "bigEnemy_2", WeUnitTypeId = "'e003'", });
            File.WriteAllText("SLK_Unit.csv", unitTab.Handler_Serialize());

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
            File.WriteAllText("SLK_EnemySpawnner.csv", spawnnerTab.Handler_Serialize());

            SlkData_Handler<SLK_EnemyGroup> enemyGroupTab = new SlkData_Handler<SLK_EnemyGroup>();
            enemyGroupTab.AddData(new SLK_EnemyGroup()
            {
                Id = "EnemyGroup_1",
                Level = 1,
                Index = 1,
                EnemyList = new List<string>()
                {
                    "enemy_1",
                    "enemy_1",
                    "enemy_2",
                    "enemy_2",
                }
            });
            File.WriteAllText("SLK_EnemyGroup.csv", enemyGroupTab.Handler_Serialize());

            SlkData_Handler<SLK_Room> roomTab = new SlkData_Handler<SLK_Room>();
            roomTab.AddData(new SLK_Room()
            {
                Id = "Room_1",
                ConfigId = "EnemyGroup_1",
            });
            File.WriteAllText("SLK_LevelRoom.csv", roomTab.Handler_Serialize());

            return;
            SlkData_Handler<SLK_Unit> newTab = new SlkData_Handler<SLK_Unit>();
            newTab.Handler_DeSerialize("SLK_Unit.csv");
            Console.WriteLine("====================");
        }

        
    }
}
