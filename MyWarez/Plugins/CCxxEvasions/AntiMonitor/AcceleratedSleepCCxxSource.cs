using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Linq;

using MyWarez.Core;
using MyWarez.Base;


namespace MyWarez.Plugins.CCxxEvasions
{
    // Contract: Returns true if Sleep is not accelerated
    public interface IAcceleratedSleep : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "AcceleratedSleep";
    }

    // TODO: User specified sleep time instead of the default time of 60000 milliseconds
    public sealed class AcceleratedSleepCCxxSource : ShellcodeCCxxSource, IAcceleratedSleep, IShellcodeCCxxSourceIParameterlessCFunction, IShellcodeParameterlessCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "CCxxEvasions", "AntiMonitor", nameof(AcceleratedSleepCCxxSource));

        private static readonly string FunctionNamePlaceholder = "AcceleratedSleep";

        public AcceleratedSleepCCxxSource()
            : base(CreateSource())
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);

        }

        string ICFunction.ReturnType => "BOOL";
        string ICFunction.Name => ((IAcceleratedSleep)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource()
        {
            var sourceFiles = SourceDirectoryToSourceFiles(ResourceDirectory);
            return new CCxxSource(sourceFiles);
        }
    }
}
