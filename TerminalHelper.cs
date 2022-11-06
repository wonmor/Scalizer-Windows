using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scalizer
{
    internal class TerminalHelper
    {
        public static string execute(string command)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe", // use cmd for example, you can run your own test.bat
                Verb = "runas", // run as administrator
                Arguments = "/C " + command,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            var cmd = new Process();
            cmd.StartInfo = startInfo;

            var output = new StringBuilder();
            // set output hander
            cmd.OutputDataReceived += (sender, args) => output.AppendLine(args.Data);
            string stdError = "";

            try
            {
                cmd.Start();
                // get standard output asynchronously
                cmd.BeginOutputReadLine();
                // get standard error synchronously
                stdError = cmd.StandardError.ReadToEnd();
                cmd.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing: " + e.Message);
            }

            if (cmd.ExitCode != 0)
            {
                throw new Exception("Finished with exit code = " + cmd.ExitCode + ": " + stdError);
            }

            return output.ToString();
        }
    }
}
