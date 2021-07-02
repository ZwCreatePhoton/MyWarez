using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyWarez.Base
{
    public class ImpacketSmbServerOutput : SmbServerOutput
    {
        public ImpacketSmbServerOutput(string sharename, Host host, int port = 445, string name = "ImpacketSMBServer", string baseDirectory = "",
            string username = null,
            string password = null)
            : base(sharename, host, port, name, baseDirectory)
        {
            Username = username;
            Password = password;
            Add(Path.Join("..", "launch.sh"), LaunchScriptSh);
        }

        public string Username { get; }
        public string Password { get; }

        public string LaunchScriptSh
        {
            get => @$"
SCRIPT_DIR=""$(cd ""$( dirname ""${{BASH_SOURCE[0]}}"" )"" &> /dev/null && pwd )""
(cd ""$SCRIPT_DIR""; exec impacket-smbserver -smb2support  -port {Port} -username '{Username}' -password '{Password}' {Sharename} {Sharename})
".Replace("\r\n", "\n");
        }
    }
}
