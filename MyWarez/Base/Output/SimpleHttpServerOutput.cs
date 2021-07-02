using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MyWarez.Base
{
    public class SimpleHttpServerOutput : HttpServerOutput
    {
        public SimpleHttpServerOutput(Host host, int port=80, string name="SimpleHTTPServer", string baseDirectory = "")
            : base(host, port, name, baseDirectory: baseDirectory)
        {
            Add(Path.Join("..", "launch.sh"), LaunchScriptSh);
        }

        public string LaunchScriptSh {
            get => $@"
SCRIPT_DIR=""$(cd ""$( dirname ""${{BASH_SOURCE[0]}}"" )"" &> /dev/null && pwd )""
(cd ""$SCRIPT_DIR/wwwroot""; exec python2 -m SimpleHTTPServer {Port})
".Replace("\r\n", "\n");
        }
    }
}
