using System.Collections.Generic;
using System.Linq;

namespace MyWarez.Core
{
    public interface IExecutable : IPortableExecutable
    {

    }

    public class Executable : IPayload, IExecutable
    {
        public Executable() { }

        public Executable(byte[] bytes, IEnumerable<string> exportedFunctions = null, string map = null)
        {
            Bytes = bytes;
            ExportedFunctions = exportedFunctions ?? Enumerable.Empty<string>();
            Map = map;
        }
        public PayloadType Type { get; } = PayloadType.Executable;

        public byte[] Bytes { get; }
        PortableExecutableType IPortableExecutable.Type => PortableExecutableType.Exe;
        public IEnumerable<string> ExportedFunctions { get; }

        // TODO: store metadata as properties instead of as a string of the map file.
        public string Map { get; set; }
    }
}