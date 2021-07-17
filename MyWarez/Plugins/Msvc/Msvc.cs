using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using MyWarez.Core;
using MyWarez.Base;

namespace MyWarez.Plugins.Msvc
{
    public enum Version
    {
        v14_0,  // _MSC_VER == 1900 (Visual Studio 2015 version 14.0)
        v14_1,  // _MSC_VER == 1910 (Visual Studio 2017 version 15.0)
        v14_11, // _MSC_VER == 1911 (Visual Studio 2017 version 15.3)
        v14_12, // _MSC_VER == 1912 (Visual Studio 2017 version 15.5)
        v14_13, // _MSC_VER == 1913 (Visual Studio 2017 version 15.6)
        v14_14, // _MSC_VER == 1914 (Visual Studio 2017 version 15.7)
        v14_15, // _MSC_VER == 1915 (Visual Studio 2017 version 15.8)
        v14_16, // _MSC_VER == 1916 (Visual Studio 2017 version 15.9)
        v14_2,  // _MSC_VER == 1920 (Visual Studio 2019 Version 16.0)
        v14_21, // _MSC_VER == 1921 (Visual Studio 2019 Version 16.1)
        v14_22, // _MSC_VER == 1922 (Visual Studio 2019 Version 16.2)
        v14_23, // _MSC_VER == 1923 (Visual Studio 2019 Version 16.3)
        v14_24, // _MSC_VER == 1924 (Visual Studio 2019 Version 16.4)
        v14_25, // _MSC_VER == 1925 (Visual Studio 2019 Version 16.5)
        v14_26, // _MSC_VER == 1926 (Visual Studio 2019 Version 16.6)
        v14_27  // _MSC_VER == 1927 (Visual Studio 2019 Version 16.7)
    }

    public enum TargetArch
    {
        x86,
        x64,
        arm,
        arm64
    }

    public static class StaticLibraries
    {
        public static StaticLibrary<Win64ObjectFile> libcmtWin64(Version version = Version.v14_16)
        {
            return Utils.LibFileToStaticLibrary("libcmt.lib", x => new Win64ObjectFile(x), TargetArch.x64, version: version, includeFilter: new[] { "libcmt" });
        }

        public static StaticLibrary<Win32ObjectFile> libcmtWin32(Version version = Version.v14_16)
        {
            return Utils.LibFileToStaticLibrary("libcmt.lib", x => new Win32ObjectFile(x), TargetArch.x86, version: version, includeFilter: new[] { "libcmt" });
        }

        public static StaticLibrary<Win64ObjectFile> libvcruntimeWin64(Version version = Version.v14_16)
        {
            return Utils.LibFileToStaticLibrary("libvcruntime.lib", x => new Win64ObjectFile(x), TargetArch.x64, version: version, includeFilter: null);
        }

        public static StaticLibrary<Win32ObjectFile> libvcruntimeWin32(Version version = Version.v14_16)
        {
            return Utils.LibFileToStaticLibrary("libvcruntime.lib", x => new Win32ObjectFile(x), TargetArch.x86, version: version, includeFilter: null);
        }

        public static StaticLibrary<Win64ObjectFile> libucrtWin64(Version version = Version.v14_16)
        {
            return Utils.LibFileToStaticLibrary("libucrt.lib", x => new Win64ObjectFile(x), TargetArch.x64, version: version, includeFilter: null);
        }

        public static StaticLibrary<Win32ObjectFile> libucrtWin32(Version version = Version.v14_16)
        {
            return Utils.LibFileToStaticLibrary("libucrt.lib", x => new Win32ObjectFile(x), TargetArch.x86, version: version, includeFilter: null);
        }

    }

    public static class Utils
    {
        internal static StaticLibrary<T> LibFileToStaticLibrary<T>(string libFile, Func<byte[], T> TConstructor, TargetArch targetArch, Version version = Version.v14_16, IEnumerable<string> includeFilter = null)
            where T : ObjectFile, new()
        {
            var objectPaths = new List<string>();

            using (new TemporaryContext())
            {
                var command = $"lib.exe /nologo /list {libFile}";
                var msvcVersion = version.ToString().Replace("v", "").Replace("_", ".");
                var listBatFile = @"call ""C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\VCVARSALL.bat"" {0} -vcvars_ver=" + msvcVersion + "\r\n" + command;
                listBatFile = String.Format(listBatFile, targetArch.ToString());
                File.WriteAllText("list.bat", listBatFile);
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/c list.bat";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    if (line.Contains("**")) continue;
                    if (line.ToLower().Contains("vcvars")) continue;
                    if (line != "")
                        objectPaths.Add(line);
                }
                process.WaitForExit();
            }

            objectPaths = objectPaths.Where(x => includeFilter == null || includeFilter.Any(y =>  x.Contains(y))).ToList();

            using (new TemporaryContext())
            {
                var msvcVersion = version.ToString().Replace("v", "").Replace("_", ".");
                var extractBatFile = @"call ""C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\VCVARSALL.bat"" {0} -vcvars_ver=" + msvcVersion + "\r\n";
                extractBatFile = String.Format(extractBatFile, targetArch.ToString());
                File.WriteAllText("extract.bat", extractBatFile);
                foreach (var objectPath in objectPaths)
                {
                    var command = $"lib.exe /nologo {libFile} /EXTRACT:{objectPath}";
                    File.AppendAllText("extract.bat", $"\r\n{command}");
                }
                Process.Start("cmd.exe", "/c extract.bat").WaitForExit();
  

                // Parse the object files for their Symbol names
                string[] objectFilePaths = Directory.GetFiles(".", "*.obj", SearchOption.AllDirectories);
                var symbolsBatFile = @"call ""C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\VCVARSALL.bat"" {0} -vcvars_ver=" + msvcVersion + "\r\n";
                symbolsBatFile = String.Format(symbolsBatFile, targetArch.ToString());
                File.WriteAllText("symbols.bat", symbolsBatFile);
                foreach (var objectFilePath in objectFilePaths)
                {
                    var command = $"dumpbin /symbols {objectFilePath} | findstr External | findstr -v UNDEF > {objectFilePath}.txt";
                    File.AppendAllText("symbols.bat", $"\r\n{command}");
                }
                Process.Start("cmd.exe", "/c symbols.bat").WaitForExit();
                var objectFiles = new List<T>();
                foreach (var objectFilePath in objectFilePaths)
                {
                    var symbols = new List<string>();
                    var objectFile = TConstructor(File.ReadAllBytes(objectFilePath));
                    var lines = File.ReadLines($"{objectFilePath}.txt");
                    foreach (var line in lines)
                    {
                        var line2 = line.Trim();
                        if (line2 == "") continue;
                        var symbol = line.Split("| ")[1];
                        symbols.Add(symbol);
                    }
                    objectFile.ExternalSymbols = symbols;
                    objectFiles.Add(objectFile);
                }

                return new StaticLibrary<T>(objectFiles);
            }
        }

        private static readonly string CompileToShellcodeResourceDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "Msvc", "CompileToShellcode");

        // http://www.exploit-monday.com/2013/08/writing-optimized-windows-shellcode-in-c.html
        private static byte[] CompileToShellcode(TargetArch targetArch, IShellcodeCCxxSourceIParameterlessCFunction ccxxSource, IEnumerable<ObjectFile> additionalShellcodeObjectFiles, bool optimize)
        {
            // __chkstk will be linked if large portion of the stack used. Still POC if we statically link crt


            additionalShellcodeObjectFiles = additionalShellcodeObjectFiles ?? new List<ObjectFile>();

            var source = new CCxxSource(((IShellcodeCCxxSource)ccxxSource).SourceFiles);

            var shellcodeEntryFunction = ccxxSource.Name;

            // Compile
            Compiler.Config compilerConfig;
            if (optimize)
                compilerConfig = new Compiler.Config(
                    GL: true, O1: true, Os: true, // Optimizations that differ
                    O2: false, Og: false, Ot: false, Od: false, Oy: true, Oi: true, // Optimizations that are the same
                    GF:true, Gy:true, GS: false, sdl: null, Zl: false, FA: true, runtime: Compiler.Config.RunTimeLibrary.MT, // Necessary for PIE
                    callingConvention: null, EHa: null, EHs: null, EHc: null, EHr: null // unsure of the effect of these, so leaving as unspecified
                    );
            else
                compilerConfig = new Compiler.Config(
                    GL: false, O1: false, Os: false, // Optimizations that differ
                    O2: false, Og: false, Ot: false, Od: false, Oy: true, Oi: true, // Optimizations that are the same
                    GF: true, Gy: true, GS: false, sdl: null, Zl: false, FA: true, runtime: Compiler.Config.RunTimeLibrary.MT, // Necessary for PIE
                    callingConvention: null, EHa: null, EHs: null, EHc: null, EHr: null // unsure of the effect of these, so leaving as unspecified
                    );

            Linker.Config linkerConfig;
            if (optimize)
                linkerConfig = new Linker.Config(
                    LTCG: Linker.Config.Ltcg.ON, // Optimizations that differ
                    OPTREF: true, OPTICF: 1, // Optimizations that are the same
                    ENTRY: "Begin", SUBSYSTEM: Linker.Config.Subsystem.CONSOLE, MAP: true, SAFESEH: false, NODEFAULTLIB: true, ORDER: new List<string>() { "Begin", shellcodeEntryFunction }, // Necessary for PIE
                    additionalDependencies: new List<string>{ "libucrt.lib", "libvcruntime.lib", "libcmt.lib"}
                    );
            else
                linkerConfig = new Linker.Config(
                    LTCG: Linker.Config.Ltcg.OFF, // Optimizations that differ
                    OPTREF: true, OPTICF: 1, // Optimizations that are the same
                    ENTRY: "Begin", SUBSYSTEM: Linker.Config.Subsystem.CONSOLE, MAP: true, SAFESEH: false, NODEFAULTLIB: true, ORDER: new List<string>() { "Begin", shellcodeEntryFunction }, // Necessary for PIE
                    additionalDependencies: new List<string> { "libucrt.lib", "libvcruntime.lib", "libcmt.lib" }
                    );

            Executable exe;

            switch (targetArch)
            {
                case TargetArch.x64:
                    {
                        var additionalShellcodeObjectFiles64 = new StaticLibrary<Win64ObjectFile>(additionalShellcodeObjectFiles.Cast<Win64ObjectFile>());
                        var staticLibrary = ((ICCxxCompiler<Win64ObjectFile>)new Compiler(compilerConfig)).Compile(source);
                        // Since this is x64, we need to ensure that the stack is aligned before we execute our C/C++'s entry function
                        var alignStackAsm = new AlignRSPMasmAssemblySource(shellcodeEntryFunction);
                        var alignStackSL = ((IAssembler<MASM, Win64ObjectFile>)new Masm()).Assemble(alignStackAsm);
                        var linkerInput = new List<StaticLibrary<Win64ObjectFile>>() { staticLibrary, additionalShellcodeObjectFiles64, alignStackSL };
                        exe = ((ILinker<Win64ObjectFile, Executable>)new Linker(linkerConfig)).Link(linkerInput);
                        break;
                    }
                case TargetArch.x86:
                    {
                        var additionalShellcodeObjectFiles32 = new StaticLibrary<Win32ObjectFile>(additionalShellcodeObjectFiles.Cast<Win32ObjectFile>());
                        source.FindAndReplace("BeginExecutePayload", shellcodeEntryFunction);
                        var staticLibrary = ((ICCxxCompiler<Win32ObjectFile>)new Compiler(compilerConfig)).Compile(source);
                        var linkerInput = new List<StaticLibrary<Win32ObjectFile>>() { staticLibrary, additionalShellcodeObjectFiles32 };
                        exe = ((ILinker<Win32ObjectFile, Executable>)new Linker(linkerConfig)).Link(linkerInput);
                        break;
                    }
                default:
                    throw new ArgumentException("Invalid TargetArch valid");
            }

            // Check if Position Independent Code was generate
            // Sometimes when building for 32-bit, char[] initialzied with an arrary of a certain length gets placed in the data section ...
            // Seems to be when the strlen >= 15  (>= 16 with null byte)
            // Possible solution, split up array into multiple smalle rarrarys?
            if (exe.Map.Split("Publics by Value")[1].Split("entry point")[0].Contains("0002:") || exe.Map.Split("Publics by Value")[1].Split("entry point")[0].Contains("0003:"))
            {
                Console.Error.Write(exe.Map);
                throw new Exception("Failed to generate shellcode: code dependent on data section. See Map file for more info");
            }
            // 'Begin' is the entry point to our shellcode
            if (!exe.Map.Split("Publics by Value")[1].Split("0001:")[1].Split("\n")[0].Contains("Begin"))
            {
                Console.Error.Write(exe.Map);
                throw new Exception("Failed to generate shellcode: function 'Begin' is not the first function in the CODE section. See Map file for more info");
            }

            // Extract shellcoode
            var shellcodeBytes = PeCode.Extract(exe);
            var codeLengthHexString = exe.Map.Split("CODE")[0].Split("H ")[0].Split(" ")[^1];
            var codeLength = int.Parse(codeLengthHexString, System.Globalization.NumberStyles.HexNumber);
            return shellcodeBytes[0..codeLength];
        }

        public static ShellcodeX64 CompileToShellcode(IShellcodeCCxxSourceIParameterlessCFunction ccxxSource, StaticLibrary<Win64ObjectFile> additionalShellcodeObjectFiles, bool optimize = false)
        {
            return new ShellcodeX64(CompileToShellcode(TargetArch.x64, ccxxSource, additionalShellcodeObjectFiles != null ? additionalShellcodeObjectFiles.ObjectFiles : null, optimize));
        }

        // Optimization is opt-in since in some cases optimizations resulted in char[] initiilziations values being placed in the data section when length >= 16
        public static ShellcodeX86 CompileToShellcode(IShellcodeCCxxSourceIParameterlessCFunction ccxxSource, StaticLibrary<Win32ObjectFile> additionalShellcodeObjectFiles, bool optimize = false)
        {
            return new ShellcodeX86(CompileToShellcode(TargetArch.x86, ccxxSource, additionalShellcodeObjectFiles != null ? additionalShellcodeObjectFiles.ObjectFiles : null, optimize));
        }

        //TODO: ARM, ARM64
    }
}
