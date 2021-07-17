using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MyWarez.Plugins.Msvc
{
    public sealed class Compiler :
        ICCxxCompiler<Win32ObjectFile>,
        ICCxxCompiler<Win64ObjectFile>
    {
        public sealed class Config
        {
            public static Config BareBones => new Config(Zc_inline: false, O2: false, Oy: false, Oi: false, Gy: false, GS: false, GL: false, sdl: null, EHs: null, EHc: null);

            public static Config BareBonesDebug
            {
                get
                {
                    var c = BareBones;
                    c.Zi = true;
                    return c;
                }
            }
            public enum Machine
            {
                ARM,
                EBC,
                X64,
                X86
            }

            public enum CallingConvention
            {
                __cdecl, // /Gd
                __fastcall, // /Gr
                __stdcall, // /Gv
                __vectorcall // /Gz
            }

            public enum ObValue
            {
                Ob0,
                Ob1,
                Ob2,
                Ob3
            }

            public enum Favor
            {
                blend,
                ATOM,
                AMD64,
                INTEL64
            }

            public enum RunTimeLibrary
            {
                MD,     // Dynamically link against the CRT: MSVCR<versionnumber>.DLL
                MDd,    // Dynamically link against the debug CRT
                MT,     // Statically link against the CRT
                MTd     // Statically link against the debug CRT
            }

            public Machine? MACHINE = null;
            public CallingConvention? callingConvention;
            public bool unicode;
            public bool permissive;
            public bool? Zc_wchar_t;
            public bool? Zc_inline;
            public bool? Zc_forScope;
            public bool O1;
            public bool O2;
            public bool Og;
            public bool Os;
            public bool? Oy;
            public ObValue? Ob;
            public bool? Oi;
            public bool Ot;
            public bool Od;
            public bool Ox;
            public Favor? favor;
            public bool GF;
            public bool? Gy;
            public bool? GS;
            public int? Gs;
            public bool? GL;
            public bool? sdl;
            public bool? EHa;
            public bool? EHs;
            public bool? EHc;
            public bool? EHr;
            public bool Zi;
            public RunTimeLibrary? runtime;
            public bool FA;
            public bool Zl;

            // TODO: Most of the other options ... https://docs.microsoft.com/en-us/cpp/build/reference/compiler-options-listed-by-category?view=vs-2019
            // The default values are the default settings from Visual Studio when building as Release
            public Config(
                bool Zl = false, // Removes default library name from .obj file (x86 only).
                bool FA = true, // Creates a listing file. (approximately a MASM file)
                RunTimeLibrary? runtime = RunTimeLibrary.MD, // Statically/Dynamically link against the release/debug version of the C RunTime Library
                bool Zi = false, // Generates complete debugging information.
                CallingConvention? callingConvention = CallingConvention.__cdecl,
                bool unicode = true, // define UNICODE and _UNICODE
                bool permissive = false, // Set standard-conformance mode. NOTE: This default is a deviation from the default for VS 2017 15.5+ since it prevents builds of some programs.
                bool? Zc_wchar_t = true, // wchar_t is a native type, not a typedef
                bool? Zc_inline = true, // Remove unreferenced function or data if it is COMDAT or has internal linkage only (off by default).
                bool? Zc_forScope = true, // Enforce Standard C++ for scoping rules (on by default).
                bool O1 = false, // Creates small code.
                bool O2 = true, // Creates fast code.
                bool Og = false, // Controls inline expansion.
                bool Os = false, // Favors small code.
                bool? Oy = true, // Omits frame pointer. (x86 only)
                ObValue? Ob = null, // Controls inline expansion.
                bool? Oi = true, // Generates intrinsic functions.
                bool Ot = false, // Favors fast code.
                bool Od = false, // Disables optimization.
                bool Ox = false, // A subset of /O2 that doesn't include /GF or /Gy.
                Favor? favor = null, // Produces code that is optimized for a specified architecture, or for a range of architectures.
                bool GF = false, // Enables string pooling. 
                bool? Gy = true, // Enables function-level linking.
                bool? GS = true, // Checks buffer security.
                int? Gs = 0, // Checks the threshold for stack probes.
                bool? GL = true, // Enables whole program optimization.
                bool? sdl = true, // Enables additional security features and warnings.
                bool? EHa = null, // Enables standard C++ stack unwinding. Catches both structured (asynchronous) and standard C++ (synchronous) exceptions when you use catch(...) syntax. /EHa overrides both /EHs and /EHc arguments.
                bool? EHs = true, // Enables standard C++ stack unwinding. Catches only standard C++ exceptions when you use catch(...) syntax. Unless /EHc is also specified, the compiler assumes that functions declared as extern "C" may throw a C++ exception.
                bool? EHc = true, // When used with /EHs, the compiler assumes that functions declared as extern "C" never throw a C++ exception. It has no effect when used with /EHa (that is, /EHca is equivalent to /EHa). /EHc is ignored if /EHs or /EHa aren't specified.
                bool? EHr = null // Tells the compiler to always generate runtime termination checks for all noexcept functions. By default, runtime checks for noexcept may be optimized away if the compiler determines the function calls only non-throwing functions. This option gives strict C++ conformance at the cost of some extra code. /EHr is ignored if /EHs or /EHa aren't specified.
                )
            {
                if (O1 && O2)
                    throw new ArgumentException("Can't specify both O1 and O2");
                this.callingConvention = callingConvention;
                this.unicode = unicode;
                this.permissive = permissive;
                this.Zc_wchar_t = Zc_wchar_t;
                this.Zc_inline = Zc_inline;
                this.Zc_forScope = Zc_forScope;
                this.O1 = O1;
                this.O2 = O2;
                this.Og = Og;
                this.Os = Os;
                this.Oy = Oy;
                this.Ob = Ob;
                this.Oi = Oi;
                this.Ot = Ot;
                this.Od = Od;
                this.Ox = Ox;
                this.favor = favor;
                this.GF = GF;
                this.Gy = Gy;
                this.GS = GS;
                this.Gs = Gs;
                this.GL = GL;
                this.sdl = sdl;
                this.EHa = EHa;
                this.EHs = EHs;
                this.EHc = EHc;
                this.EHr = EHr;
                this.Zi = Zi;
                this.runtime = runtime;
                this.FA = FA;
                this.Zl = Zl;
            }

            public override string ToString()
            {
                var args = new List<string>();

                if (Zl) args.Add("/Zl");
                if (FA) args.Add("/FA");

                if (runtime.HasValue)
                    args.Add("/"+runtime.ToString());

                switch (callingConvention)
                {
                    case CallingConvention.__cdecl:
                        args.Add("/Gd");
                        break;
                    case CallingConvention.__fastcall:
                        args.Add("/Gr");
                        break;
                    case CallingConvention.__stdcall:
                        args.Add("/Gv");
                        break;
                    case CallingConvention.__vectorcall:
                        args.Add("/Gz");
                        break;
                }
                if (Zi)
                    args.Add("/Zi");
                if (unicode)
                    args.Add("/D \"_UNICODE\" /D \"UNICODE\"");
                if (permissive)
                    args.Add("/permissive-");
                if (Zc_wchar_t.HasValue)
                    args.Add("/Zc:wchar_t" + (Zc_wchar_t.Value ? "" : "-"));
                if (Zc_inline.HasValue)
                    args.Add("/Zc:inline" + (Zc_inline.Value ? "" : "-"));
                if (Zc_forScope.HasValue)
                    args.Add("/Zc:forScope" + (Zc_forScope.Value ? "" : "-"));
                if (EHa.HasValue || EHc.HasValue || EHs.HasValue || EHr.HasValue)
                {
                    if ((EHa.HasValue && EHa.Value) || (EHs.HasValue && EHs.Value) || (EHc.HasValue && EHc.Value) || (EHr.HasValue && EHr.Value))
                    {
                        var arg = "/EH";
                        if (EHa.HasValue) arg += EHa.Value ? "a" : "";
                        if (EHs.HasValue) arg += EHs.Value ? "s" : "";
                        if (EHc.HasValue) arg += EHc.Value ? "c" : "";
                        if (EHr.HasValue) arg += EHr.Value ? "r" : "";
                        args.Add(arg);
                    }
                    if (EHa.HasValue && !EHa.Value) args.Add("/EHa-");
                    if (EHs.HasValue && !EHs.Value) args.Add("/EHs-");
                    if (EHc.HasValue && !EHc.Value) args.Add("/EHc-");
                    if (EHr.HasValue && !EHr.Value) args.Add("/EHr-");
                }

                // Optimizations
                if (O1) args.Add("/O1");
                if (O2) args.Add("/O2");
                if (Og) args.Add("/Og");
                if (Os) args.Add("/Os");
                if (Oy.HasValue)
                    args.Add("/Oy" + (Oy.Value ? "" : "-"));
                if (Ob.HasValue)
                    args.Add("/" + Ob.ToString());
                if (Oi.HasValue)
                    args.Add("/Oi" + (Oi.Value ? "" : "-"));
                if (Ot) args.Add("/Ot");
                if (Od) args.Add("/Od");
                if (Ox) args.Add("/Ox");
                if (favor.HasValue)
                    args.Add("/favor:" + favor.ToString());

                // Code generation
                // https://developercommunity.visualstudio.com/content/problem/309307/d9025-overriding-sdl-with-gs.html
                // msvc bug: combining /GS- /sdl- results in security cookies (GS) ...
                if (sdl.HasValue && !(GS.HasValue && GS.Value == false))
                    args.Add("/sdl" + (sdl.Value ? "" : "-")); // SDL should be before GS
                if (GS.HasValue)
                    args.Add("/GS" + (GS.Value ? "" : "-"));
                if (Gs.HasValue && !(Gs.HasValue && Gs.Value == 0))
                    args.Add("/Gs" + Gs.Value.ToString());
                if (Gy.HasValue)
                    args.Add("/Gy" + (Gy.Value ? "" : "-"));
                if (GL.HasValue)
                    args.Add("/GL" + (GL.Value ? "" : "-"));
                if (GF) args.Add("/GF");


                return string.Join(" ", args);
            }
        }

        public readonly Version version;

        public Compiler(Version version = Version.v14_16) : this(new Config()) { }

        public Compiler(
            Config config,
            Version version = Version.v14_16 // MSVC++ buildtools version. The target host must include at least this version.
            )
        {
            CConfig = config;
            this.version = version;
        }

        private Config CConfig { get; } 



        // TODO: Support compiling source files recursively
        private StaticLibrary<T> Compile<T>(ICCxxSource source, TargetArch targetArch, Func<byte[],T> TConstructor)
            where T : ObjectFile, new()
        {
            switch (targetArch)
            {
                case TargetArch.x86:
                    CConfig.MACHINE = Config.Machine.X86;
                    break;
                case TargetArch.x64:
                    CConfig.MACHINE = Config.Machine.X64;
                    break;
                case TargetArch.arm:
                case TargetArch.arm64:
                    CConfig.MACHINE = Config.Machine.ARM;
                    break;
            }

            using (new TemporaryContext())
            {
                var exts = new HashSet<string>();
                foreach (var sourceFile in source.SourceFiles)
                {
                    File.WriteAllText(sourceFile.Filename, sourceFile.Source);
                    if (sourceFile.Type == CCxxSourceFileType.C || sourceFile.Type == CCxxSourceFileType.Cxx)
                        exts.Add(Path.GetExtension(sourceFile.Filename));
                }
                var sourceFilenames = exts.Select(ext => "*"+ext);

                var compilerOptions = CConfig.ToString();
                var compileCommand = $"cl.exe /c /nologo /diagnostics:column {compilerOptions} {string.Join(" ", sourceFilenames)}";
                var msvcVersion = version.ToString().Replace("v", "").Replace("_", ".");
                var compileBatFile = @"call ""C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\VCVARSALL.bat"" {0} -vcvars_ver=" + msvcVersion + "\r\n" + compileCommand;
                compileBatFile = String.Format(compileBatFile, targetArch.ToString());
                File.WriteAllText("compile.bat", compileBatFile);
                var proc = Process.Start("cmd.exe", "/c " + "compile.bat");
                proc.WaitForExit();

                string[] objectFilePaths = Directory.GetFiles(".", "*.obj", SearchOption.AllDirectories);
                var objectFiles = objectFilePaths.ToList().Select((file) => TConstructor(File.ReadAllBytes(file))).ToList();
                return new StaticLibrary<T>(objectFiles);
            }
        }

        StaticLibrary<Win32ObjectFile> ICCxxCompiler<Win32ObjectFile>.Compile(ICCxxSource source)
        {
            return Compile<Win32ObjectFile>(source, TargetArch.x86, x => new Win32ObjectFile(x));
        }

        StaticLibrary<Win64ObjectFile> ICCxxCompiler<Win64ObjectFile>.Compile(ICCxxSource source)
        {
            return Compile<Win64ObjectFile>(source, TargetArch.x64, x => new Win64ObjectFile(x));
        }
    }
}
