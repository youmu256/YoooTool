using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code.Utils;

namespace ItemParse.LniParser
{
    public class InfoObject
    {
        public string Id { get; private set; }
        public List<string> DataList { get; private set; }

        public InfoObject(string id, List<string> datas)
        {
            Id = id;
            DataList = new List<string>(datas);
        }

        public string GetCsv()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var data in DataList)
            {
                sb.Append(data);
                sb.Append(",");
            }
            //if(sb.Length>0)
            sb=sb.Remove(sb.Length - 1, 1);//最后的逗号
            return sb.ToString();
        }
    }
    

    public class LniInfoPick
    {
        const string IDLabel = "ID";
        public string FilePath { get; private set; }
        public string OutPutFile { get; private set; }
        public LniInfoPick(string file,string outputFile)
        {
            FilePath = file;
            OutPutFile = outputFile;

            
        }

        public void SaveOut(string path)
        {
            //--第一行是要取出的数据--
            CsvStreamReader reader = CsvStreamReader.CreateReader(OutPutFile, Encoding.GetEncoding("UTF-8"));
            List<string> dataFilterList = new List<string>();
            dataFilterList.Add(IDLabel);
            for (int i = 1; i <= reader.ColCount; i++)
            {
                string data = reader[1, i];
                dataFilterList.Add(data);
            }
            List<InfoObject> table = new List<InfoObject>();
            List<string> sectionList = LniUtils.ReadAllSection(FilePath);
            foreach (var section in sectionList)
            {
                List<string> datas = new List<string>();
                foreach (var dataFilter in dataFilterList)
                {
                    if (dataFilter == IDLabel)
                    {
                        datas.Add(section);
                    }
                    else
                    {
                        //引号会丢失...貌似没问题？
                        string value = INIHelper.ReadString(section, dataFilter, "", FilePath);
                        datas.Add(value);
                    }
                    //Console.WriteLine(value);
                }
                table.Add(new InfoObject(section, datas));
            }
            StringBuilder csv = new StringBuilder();
            foreach (var f in dataFilterList)
            {
                csv.Append(f);
                csv.Append(",");
            }
            csv = csv.Remove(csv.Length - 1, 1);//最后的逗号
            csv.AppendLine();
            foreach (var infoObject in table)
            {
                csv.AppendLine(infoObject.GetCsv());
            }
            File.WriteAllText(path, csv.ToString());
        }

    }
}
