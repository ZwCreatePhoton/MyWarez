using System;
using System.Collections.Generic;
using System.Text;

using MyWarez.Core;
using System.IO;

namespace MyWarez.Plugins.Msvc
{
    public sealed class AlignRSPMasmAssemblySource : AssemblySource<MASM>, IShellcodeParameterlessCFunction
    {
        private static readonly string ResourceFilepath = Path.Join(Core.Constants.PluginsResourceDirectory, "Msvc", "CompileToShellcode", "AdjustStack.asm");
        private static readonly string EntryFunctionNamePlaceholder = "ExecutePayload";

        public AlignRSPMasmAssemblySource(string entryFunctionName)
            : base(CreateSource(entryFunctionName))
        {}

        private static IEnumerable<AssemblySourceFile<MASM>> CreateSource(string entryFunctionName)
        {
            var sourceFiles = new List<AssemblySourceFile<MASM>>();
            var sourceCode = File.ReadAllText(ResourceFilepath).Replace(EntryFunctionNamePlaceholder, entryFunctionName);
            sourceFiles.Add(new AssemblySourceFile<MASM>(sourceCode, "AdjustStack.asm"));
            return sourceFiles;
        }

        string ICFunction.Name => "AlignRSP";

        public IEnumerable<string> ParameterTypes => null;
    }
}
