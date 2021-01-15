using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWarez.Base
{
    // TODO: Mechanism to upload files to server and start SMB service
    public class SmbServerOutput : RemoteFileServerOutput
    {
        public SmbServerOutput(string sharename, Host host, int port = 445, string name = "SMB_Server")
            : base(host, port, name, sharename)
        {
            Sharename = sharename;
        }

        public string Sharename { get; }
    }
}
