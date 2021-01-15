using Microsoft.CodeAnalysis.CSharp.Syntax;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWarez.Core
{
    // T: the input object file type
    // U: the output executable file type
    public interface ILinker<T, U>
        where T : ObjectFile, new()
    {
        public U Link(StaticLibrary<T> objs) => Link(new List<StaticLibrary<T>>() { objs });
        public U Link(IEnumerable<StaticLibrary<T>> objs);
    }

/*    public interface INameManglingScheme
    {
        public string MangleSymbol(string functionName, string returnType, IEnumerable<string> parameterTypes) => throw new NotImplementedException();
    }

    public interface INoNameManglingScheme : INameManglingScheme
    {
        public new string MangleSymbol(string functionName, string returnType, IEnumerable<string> parameterTypes) => functionName;
    }

    public interface ICallingConvention { }

    public interface ICdeclCallingConvention : ICallingConvention { }

    public interface IAbi { }

    public interface IAbi<T, U> : IAbi
        where T : ICallingConvention
        where U : INameManglingScheme
    {}

    public interface ICAbi : IAbi<ICdeclCallingConvention, INoNameManglingScheme>
    {}

    public interface IObjectFile { }

    public interface IObjectFile<T> : IObjectFile
        where T : IAbi
    {
        public IEnumerable<string> Symbols => throw new NotImplementedException();
    }
*/

/*    public abstract class ObjectFile<T> : IObjectFile<T>
        where T : IAbi*/
    public abstract class ObjectFile
    {
        public ObjectFile() { }

        public ObjectFile(byte[] bytes)
        {
            Bytes = bytes;
        }

        public byte[] Bytes { get; }

        public List<string> ExternalSymbols = new List<string>();
    }

    public sealed class Win32ObjectFile : ObjectFile
    {
        public Win32ObjectFile() : base() { }

        public Win32ObjectFile(byte[] bytes) : base(bytes) { }
    }

    public sealed class Win64ObjectFile : ObjectFile
    {
        public Win64ObjectFile() : base() { }

        public Win64ObjectFile(byte[] bytes) : base(bytes) { }
    }



    // Logical collection of object files and/or static libraries
    public class StaticLibrary<T> : IPayload
        where T : ObjectFile, new()
    {
        public StaticLibrary()
        {
            ObjectFiles = new List<T>();
        }

        public StaticLibrary(T objectFile) : this(new List<T>() { objectFile }) { }

        public StaticLibrary(IEnumerable<T> objectFiles)
        {
            ObjectFiles = objectFiles;
        }

        public StaticLibrary(IEnumerable<StaticLibrary<T>> staticLibraries)
        {
            List<T> objectFiles = new List<T>();
            foreach (var staticLibrary in staticLibraries)
                objectFiles.AddRange(staticLibrary.ObjectFiles);
            ObjectFiles = objectFiles;
        }

        public IEnumerable<T> ObjectFiles { get; }

        public PayloadType Type { get; } = PayloadType.StaticLibrary;
    }
}
