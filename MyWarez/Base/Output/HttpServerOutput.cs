using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWarez.Base
{
    // TODO: Mechanism to upload files to server and start HTTP service
    public class HttpServerOutput : RemoteFileServerOutput
    {
        public HttpServerOutput(Host host, int port=80, string name="HTTP_Server", string baseDirectory = "")
            : base(host, port, name, relativeDirectory: "wwwroot", baseDirectory: baseDirectory)
        { }
    }
}
