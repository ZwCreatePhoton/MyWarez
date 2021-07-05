using System;
using System.Collections.Generic;
using System.Text;

using MyWarez.Core;

namespace MyWarez.Plugins.MacroPack
{
    public sealed class WordVBAMacro : MacroPackVBAMacro, IMacroWordDocument
    {
        public new enum OutputExtension
        {
            DOC,
            DOCM,
            DOT,
            DOTM,
        }

        public WordVBAMacro(IVbaMacro vbaMacro, OutputExtension extension = OutputExtension.DOCM)
            : base(vbaMacro, (MacroPack.OutputExtension)Enum.Parse(typeof(MacroPackVBAMacro.OutputExtension), extension.ToString(), true))
        {
        }
    }
}
