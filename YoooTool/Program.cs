using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code;
using YoooTool.Code.Slk;
using YoooTool.Code.Utils;

namespace YoooTool
{
    class Program
    {
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
            //new Slk().Test();
            //Console.ReadKey();
            //return;
            var game = new Game();
            game.Init();
            game.StartRun();
            Console.ReadKey();
            return;

        }
    }
}
