using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyWarez.Plugins.MacroPack
{
    public abstract class MacroPackDDE : MacroPack
    {
        public MacroPackDDE(ProcessList processList, OutputExtension outputExtension)
        {
            ProcessList = processList;
            OutputExtensionValue = outputExtension;
        }

        private ProcessList ProcessList { get; }
        private OutputExtension OutputExtensionValue { get; }

        private byte[] Generate()
        {
            var cmdlines = ((ProcessList)ProcessList).Processes.Select(p => p.CmdLine.ToString());
            string inputContent = string.Join("\n", cmdlines);
            return MacroPack.Generate(OutputType.DDE, inputContent, OutputExtensionValue);
        }
        public byte[] Bytes
        {
            get
            {
                if (CachedBytes == null)
                    CachedBytes = Generate();
                return CachedBytes;
            }
        }
        private byte[] CachedBytes = null;
    }
}
