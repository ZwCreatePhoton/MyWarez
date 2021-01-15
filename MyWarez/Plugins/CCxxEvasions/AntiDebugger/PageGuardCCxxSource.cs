using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Linq;

using MyWarez.Core;


namespace MyWarez.Plugins.CCxxEvasions
{
    // Contract: Returns true if <something about guard memory pages>
    public interface IPageGuard : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "PageGuard";
    }

    // TODO: confirm if source is shellcodable. It looks like it might be. But if it's not, then modify it to be so
    public sealed class PageGaurdCCxxSource : CCxxSource, IPageGuard
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "CCxxEvasions", "AntiDebugger", nameof(PageGaurdCCxxSource));

        private static readonly string FunctionNamePlaceholder = "PageGuard";


        public PageGaurdCCxxSource()
            : base(CreateSource())
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.ReturnType => "BOOL";
        string ICFunction.Name => ((IPageGuard)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource()
        {
            var sourceFiles = SourceDirectoryToSourceFiles(ResourceDirectory);
            return new CCxxSource(sourceFiles);
        }
    }
}
