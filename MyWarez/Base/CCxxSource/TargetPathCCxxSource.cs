using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyWarez.Base
{
    // Contracts: Return parts of a filepath, in both ascii and wide variants

    public interface IGetTargetDirectoryWithoutSlashA : ICCxxSourceICFunction
    {
        public new string Name => "GetTargetDirectoryWithoutSlashA";
        public new string ReturnType => "char*";
        public new IEnumerable<string> ParameterTypes => new[] { "char* const" };
    }
    public interface IGetTargetDirectoryA : ICCxxSourceICFunction
    {
        public new string Name => "GetTargetDirectoryA";
        public new string ReturnType => "char*";
        public new IEnumerable<string> ParameterTypes => new[] { "char* const" };

    }
    public interface IGetTargetFilenameA : ICCxxSourceICFunction
    {
        public new string Name => "GetTargetFilenameA";
        public new string ReturnType => "char*";
        public new IEnumerable<string> ParameterTypes => new[] { "char* const" };

    }
    public interface IGetTargetPathA : ICCxxSourceICFunction
    {
        public new string Name => "GetTargetPathA";
        public new string ReturnType => "char*";
        public new IEnumerable<string> ParameterTypes => new[] { "char* const" };

    }
    public interface IGetTargetDirectoryWithoutSlashW : ICCxxSourceICFunction
    {
        public new string Name => "GetTargetDirectoryWithoutSlashW";
        public new string ReturnType => "wchar_t*";
        public new IEnumerable<string> ParameterTypes => new[] { "wchar_t* const" };

    }
    public interface IGetTargetDirectoryW : ICCxxSourceICFunction
    {
        public new string Name => "GetTargetDirectoryW";
        public new string ReturnType => "wchar_t*";
        public new IEnumerable<string> ParameterTypes => new[] { "wchar_t* const" };

    }
    public interface IGetTargetFilenameW : ICCxxSourceICFunction
    {
        public new string Name => "GetTargetFilenameW";
        public new string ReturnType => "wchar_t*";
        public new IEnumerable<string> ParameterTypes => new[] { "wchar_t* const" };

    }
    public interface IGetTargetPathW : ICCxxSourceICFunction
    {
        public new string Name => "GetTargetPathW";
        public new string ReturnType => "wchar_t*";
        public new IEnumerable<string> ParameterTypes => new[] { "wchar_t* const" };

    }
    public interface ITargetPathW : IGetTargetDirectoryWithoutSlashW, IGetTargetDirectoryW, IGetTargetFilenameW, IGetTargetPathW
    {
        string ICFunction.Name => throw new NotImplementedException();
    }
    public interface ITargetPathA : IGetTargetDirectoryWithoutSlashA, IGetTargetDirectoryA, IGetTargetFilenameA, IGetTargetPathA
    {
        string ICFunction.Name => throw new NotImplementedException();
    }
    public interface ITargetPath : ITargetPathA, ITargetPathW
    {
        string ICFunction.Name => throw new NotImplementedException();
    }

    public class StaticTargetPathCCxxSource : ShellcodeCCxxSource, ITargetPath, IShellcodeCCxxSourceICFunction, IShellcodeCFunction
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "CCxxSource", nameof(StaticTargetPathCCxxSource));
        private static readonly IEnumerable<string> ExcludeFiles = new List<string>() { "TargetPath.cpp" };
        private static readonly string DirectoryWPlaceholder = Utils.StringToCArrary(@"C:\Users\Public\", wide: true);
        private static readonly string DirectoryWithoutSlashWPlaceholder = Utils.StringToCArrary(@"C:\Users\Public", wide: true);
        private static readonly string FilenameWPlaceholder = Utils.StringToCArrary(@"somefile.txt", wide: true);
        private static readonly string PathWPlaceholder = Utils.StringToCArrary(@"C:\Users\Public\somefile.txt", wide: true);
        private static readonly string DirectoryAPlaceholder = Utils.StringToCArrary(@"C:\Users\Public\", wide: false);
        private static readonly string DirectoryWithoutSlashAPlaceholder = Utils.StringToCArrary(@"C:\Users\Public", wide: false);
        private static readonly string FilenameAPlaceholder = Utils.StringToCArrary(@"somefile.txt", wide: false);
        private static readonly string PathAPlaceholder = Utils.StringToCArrary(@"C:\Users\Public\somefile.txt", wide: false);

        public StaticTargetPathCCxxSource(string targetFilepath)
            : base(SourceDirectoryToSourceFiles(ResourceDirectory, excludeFiles: ExcludeFiles))
        {
            string path = targetFilepath;
            string directorywithoutslash = Path.GetDirectoryName(path);
            string directory = directorywithoutslash + @"\";
            string filename = Path.GetFileName(path);
            FindAndReplace(SourceFiles, DirectoryWPlaceholder, Utils.StringToCArrary(directory, wide: true));
            FindAndReplace(SourceFiles, DirectoryWithoutSlashWPlaceholder, Utils.StringToCArrary(directorywithoutslash, wide: true));
            FindAndReplace(SourceFiles, FilenameWPlaceholder, Utils.StringToCArrary(filename, wide: true));
            FindAndReplace(SourceFiles, PathWPlaceholder, Utils.StringToCArrary(path, wide: true));
            FindAndReplace(SourceFiles, DirectoryAPlaceholder, Utils.StringToCArrary(directory, wide: false));
            FindAndReplace(SourceFiles, DirectoryWithoutSlashAPlaceholder, Utils.StringToCArrary(directorywithoutslash, wide: false));
            FindAndReplace(SourceFiles, FilenameAPlaceholder, Utils.StringToCArrary(filename, wide: false));
            FindAndReplace(SourceFiles, PathAPlaceholder, Utils.StringToCArrary(path, wide: false));

            FindAndReplace(SourceFiles, "GetTargetPathW(", ((IGetTargetPathW)this).Name + "(");
            FindAndReplace(SourceFiles, "GetTargetDirectoryW(", ((IGetTargetDirectoryW)this).Name + "(");
            FindAndReplace(SourceFiles, "GetTargetDirectoryWithoutSlashW(", ((IGetTargetDirectoryWithoutSlashW)this).Name + "(");
            FindAndReplace(SourceFiles, "GetTargetFilenameW(", ((IGetTargetFilenameW)this).Name + "(");
            FindAndReplace(SourceFiles, "GetTargetPathA(", ((IGetTargetPathA)this).Name + "(");
            FindAndReplace(SourceFiles, "GetTargetDirectoryA(", ((IGetTargetDirectoryA)this).Name + "(");
            FindAndReplace(SourceFiles, "GetTargetDirectoryWithoutSlashA(", ((IGetTargetDirectoryWithoutSlashA)this).Name + "(");
            FindAndReplace(SourceFiles, "GetTargetFilenameA(", ((IGetTargetFilenameA)this).Name+"(");
        }

        public IEnumerable<string> ParameterTypes => null;


        string IGetTargetPathW.Name => "GetTargetPathW" + GetHashCode();
        string IGetTargetDirectoryW.Name => "GetTargetDirectoryW" + GetHashCode();
        string IGetTargetDirectoryWithoutSlashW.Name => "GetTargetDirectoryWithoutSlashW" + GetHashCode();
        string IGetTargetFilenameW.Name => "GetTargetFilenameW" + GetHashCode();
        string IGetTargetPathA.Name => "GetTargetPathA" + GetHashCode();
        string IGetTargetDirectoryA.Name => "GetTargetDirectoryA" + GetHashCode();
        string IGetTargetDirectoryWithoutSlashA.Name => "GetTargetDirectoryWithoutSlashA" + GetHashCode();
        string IGetTargetFilenameA.Name => "GetTargetFilenameA" + GetHashCode();
    }
}