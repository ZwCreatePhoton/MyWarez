using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyWarez.Core
{
    public class Host
    {
        public static List<Host> All = new List<Host>();

        public static Host GetHostByHostName(string hostName)
        {
            foreach (var host in All)
                if (host.HostName == hostName)
                    return host;
            return null;
        }

        public Host(
            string hostId,      // Unique identifier for a host machine
            string hostName,    // DNS name
            string ipAddress    // Ip address the DNS name resolves to
            )
        {
            HostId = hostId;
            HostName = hostName;
            IpAddress = ipAddress;
            All.Add(this);
        }

        public string HostId { get; }
        public string HostName { get; }
        public string IpAddress { get; }

        public override string ToString()
        {
            return HostName;
        }

        public static implicit operator string(Host host)
        {
            return host.ToString();
        }
    }
}
