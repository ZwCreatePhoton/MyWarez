using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Linq;

using MyWarez.Core;
using MyWarez.Base;


namespace MyWarez.Plugins.CCxxEvasions
{
    // Contract: Returns true if Sleep is not patched
    public interface ISleepPatched : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "SleepPatched";
    }

    // TODO: User specified sleep time instead of the default time
    public sealed class SleepPatchedCCxxSource : ShellcodeCCxxSource, ISleepPatched, IShellcodeCCxxSourceIParameterlessCFunction, IShellcodeParameterlessCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "CCxxEvasions", "AntiMonitor", nameof(SleepPatchedCCxxSource));

        private static readonly string FunctionNamePlaceholder = "SleepPatched";

        public SleepPatchedCCxxSource()
            : base(CreateSource())
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.ReturnType => "BOOL";
        string ICFunction.Name => ((ISleepPatched)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource()
        {
            var sourceFiles = SourceDirectoryToSourceFiles(ResourceDirectory);
            return new CCxxSource(sourceFiles);
        }
    }
}
