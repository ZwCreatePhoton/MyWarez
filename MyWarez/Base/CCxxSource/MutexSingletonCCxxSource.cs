using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Linq;

namespace MyWarez.Base
{
    // Contract: Attempts to creates a mutex. If successful then execute a parameterless function
    public interface IMutexSingleton : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "MutexSingleton";
    }

    public sealed class MutexSingletonCCxxSource : CCxxSource, IMutexSingleton
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "CCxxSource", nameof(MutexSingletonCCxxSource));
        private static readonly string ExecutePayloadSignaturePlaceholder = "void ExecutePayload(void);";
        private static readonly string ExecutePayloadPlaceholder = "ExecutePayload";
        private static readonly string MutexNamePlaceholder = Utils.StringToCArrary(@"Global\MutexSingleton", wide: true);

        private static readonly string FunctionNamePlaceholder = "MutexSingleton";


        // Merges input source into this object
        public MutexSingletonCCxxSource(ICCxxSourceIParameterlessCFunction functionSource, string mutexName = @"Global\MutexSingleton") 
            : base(MergeSourceFiles(MutexSingletonCCxxSource.CreateSource((IParameterlessCFunction)functionSource, mutexName), new List<ICCxxSource>() { functionSource }))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        // Does not merge input source into this object
        public MutexSingletonCCxxSource(IParameterlessCFunction functionSource, string mutexName = @"Global\MutexSingleton")
            : base(MutexSingletonCCxxSource.CreateSource((IParameterlessCFunction)functionSource, mutexName))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.Name => ((IMutexSingleton)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource(IParameterlessCFunction function, string mutexName)
        {
            var sourceFiles = SourceDirectoryToSourceFiles(ResourceDirectory);
            FindAndReplace(sourceFiles, ExecutePayloadSignaturePlaceholder, function.Signature);
            FindAndReplace(sourceFiles, ExecutePayloadPlaceholder, function.Name);
            FindAndReplace(sourceFiles, MutexNamePlaceholder, Utils.StringToCArrary(mutexName, wide: true));
            return new CCxxSource(sourceFiles);
        }
    }
    public sealed class MutexSingletonShellcodeCCxxSource : ShellcodeCCxxSource, IMutexSingleton, IShellcodeCCxxSourceIParameterlessCFunction
    {
        private static readonly string FunctionNamePlaceholder = "MutexSingleton";

        // Merges input source into this object
        public MutexSingletonShellcodeCCxxSource(IShellcodeCCxxSourceIParameterlessCFunction functionSource, string mutexName = @"Global\MutexSingleton")
            : base(MergeSourceFiles(MutexSingletonCCxxSource.CreateSource((IParameterlessCFunction)functionSource, mutexName), new List<ICCxxSource>() { functionSource }))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        // Does not merge input source into this object.
        public MutexSingletonShellcodeCCxxSource(IParameterlessCFunction functionSource, string mutexName = @"Global\MutexSingleton")
            : base(MutexSingletonCCxxSource.CreateSource((IParameterlessCFunction)functionSource, mutexName))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.Name => ((IMutexSingleton)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;
    }
}
