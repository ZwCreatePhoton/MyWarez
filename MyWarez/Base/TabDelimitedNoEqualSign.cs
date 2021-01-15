
using MyWarez.Core;

namespace MyWarez.Base
{
    public class TabDelimitedNoEqualSign : TabDelimited
    {
        public TabDelimitedNoEqualSign(ExcelWorksheet document) : base(document) { }
        public override string Text => NewLine + base.Text.Replace("=", "");
    }
}
