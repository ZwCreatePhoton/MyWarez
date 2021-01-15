using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyWarez.Core
{
    public interface IPortableExecutable : IFile
    {
        public bool HasEntryPoint => false;
        public PortableExecutableType Type { get; }
        public IEnumerable<string> ExportedFunctions { get; }
        public string Map { get; set; }
    }

    public enum PortableExecutableType
    {
        Exe,
        Dll,
        Sys,
        Dotnet
    }
}