/*using System;
using System.Collections.Generic;
using System.Text;

namespace MyWarez.Payloads
{
    public class StaticHtmlApplication : StaticFilePayload
    {
        public StaticHtmlApplication(string sourceCode) : base(sourceCode) { }
        public StaticHtmlApplication(byte[] bytes) : base(bytes) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => fileType == Tonsil.Files.FileType.Hta;
        public override PayloadType Type { get; } = PayloadType.HtmlApplication;
        public override string Name { get; } = "StaticHtmlApplication";
        public override string Description { get; } = "Static HTML Application File";
    }
}*/