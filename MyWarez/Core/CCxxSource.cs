using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyWarez.Core
{
    public interface ICCxxCompiler<T>
        where T : ObjectFile, new()
    {
        public StaticLibrary<T> Compile(ICCxxSource source);
    }

    // TODO: add symbol renaming so that function names can be changed/randomized to avoid symbol collisions
    // TODO: Find way to specify the compatiable compiler(s) that can be used with a particular object that implements ICCxxSource
    public interface ICCxxSource
    {
        public IEnumerable<CCxxSourceFile> SourceFiles { get; }
        public IEnumerable<ICFunction> Functions { get; } // TODO: Find a way to proramentatically extract function names 
        public string EntryPointSymbolName { get; }
    }

    public interface ICCxxSourceICFunction : ICCxxSource, ICFunction { }
    public interface ICCxxSourceIParameterlessCFunction : ICCxxSourceICFunction, IParameterlessCFunction { }


    public interface ICFunction
    {
        public string Name { get; }
        public string Signature
        {
            get
            {
                var signature = ReturnType.ToString() + " " + Name + "(";
                char parameterName = 'a';
                if (ParameterTypes != null)
                {
                    foreach (var parmeterType in ParameterTypes)
                        signature += parmeterType.ToString() + " " + (parameterName++) + ",";
                    if (signature.EndsWith(","))
                        signature = signature.Substring(0, signature.Length - 1);
                }
                else
                {
                    signature += "void";
                }
                signature += ");";
                return signature;
            }
        }
        public string ReturnType => "void";
        public IEnumerable<string> ParameterTypes { get; }// => new List<string>() { };

    }

    public interface IParametricCFunction : ICFunction {}
    public interface IParameterlessCFunction : ICFunction
    {
        public new IEnumerable<string> ParameterTypes => null;
    }

    public interface IShellcodeCFunction : ICFunction, IShellcodeable
    {}

    public interface IShellcodeParameterlessCFunction : IShellcodeCFunction, IParameterlessCFunction, IShellcodeable
    { }

    public enum CCxxSourceFileType
    {
        C,
        Cxx,
        H,
        Hxx
    }

    public class CCxxSourceFile
    {
        public CCxxSourceFile(string source, string filename, CCxxSourceFileType type)
        {
            this.Source = source;
            this.Filename = filename;
            this.Type = type;
        }

        public CCxxSourceFile(string source, string filename) : this(source, filename, GuessType(filename)) { }

        private static CCxxSourceFileType GuessType(string filename)
        {
            switch (Path.GetExtension(filename).ToLower())
            {
                case ".cpp":
                case ".cxx":
                    return CCxxSourceFileType.Cxx;
                case ".h":
                    return CCxxSourceFileType.H;
                case ".hpp":
                    return CCxxSourceFileType.Hxx;
                case ".c":
                default:
                    return CCxxSourceFileType.C;
            }
        }

        public string Source { get; set; }
        public string Filename { get; set; }
        public CCxxSourceFileType Type { get; set; }
    }

    public class CCxxSource : IPayload, ICCxxSource
    {
        public CCxxSource() { }
        public CCxxSource(ICCxxSource source) : this(source.SourceFiles)
        {
            EntryPointSymbolName = source.EntryPointSymbolName;
        }

        public CCxxSource(CCxxSourceFile sourceFile, string entryPointSymbolName = null) : this(new List<CCxxSourceFile>() { sourceFile }, entryPointSymbolName) { }

        public CCxxSource(IEnumerable<CCxxSourceFile> sourceFiles, string entryPointSymbolName = null)
        {
            SourceFiles = sourceFiles.ToList();
            EntryPointSymbolName = entryPointSymbolName;
        }

        public string EntryPointSymbolName { get; }

        public IEnumerable<CCxxSourceFile> SourceFiles { get; }

        private static IEnumerable<string> SupportFileExtensions => new List<string>() { ".c", ".cpp", ".h", ".hpp" };

        // TODO: merge duplicates source files?
        public static IEnumerable<CCxxSourceFile> SourceDirectoryToSourceFiles(string sourceDirectory, IEnumerable<string> excludeFiles = null, IEnumerable<ICCxxSource> additionalSources = null, bool randomNameSuffix = true)
        {
            excludeFiles ??= new List<string>();
            additionalSources ??= new List<ICCxxSource>();
            List<CCxxSourceFile> files = new List<CCxxSourceFile>();
            foreach (var fileExtension in SupportFileExtensions)
                files.AddRange(Directory.GetFiles(sourceDirectory, "*" + fileExtension, SearchOption.AllDirectories).ToList().Where(file => !excludeFiles.Contains(Path.GetRelativePath(sourceDirectory, file))).Select((file) => new CCxxSourceFile(File.ReadAllText(file), Path.GetRelativePath(sourceDirectory, file))));
            if (randomNameSuffix)
                foreach (var f in files)
                {
                    if (f.Type == CCxxSourceFileType.C || f.Type == CCxxSourceFileType.Cxx)
                    {
                        var ext = "." + f.Filename.Split(".")[^1];
                        f.Filename = f.Filename.Replace(ext, Utils.RandomString(5) + ext);
                    }
                }
            
            foreach (var source in additionalSources)
                files.AddRange(source.SourceFiles);
            return files;
        }

        public static IEnumerable<CCxxSourceFile> MergeSourceFiles(ICCxxSource source, IEnumerable<ICCxxSource> additionalSources = null)
        {
            additionalSources ??= new List<ICCxxSource>();
            List<CCxxSourceFile> files = new List<CCxxSourceFile>();
            files.AddRange(source.SourceFiles);
            foreach (var s in additionalSources)
                files.AddRange(s.SourceFiles);
            return files;
        }

        public static void FindAndReplace(IEnumerable<CCxxSourceFile> sourceFiles, string oldString, string newString)
        {
            foreach (var sourceFile in sourceFiles)
                sourceFile.Source = sourceFile.Source.Replace(oldString, newString);
        }

        public void FindAndReplace(string oldString, string newString)
        {
            foreach (var sourceFile in SourceFiles)
                sourceFile.Source = sourceFile.Source.Replace(oldString, newString);
        }

        PayloadType IPayload.Type => PayloadType.CCxxSource;

        public IEnumerable<ICFunction> Functions => throw new NotImplementedException();
        public override int GetHashCode()
        {
            unchecked
            {
                if (!CachedHashCode.HasValue)
                    CachedHashCode = base.GetHashCode();
                return Math.Abs(CachedHashCode.Value);
            }
        }
        private int? CachedHashCode = null;
    }
}
