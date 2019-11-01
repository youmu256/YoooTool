using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemParse.Utils
{
    public class CmdHelper
    {
        public static void processCommand(string command, string argument)
        {
            ProcessStartInfo start = new ProcessStartInfo(command);
            start.Arguments = argument;
            start.CreateNoWindow = false;
            start.ErrorDialog = true;
            start.UseShellExecute = true;
            if (start.UseShellExecute)
            {
                start.RedirectStandardOutput = false;
                start.RedirectStandardError = false;
                start.RedirectStandardInput = false;
            }
            else
            {
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
                start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
            }
            Process p = Process.Start(start);
            if (!start.UseShellExecute)
            {
                Console.WriteLine(p.StandardOutput);
                Console.WriteLine(p.StandardError);
            }
            p.WaitForExit();
            p.Close();
        }
    }
}
