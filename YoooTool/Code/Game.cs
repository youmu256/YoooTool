using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoooTool.Code
{
    public class GameLevelExcutor
    {
        public GameLevelConfig Config { get; private set; }

        public int CurrentIndex { get; private set; } = 0;
        public RoomBaseConfig CurrentRoom { get; private set; } = null;
        
        public void LoadLevelConfig(GameLevelConfig cfg)
        {
            Config = cfg;
            CurrentIndex = 0;
            CurrentRoom = null;
        }

        public void ShowCurrentRoom()
        {
            if (CurrentRoom == null)
            {
                Console.WriteLine("------Level Is End------");
                return;
            }
            Console.WriteLine("------Room Start------");
            Console.WriteLine(CurrentRoom.ToConfig());
            Console.WriteLine("------Room End------");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Is Level Finish</returns>
        public bool MoveNextRoom()
        {
            CurrentIndex++;
            if (CurrentIndex > Config.Rooms.Count)
            {
                CurrentRoom = null;
                return true;
            }
            CurrentRoom = Config.Rooms[CurrentIndex - 1];
            return false;
        }

    }

    public class Game
    {
        public GameLevelExcutor CurrentLevel { get; private set; }
        public void Init()
        {
            CurrentLevel = new GameLevelExcutor();
            CurrentLevel.LoadLevelConfig(GameLevelConfig.TestConfig);
        }

        public void StartRun()
        {
            while (!CurrentLevel.MoveNextRoom())
            {
                CurrentLevel.ShowCurrentRoom();
            }
        }
    }
}
