using System.Collections.Generic;
using System.Linq;

namespace MyWarez.Core
{
    public interface IDynamicLinkLibrary : IPortableExecutable
    {

    }

    public class DynamicLinkLibrary : IPayload, IDynamicLinkLibrary
    {
        public DynamicLinkLibrary() { }
        public DynamicLinkLibrary(byte[] bytes, IEnumerable<string> exportedFunctions = null, string map = null)
        {
            Bytes = bytes;
            ExportedFunctions = exportedFunctions ?? Enumerable.Empty<string>();
            Map = map;
        }
        public PayloadType Type { get; } = PayloadType.DynamicLinkLibrary;

        public byte[] Bytes { get; }

        PortableExecutableType IPortableExecutable.Type => PortableExecutableType.Dll;

        public IEnumerable<string> ExportedFunctions { get ; }
        public string Map { get; set; }
    }
}