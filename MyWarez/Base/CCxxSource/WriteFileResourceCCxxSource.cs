using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Linq;

namespace MyWarez.Base
{
    // Contract: Write data to a file on the filesystem
    public interface IWriteFileResource : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "WriteFileResource";
    }


    // TODO: Non-Shellcode variant of WriteFileResourceShellcodeCCxxSource ?

    // TODO: a variant that allocates memory in the heap instead of the stack ?
    // Will be limited by the maximum stack size. (Default is 1 MB, is configurable in EXEs)
    // TODO: a variant that uses multiple functions GetFileResource functions to return chunked data (10K ? 100K ?), after each check, process it and fetch the next one. Neccessary so that we don't use too much stack space.
    public class WriteFileResourceCCxxSource<T, U> : ShellcodeCCxxSource, IWriteFileResource, IShellcodeCCxxSourceIParameterlessCFunction, IShellcodeParameterlessCFunction
        where T : IGetTargetPathW, IShellcodeCCxxSource
        where U : IGetFileResource, IShellcodeCCxxSource
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "CCxxSource", nameof(WriteFileResourceCCxxSource<T, U>));
        private const int BufferSizePlaceholder = 99999;
        private static readonly string FunctionNamePlaceholder = "WriteFileResource";
        private static readonly string GetTargetPathWFunctionNamePlaceholder = "GetTargetPathW";
        private static readonly string GetFileResourceFunctionNamePlaceholder = "GetFileResource";

        public WriteFileResourceCCxxSource(T getTargetPathW, StaticFileResourceCCxxSource getFileResource)
            : base(SourceDirectoryToSourceFiles(ResourceDirectory, additionalSources: new List<ICCxxSource>(){getTargetPathW, getFileResource }))
        {
            FindAndReplace(SourceFiles, BufferSizePlaceholder.ToString(), getFileResource.Size.ToString());
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
            FindAndReplace(SourceFiles, GetTargetPathWFunctionNamePlaceholder+"(", ((IGetTargetPathW)getTargetPathW).Name + "(");
            FindAndReplace(SourceFiles, GetFileResourceFunctionNamePlaceholder + "(", ((ICFunction)getFileResource).Name + "(");
        }

        public WriteFileResourceCCxxSource(T getTargetPathW, U getFileResource, int bufferSize = BufferSizePlaceholder) 
            : base(SourceDirectoryToSourceFiles(ResourceDirectory, additionalSources: new List<ICCxxSource>(){getTargetPathW, getFileResource}))
        {
            FindAndReplace(SourceFiles, BufferSizePlaceholder.ToString(), bufferSize.ToString());
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
            FindAndReplace(SourceFiles, GetTargetPathWFunctionNamePlaceholder + "(", ((IGetTargetPathW)getTargetPathW).Name + "(");
            FindAndReplace(SourceFiles, GetFileResourceFunctionNamePlaceholder + "(", ((ICFunction)getFileResource).Name + "(");
        }

        string ICFunction.Name => ((IWriteFileResource)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

    }
}
