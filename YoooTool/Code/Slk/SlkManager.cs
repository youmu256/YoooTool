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
    //避免把其他复DataObject作为需要序列化的属性。使用对象的Id,当做string来序列化
    //反序列化的时候可以再延迟反序列化中重新赋值对象 ----如果有需要
    //

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
            return "'"+WeUnitTypeId+"'";
        }
    }
    
    public class SLK_Spawnner : SlkDataObject
    {
        [SlkProperty(1)]
        public float PoolLastTime { get; set; }
        [SlkProperty(2)]
        public float PoolInterval { get; set; }
        [SlkProperty(3)]
        public RandomWeightPool<string> UnitPool { get; set; }
        [SlkProperty(4)]
        public float ListInterval { get; set; }
        [SlkProperty(5)]
        public List<string> UnitList { get; set; }
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
                PoolLastTime = SlkParseUtil.Parse2Float(srr[1]);
                PoolInterval = SlkParseUtil.Parse2Float(srr[2]);
                UnitPool = SlkParseUtil.Config2IdPool(srr[3]);
                ListInterval = SlkParseUtil.Parse2Float(srr[4]);
                UnitList = SlkParseUtil.Config2IdList(srr[5]);
            }
        }

        public override string GetJass()
        {
            //并不需要导出 实际JASS中整个Spawnner会再抽象成一个ID，指向该配置
            //对应WE解析所需
            return "Not Implement GetJass Method";
            string jass = string.Format("{0}@{1}@{2}", PoolLastTime, PoolInterval,Id );
            return String.Format("{0}#{1}", GetLabel(), jass);
            //return Id;
        }
    }

    public enum RuleType
    {
        Battle,//战斗
        Interact,//交互事件
        Rest,//休息
        Shop,//商店
        RandomEvent,//随机事件 没的选
    }
    public class SLK_Room : SlkDataObject
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
            //直接就是引用的
            return string.Format("{0}#{1}", Type, Parameters);
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

        //[SlkProperty(3)]
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
            if (RefRooms != null)
            {
                //Ins实例转序列化数据
                RefRooms.Clear();
                foreach (var room in RefRooms_Ins)
                {
                    RefRooms.Add(room.Id);
                }
            }
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
                //引用Ins类型没有保存出去的数据
                //存在的意义是什么呢？在代码中修改的时候，能按照引用正确记录？比如改名
                RefRooms_Ins = SlkParseUtil.Config2SlkList<SLK_Room>(srr[2]);
                foreach (var refRoomsIn in RefRooms_Ins)
                {
                    Console.WriteLine(refRoomsIn.Id);
                }
                Console.WriteLine("----------------");
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
                sb.AppendLine(string.Format("set dataArr[{0}] = \"{1}\"", i + 1, SlkParseUtil.GetIdRefObjectJass<SLK_Room>(RefRooms[i])));
                //sb.AppendLine(string.Format("set DungeonLevel_dataArr[{0}] = \"{1}\"", i + 1, SlkParseUtil.GetIdRefObjectJass<SLK_Room>(RefRooms[i])));
            }
            sb.AppendLine(string.Format("call RecordConfig({0},{1})", RefRooms.Count, this.IsRandom.ToString().ToLower()));
            return sb.ToString();
        }
    }

    /// <summary>
    /// 不一定对应真正的一个物品
    /// 比如金币 经验都可以作为一个LootItem
    /// 金币单位为100 经验书
    /// </summary>
    public class SLK_LootItem : SlkDataObject
    {
        [SlkProperty(1)]
        public string WeId { get; set; }

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
                WeId = srr[1];
            }
        }

        public override string GetJass()
        {
            return "'" + WeId + "'";
        }
    }

    /// <summary>
    /// 奖励
    /// 可以用开宝箱的形式，在宝箱附近刷出LootItem
    /// </summary>
    public class SLK_Loot : SlkDataObject
    {
        //固定
        [SlkProperty(1)]
        public List<string> Items { get; set; }
        [SlkProperty(2)]
        public int ConstCount { get; set; }
        //随机
        [SlkProperty(3)]
        public RandomWeightPool<string> ItemPool { get; set; }
        [SlkProperty(4)]
        public int RandomCount { get; set; }
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
                Items = SlkParseUtil.Config2IdList(srr[1]);
                ConstCount = SlkParseUtil.Parse2Int(srr[2]);
                ItemPool = SlkParseUtil.Config2IdPool(srr[3]);
                RandomCount = SlkParseUtil.Parse2Int(srr[4]);
            }
        }

        public override string GetJass()
        {
            return Id;
            //string jass = string.Format("{0}@{1}@{2}", LastTime, Interval, Id);//SlkParseUtil.IdPool2Config(EnemyIdPool)
            //return String.Format("{0}#{1}", "Loot", jass);
        }
        //导出配置
        public string GetJassConfig()
        {
            StringBuilder sb = new StringBuilder();
            return sb.ToString();
        }
    }

    #endregion


    public class SlkManager
    {
        public event Action OnDeserialize;
        protected virtual void ExcuteOnDeserialize()
        {
            OnDeserialize?.Invoke();
        }
        public  SlkData_Handler<SLK_Interact> InteractTab { get; set; } = new SlkData_Handler<SLK_Interact>();
        public  SlkData_Handler<SLK_Unit> UnitTab { get; set; } = new SlkData_Handler<SLK_Unit>();
        public  SlkData_Handler<SLK_Spawnner> UnitSpawnnerTab { get; set; } = new SlkData_Handler<SLK_Spawnner>();
        public  SlkData_Handler<SLK_Room> RoomTab { get; set; } = new SlkData_Handler<SLK_Room>();
        public SlkData_Handler<SLK_Level> LevelTab { get; set; } = new SlkData_Handler<SLK_Level>();
        public SlkData_Handler<SLK_Loot> LootTab { get; set; } = new SlkData_Handler<SLK_Loot>();
        public SlkData_Handler<SLK_LootItem> LootItemTab { get; set; } = new SlkData_Handler<SLK_LootItem>();


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
                //string file = propertyInfo.PropertyType.GetGenericArguments()[0].Name + ".csv";
                //Console.WriteLine("Handler_DeSerialize : " + file);
                Console.WriteLine("Handler_DeSerialize : " + propertyInfo.PropertyType.GetGenericArguments()[0].Name);
                var des = propertyInfo.PropertyType.GetMethod("Handler_DeSerialize", BindingFlags.Public | BindingFlags.Instance);
                des.Invoke(propertyInfo.GetValue(this), new object[] { null });
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

        /// <summary>
        /// 保存出所有SLK数据
        /// </summary>
        public void SaveOut()
        {
            return;
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            foreach (var propertyInfo in properties)
            {
                string file = ExportHelper.GetPathFileName("save_"+propertyInfo.PropertyType.GetGenericArguments()[0].Name,".csv");
                Console.WriteLine("Save... Handler_Serialize : " + file);
                var des = propertyInfo.PropertyType.GetMethod("Handler_Serialize", BindingFlags.Public | BindingFlags.Instance);
                string content = des.Invoke(propertyInfo.GetValue(this), new object[] { }) as string;
                File.WriteAllText(file, content);
            }
        }
    }
}
