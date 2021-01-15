using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyWarez.Core
{
    public interface IAssembler<T, U>
        where T : IAssemblyLanguage
        where U : ObjectFile, new()
    {
        public U Assemble(AssemblySourceFile<T> assemblySourceFile);
        public StaticLibrary<U> Assemble(AssemblySource<T> assemblySource);
    }

    public interface IAssemblyLanguage
    {}

    public enum AssemblySourceFileType
    {
        Asm,
        Raw
    }

    public class AssemblySourceFile<T>
        where T : IAssemblyLanguage
    {
        public AssemblySourceFile(byte[] bytes, string filename, AssemblySourceFileType type)
        {
            Bytes = bytes;
            Filename = filename;
            Type = type;
        }

        public AssemblySourceFile(string source, string filename)
            : this(Encoding.ASCII.GetBytes(source), filename, AssemblySourceFileType.Asm)
        { }

        public AssemblySourceFile(byte[] bytes, string filename)
            : this(bytes, filename, AssemblySourceFileType.Raw)
        { }

        public byte[] Bytes { get; }
        public string Filename { get; }
        public AssemblySourceFileType Type { get; }
    }

    public interface IAssemblySource
    { }

    public interface IAssemblySource<T> : IAssemblySource
        where T : IAssemblyLanguage
    {
        public IEnumerable<AssemblySourceFile<T>> SourceFiles { get; }
        public IEnumerable<ICFunction> Functions => throw new NotImplementedException();
    }

    public class AssemblySource<T> : IAssemblySource<T>
        where T : IAssemblyLanguage
    {
        public AssemblySource() { }
        public AssemblySource(AssemblySourceFile<T> assemblySourceFile) : this(new List<AssemblySourceFile<T>>() { assemblySourceFile })
        { }
        public AssemblySource(IEnumerable<AssemblySourceFile<T>> assemblySourceFiles)
        {
            SourceFiles = assemblySourceFiles.ToList();
        }

        public IEnumerable<AssemblySourceFile<T>> SourceFiles { get; }
    }

    public interface IShellcodeAssemblySource : IAssemblySource
    {
        // return a variant of ICCxxSource.SourceFiles (for example, change imported WinAPI calls to dymically imported WinAPI calls)
        public new IEnumerable<CCxxSourceFile> SourceFiles { get; }
    }
}
