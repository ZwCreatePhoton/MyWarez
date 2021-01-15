using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Linq;

using MyWarez.Core;
using MyWarez.Base;


namespace MyWarez.Plugins.CCxxEvasions
{
    // Contract: Returns true if the mouse moves within the first N (=10) seconds
    public interface IMouseCursor : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "MouseCursor";
    }

    // TODO: User specified sleep time
    public sealed class MouseCursorCCxxSource : ShellcodeCCxxSource, IMouseCursor, IShellcodeCCxxSourceIParameterlessCFunction, IShellcodeParameterlessCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "CCxxEvasions", "AntiMonitor", nameof(MouseCursorCCxxSource));

        private static readonly string FunctionNamePlaceholder = "MouseCursor";

        public MouseCursorCCxxSource()
            : base(CreateSource())
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);

        }

        string ICFunction.ReturnType => "BOOL";
        string ICFunction.Name => ((IMouseCursor)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource()
        {
            var sourceFiles = SourceDirectoryToSourceFiles(ResourceDirectory);
            return new CCxxSource(sourceFiles);
        }
    }
}
