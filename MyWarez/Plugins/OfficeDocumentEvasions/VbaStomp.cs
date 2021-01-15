using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MyWarez.Plugins.OfficeDocumentEvasions
{
    // Not working ?
    public static class VbaStomp
    {
        private static string ResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "OfficeDocumentEvasions", "vba_stomp");
        private static string ScriptName = "vba_stomp.py";

        private static byte[] _Stomp(IMacroDocument input)
        {
            using (new TemporaryContext())
            {
                Utils.CopyFilesRecursively(ResourceDirectory, ".");
                var inputFilename = "input." + input.Extension;
                File.WriteAllBytes(inputFilename, input.Bytes);
                Process.Start("python", $"{ScriptName} {inputFilename}").WaitForExit();
                var outputFilename = inputFilename + ".stomped";
                return File.ReadAllBytes(outputFilename);
            }
        }

        public static MacroExcelDocument Stomp(IMacroExcelDocument document)
        {
            return new MacroExcelDocument(_Stomp(document));
        }

        public static MacroWordDocument Stomp(IMacroWordDocument document)
        {
            return new MacroWordDocument(_Stomp(document));
        }

        public static MacroAccessDocument Stomp(IMacroAccessDocument document)
        {
            return new MacroAccessDocument(_Stomp(document));
        }
        public static MacroPowerPointDocument Stomp(IMacroPowerPointDocument document)
        {
            return new MacroPowerPointDocument(_Stomp(document));
        }

    }
}
