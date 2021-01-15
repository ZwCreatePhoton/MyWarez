using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;


using MyWarez.Core;
using MyWarez.Base;

namespace MyWarez.Payloads
{
    public class DiaghubDllCCxxSource : SkeletonDllMainCCxxSource
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PayloadsResourceDirectory, "Windows", "PrivilegeEscalation", nameof(DiaghubDllCCxxSource));
        private static readonly IEnumerable<string> ExcludeFiles = new List<string>() { "payload.c", "dllmain.c" };
        private static readonly IEnumerable<string> Exports = new List<string>() { "DllGetClassObject" };
        private static readonly string PayloadFunctionPlaceholder = "ExecutePayload";

        // Merges only the elements that implement ICCxxSourceIParameterlessCFunction
        public DiaghubDllCCxxSource(IParameterlessCFunction source, bool mergeCCxxSources = true)
            : base(new[]{new CCxxSource(
                    SourceDirectoryToSourceFiles(
                        ResourceDirectory,
                        excludeFiles: ExcludeFiles,
                        additionalSources: new[] { source }.Where(x => mergeCCxxSources && x is ICCxxSourceIParameterlessCFunction).Cast<ICCxxSource>()
                    ))},
                  Exports)
        {
            FindAndReplace(SourceFiles, PayloadFunctionPlaceholder, ((IParameterlessCFunction)source).Name);
        }
    }
}
