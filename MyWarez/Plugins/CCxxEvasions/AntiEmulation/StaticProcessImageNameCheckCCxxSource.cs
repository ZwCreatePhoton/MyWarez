using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Linq;

using MyWarez.Core;
using MyWarez.Base;


namespace MyWarez.Plugins.CCxxEvasions
{
    // Contract: Returns true if the current process image path equals a user specified path
    public interface IStaticProcessImageNameCheck : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "StaticProcessImageNameCheck";
    }

    public sealed class StaticProcessImageNameCheckCCxxSource : ShellcodeCCxxSource, IStaticProcessImageNameCheck, IShellcodeCCxxSourceIParameterlessCFunction, IShellcodeParameterlessCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "CCxxEvasions", "AntiEmulation", nameof(StaticProcessImageNameCheckCCxxSource));
        private static readonly string ProcessImageNamePlaceholder = Utils.StringToCArrary(@"C:\Windows\explorer.exe", wide: true);

        private static readonly string FunctionNamePlaceholder = "StaticProcessImageNameCheck";

        public StaticProcessImageNameCheckCCxxSource(string processImageName = @"C:\Windows\explorer.exe")
            : base(MergeSourceFiles(CreateSource(processImageName)))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);

        }

        string ICFunction.ReturnType => "BOOL";
        string ICFunction.Name => ((IStaticProcessImageNameCheck)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource(string processImageName)
        {
            var sourceFiles = SourceDirectoryToSourceFiles(ResourceDirectory);
            FindAndReplace(sourceFiles, ProcessImageNamePlaceholder, Utils.StringToCArrary(processImageName, wide: true));
            return new CCxxSource(sourceFiles);
        }
    }
}
