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
        public PowerPointVBAMacro(IVbaMacro vbaMacro, OutputExtension extension = OutputExtension.PPTM, PowerPointDocument template = null)
            : base(vbaMacro,
                  (MacroPack.Extension)Enum.Parse(typeof(MacroPackVBAMacro.Extension), extension.ToString(), true),
                  template is null ? null : template.Bytes,
                  template is null ? MacroPack.Extension.NONE : (MacroPack.Extension)Enum.Parse(typeof(MacroPackVBAMacro.Extension), template.Type.ToUpper(), true)
                  )
        { }
    }
}
