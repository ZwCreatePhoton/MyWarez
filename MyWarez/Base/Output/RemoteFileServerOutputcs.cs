using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;

namespace MyWarez.Base
{
    public interface IServerOutput
    {
        public static string ServerOutputDirectoryName = "Server";
        public Host Host { get; }
        public int Port { get; }
        public string Name { get; }
    }

    public class RemoteFileServerOutput : FileOutput, IServerOutput
    {
        public RemoteFileServerOutput(Host host, int port, string name, string relativeDirectory="")
            : base(Path.Join(Path.Join(Core.Constants.OutputDirectory, IServerOutput.ServerOutputDirectoryName), host.HostId, port.ToString() + "_" + name, relativeDirectory))
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
