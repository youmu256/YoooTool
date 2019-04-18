using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code.Utils;
using YoooTool.Code.Slk;
namespace YoooTool.Code
{

    /// <summary>
    /// 生成给WE用的配置字符串
    /// 解析部分用LUA预编译成JASS
    /// </summary>
    public interface I2WeConfig
    {
        string ToConfig();
    }
    

    

    public class SLK
    {
        //TODO GetTab With Type And Get Tab's Data By Id
        public class UnitType_Tab
        {
            public static UnitTypeObject Enemy1 = new UnitTypeObject() {Id = "1",Name = "monster1"};
            public static UnitTypeObject Enemy2 = new UnitTypeObject() {Id = "2",Name = "monster2"};
            public static UnitTypeObject BigEnemy1 = new UnitTypeObject() { Id = "3", Name = "bigMonster1" };
            public static UnitTypeObject BigEnemy2 = new UnitTypeObject() { Id = "4", Name = "bigMonster2" };
            public static UnitTypeObject Boss1 = new UnitTypeObject() { Id = "5", Name = "boss1" };

        }

        public class Interactive_Tab
        {
            public static InteractiveObject BookShelf = new InteractiveObject() { Id = "0", Desc = "书架" };
            public static InteractiveObject TreasureBox = new InteractiveObject() { Id = "1", Desc = "宝箱" };
        }

        public class EnemySpawnner_Tab
        {
            public static EnemySpawnner Spawnner1 = new EnemySpawnner()
            {
                LastTime = 10,
                SpawnInterval = 1,
                EnemyTypePool = new RandomWeightPool<UnitTypeObject>()
                .SetItemWeight(UnitType_Tab.Enemy1, 5)
                .SetItemWeight(UnitType_Tab.Enemy2, 5)
                .SetItemWeight(UnitType_Tab.BigEnemy1, 2)
                .SetItemWeight(UnitType_Tab.BigEnemy2, 2)
                ,
            };
        }

        public class EnemyGroup_Tab
        {
            public static EnemyGroup Group1 = new EnemyGroup(new List<UnitTypeObject>()
            {
                UnitType_Tab.Enemy1,
                UnitType_Tab.Enemy1,
                UnitType_Tab.Enemy2,
                UnitType_Tab.Enemy2,
            });
            public static EnemyGroup Group2 = new EnemyGroup(new List<UnitTypeObject>()
            {
                UnitType_Tab.Enemy1,
                UnitType_Tab.Enemy1,
                UnitType_Tab.Enemy2,
                UnitType_Tab.Enemy2,
                UnitType_Tab.BigEnemy1,
                UnitType_Tab.BigEnemy2,
            });
            public static EnemyGroup Group3 = new EnemyGroup(new List<UnitTypeObject>()
            {
                UnitType_Tab.BigEnemy1,
                UnitType_Tab.BigEnemy2,
                UnitType_Tab.BigEnemy1,
                UnitType_Tab.BigEnemy2,
                UnitType_Tab.Boss1,
            });
        }
    }


    public class InteractiveObject :I2WeConfig
    {
        public string Id { get; set; }
        public string Desc { get; set; }

        public string ToConfig()
        {
            return Id;
        }
    }

    public class UnitTypeObject :I2WeConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ToConfig()
        {
            return Id;
        }
    }

    public class EnemySpawnner : I2WeConfig
    {
        public float LastTime;
        public float SpawnInterval;
        public RandomWeightPool<UnitTypeObject> EnemyTypePool;

        public string ToConfig()
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (var pair in EnemyTypePool.GetMapCopy())
            {
                if (!isFirst)
                {
                    sb.Append("_");
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(string.Format("{0}|{1}", pair.Key.ToConfig(), pair.Value));
            }
            return sb.ToString();
        }
    }
    public class EnemyGroup :I2WeConfig
    {
        public EnemyGroup(List<UnitTypeObject> enemyList)
        {
            EnemyList.AddRange(enemyList);
        }
        public List<UnitTypeObject> EnemyList = new List<UnitTypeObject>();
        public string ToConfig()
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (var unitTypeObject in EnemyList)
            {
                if (!isFirst)
                {
                    sb.Append("_");
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(unitTypeObject.Id);
            }
            return sb.ToString();
        }
    }


    public abstract class RoomBaseConfig : I2WeConfig
    {
        public abstract string ToConfig();
    }

    /// <summary>
    /// interactive
    /// </summary>
    public class Room_Interactive :RoomBaseConfig
    {
        public Room_Interactive(InteractiveObject target)
        {
            InteractiveTarget = target;
        }

        public InteractiveObject InteractiveTarget { get; private set; }
        public override string ToConfig()
        {
            return this.GetType().Name + "@" + InteractiveTarget.ToConfig();
        }
    }
    /// <summary>
    /// certain enemy battle
    /// </summary>
    public class Room_Fight : RoomBaseConfig
    {
        public Room_Fight(EnemyGroup group)
        {
            EnemyGroup = group;
        }

        public EnemyGroup EnemyGroup { get; private set; }

        public override string ToConfig()
        {
            return this.GetType().Name + "@" + EnemyGroup.ToConfig();
        }
    }


    /// <summary>
    /// keep alive in time
    /// </summary>
    public class Room_KeepAlive : RoomBaseConfig
    {
        public Room_KeepAlive(EnemySpawnner spawnner)
        {
            Spawnner = spawnner;
        }
        public EnemySpawnner Spawnner { get; private set; }
        public override string ToConfig()
        {
            return this.GetType().Name + "@" +Spawnner.ToConfig();
        }
    }
    
    


    public class GameLevelConfig
    {
        public static GameLevelConfig TestConfig
        {
            get
            {
                var cfg = new GameLevelConfig();
                cfg.TestInit();
                return cfg;
            }
        }

        public List<RoomBaseConfig> Rooms = new List<RoomBaseConfig>();

        public void AddRoom(RoomBaseConfig room)
        {
            Rooms.Add(room);
        }
        
        public void TestInit()
        {
            RoomBaseConfig room = new Room_Interactive(SLK.Interactive_Tab.BookShelf);
            AddRoom(room);
            room = new Room_Interactive(SLK.Interactive_Tab.TreasureBox);
            AddRoom(room);

            room = new Room_Fight(SLK.EnemyGroup_Tab.Group1);
            AddRoom(room);
            room = new Room_Fight(SLK.EnemyGroup_Tab.Group2);
            AddRoom(room);
            room = new Room_Fight(SLK.EnemyGroup_Tab.Group3);
            AddRoom(room);

            room = new Room_KeepAlive(SLK.EnemySpawnner_Tab.Spawnner1);
            AddRoom(room);
        }
    }
}
