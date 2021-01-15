using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Reflection.PortableExecutable;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace MyWarez.Plugins.Msvc
{
    public sealed class Linker :
        ILinker<Win32ObjectFile, Executable>,
        ILinker<Win64ObjectFile, Executable>,
        ILinker<Win32ObjectFile, DynamicLinkLibrary>,
        ILinker<Win64ObjectFile, DynamicLinkLibrary>
    {
        public sealed class Config
        {
            public static Config BareBones => new Config(LTCG:Ltcg.OFF, OPTREF:false, OPTICF:0, MANIFESTUAC:false, SAFESEH:null);
            public static Config BareBonesDebug
            {
                get
                {
                    var c = BareBones;
                    c.debug = true;
                    return c;
                }
            }

            public OutputType outputType;
            public Manifest MANIFEST;
            public Ltcg? LTCG;
            public bool NXCOMPAT;
            public bool DYNAMICBASE;
            public bool OPTREF;
            public int? OPTICF;
            public bool INCREMENTAL;
            public bool MANIFESTUAC;
            public ManifestUacLevel MANIFESTUAC_Level;
            public bool MANIFESTUAC_UiAccess;
            public Machine? MACHINE;
            public bool? SAFESEH;
            public bool debug;
            public List<string> ExportedFunctions;
            public List<string> AdditionalDependencies;
            public int? StackSize;
            public int? StackCommitSize;
            public bool MAP;
            public bool NODEFAULTLIB;
            public string ENTRY;
            public Subsystem? SUBSYSTEM;
            public List<string> ORDER;


            public enum Ltcg
            {
                INCREMENTAL, // Specifies that the linker only applies whole program optimization or link-time code generation (LTCG) to files affected by an edit, instead of the entire project.
                OFF, // Disables link-time code generation
                ON, // Enables link-time code generation
            }

            public enum ManifestUacLevel
            {
                asInvoker, // The application runs at the same permission level as the process that started it. You can elevate the application to a higher permission level by selecting Run as Administrator.
                highestAvailable, // The application runs at the highest permission level that it can. If the user who starts the application is a member of the Administrators group, this option is the same as level='requireAdministrator'. If the highest available permission level is higher than the level of the opening process, the system prompts for credentials.
                requireAdministrator, // The application runs using administrator permissions. The user who starts the application must be a member of the Administrators group. If the opening process isn't running with administrative permissions, the system prompts for credentials.

            }

            public enum Manifest
            {
                NOTSPECIFIED,
                EMBED, // Specifies that the linker should embed the manifest file in the image as a resource of type RT_MANIFEST.
                NO, // specifies that the linker should not create a side-by-side manifest file.
                YES // Specifies that the linker should create a side-by-side manifest file.
            }

            public enum Machine
            {
                ARM,
                EBC,
                X64,
                X86
            }

            public enum Subsystem
            {
                BOOT_APPLICATION,
                CONSOLE,
                EFI_APPLICATION,
                EFI_BOOT_SERVICE_DRIVER,
                EFI_ROM,
                EFI_RUNTIME_DRIVER,
                NATIVE,
                POSIX,
                WINDOWS
            }

            public enum OutputType
            {
                EXE,
                DLL,
                DRIVER
            }

            // TODO: most options from https://docs.microsoft.com/en-us/cpp/build/reference/linker-options?view=vs-2019
            public Config(
                List<string> ORDER = null,
                Subsystem? SUBSYSTEM = null,
                string ENTRY = null,
                bool NODEFAULTLIB = false,
                bool MAP = false,
                int? stackSize = null,
                int? stackCommitSize = null,
                List<string> additionalDependencies = null,
                List<string> exportedFunctions = null,
                bool debug = false,
                OutputType outputType = OutputType.EXE,
                Manifest MANIFEST = Manifest.YES, // Creates a side-by-side manifest file and optionally embeds it in the binary.
                Ltcg? LTCG = Ltcg.OFF, // Specifies link-time code generation. NOTE: Deviation from VS default due to forever link times
                bool NXCOMPAT = true, // Marks an executable as verified to be compatible with the Windows Data Execution Prevention feature.
                bool DYNAMICBASE = true, // Specifies whether to generate an executable image that's rebased at load time by using the address space layout randomization (ASLR) feature.
                bool OPTREF = true, // Eliminates functions and data that are never referenced
                int? OPTICF = 1, // perform N iterations of identical COMDAT folding
                bool INCREMENTAL = false, // Controls incremental linking.
                bool MANIFESTUAC = true, // Specifies whether User Account Control (UAC) information is embedded in the program manifest.
                ManifestUacLevel MANIFESTUAC_level = ManifestUacLevel.asInvoker, //
                bool MANIFESTUAC_uiAccess = false, // uiAccess='true' if you want the application to bypass user interface protection levels and drive input to higher-permission windows on the desktop;
                Machine? MACHINE = null, // Specifies the target platform.
                bool? SAFESEH = true // Specifies that the image will contain a table of safe exception handlers.
                )
            {
                this.outputType = outputType;
                this.MANIFEST = MANIFEST;
                this.LTCG = LTCG;
                this.NXCOMPAT = NXCOMPAT;
                this.DYNAMICBASE = DYNAMICBASE;
                this.OPTREF = OPTREF;
                this.OPTICF = OPTICF;
                this.INCREMENTAL = INCREMENTAL;
                this.MANIFESTUAC = MANIFESTUAC;
                this.MANIFESTUAC_Level = MANIFESTUAC_level;
                this.MANIFESTUAC_UiAccess = MANIFESTUAC_uiAccess;
                this.MACHINE = MACHINE;
                this.SAFESEH = SAFESEH;
                this.debug = debug;
                this.ExportedFunctions = exportedFunctions ?? new List<string>();
                this.AdditionalDependencies = additionalDependencies ?? new List<string>();
                this.StackSize = stackSize;
                this.StackCommitSize = stackCommitSize;
                this.MAP = MAP;
                this.NODEFAULTLIB = NODEFAULTLIB;
                this.ENTRY = ENTRY;
                this.SUBSYSTEM = SUBSYSTEM;
                this.ORDER = ORDER ?? new List<string>();
            }

            public override string ToString()
            {
                var args = new List<string>();

                if (ORDER.Count != 0)
                {
                    File.WriteAllText("order.txt", string.Join("\r\n", ORDER));
                    args.Add("/ORDER:@\"order.txt\"");
                }

                if (SUBSYSTEM.HasValue)
                    args.Add("/SUBSYSTEM:" + SUBSYSTEM.Value);
                if (ENTRY != null)
                    args.Add(string.Format("/ENTRY:\"{0}\"", ENTRY));

                foreach (var dependency in AdditionalDependencies)
                    args.Add(dependency);

                if (NODEFAULTLIB)
                    args.Add("/NODEFAULTLIB");
                if (MAP)
                    args.Add("/MAP");

                if (StackSize.HasValue)
                {
                    var arg = "/STACK:" + StackSize.Value;
                    if (StackCommitSize.HasValue)
                        arg += "," + StackCommitSize.Value;
                    args.Add(arg);
                }

                foreach (var exportedFunction in ExportedFunctions)
                    args.Add("/EXPORT:" + exportedFunction);

                if (debug)
                    args.Add("/DEBUG:FULL");

                if (outputType == OutputType.DLL)
                    args.Add("/DLL");

                if (MANIFEST != Manifest.NOTSPECIFIED)
                {
                    var arg = "/MANIFEST";
                    switch (MANIFEST)
                    {
                        case Manifest.EMBED:
                            arg += ":EMBED";
                            break;
                        case Manifest.NO:
                            arg += ":NO";
                            break;
                    }
                    args.Add(arg);
                }

                if (LTCG.HasValue)
                {
                    var arg = "/LTCG";
                    switch (LTCG.Value)
                    {
                        case Ltcg.INCREMENTAL:
                            arg += ":INCREMENTAL";
                            break;
                        case Ltcg.OFF:
                            arg += ":OFF";
                            break;
                    }
                    args.Add(arg);
                }

                if (MACHINE.HasValue)
                    args.Add("/MACHINE:" + MACHINE.Value.ToString());
                if (MACHINE.HasValue && MACHINE.Value == Machine.X86)
                    if (SAFESEH.HasValue)
                        args.Add("/SAFESEH" + (SAFESEH.Value ? "" : ":NO"));

                args.Add("/NXCOMPAT"+(NXCOMPAT ? "" : ":NO"));
                args.Add("/DYNAMICBASE"+(DYNAMICBASE ? "" : ":NO"));
                args.Add("/OPT"+(OPTREF ? ":REF" : ":NOREF"));
                if (OPTICF.HasValue)
                    args.Add("/OPT"+(OPTICF.Value != 0 ? ":ICF="+OPTICF.Value : ":NOICF"));
                args.Add("/INCREMENTAL" + (INCREMENTAL ? "" : ":NO"));

                var uacArg = "/MANIFESTUAC";
                if (MANIFESTUAC == false)
                    uacArg += ":NO";
                else
                    uacArg += string.Format(":\"level='{0}' uiAccess='{1}'\"", MANIFESTUAC_Level, MANIFESTUAC_UiAccess);
                args.Add(uacArg);

                return string.Join(" ", args);
            }
        }

        public readonly Version version;

        public Linker(
            Config config = null,
            Version version = Version.v14_16, // MSVC++ buildtools version. The target host must include at least this version.
            IEnumerable<string> exportedFunctions = null // functions to export
            )
        {
            CConfig = config ?? new Config();
            CConfig.ExportedFunctions = (exportedFunctions ?? Enumerable.Empty<string>()).ToList();
            // why did i add this?
            /*
            if (CConfig.DefaultLibraries.Count() == 0)
            {
                // TODO: Automatically add library references 
                // OR Add all references now ?
                CConfig.DefaultLibraries.Add("advapi32.lib");
                CConfig.DefaultLibraries.Add("ws2_32.lib");
            }
            */
            this.version = version;
        }

        private Config CConfig { get; }


        private U Link<T, U>(IEnumerable<StaticLibrary<T>> staticLibraries, TargetArch targetArch, Func<byte[], U> UConstructor, Config.OutputType outputType)
            where T : ObjectFile, new()
            where U : IPortableExecutable, new()
        {
            switch(targetArch)
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
            var outputExtension = ".exe";
            switch (outputType)
            {
                case Config.OutputType.EXE:
                    CConfig.outputType = Config.OutputType.EXE;
                    outputExtension = ".exe";
                    break;
                case Config.OutputType.DLL:
                    CConfig.outputType = Config.OutputType.DLL;
                    outputExtension = ".dll";
                    break;
                case Config.OutputType.DRIVER:
                    CConfig.outputType = Config.OutputType.DRIVER;
                    outputExtension = ".sys";
                    break;
            }

            using (new TemporaryContext())
            {
                foreach (var staticLibrary in staticLibraries)
                    foreach (var objectFile in staticLibrary.ObjectFiles)
                        File.WriteAllBytes(Core.Utils.RandomString(10)+".obj", objectFile.Bytes);
                var outputfilename = "OUTPUT" + outputExtension;
                var linkerOptions = CConfig.ToString();
                if (linkerOptions.Length > 0)
                    linkerOptions = " " + linkerOptions;
                var linkCommand = "link.exe";
                linkCommand += " /out:" + outputfilename;
                linkCommand += " /NOLOGO";
                linkCommand += linkerOptions;
                linkCommand += " *.obj";
                var msvcVersion = version.ToString().Replace("v", "").Replace("_", ".");
                var linkBatFile = @"call ""C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\VCVARSALL.bat"" {0} -vcvars_ver=" + msvcVersion + "\r\n" + linkCommand;
                linkBatFile = String.Format(linkBatFile, targetArch.ToString());
                File.WriteAllText("link.bat", linkBatFile);
                var proc = Process.Start("cmd.exe", "/c " + "link.bat");
                proc.WaitForExit();
                var pe = UConstructor(File.ReadAllBytes(outputfilename));
                if (CConfig.MAP)
                    pe.Map = File.ReadAllText("OUTPUT.map");
                return pe;
            }
        }

        Executable ILinker<Win32ObjectFile, Executable>.Link(IEnumerable<StaticLibrary<Win32ObjectFile>> objs)
        {
            return Link<Win32ObjectFile, Executable>(objs, TargetArch.x86, x => new Executable(x, CConfig.ExportedFunctions), Config.OutputType.EXE);
        }

        Executable ILinker<Win64ObjectFile, Executable>.Link(IEnumerable<StaticLibrary<Win64ObjectFile>> objs)
        {
            return Link<Win64ObjectFile, Executable>(objs, TargetArch.x64, x => new Executable(x, CConfig.ExportedFunctions), Config.OutputType.EXE);
        }

        DynamicLinkLibrary ILinker<Win64ObjectFile, DynamicLinkLibrary>.Link(IEnumerable<StaticLibrary<Win64ObjectFile>> objs)
        {
            return Link<Win64ObjectFile, DynamicLinkLibrary>(objs, TargetArch.x64, x => new DynamicLinkLibrary(x, CConfig.ExportedFunctions), Config.OutputType.DLL);
        }

        DynamicLinkLibrary ILinker<Win32ObjectFile, DynamicLinkLibrary>.Link(IEnumerable<StaticLibrary<Win32ObjectFile>> objs)
        {
            return Link<Win32ObjectFile, DynamicLinkLibrary>(objs, TargetArch.x86, x => new DynamicLinkLibrary(x, CConfig.ExportedFunctions), Config.OutputType.DLL);
        }
    }
}
