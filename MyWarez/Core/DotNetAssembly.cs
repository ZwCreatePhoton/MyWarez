using System.Collections.Generic;

namespace MyWarez.Core
{
    public interface IDotNetAssembly : IFile, IPortableExecutable
    {
        public bool ImplementsInstaller => false;
        IEnumerable<string> IPortableExecutable.ExportedFunctions => throw new System.NotImplementedException();
        string IPortableExecutable.Map
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }
    }

    public class DotNetAssembly : IPayload, IDotNetAssembly
    {
        public DotNetAssembly(byte[] bytes)
        {
            Bytes = bytes;
        }
        public PayloadType Type { get; } = PayloadType.DotNetAssembly;

        public byte[] Bytes { get; }
        PortableExecutableType IPortableExecutable.Type => PortableExecutableType.Dotnet;
    }
}
