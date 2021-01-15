using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MyWarez.Plugins.Yasm
{
    public interface YASM : IAssemblyLanguage { }

    public sealed class Yasm :
        IAssembler<YASM, Win32ObjectFile>,
        IAssembler<YASM, Win64ObjectFile>
    {

        private enum Format
        {
            dbg,
            bin,
            dosexe,
            elf,
            elf32,
            elf64,
            coff,
            macho,
            macho32,
            macho64,
            rdf,
            win32,
            win64,
            x64,
            xdf
        }

        private static IEnumerable<T> Assmeble<T>(AssemblySource<YASM> assemblySource, Format format, Func<byte[], T> TConstructor)
            where T : ObjectFile, new()
        {
            List<T> objectFiles = new List<T>();
            using (new TemporaryContext())
            {
                foreach (var sourceFile in assemblySource.SourceFiles)
                    System.IO.File.WriteAllBytes(sourceFile.Filename, sourceFile.Bytes);
                foreach (var sourceFile in assemblySource.SourceFiles.Where(sf => sf.Type == AssemblySourceFileType.Asm))
                {
                    var arguments = String.Format("-f {0} -o object.obj {1}", format.ToString(), sourceFile.Filename);
                    Process.Start("yasm", arguments).WaitForExit();
                    if (!System.IO.File.Exists("object.obj"))
                        throw new Exception("yasm error. Object file not found.");
                    var objectFileBytes = System.IO.File.ReadAllBytes("object.obj");
                    objectFiles.Add(TConstructor(objectFileBytes));
                }
            }
            return objectFiles;
        }

        Win32ObjectFile IAssembler<YASM, Win32ObjectFile>.Assemble(AssemblySourceFile<YASM> assemblySourceFile)
        {
            return Assmeble<Win32ObjectFile>(new AssemblySource<YASM>(assemblySourceFile), Format.win32, x => new Win32ObjectFile(x)).First();
        }

        StaticLibrary<Win32ObjectFile> IAssembler<YASM, Win32ObjectFile>.Assemble(AssemblySource<YASM> assemblySource)
        {
            return new StaticLibrary<Win32ObjectFile>(Assmeble<Win32ObjectFile>(assemblySource, Format.win32, x => new Win32ObjectFile(x)));
        }

        Win64ObjectFile IAssembler<YASM, Win64ObjectFile>.Assemble(AssemblySourceFile<YASM> assemblySourceFile)
        {
            return Assmeble<Win64ObjectFile>(new AssemblySource<YASM>(assemblySourceFile), Format.win64, x => new Win64ObjectFile(x)).First();
        }

        StaticLibrary<Win64ObjectFile> IAssembler<YASM, Win64ObjectFile>.Assemble(AssemblySource<YASM> assemblySource)
        {
            return new StaticLibrary<Win64ObjectFile>(Assmeble<Win64ObjectFile>(assemblySource, Format.win64, x => new Win64ObjectFile(x)));
        }
    }
}
