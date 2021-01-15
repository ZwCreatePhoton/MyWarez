using System.Collections.Generic;
using System.IO;

using MyWarez.Core;
using MyWarez.Base;

namespace MyWarez.Payloads
{
    // Gives Full access to Authenticated Users for a given target file
    // Some restrictions apply

    // TODO: simultatansiosuly implement ISingleOutputVisualStudioSolution, IDynamicLinkLibrary, IExecutable interfaces as well (for pure convenience)
    // TODO: Implement a caching mechanism for IDynamicLinkLibrary.Bytes, IExecutable.Bytes
    public class CVE_2018_8584 : CCxxSource, ICCxxSourceIParameterlessCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PayloadsResourceDirectory, "Windows", "PrivilegeEscalation", nameof(CVE_2018_8584));
        private static readonly IEnumerable<string> ExcludeFiles = new List<string>() { "TargetPath.c", "main.cpp" };
        private static readonly string FunctionNamePlaceholder = "CVE_2018_8584";

        public CVE_2018_8584() : this(new LicenseTargetPath()) { }
        public CVE_2018_8584(ITargetPathW targetPathW) : base(SourceDirectoryToSourceFiles(ResourceDirectory, ExcludeFiles, new List<ICCxxSource>() { targetPathW }))
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }
        string ICFunction.Name => "CVE_2018_8584" + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

    }
}
