using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MyWarez.Core;

namespace MyWarez.Plugins.MacroPack
{
    public abstract class MacroPack
    {
        protected static string ResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, nameof(Plugins.MacroPack), "macro_pack", "src");

        protected enum OutputType
        {
            DDE,
            VBA
        }

        public enum OutputExtension
        {
            // DDE
            DOCX,
            XLSX,

            // VbaMacro
            XLS,
            XLSM,
            XLSB,
            DOC,
            DOCM,
            DOT,
            DOTM,
            ACCDB,
            PPT,
            PPTM
        }

        protected static byte[] Generate(OutputType outputType, string inputFileContent, OutputExtension outputExtension)
        {
            // Kill running processes
            string processName = "";
            switch (outputExtension)
            {
                case OutputExtension.DOCX:
                case OutputExtension.DOCM:
                    processName = "winword.exe";
                    break;
                case OutputExtension.XLSX:
                case OutputExtension.XLSM:
                    processName = "excel.exe";
                    break;
                case OutputExtension.ACCDB:
                    processName = "msaccess.exe";
                    break;
                case OutputExtension.PPTM:
                    processName = "powerpnt.exe";
                    break;
            }
            string friendlyProcessName = processName.Split(".exe")[0];
            foreach (var p in Process.GetProcessesByName(friendlyProcessName))
                p.Kill();

            using (new TemporaryContext())
            {
                Utils.CopyFilesRecursively(ResourceDirectory, ".");
                string inputContent = inputFileContent;
                string inputFileName = Utils.RandomString(5) + ".vba";
                File.WriteAllText(inputFileName, inputContent);
                string outputFileName = Utils.RandomString(5) + "." + outputExtension.ToString().ToLower();
                var process = new Process();
                process.StartInfo.FileName = "python";
                process.StartInfo.Arguments = $"macro_pack.py {(outputType == OutputType.DDE ? "--dde" : "")} -f {inputFileName} -G {outputFileName}";
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
                return File.ReadAllBytes(outputFileName);
            }
        }
    }
}
