
using MyWarez.Base;

namespace MyWarez.Payloads
{
    public sealed class LicenseTargetPath : StaticTargetPathCCxxSource
    {
        private static readonly string LicenseRtfPath = @"C:\Windows\System32\License.rtf";
        public LicenseTargetPath() : base(LicenseRtfPath) { }
    }
}