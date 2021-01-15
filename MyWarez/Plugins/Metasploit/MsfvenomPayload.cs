using MyWarez.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace MyWarez.Plugins.Metasploit
{
    public static class MetasploitPayloadFactory
    {
        public static byte[] Generate(string format, string payload, string options)
        {
            string outputfilename = "OUTPUT.bin";
            string program = "msfvenom";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                program += ".bat";
            using (new TemporaryContext())
            {
                var workingDirectory = ".";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string originalPathEnv = Environment.GetEnvironmentVariable("PATH");
                    string[] paths = originalPathEnv.Split(new char[1] { Path.PathSeparator });
                    foreach (string s in paths)
                    {
                        string pathEnv = Environment.ExpandEnvironmentVariables(s);
                        if (pathEnv.ToLower().Contains("metasploit"))
                            workingDirectory = pathEnv;
                    }
                    outputfilename = Path.Join(Path.GetFullPath("."), outputfilename);
                }
                string arguments = "-o " + outputfilename + " -f " + format + " -p " + payload + " " + options;
                Process process = new Process();
                process.StartInfo.FileName = program;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();
                return File.ReadAllBytes(outputfilename);
            }
        }
    }
}