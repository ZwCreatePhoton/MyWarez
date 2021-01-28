using System.IO;
using MyWarez.Core;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            // This function needs to be run at the start.
            // This reads in a mapping of (dns/host names <-> Ip addresses <-> virtual host identifiers)
            // The mapping is used to output all the server dependencies for a virtual host in one folder
            Utils.InitHosts(File.ReadAllText(Path.Join(MyWarez.Core.Constants.ResourceDirectory, "hosts.yaml")));

            // Generate all example attacks
            AttackExamples.GenerateAll();
        }
    }
}
