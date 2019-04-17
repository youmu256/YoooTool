using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code;

namespace YoooTool
{
    class Program
    {
        public class Item
        {
            public string Name;
        }

        static void Main(string[] args)
        {
            /*
            Random random = new Random();
            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine(random.NextDouble()*100);
            }
            Console.ReadKey();
            return;
            */
            RandomPool<Item> rp = new RandomPool<Item>();
            rp.SetItemWeight(new Item() {Name = "5"}, 5);
            rp.SetItemWeight(new Item() {Name = "10"}, 10);
            Dictionary<Item,int> counter = new Dictionary<Item, int>();
            for (int i = 0; i < 100000; i++)
            {
                var it = rp.GetItem();
                if (!counter.ContainsKey(it))
                {
                    counter.Add(it,0);
                }
                counter[it]++;
            }

            foreach (var p in counter)
            {
                Console.WriteLine(p.Key.Name + "__" + p.Value);
            }

            Console.ReadKey();
        }
    }
}
