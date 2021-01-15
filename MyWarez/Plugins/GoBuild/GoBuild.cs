using MyWarez.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MyWarez.Plugins.GoBuild
{
    public static class GoBuild
    {
        private enum TargetOSArch
        {
            windows_386,
            windows_amd64,
        }

        private static byte[] Build(string script, TargetOSArch targetOsArch, string additionalArgs="")
        {
            string targetOS = targetOsArch.ToString().Split("_")[0];
            string targetArch = targetOsArch.ToString().Split("_")[1];
            using (new TemporaryContext())
            {
                Environment.SetEnvironmentVariable("GOOS", targetOS);
                Environment.SetEnvironmentVariable("GOARCH", targetArch);
                var scriptText = script + "\r\n//" + Utils.RandomString(10); // non-functional change that will result in the compiled exe having a varying hash
                File.WriteAllText("input.go", scriptText);
                Process.Start("go", "build -o output " + additionalArgs + " input.go").WaitForExit();
                return File.ReadAllBytes("output");
            }
        }

        public static Executable BuildExeX64(IGoScript script)
        {
            return new Executable(Build(script.Text, TargetOSArch.windows_amd64));
        }

        public static Executable BuildExeX86(IGoScript script)
        {
            return new Executable(Build(script.Text, TargetOSArch.windows_386));
        }

        public static DynamicLinkLibrary BuildDllX64(IGoScript script, string exportedFunctionName = "Execute")
        {
            // TODO: Verify that OS is Windows and that Arch is 64bit (Can't cross compile when using buildmode c-shared)

            var dllScript = ConvertToDllScript(script.Text, exportedFunctionName);
            return new DynamicLinkLibrary(Build(dllScript, TargetOSArch.windows_amd64, "-buildmode=c-shared"));
        }

        public static DynamicLinkLibrary BuildDllX86(IGoScript script, string exportedFunctionName = "Execute")
        {
            // TODO: Verify that OS is Windows and that Arch is 32bit (Can't cross compile when using buildmode c-shared)
            throw new NotImplementedException();
/*
            var dllScript = ConvertToDllScript(script.Text, exportedFunctionName);
            return new DynamicLinkLibrary(Build(dllScript, TargetOSArch.windows_386, "-buildmode=c-shared"));*/
        }

        private static string ConvertToDllScript(string script, string exportedFunctionName)
        {
            script += ExportedFunctionAppendedString(exportedFunctionName);
            if (!ContainsImport(script, "C"))
                script = AddImport(script, "C");
            return script;
        }

        private static string ExportedFunctionAppendedString(string functionName)
        {
            var script = "";
            script += "\r\n";
            script += "//export " + functionName + "\r\n";
            script += "func " + functionName + "() {" + "\r\n";
            script += "    " + "main()" + "\r\n";
            script += "}" + "\r\n";
            return script;
        }
        private static bool ContainsImport(string script, string importName)
        {
            string importStatement = @"import """ + importName + @"""";
            return script.Contains(importStatement);
        }
        private static string AddImport(string script, string importName)
        {
            string importStatement = @"import """ + importName + @"""";
            // Assuming there is at least 1 import
            int firstImportIndex = script.IndexOf("import");
            return script.Substring(0, firstImportIndex) + importStatement + "\r\n" + script.Substring(firstImportIndex);
        }
    }
}
