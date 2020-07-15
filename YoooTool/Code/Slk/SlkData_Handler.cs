using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code.Utils;

namespace YoooTool.Code.Slk
{

    public interface ISlkData_Handler
    {
        string Handler_Serialize();
        /// <summary>
        /// 反射调用
        /// </summary>
        /// <param name="data"></param>
        void Handler_DeSerialize(object data = null);
        /// <summary>
        /// 处理其他实例引用
        /// </summary>
        void Handler_LateDeSerialize();

    }
    public class SlkData_Handler<T> : ISlkData_Handler where T : SlkDataObject
    {
        public string GetExportFileName()
        {
            return typeof(T).Name;
        }

        //reflect call
        public void RegistToMap(Dictionary<string, SlkDataObject> map)
        {
            //add type as key ext? typeof(T).Name +
            foreach (var pair in IdMap)
            {
                string fkey = SlkManager.SlkIdSearchFix<T>(pair.Key);
                map.Add(fkey, pair.Value);
            }
        }

        protected Dictionary<string, T> IdMap = new Dictionary<string, T>();

        public T GetData(string id)
        {
            T t = null;
            if (IdMap.TryGetValue(id, out t))
            {
                return t;
            }
            return null;
        }

        public bool AddData(T data)
        {
            if (data == null) return false;
            if (IdMap.ContainsKey(data.Id))
            {
                return false;
            }
            IdMap.Add(data.Id, data);
            return true;
        }

        public List<T> GetAllData()
        {
            return IdMap.Values.ToList();
        }

        #region Serialize

        public string Handler_Serialize()
        {
            StringBuilder sb = new StringBuilder();
            var title = SlkDataObject.GetProperty2Csv(typeof(T));
            sb.AppendLine(title);
            foreach (var pair in IdMap)
            {
                sb.AppendLine(pair.Value.Slk_Serialize());
            }
            return sb.ToString();
        }

        private CsvStreamReader reader;
        /// <summary>
        /// 反射调用
        /// SLKManager.Init
        /// </summary>
        /// <param name="data"></param>
        public void Handler_DeSerialize(object data = null)
        {
            if (data == null)
            {
                //自动尝试找到file
                string fileName = typeof(T).Name;
                data = ExportHelper.GetPathFileName(fileName, ".csv");
                if (!File.Exists(data.ToString()))
                {
                    Console.WriteLine("Not Load : "+fileName);
                }
            }
            Console.WriteLine(data);
            IdMap.Clear();
            reader = CsvStreamReader.CreateReader(data.ToString(), Encoding.UTF8);
            bool isTitleLine = true;
            for (int i = 1; i <= reader.RowCount; i++)
            {
                if (isTitleLine)
                {
                    isTitleLine = false;
                    continue;
                }
                string[] srr = new string[reader.ColCount];
                for (int j = 1; j <= reader.ColCount; j++)
                {
                    srr[j - 1] = reader[i, j];
                }
                T t = SlkDataObjectFactory<T>.Create();
                t.Slk_DeSerialize(srr);
                AddData(t);
            }
        }

        public void Handler_LateDeSerialize()
        {
            if (reader == null)
            {
                return;
            }
            bool isTitleLine = true;
            for (int i = 1; i <= reader.RowCount; i++)
            {
                if (isTitleLine)
                {
                    isTitleLine = false;
                    continue;
                }
                string[] srr = new string[reader.ColCount];
                for (int j = 1; j <= reader.ColCount; j++)
                {
                    srr[j - 1] = reader[i, j];
                }
                var slk = GetAllData()[i - 2];
                slk.Slk_LateDeSerialize(srr);
            }
        }
        #endregion

    }
}
