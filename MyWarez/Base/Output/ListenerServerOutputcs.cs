using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;

namespace MyWarez.Base
{

    public class ListenerServerOutput : FileOutput, IServerOutput
    {
        public ListenerServerOutput(Host host, int port, string name, string relativeDirectory="", string baseDirectory="")
            : base(Path.Join(Path.Join(baseDirectory, IServerOutput.ServerOutputDirectoryName), host.HostId, port.ToString() + "_" + name, relativeDirectory))
        {
            Host = host;
            Port = port;
            Name = name;
        }

        public Host Host { get; }
        public int Port { get; }
        public string Name { get; }
    }
}
