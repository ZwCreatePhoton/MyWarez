using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Linq;

namespace MyWarez.Base
{
    public abstract class ExeMainCCxxSource : CCxxSource
    {
        public ExeMainCCxxSource(ICCxxSource source, string entryPoint="main")
            : this(source.SourceFiles, entryPoint:entryPoint) { }
        public ExeMainCCxxSource(IEnumerable<CCxxSourceFile> source, string entryPoint = "main") 
            : base(source)
        {
            EntryPoint = entryPoint;
        }

        private string EntryPoint { get; }
    }

    public class FunctionCallExeMainCCxxSource : ExeMainCCxxSource
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "CCxxSource", nameof(FunctionCallExeMainCCxxSource));
        private static readonly string PayloadFunctionPlaceholder = "ExecutePayload";

        // Merges input source into this object
        public FunctionCallExeMainCCxxSource(ICCxxSourceIParameterlessCFunction source)
            : base(SourceDirectoryToSourceFiles(ResourceDirectory, additionalSources: new List<ICCxxSource>() { source }))
        {
            FindAndReplace(SourceFiles, PayloadFunctionPlaceholder, source.Name);
        }
        // Does not merge input source into this object
        public FunctionCallExeMainCCxxSource(IParameterlessCFunction source)
            : base(SourceDirectoryToSourceFiles(ResourceDirectory))
        {
            FindAndReplace(SourceFiles, PayloadFunctionPlaceholder, source.Name);
        }
    }

    public class FunctionCallExeWinMainCCxxSource : ExeMainCCxxSource
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "CCxxSource", nameof(FunctionCallExeWinMainCCxxSource));
        private static readonly string PayloadFunctionPlaceholder = "ExecutePayload";

        // Merges input source into this object
        public FunctionCallExeWinMainCCxxSource(ICCxxSourceIParameterlessCFunction source)
            : base(SourceDirectoryToSourceFiles(ResourceDirectory, additionalSources: new List<ICCxxSource>() { source }))
        {
            FindAndReplace(SourceFiles, PayloadFunctionPlaceholder, source.Name);
        }
        // Does not merge input source into this object
        public FunctionCallExeWinMainCCxxSource(IParameterlessCFunction source)
            : base(SourceDirectoryToSourceFiles(ResourceDirectory))
        {
            FindAndReplace(SourceFiles, PayloadFunctionPlaceholder, source.Name);
        }
    }
}
