/*using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Tonsil.Files;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{
    public class PrintConfigDll : DynamicDynamicLinkLibrary
    {
        public PrintConfigDll(Payload cppPayload) : base(Constants.DynamicFilePayloadDirectory + "DynamicLinkLibrary" + Path.DirectorySeparatorChar + typeof(PrintConfigDll).Name)
        {
            if (!(cppPayload.Type == PayloadType.Cpp))
                throw new ArgumentException();
            if (!(cppPayload is IFilePayload))
                throw new ArgumentException();
            // cppPayload needs to implement PrintConfigDll.PayloadFunctionName
            Payload = cppPayload;
        }
        public override string Name { get; } = "PrintConfigDll";
        public override byte[] Bytes
        {
            get
            {
                var myTempDir = Constants.TempDirectory + Utils.RandomString(5);
                Directory.CreateDirectory(myTempDir);
                Utils.CopyFilesRecursively(SourceDirectory, myTempDir);
                var cwd = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(myTempDir);
                System.IO.File.WriteAllText(PayloadSourceFileName, ((IFilePayload)Payload).Text);
                var proc2 = Process.Start("cmd.exe", "/c " + CompileScriptName);
                proc2.WaitForExit();
                var outputFileName = ExecutableName + ".dll";
                var outputBytes = System.IO.File.ReadAllBytes(outputFileName);
                Directory.SetCurrentDirectory(cwd);
                return outputBytes;
            }
        }
        public Payload Payload { get; }
        private static string PayloadSourceFileName = "payload.cpp";
        private static string PayloadFunctionName = "ExecutePayload";
    }
}
*/