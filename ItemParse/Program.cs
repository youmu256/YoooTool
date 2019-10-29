using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemParse.LniParser;
using YoooTool.Code.Utils;

namespace ItemParse
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            foreach (var s in args)
            {
                Console.WriteLine(s);
            }
            ItemTest();
        }
        static void ItemTest()
        {
            InfoPick();
            //new LniDataTable("item.ini");
            //ModifyReadTest();
            //ItemLniInfoPick.ReadFile("item.ini");
            Console.ReadKey();
        }

        static void InfoPick()
        {
            //从ini中读取需要的数据，导出成csv表格，要抽取的数据在InfoTemplate中，默认会抽取ID
            string pp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "item.ini");
            string pc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InfoTemplate.csv");
            var pick = new LniInfoPick(pp, pc);
            string outFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InfoPicked.csv");
            pick.SaveOut(outFile);
            ModifyReadTest();
        }
        static void ModifyReadTest()
        {
            string fp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InfoPicked.csv");;
            var data = new LniModifyData(fp,Encoding.UTF8);
            string tablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "item.ini");
            LniModifyData.Apply(tablePath, data);
            Console.WriteLine(tablePath);
        }
    }
}
