using System;
using System.Collections.Generic;
using System.Text;

using MyWarez.Core;

namespace MyWarez.Plugins.MacroPack
{
    public sealed class ExcelVBAMacro : MacroPackVBAMacro, IMacroExcelDocument
    {
        public new enum OutputExtension
        {
            XLS,
            XLSM,
            XLSB
        }

        public ExcelVBAMacro(IVbaMacro vbaMacro, OutputExtension extension = OutputExtension.XLSM, ExcelDocument template = null)
            : base(vbaMacro,
                  (MacroPack.Extension)Enum.Parse(typeof(MacroPackVBAMacro.Extension), extension.ToString(), true),
                  template is null ? null : template.Bytes,
                  template is null ? MacroPack.Extension.NONE : (MacroPack.Extension)Enum.Parse(typeof(MacroPackVBAMacro.Extension), template.Type.ToUpper(), true)
                  )
        { }
    }
}
