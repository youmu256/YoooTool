using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoooTool.Code.Utils
{
    
    public class ItemData
    {
        public string Id;
        public string Name = "DEFAULT";
        public string Ubertip = "DEFAULT";
        public string GetCsv()
        {
            return Id + "," + Name+","+Ubertip;
        }

        public static ItemData Create(List<string> content)
        {
            ItemData data = new ItemData();
            int index = 0;
            foreach (var line in content)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    data.Id = line.Substring(1, line.Length - 2);
                }
                if (line.StartsWith("Name ="))
                {
                    data.Name = line.Replace("Name =", "").Trim();
                }
                if (line.StartsWith("Ubertip ="))
                {
                    data.Ubertip = line.Replace("Ubertip =", "").Trim();
                }
            }
            if (string.IsNullOrEmpty(data.Name))
            {
                data.Name = "DEFAULT NAME";
            }
            return data;
        }
    }



    public class ItemLniInfoPick
    {


        public static ItemData ParseSingle(List<string> content)
        {
            if (content.Count <= 0) return null;
            return ItemData.Create(content);
        }


        public static void Parse(List<string> content)
        {
            List<string> cureentItemData = new List<string>();
            List<ItemData> dataList = new List<ItemData>();
            foreach (var line in content)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    var data = ParseSingle(cureentItemData);
                    if (data != null)
                    {
                        dataList.Add(data);
                    }
                    cureentItemData.Clear();
                }
                cureentItemData.Add(line);
            }
            WriteFile(dataList);
        }

        static void WriteFile(List<ItemData> itemDatas)
        {
            StringBuilder content = new StringBuilder();
            foreach (var itemData in itemDatas)
            {
                content.AppendLine(itemData.GetCsv());
            }
            File.WriteAllText("itemData.csv",content.ToString());
            Console.WriteLine("Write Over");
        }

        public static void ReadFile(string filePath)
        {
            StreamReader sr = new StreamReader(filePath,Encoding.UTF8);
            string line;
            List<string> content = new List<string>();
            while ((line = sr.ReadLine())!=null)
            {
                content.Add(line);
            }
            sr.Close();
            Parse(content);
        }
    }
}
