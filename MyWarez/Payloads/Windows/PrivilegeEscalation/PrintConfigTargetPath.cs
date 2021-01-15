using System.IO;
using System.Collections.Generic;
using MyWarez.Base;

namespace MyWarez.Payloads
{
    public sealed class PrintConfigTargetPath : ShellcodeCCxxSource, ITargetPathW, IShellcodeCCxxSourceIParameterlessCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PayloadsResourceDirectory, "Windows", "PrivilegeEscalation", nameof(PrintConfigTargetPath));
        public PrintConfigTargetPath() : base(SourceDirectoryToSourceFiles(ResourceDirectory)) { }
        public IEnumerable<string> ParameterTypes => null;
    }
}