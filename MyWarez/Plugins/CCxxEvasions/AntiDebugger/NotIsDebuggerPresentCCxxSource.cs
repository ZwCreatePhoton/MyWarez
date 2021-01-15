using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Linq;

using MyWarez.Core;
using MyWarez.Base;


namespace MyWarez.Plugins.CCxxEvasions
{
    // Contract: Returns true if IsDebuggerPresent returns false
    public interface INotIsDebuggerPresent : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "NotIsDebuggerPresent";
    }

    public sealed class NotIsDebuggerPresentCCxxSource : ShellcodeCCxxSource, INotIsDebuggerPresent, IShellcodeCCxxSourceIParameterlessCFunction, IShellcodeParameterlessCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "CCxxEvasions", "AntiDebugger", nameof(NotIsDebuggerPresentCCxxSource));

        private static readonly string FunctionNamePlaceholder = "NotIsDebuggerPresent";


        public NotIsDebuggerPresentCCxxSource()
            : base(CreateSource())
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.ReturnType => "BOOL";
        string ICFunction.Name => ((INotIsDebuggerPresent)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource()
        {
            var sourceFiles = SourceDirectoryToSourceFiles(ResourceDirectory);
            return new CCxxSource(sourceFiles);
        }
    }
}
