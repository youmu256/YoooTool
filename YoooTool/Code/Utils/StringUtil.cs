using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoooTool.Code.Utils
{
    public class StringUtil
    {

        public static string Replace(string content, string from,string to)
        {

            StringBuilder passedString = new StringBuilder();
            StringBuilder backString = new StringBuilder();//开始匹配但是最终没完全匹配的缓存
            int matchIndex = 0;
            for (int i = 0; i < content.Length; i++)
            {
                bool curMatch = content[i] == from[matchIndex];
                bool matchOk = false;
                bool needAppendBack = false;
                if (curMatch)
                {
                    //当前字符匹配-匹配+1 检查是否全部匹配
                    if (matchIndex < from.Length)
                        matchIndex++;
                    if (matchIndex == from.Length)
                    {
                        backString.Clear();
                        //全部匹配完毕
                        matchIndex = 0;
                        matchOk = true;
                    }
                    else
                    {
                        backString.Append(content[i]);
                    }
                }
                else
                {
                    needAppendBack = true;
                    //当前不匹配
                    matchIndex = 0;//从0重新开始尝试匹配
                }

                if (matchOk)
                {
                    passedString.Append(to);
                }
                if (needAppendBack)
                {
                    passedString.Append(backString);
                    backString.Clear();
                }
                if (!curMatch)
                {
                    passedString.Append(content[i]);
                }

            }
            return passedString.ToString();
        }

        public static string[] Split(string content, string flag)
        {
            string[] srr = new string[0];
            Console.WriteLine(content);
            StringBuilder backString = new StringBuilder();//开始匹配但是最终没完全匹配的缓存
            StringBuilder eachString = new StringBuilder();
            int matchIndex = 0;
            for (int i = 0; i < content.Length; i++)
            {
                bool curMatch = content[i] == flag[matchIndex];
                bool matchOk = false;
                bool needAppendBack = false;
                if (curMatch)
                {
                    //当前字符匹配-匹配+1 检查是否全部匹配
                    if (matchIndex < flag.Length)
                        matchIndex++;
                    if (matchIndex == flag.Length)
                    {
                        backString.Clear();
                        //全部匹配完毕
                        matchIndex = 0;
                        matchOk = true;
                    }
                    else
                    {
                        backString.Append(content[i]);
                    }
                }
                else
                {
                    needAppendBack = true;
                    //当前不匹配
                    matchIndex = 0;//从0重新开始尝试匹配
                }

                if (matchOk)
                {
                    string[] nsrr = new string[srr.Length+1];
                    Array.Copy(srr,nsrr,srr.Length);
                    srr = nsrr;
                    srr[srr.Length - 1] = eachString.ToString();
                    eachString.Clear();
                }
                if (needAppendBack)
                {
                    eachString.Append(backString);
                    backString.Clear();
                }
                if (!curMatch)
                {
                    eachString.Append(content[i]);
                }

            }
            string[] fsrr = new string[srr.Length + 1];
            Array.Copy(srr, fsrr, srr.Length);
            srr = fsrr;
            srr[srr.Length - 1] = eachString.ToString();
            eachString.Clear();
            return srr;
        }
    }
}