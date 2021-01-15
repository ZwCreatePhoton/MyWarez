using System;
using System.Collections.Generic;
using System.Text;

using MyWarez.Core;

namespace MyWarez.Plugins.MacroPack
{
    public sealed class PowerPointVBAMacro : MacroPackVBAMacro, IMacroPowerPointDocument
    {
        public new enum OutputExtension
        {
            PPT,
            PPTM
        }
        public PowerPointVBAMacro(IVbaMacro vbaMacro, OutputExtension extension = OutputExtension.PPTM)
            : base(vbaMacro, (MacroPack.OutputExtension)Enum.Parse(typeof(MacroPackVBAMacro.OutputExtension), extension.ToString(), true))
        { }
    }
}
