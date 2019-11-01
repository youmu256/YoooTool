using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemParse.LniParser;

namespace ItemParse.Operate
{
    public class OperateMgr
    {
        public static OperateMgr Instance { get; private set; } = new OperateMgr();

        private OperateMgr()
        {
            //具体操作行为
        }
        private static string InfoTemplateFileName = "InfoTemplate.csv";
        public void ModifyTableWithInfoFile(string tablePath,string infoFile)
        {
            var fileEncode = FileEncodeUtil.EncodingType.GetType(infoFile);
            var data = new LniModifyData(infoFile, fileEncode);
            LniModifyData.Apply(tablePath, data);
            Console.WriteLine("modify info finish..");
        }

        public void InfoPickFromTable(string tablePath,string exportInfoPath)
        {
            string pc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, InfoTemplateFileName);
            var pick = new LniInfoPick(tablePath, pc);
            pick.SaveOut(exportInfoPath);
            Console.WriteLine("pick info finish..");
        }

    }
}
