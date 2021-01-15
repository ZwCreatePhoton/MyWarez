using System;
using System.Collections.Generic;
using System.Text;

using MyWarez.Core;

namespace MyWarez.Plugins.MacroPack
{
    public sealed class ExcelDDE : MacroPackDDE, IExcelDocument
    {
        public ExcelDDE(ProcessList processList) : base(processList, OutputExtension.XLSX)
        { }

        public string Extension => OutputExtension.XLSX.ToString().ToLower();
    }
}
