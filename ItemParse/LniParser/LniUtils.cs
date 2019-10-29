using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ItemParse.LniParser
{
    public class LniUtils
    {
        public static List<string> ReadAllSection(string file)
        {
            //正则 \[\w*\]\s
            string content = File.ReadAllText(file);
            Regex regex = new Regex(@"\[\w*\]\s");
            List<string> sectionList = new List<string>();
            foreach (Match match in regex.Matches(content))
            {
                string p = match.Value.Trim();
                sectionList.Add(p.Substring(1, p.Length - 2));
            }
            return sectionList;
        }
    }
}
