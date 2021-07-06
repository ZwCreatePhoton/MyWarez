using System;
using System.Collections.Generic;
using System.Text;

using MyWarez.Core;

namespace MyWarez.Plugins.MacroPack
{
    public sealed class WordDDE : MacroPackDDE, IWordDocument
    {
        public WordDDE(ProcessList processList) : base(processList, MacroPack.Extension.DOCX)
        { }

        public string Extension => MacroPack.Extension.DOCX.ToString().ToLower();
    }
}
