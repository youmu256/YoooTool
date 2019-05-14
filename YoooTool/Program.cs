using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoooTool.Code;
using YoooTool.Code.Slk;
using YoooTool.Code.Utils;

namespace YoooTool
{
    class Program
    {
        public class Parent
        {
            public string Name;
        }

        public class Child:Parent
        {
            
        }
        public class Room<T> where  T: Parent
        {
            public List<T> List = new List<T>();
        }

        public class PC_Test
        {
            public static void Test()
            {
                List<Room<Parent>> rooms = new List<Room<Parent>>();
                var room = new Room<Child>();
                //rooms.Add(room); how to make this
            }
        }

        public class Item
        {
            public string Name;
        }

        static void RandomPoolTest()
        {

            RandomWeightPool<Item> rp = new RandomWeightPool<Item>();
            rp.SetItemWeight(new Item() { Name = "5" }, 5);
            rp.SetItemWeight(new Item() { Name = "10" }, 10);
            Dictionary<Item, int> counter = new Dictionary<Item, int>();
            for (int i = 0; i < 100000; i++)
            {
                var it = rp.GetItem();
                if (!counter.ContainsKey(it))
                {
                    counter.Add(it, 0);
                }
                counter[it]++;
            }

            foreach (var p in counter)
            {
                Console.WriteLine(p.Key.Name + "__" + p.Value);
            }
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            SlkManager.CreateInstance();
            /*
            var l = new LevelManager();
            l.TestInit();
            l.Export();
            Console.ReadKey();
            */
            OpenLevelForm();
            Console.ReadKey();
            return;
            new SlkManager().OutPutTest();
            Console.ReadKey();
            return;
            var game = new Game();
            game.Init();
            game.StartRun();
            Console.ReadKey();
            return;

        }

        static void OpenLevelForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LevelEdit());
        }

    }
}
