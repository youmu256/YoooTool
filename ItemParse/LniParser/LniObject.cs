using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ItemParse.LniParser
{

    public class LniConstant_Item
    {
        public static string Ubertip = "Ubertip";
    }


    public class LniDataTable
    {
        public string TableName { get; set; }

        public Dictionary<string,LniObject> IdObjectMap { get; private set; }= new Dictionary<string, LniObject>();

        public LniDataTable(string filePath)
        {
            TableName = Path.GetFileNameWithoutExtension(filePath);
            List<string> sectionList = LniUtils.ReadAllSection(filePath);
            foreach (var s in sectionList)
            {
                IdObjectMap.Add(s,new LniObject(filePath,s));
            }
        }
    }

    public class LniObject
    {
        public string FilePath { get; private set; }
        public string ObjectName { get; private set; }
        //public Dictionary<string,string> KvMap { get; private set; }= new Dictionary<string, string>();
        public LniObject(string path,string name)
        {
            FilePath = path;
            ObjectName = name;
            Console.WriteLine(name+" : "+Read("Ubertip"));
        }

        public string Read(string key)
        {
            return INIHelper.Read(ObjectName, key, "", FilePath);
        }

        public void Write(string key, string value)
        {
            INIHelper.Write(ObjectName, key, value, FilePath);
        }
    }
}
