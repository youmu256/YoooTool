using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace YoooTool.Code.Utils
{
    public class XMLReader
    {
        public List<Dictionary<string, string>> ReadTabelXml(string filePath)
        {
            List<Dictionary<string, string>> mainData = new List<Dictionary<string, string>>();
            string xmlData = File.ReadAllText(filePath);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);
            var selectSingleNode = xmlDoc.SelectSingleNode(rootName);
            if (selectSingleNode != null)
            {
                XmlNodeList nodeList = selectSingleNode.ChildNodes;
                foreach (XmlElement node in nodeList)
                {
                    Dictionary<string, string> dct = new Dictionary<string, string>();
                    foreach (XmlElement t in node.ChildNodes)
                    {
                        dct[t.Name] = t.InnerText;
                    }
                    mainData.Add(dct);
                }
            }
            return mainData;
        }

        public XMLReader()
        {
            srcPath = "";
            dstPath = "";
        }
        const string rootName = "mysql";
        static string srcPath;
        static string dstPath;

        public List<Dictionary<string, string>> ReadBinData(string file)
        {
            file = Path.GetFileNameWithoutExtension(file);
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            using (FileStream fs = new FileStream(dstPath + "bin_" + file + ".bytes", FileMode.Open, FileAccess.Read))
            {
                BinaryReader br = new BinaryReader(fs);
                int dictLenght = br.ReadInt32();
                string[] keys = new string[dictLenght];
                for (int i = 0; i < dictLenght; i++)
                {
                    keys[i] = br.ReadString();
                }

                int listCount = br.ReadInt32();
                for (int i = 0; i < listCount; i++)
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    for (int j = 0; j < dictLenght; j++)
                    {
                        var value = br.ReadString();
                        dict[keys[j]] = value;
                    }
                    list.Add(dict);
                }

                br.Close();
                fs.Close();
            }
            return list;
        }

        public void XmlToBin(string filenamePath)
        {
            List<Dictionary<string, string>> dataList = ReadTabelXml(filenamePath);

            var keys = new string[dataList[0].Count];
            int keyID = 0;
            foreach (var key in dataList[0].Keys)
            {
                keys[keyID++] = key;
            }
            /*
            var dataType = new int[dataList[0].Count];
            foreach (var data in dataList)
            {
                int test = 0;
                for (int i = 0; i < dataType.Length; i++)
                {
                    if (dataType[i] == 0)
                    {
                        if (!int.TryParse(data[keys[i]], out test)) //不是整数
                            dataType[i] = 1;
                    }
                }
            }
            */
            filenamePath = Path.GetFileNameWithoutExtension(filenamePath);
            using (FileStream fs = new FileStream(dstPath + "bin_" + filenamePath + ".bytes", FileMode.OpenOrCreate))
            {
                BinaryWriter bw = new BinaryWriter(fs);

                bw.Write(keys.Length); //先写入字段长度

                for (int i = 0; i < keys.Length; i++)
                {
                    bw.Write(keys[i]);
                    //bw.Write(dataType[i]);
                }

                bw.Write(dataList.Count); //先写入字段长度
                foreach (var tmpDict in dataList)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        var value = tmpDict[keys[i]];
                        bw.Write(value);
                        /*
                        if (dataType[i] == 0)
                        {
                            bw.Write(int.Parse(value));
                        }
                        else
                        {
                            bw.Write(value);
                        }
                        */
                    }
                }

                //关闭流
                bw.Close();
                fs.Close();
            }
        }
    }
}
