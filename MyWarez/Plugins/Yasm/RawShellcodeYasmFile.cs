using System;
using System.Collections.Generic;
using System.Text;

using MyWarez.Core;

namespace MyWarez.Plugins.Yasm
{
    public sealed class RawShellcodeYasmAssemblySource : AssemblySource<YASM> , IShellcodeParameterlessCFunction
    {
        public RawShellcodeYasmAssemblySource(IShellcode shellcode, string symbolName, string sectionName=null, bool? sectionWritable = null)
            : base(CreateSource(shellcode, symbolName, sectionName, sectionWritable))
        {
            SymbolName = symbolName;
        }

        private static IEnumerable<AssemblySourceFile<YASM>> CreateSource(IShellcode shellcode, string symbolname, string sectionName = null, bool? sectionWritable = null)
        {
            var sourceFiles = new List<AssemblySourceFile<YASM>>();
            var shellcodeFilename = Utils.RandomString(10) + ".bin";
            sourceFiles.Add(new AssemblySourceFile<YASM>(shellcode.Bytes, shellcodeFilename));
            string sectionLine;
            if (sectionName == null && !sectionWritable.HasValue)
                sectionLine = "";//SECTION '{1}' {2} execute,read\r\n   //sectionName, sectionWritable ? "write," : ""
            else
            {
                sectionName = sectionName ?? ".text";
                sectionLine = $"SECTION '{sectionName}'";
                if (sectionWritable.HasValue && sectionWritable.Value)
                    sectionLine += " write, execute, read";
                sectionLine += "\r\n";
            }
            var source = $"Global {symbolname}\r\n{sectionLine}{symbolname}:\r\n\tincbin '{shellcodeFilename}'";
            sourceFiles.Add(new AssemblySourceFile<YASM>(source, Utils.RandomString(10) + ".asm"));
            return sourceFiles;
        }

        string ICFunction.Name => SymbolName;

        public IEnumerable<string> ParameterTypes => null;

        public string SymbolName { get; }
    }
}
