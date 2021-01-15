using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyWarez.Plugins.MacroPack
{
    public abstract class MacroPackVBAMacro : MacroPack, IMacroDocument
    {
        public MacroPackVBAMacro(IVbaMacro vbaMacro, OutputExtension outputExtension)
        {
            VbaMacro = vbaMacro;
            OutputExtensionValue = outputExtension;
        }
        private IVbaMacro VbaMacro { get; }
        private OutputExtension OutputExtensionValue { get; }

        public string Extension => OutputExtensionValue.ToString().ToLower();

        private byte[] Generate()
        {
            string inputContent = VbaMacro.Text;
            return MacroPack.Generate(OutputType.VBA, inputContent, OutputExtensionValue);
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
