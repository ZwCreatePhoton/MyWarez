using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Linq;

using MyWarez.Core;
using MyWarez.Base;


namespace MyWarez.Plugins.CCxxEvasions
{
    // Contract: Returns true if CheckRemoteDebuggerPresent returns false
    public interface IRemoteDebugger : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "RemoteDebugger";
    }

    public sealed class RemoteDebuggerCCxxSource : ShellcodeCCxxSource, IRemoteDebugger, IShellcodeCCxxSourceIParameterlessCFunction, IShellcodeParameterlessCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "CCxxEvasions", "AntiDebugger", nameof(RemoteDebuggerCCxxSource));

        private static readonly string FunctionNamePlaceholder = "RemoteDebugger";

        public RemoteDebuggerCCxxSource()
            : base(CreateSource())
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder+"(", ((ICFunction)this).Name+"(");

        }

        string ICFunction.ReturnType => "BOOL";
        string ICFunction.Name => ((IRemoteDebugger)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource()
        {
            var sourceFiles = SourceDirectoryToSourceFiles(ResourceDirectory);
            return new CCxxSource(sourceFiles);
        }
    }
}
