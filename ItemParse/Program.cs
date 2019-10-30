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
            if (args.Length > 0)
            {
                string key = args[0];
                Console.WriteLine(key);
                int argsCount = args.Length;
                if (argsCount == 1)
                {
                    switch (key)
                    {
                        case "-pick":
                            InfoPick();
                            break;
                        case "-modify":
                            ModifyApply();
                            break;
                    }
                }else if (argsCount == 2)
                {
                    switch (key)
                    {
                        case "-pick":
                            InfoPick();
                            break;
                        case "-modify":
                            CustomModify(args[1]);
                            break;
                    }
                }
                
            }
        }

        static void CustomModify(string customFile)
        {
            string fp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InfoPicked.csv");
            var fileEncode = FileEncodeUtil.EncodingType.GetType(fp);
            var data = new LniModifyData(fp, fileEncode);
            string tablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, customFile);
            LniModifyData.Apply(tablePath, data);
            Console.WriteLine("modify info finish..");
        }

        static void InfoPick()
        {
            //从ini中读取需要的数据，导出成csv表格，要抽取的数据在InfoTemplate中，默认会抽取ID
            string pp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "item.ini");
            string pc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InfoTemplate.csv");
            var pick = new LniInfoPick(pp, pc);
            string outFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InfoPicked.csv");
            pick.SaveOut(outFile);
            Console.WriteLine("pick info finish..");
        }
        static void ModifyApply()
        {
            CustomModify("item.ini");
        }
    }
}
