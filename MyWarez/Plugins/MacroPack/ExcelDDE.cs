using System;
using System.Collections.Generic;
using System.Text;

using MyWarez.Core;

namespace MyWarez.Plugins.MacroPack
{
    public sealed class ExcelDDE : MacroPackDDE, IExcelDocument
    {
        public ExcelDDE(ProcessList processList) : base(processList, MacroPack.Extension.XLSX)
        { }

        public string Extension => MacroPack.Extension.XLSX.ToString().ToLower();
    }
}
