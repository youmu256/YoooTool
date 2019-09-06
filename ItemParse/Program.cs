using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code.Utils;

namespace ItemParse
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ItemTest();
        }
        static void ItemTest()
        {
            ItemLniInfoPick.ReadFile("item.ini");
            Console.ReadKey();
        }
    }
}
