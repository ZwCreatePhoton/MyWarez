using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MyWarez.Base
{
    public class WsgiDAVServerOutput : HttpServerOutput
    {
        public WsgiDAVServerOutput(Host host, int port=80, string name="WsgiDAV", string baseDirectory = "")
            : base(host, port, name, baseDirectory: baseDirectory)
        {
            Add(Path.Join("..", "launch.sh"), LaunchScriptSh);
        }

        public string LaunchScriptSh {
            get => $@"
SCRIPT_DIR=""$(cd ""$( dirname ""${{BASH_SOURCE[0]}}"" )"" &> /dev/null && pwd )""
(cd ""$SCRIPT_DIR""; exec wsgidav --auth anonymous --host=0.0.0.0 --port {Port} --root wwwroot)
".Replace("\r\n", "\n");
        }
    }
}
