using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code.Utils;

namespace ItemParse.LniParser
{

    public class LniObjectModify
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public LniObjectModify(string k, string v)
        {
            Key = k;
            Value = v;
        }
    }
    public class LniModifyData
    {
        public string FilePath { get; private set; }
        public Dictionary<string,List<LniObjectModify>> ObjModifyMap = new Dictionary<string, List<LniObjectModify>>();
        public LniModifyData(string path,Encoding encod)
        {
            FilePath = path;
            CsvStreamReader reader = CsvStreamReader.CreateReader(FilePath,encod);//Encoding.GetEncoding("GB2312")
            for (int i = 1; i <= reader.RowCount; i++)
            {
                bool isIgonreLine = i == 1;
                if(isIgonreLine)continue;
                string[] srr = new string[reader.ColCount];
                for (int j = 1; j <= reader.ColCount; j++)
                {
                    srr[j - 1] = reader[i, j];
                    bool isIgnoreCol = j == 1;
                    if (isIgnoreCol) continue;
                    string section = srr[0];
                    //if (section == null) section = "error";
                    string modifyK = reader[1,j];//第一行 第j列
                    string modifyV = srr[j-1];
                    List<LniObjectModify> modifies = null;
                    if (!ObjModifyMap.TryGetValue(section, out modifies))
                    {
                        modifies = new List<LniObjectModify>();
                        ObjModifyMap.Add(section, modifies);
                    }
                    modifies.Add(new LniObjectModify(modifyK, modifyV));
                }
            }

            /*
            foreach (var pair in ObjModifyMap)
            {
                Console.WriteLine(pair.Key + " ========== ");
                foreach (var modify in pair.Value)
                {
                    Console.WriteLine(modify.Key + " ### "+modify.Value);
                }
            }
            */
        }

        public static void Apply(string dataTablePath, LniModifyData modifyData)
        {
            foreach (var pair in modifyData.ObjModifyMap)
            {
                string section = pair.Key;
                foreach (var modify in pair.Value)
                {
                    string k = modify.Key;
                    string v = modify.Value;
                    bool r = INIHelper.WriteString(section, k, v, dataTablePath);
                    Console.WriteLine(string.Format("modify {3}:{0},{1},{2}", section,k,v,r));
                }
            }
        }
    }
}
