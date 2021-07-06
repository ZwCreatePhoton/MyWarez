using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyWarez.Plugins.MacroPack
{
    public abstract class MacroPackVBAMacro : MacroPack, IMacroDocument
    {
        public MacroPackVBAMacro(IVbaMacro vbaMacro, Extension outputExtension, byte[] templateBytes = null, Extension templateExtension = MacroPack.Extension.NONE, string password = null)
        {
            VbaMacro = vbaMacro;
            OutputExtensionValue = outputExtension;
            TemplateBytes = templateBytes;
            TemplateExtensionValue = templateExtension;
            Password = password;
        }
        private IVbaMacro VbaMacro { get; }
        private Extension OutputExtensionValue { get; }
        private Extension TemplateExtensionValue { get; }
        private byte[] TemplateBytes { get; }

        protected string Password { get; }

        public string Extension => OutputExtensionValue.ToString().ToLower();

        private byte[] Generate()
        {
            string inputContent = VbaMacro.Text;
            return MacroPack.Generate(OutputType.VBA, inputContent, OutputExtensionValue, TemplateBytes, TemplateExtensionValue, Password);
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
