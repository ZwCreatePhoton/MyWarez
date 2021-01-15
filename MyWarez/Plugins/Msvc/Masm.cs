using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace MyWarez.Plugins.Msvc
{
    public interface MASM : IAssemblyLanguage { }

    public sealed class Masm :
        IAssembler<MASM, Win32ObjectFile>,
        IAssembler<MASM, Win64ObjectFile>
    {

        public readonly Version version;

        public Masm(
            Version version = Version.v14_16 // MSVC++ buildtools version. The target host must include at least this version.
            )
        {
            this.version = version;
        }

        private enum TargetArch
        {
            x86,
            x64
        }

        private IEnumerable<T> Assmeble<T>(AssemblySource<MASM> assemblySource, TargetArch targetArch, Func<byte[], T> TConstructor)
            where T : ObjectFile, new()
        {
            List<T> objectFiles = new List<T>();
            using (new TemporaryContext())
            {
                foreach (var sourceFile in assemblySource.SourceFiles)
                    System.IO.File.WriteAllBytes(sourceFile.Filename, sourceFile.Bytes);
                foreach (var sourceFile in assemblySource.SourceFiles.Where(sf => sf.Type == AssemblySourceFileType.Asm))
                {
                    var program = targetArch == TargetArch.x64 ? "ml64.exe" : "ml.exe";
                    var arguments = String.Format("/nologo /Fo object.obj /c /Cx {0}", sourceFile.Filename);
                    var assembleCommand = program + " " + arguments;
                    var msvcVersion = version.ToString().Replace("v", "").Replace("_", ".");
                    var assembleBatFile = @"call ""C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\VCVARSALL.bat"" {0} -vcvars_ver=" + msvcVersion + "\r\n" + assembleCommand;
                    assembleBatFile = String.Format(assembleBatFile, targetArch.ToString());
                    File.WriteAllText("assemble.bat", assembleBatFile);
                    var proc = Process.Start("cmd.exe", "/c " + "assemble.bat");
                    proc.WaitForExit();
                    if (!System.IO.File.Exists("object.obj"))
                        throw new Exception("Masm error. Object file not found.");
                    var objectFileBytes = System.IO.File.ReadAllBytes("object.obj");
                    objectFiles.Add(TConstructor(objectFileBytes));
                }
            }
            return objectFiles;
        }

        Win32ObjectFile IAssembler<MASM, Win32ObjectFile>.Assemble(AssemblySourceFile<MASM> assemblySourceFile)
        {
            return Assmeble<Win32ObjectFile>(new AssemblySource<MASM>(assemblySourceFile), TargetArch.x86, x => new Win32ObjectFile(x)).First();
        }

        StaticLibrary<Win32ObjectFile> IAssembler<MASM, Win32ObjectFile>.Assemble(AssemblySource<MASM> assemblySource)
        {
            return new StaticLibrary<Win32ObjectFile>(Assmeble<Win32ObjectFile>(assemblySource, TargetArch.x86, x => new Win32ObjectFile(x)));
        }

        Win64ObjectFile IAssembler<MASM, Win64ObjectFile>.Assemble(AssemblySourceFile<MASM> assemblySourceFile)
        {
            return Assmeble<Win64ObjectFile>(new AssemblySource<MASM>(assemblySourceFile), TargetArch.x64, x => new Win64ObjectFile(x)).First();
        }

        StaticLibrary<Win64ObjectFile> IAssembler<MASM, Win64ObjectFile>.Assemble(AssemblySource<MASM> assemblySource)
        {
            return new StaticLibrary<Win64ObjectFile>(Assmeble<Win64ObjectFile>(assemblySource, TargetArch.x64, x => new Win64ObjectFile(x)));
        }
    }
}
