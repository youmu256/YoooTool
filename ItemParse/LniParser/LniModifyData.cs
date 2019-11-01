using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemParse.Utils;
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
            //value 有一些需要加双引号...都加好像没问题？
            Value = string.Format("\"{0}\"", v);
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
                    if(string.IsNullOrEmpty(modifyK)||string.IsNullOrEmpty(modifyV))continue;
                    //Console.WriteLine(modifyK + "," + modifyV);
                    modifies.Add(new LniObjectModify(modifyK, modifyV));
                }
            }
        }

        public static void Apply(string dataTablePath, LniModifyData modifyData)
        {
            //会丢失引号..
            foreach (var pair in modifyData.ObjModifyMap)
            {
                string section = pair.Key;
                foreach (var modify in pair.Value)
                {
                    string k = modify.Key;
                    string v = modify.Value;
                    if (INIHelper.ReadString(section, k, null, dataTablePath) == null)
                    {
                        Console.WriteLine("Can't To Write New Section : "+section);
                        continue;
                    }
                    INIHelper.WriteString(section, k, v, dataTablePath);
                    Console.WriteLine(string.Format("modify:{0}:{1},{2}", section,k,v));
                }
            }
        }
    }
}
