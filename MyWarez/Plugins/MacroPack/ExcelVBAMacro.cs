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

        public ExcelVBAMacro(IVbaMacro vbaMacro, OutputExtension extension = OutputExtension.XLSM)
            : base(vbaMacro, (MacroPack.OutputExtension)Enum.Parse(typeof(MacroPackVBAMacro.OutputExtension), extension.ToString(), true))
        { }
    }
}
