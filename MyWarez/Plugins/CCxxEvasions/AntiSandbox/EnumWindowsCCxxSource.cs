using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Linq;

using MyWarez.Core;


namespace MyWarez.Plugins.CCxxEvasions
{
    // Contract: Returns true if analysis tools' windows are not present
    public interface IEnumWindows : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "EnumWindowsFunction";
    }

    // TODO: confirm if source is shellcodable. It looks like it'll be easy to modify it to be so
    // TODO: User specified window titles
    public sealed class EnumWindowsCCxxSource : CCxxSource, IEnumWindows
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "CCxxEvasions", "AntiSandbox", nameof(EnumWindowsCCxxSource));

        private static readonly string FunctionNamePlaceholder = "EnumWindowsFunction";

        public EnumWindowsCCxxSource()
            : base(CreateSource())
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.ReturnType => "BOOL";
        string ICFunction.Name => ((IEnumWindows)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource()
        {
            var sourceFiles = SourceDirectoryToSourceFiles(ResourceDirectory);
            return new CCxxSource(sourceFiles);
        }
    }
}
