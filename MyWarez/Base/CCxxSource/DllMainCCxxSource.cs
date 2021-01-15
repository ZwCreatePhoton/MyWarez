using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Linq;

namespace MyWarez.Base
{

    public abstract class DllMainCCxxSource : CCxxSource
    {
        public DllMainCCxxSource(ICCxxSource source, IEnumerable<string> exportedFunctions = null, string entryPoint="DllMain")
            : this(source.SourceFiles, exportedFunctions ?? Enumerable.Empty<string>(), entryPoint: entryPoint) { }
        public DllMainCCxxSource(IEnumerable<CCxxSourceFile> source, IEnumerable<string> exportedFunctions, string entryPoint = "DllMain") 
            : base(source)
        {
            EntryPoint = entryPoint;
            ExportedFunctions = exportedFunctions;
        }

        public IEnumerable<string> ExportedFunctions { get; }

        private string EntryPoint { get; }
    }

    public class SkeletonDllMainCCxxSource : DllMainCCxxSource
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "CCxxSource", nameof(SkeletonDllMainCCxxSource));

        public SkeletonDllMainCCxxSource(ICCxxSource source, IEnumerable<string> exportedFunctions = null, bool mergeCCxxSources = true)
            : this(new []{ source}, exportedFunctions, mergeCCxxSources)
        { }
        public SkeletonDllMainCCxxSource(IEnumerable<ICCxxSource> source, IEnumerable<string> exportedFunctions = null, bool mergeCCxxSources = true)
            : base(
                  SourceDirectoryToSourceFiles(
                      ResourceDirectory,
                      additionalSources: mergeCCxxSources ? source : new ICCxxSource[] { }
                      ),
                  exportedFunctions)
        { }
    }

    public class ProcessAttachDllMainCCxxSource : DllMainCCxxSource
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "CCxxSource", nameof(ProcessAttachDllMainCCxxSource));
        private static readonly string PayloadFunctionPlaceholder = "ExecutePayload";

        // Merges only the elements that implement ICCxxSourceIParameterlessCFunction
        public ProcessAttachDllMainCCxxSource(IParameterlessCFunction source, IEnumerable<string> exportedFunctions = null, bool mergeCCxxSources = true)
            : base( SourceDirectoryToSourceFiles(
                        ResourceDirectory,
                        additionalSources: new[] { source }.Where(x => mergeCCxxSources && x is ICCxxSourceIParameterlessCFunction).Cast<ICCxxSource>()
                    ),
                    exportedFunctions)
        {
            FindAndReplace(SourceFiles, PayloadFunctionPlaceholder, source.Name);
        }
    }
}
