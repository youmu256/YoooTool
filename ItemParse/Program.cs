using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemParse.LniParser;
using ItemParse.Operate;

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
                            InfoPick("item.ini");
                            break;
                        case "-modify":
                            ModifyTable("item.ini");
                            break;
                    }
                }else if (argsCount == 2)
                {
                    switch (key)
                    {
                        case "-pick":
                            InfoPick(args[1]);
                            break;
                        case "-modify":
                            ModifyTable(args[1]);
                            break;
                    }
                }
            }
        }

        //-pick 读取某个ini文件,把数据抽到InfoFile文件
        //-modify 应用某个csv文件的内容，按照表中的各项装填给某个ini文件

        private static string InfoFileName = "InfoPicked.csv";
        static void ModifyTable(string table)
        {
            string tablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, table);
            string infoFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, InfoFileName);
            OperateMgr.Instance.ModifyTableWithInfoFile(tablePath, infoFile);
        }

        static void InfoPick(string table)
        {
            string tablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, table);
            string outFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, InfoFileName);
            OperateMgr.Instance.InfoPickFromTable(tablePath, outFile);
        }
    }
}
