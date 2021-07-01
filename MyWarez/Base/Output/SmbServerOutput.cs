using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWarez.Base
{
    // TODO: Mechanism to upload files to server and start SMB service
    public class SmbServerOutput : RemoteFileServerOutput
    {
        public SmbServerOutput(string sharename, Host host, int port = 445, string name = "SMB_Server", string baseDirectory = "")
            : base(host, port, name, relativeDirectory: sharename, baseDirectory: baseDirectory)
        {
            Sharename = sharename;
        }

        public string Sharename { get; }
    }
}
