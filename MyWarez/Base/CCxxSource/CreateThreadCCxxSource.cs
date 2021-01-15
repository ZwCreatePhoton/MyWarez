using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Linq;

namespace MyWarez.Base
{
    // Contract: Creates a thread to execute a parameterless function
    public interface ICreateThread : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "CreateThreadFunc";
    }

    public sealed class CreateThreadCCxxSource : CCxxSource, ICreateThread
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "CCxxSource", nameof(CreateThreadCCxxSource));
        private static readonly string ThreadFunctionSignaturePlaceholder = "void ThreadFunction(void);";
        private static readonly string ThreadFunctionPlaceholder = "ThreadFunction";

        private static readonly string FunctionNamePlaceholder = "CreateThreadFunc";


        // Merges input source into this object
        public CreateThreadCCxxSource(ICCxxSourceIParameterlessCFunction functionSource) 
            : base(MergeSourceFiles(CreateThreadCCxxSource.CreateSource((IParameterlessCFunction)functionSource), new List<ICCxxSource>() { functionSource }))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        // Does not merge input source into this object
        public CreateThreadCCxxSource(IParameterlessCFunction functionSource)
            : base(CreateThreadCCxxSource.CreateSource((IParameterlessCFunction)functionSource))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.Name => ((ICreateThread)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource(IParameterlessCFunction function)
        {
            var sourceFiles = SourceDirectoryToSourceFiles(ResourceDirectory);
            FindAndReplace(sourceFiles, ThreadFunctionSignaturePlaceholder, function.Signature);
            FindAndReplace(sourceFiles, ThreadFunctionPlaceholder, function.Name);
            return new CCxxSource(sourceFiles);
        }
    }
    public sealed class CreateThreadShellcodeCCxxSource : ShellcodeCCxxSource, ICreateThread, IShellcodeCCxxSourceIParameterlessCFunction
    {
        private static readonly string FunctionNamePlaceholder = "CreateThreadFunc";

        // Merges input source into this object
        public CreateThreadShellcodeCCxxSource(IShellcodeCCxxSourceIParameterlessCFunction functionSource)
            : base(MergeSourceFiles(CreateThreadCCxxSource.CreateSource((IParameterlessCFunction)functionSource), new List<ICCxxSource>() { functionSource }))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        // Does not merge input source into this object.
        public CreateThreadShellcodeCCxxSource(IParameterlessCFunction functionSource)
            : base(CreateThreadCCxxSource.CreateSource((IParameterlessCFunction)functionSource))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.Name => ((ICreateThread)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

    }
}
