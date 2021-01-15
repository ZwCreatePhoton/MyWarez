using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;

namespace MyWarez.Base
{
    // Contract: Return data (typically representative of a file)
    public interface IGetFileResource : ICCxxSourceICFunction
    {
        public new string Name => "GetFileResource";
        public new string ReturnType => "struct FileResource*";
        public new IEnumerable<string> ParameterTypes => new[] { "struct FileResource* const" };
    }


    // Is it possible to write to the destination pointer without reserving ~N bytes using C? 
    public class StaticFileResourceCCxxSource : ShellcodeCCxxSource, IGetFileResource, IShellcodeCCxxSourceICFunction, IShellcodeCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "CCxxSource", nameof(StaticFileResourceCCxxSource));
        private static readonly string DataPlaceholder = Utils.BytesToCArray(new byte[] { 1, 3, 3, 7, 1, 3, 3, 7, 1, 3, 3, 7, 1, 3, 3, 7 });
        private static readonly string SizePlaceholder = 16.ToString();
        private static readonly string FunctionNamePlaceholder = "GetFileResource";

        public StaticFileResourceCCxxSource(byte[] bytes) : base(SourceDirectoryToSourceFiles(ResourceDirectory))
        {
            if (bytes.Length > 99999)
                Console.Error.WriteLine(nameof(StaticFileResourceCCxxSource) + ": data may be too large. Ensure proper stack memory space will be available at runtime");
            string data = Utils.BytesToCArray(bytes);
            string size = bytes.Length.ToString();
            FindAndReplace(SourceFiles, SizePlaceholder, size);
            FindAndReplace(SourceFiles, DataPlaceholder, data);
            Size = bytes.Length;
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        public int Size { get; }

        string ICFunction.Name => (((IGetFileResource)this).Name) + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

    }
}