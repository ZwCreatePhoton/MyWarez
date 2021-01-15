using System.Collections.Generic;
using System.IO;

using MyWarez.Core;
using MyWarez.Base;
using System;

namespace MyWarez.Payloads
{
    public sealed class CreateProcess : ShellcodeCCxxSource, IShellcodeCCxxSourceIParameterlessCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PayloadsResourceDirectory, "Windows", "Execution", "NativeAPI", nameof(CreateProcess));
        private static readonly string CmdlinePlaceholder = Utils.StringToCArrary(@"notepad", wide: false);

        private static readonly string FunctionNamePlaceholder = "CreateProcessFunction";


        public CreateProcess(string cmdline)
            : base(SourceDirectoryToSourceFiles(ResourceDirectory))
        {
            FindAndReplace(SourceFiles, CmdlinePlaceholder, Utils.StringToCArrary(cmdline, wide: false));
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.Name => FunctionNamePlaceholder + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;
    }
}