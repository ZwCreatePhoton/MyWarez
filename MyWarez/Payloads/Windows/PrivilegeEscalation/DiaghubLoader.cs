using System;
using System.IO;
using System.Collections.Generic;

using MyWarez.Core;
using MyWarez.Base;

namespace MyWarez.Payloads
{
    // Turning this Diaghub loader code into shellcodable C code may be difficult due to COM
    public sealed class DiaghubLoader : CCxxSource, ILoadDll
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PayloadsResourceDirectory, "Windows", "PrivilegeEscalation", nameof(DiaghubLoader));

        private static readonly string FunctionNamePlaceholder = "LoadDll";

        public DiaghubLoader(IGetTargetFilenameW getTargetFilenameW)
            : base(SourceDirectoryToSourceFiles(ResourceDirectory, additionalSources: new List<ICCxxSource>() {getTargetFilenameW}))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }
        string ICFunction.Name => ((ILoadDll)this).Name;
        public IEnumerable<string> ParameterTypes => null;

    }
}