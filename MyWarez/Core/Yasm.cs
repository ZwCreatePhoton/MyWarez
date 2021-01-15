/*using System;
using System.Collections.Generic;
using System.Text;

namespace MyWarez.Payloads
{
    public class StaticYasm : StaticFilePayload
    {
        public StaticYasm(string sourceCode) : base(sourceCode) { }
        public StaticYasm(byte[] bytes) : base(bytes) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => false;
        public override PayloadType Type { get; } = PayloadType.Cpp;
        public override string Name { get; } = "StaticYasm";
        public override string Description { get; } = "Static YASM File";
    }
}*/