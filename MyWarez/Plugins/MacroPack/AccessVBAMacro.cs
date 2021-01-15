using System;
using System.Collections.Generic;
using System.Text;

using MyWarez.Core;

namespace MyWarez.Plugins.MacroPack
{
    public sealed class AccessVBAMacro : MacroPackVBAMacro, IMacroAccessDocument
    {
        public new enum OutputExtension
        {
            ACCDB
        }

        public AccessVBAMacro(IVbaMacro vbaMacro, OutputExtension extension = OutputExtension.ACCDB)
            : base(vbaMacro, (MacroPack.OutputExtension)Enum.Parse(typeof(MacroPackVBAMacro.OutputExtension), extension.ToString(), true))
        { }
    }
}
