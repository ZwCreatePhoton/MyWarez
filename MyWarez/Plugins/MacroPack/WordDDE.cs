using System;
using System.Collections.Generic;
using System.Text;

using MyWarez.Core;

namespace MyWarez.Plugins.MacroPack
{
    public sealed class WordDDE : MacroPackDDE, IWordDocument
    {
        public WordDDE(ProcessList processList) : base(processList, OutputExtension.DOCX)
        { }

        public string Extension => OutputExtension.DOCX.ToString().ToLower();
    }
}
