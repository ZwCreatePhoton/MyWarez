using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyWarez.Base
{
    public class NetcatListenerServerOutput : ListenerServerOutput
    {
        public NetcatListenerServerOutput(Host host, int port, string name = "NetcatListener", string baseDirectory = "")
            : base(host, port, name, relativeDirectory: "", baseDirectory: baseDirectory)
        {
            Add("launch.sh", LaunchScriptSh);
        }

        public string LaunchScriptSh
        {
            get => $@"
SCRIPT_DIR=""$(cd ""$( dirname ""${{BASH_SOURCE[0]}}"" )"" &> /dev/null && pwd )""
(cd ""$SCRIPT_DIR""; exec nc -lvp {Port})
".Replace("\r\n", "\n");
        }
    }
}
