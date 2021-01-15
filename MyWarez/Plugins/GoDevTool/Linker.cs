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
using System.Reflection;
using System.Security.Cryptography;

namespace MyWarez.Plugins.GoDevTool
{
    public sealed class Linker :
        ILinker<Win32ObjectFile, Executable>,
        ILinker<Win64ObjectFile, Executable>,
        ILinker<Win32ObjectFile, DynamicLinkLibrary>,
        ILinker<Win64ObjectFile, DynamicLinkLibrary>
    {
        private static readonly string GoLinkResourcePath = Path.Join(Core.Constants.ResourceDirectory, "Plugins", nameof(GoDevTool), "GoLink.exe");


        // TODO: the rest of the options at http://www.godevtool.com/GolinkFrame.htm
        public sealed class Config
        {
            public OutputType outputType;
            public bool NXCOMPAT;
            public bool DYNAMICBASE;
            public bool debug;
            public List<string> ExportedFunctions;
            public List<string> ImportLibraries;
            public string StackSize;
            public string ENTRY;
            public Subsystem? SUBSYSTEM;

            public enum Subsystem
            {
                CONSOLE,
                WINDOWS
            }

            public enum OutputType
            {
                EXE,
                DLL,
                DRIVER
            }

            public Config(
                Subsystem SUBSYSTEM = Subsystem.WINDOWS,
                string ENTRY = null,
                string stackSize = null,
                List<string> importLibraries = null,
                List<string> exportedFunctions = null,
                bool debug = false,
                OutputType outputType = OutputType.EXE,
                bool NXCOMPAT = true, // Marks an executable as verified to be compatible with the Windows Data Execution Prevention feature.
                bool DYNAMICBASE = true // Specifies whether to generate an executable image that's rebased at load time by using the address space layout randomization (ASLR) feature.
                )
            {
                this.outputType = outputType;
                this.NXCOMPAT = NXCOMPAT;
                this.DYNAMICBASE = DYNAMICBASE;
                this.debug = debug;
                this.ExportedFunctions = exportedFunctions ?? new List<string>();
                this.ImportLibraries = importLibraries ?? new List<string>();
                this.StackSize = stackSize;
                this.ENTRY = ENTRY;
                this.SUBSYSTEM = SUBSYSTEM;
            }

            public override string ToString()
            {
                var args = new List<string>();

                if (SUBSYSTEM == Subsystem.CONSOLE)
                    args.Add("/console");

                if (ENTRY != null)
                    args.Add("/entry " + ENTRY);

                if (StackSize != null)
                    args.Add("/stacksize " + StackSize);

                foreach (var exportedFunction in ExportedFunctions)
                if (ExportedFunctions.Count > 0)
                    args.Add("/export " + string.Join(", ", exportedFunction.ToArray()));

                if (debug)
                    args.Add("/debug coff");

                if (outputType == OutputType.DLL)
                    args.Add("/dll");
                else if (outputType == OutputType.DRIVER)
                    args.Add("/driver");

                if (NXCOMPAT)
                    args.Add("/nxcompat");
                if (DYNAMICBASE)
                    args.Add("/dynamicbase");

                foreach (var importLib in ImportLibraries)
                    args.Add(" " + importLib);

                return string.Join(" ", args);
            }
        }

        /*
         * GoLink does not work with linking against MSVC's C/C++ entry points (mainCRTStartup, etc) from libcmt.lib
         * When you try to link against mainCRTStartup, you'll need the implementation of _is_c_termination_complete
         * But GoLink views the object file that contains _is_c_termination_complete in libucrt.lib as corrupt...
         */

        public Linker(
            Config config = null,
            IEnumerable<string> exportedFunctions = null, // functions to export
            IEnumerable<ObjectFile> importStaticLibraryObjects = null
            )
        {
            CConfig = config ?? new Config();
            CConfig.ExportedFunctions = (exportedFunctions ?? Enumerable.Empty<string>()).ToList();
            if (CConfig.ImportLibraries.Count() == 0)
            {
                CConfig.ImportLibraries.Add("kernel32.dll");
                CConfig.ImportLibraries.Add("user32.dll");
                CConfig.ImportLibraries.Add("advapi32.dll");
                CConfig.ImportLibraries.Add("MSVCRT.dll");
                CConfig.ImportLibraries.Add("ucrtbase.dll");
            }
            
            ImportStaticLibraryObjects = importStaticLibraryObjects ?? new List<ObjectFile>();
            // GoLink doesn't support static libraries (.lib) as input http://masm32.com/board/index.php?topic=4662.0
            // As a work around the object files extract from a .lib file can be provided
        }

        private Config CConfig { get; }
        public IEnumerable<ObjectFile> ImportStaticLibraryObjects { get; }

        private enum TargetArch
        {
            x86,
            x64,
        }

        private U Link<T, U>(IEnumerable<StaticLibrary<T>> staticLibraries, Func<byte[], U> UConstructor, Config.OutputType outputType)
            where T : ObjectFile, new()
            where U : IPortableExecutable, new()
        {
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
                File.Copy(GoLinkResourcePath, "GoLink.exe");
                File.WriteAllText("input.txt", "");
                var inputFiles = new List<string>();
                foreach (var staticLibrary in staticLibraries)
                    foreach (var objectFile in staticLibrary.ObjectFiles)
                    {
                        var inputFilename = Utils.RandomString(5) + ".obj";
                        inputFiles.Add(inputFilename);
                        File.WriteAllBytes(inputFilename, objectFile.Bytes);
                        File.AppendAllText("input.txt", inputFilename + "\r\n");
                    }
                var outputfilename = "OUTPUT" + outputExtension;
                var linkerOptions = CConfig.ToString();
                if (linkerOptions.Length > 0)
                    linkerOptions = " " + linkerOptions;
                var linkCommand = "";
                linkCommand += " /fo " + outputfilename;
                linkCommand += linkerOptions;
                linkCommand += " " + "@input.txt";

                // Attempt to link
                // If there are any mising symbols, then try to locate them in ImportStaticLibraryObjects
                // & add them to the linker input list if found and try again until there are no more missing symbols
                var missingSymbols = new List<string>();

                switch (CConfig.ENTRY)
                {
                    case "mainCRTStartup":
                    case "wmainCRTStartup":
                    case "WinMainCRTStartup":
                    case "wWinMainCRTStartup":
                    case "_DllMainCRTStartup":
                        missingSymbols.Add(CConfig.ENTRY);
                        break;
                }

                for (var i =0; i < 10; i++)
                {
                    // Find the object file(s) that contain the symbols that are missing
                    foreach (var symbol in missingSymbols)
                    {
                        foreach (var slObject in ImportStaticLibraryObjects)
                        {
                            if (slObject.ExternalSymbols.Contains(symbol))
                            {
                                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                                {
                                    string hash = BitConverter.ToString(sha1.ComputeHash(slObject.Bytes)).Replace("-", "");
                                    File.WriteAllBytes($"{hash}.obj", slObject.Bytes);
                                    File.AppendAllText("input.txt", $"{hash}.obj\r\n");
                                }
                                break;
                            }
                        }
                    }

                    missingSymbols.Clear();

                    Process process = new Process();
                    process.StartInfo.FileName = "GoLink.exe";
                    process.StartInfo.Arguments = linkCommand;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    bool isMissingSymbols = false;
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string line = process.StandardOutput.ReadLine().Trim();
                        if (isMissingSymbols)
                            if (line != "")
                                missingSymbols.Add(line);
                        if (line.Contains("symbols were not defined"))
                            isMissingSymbols = true;
                    }

                    if (!isMissingSymbols)
                        break;

                    if (ImportStaticLibraryObjects.Count() == 0)
                        break;
                }

                var pe = UConstructor(File.ReadAllBytes(outputfilename));
                return pe;
            }
        }

        Executable ILinker<Win32ObjectFile, Executable>.Link(IEnumerable<StaticLibrary<Win32ObjectFile>> objs)
        {
            return Link<Win32ObjectFile, Executable>(objs, x => new Executable(x, CConfig.ExportedFunctions), Config.OutputType.EXE);
        }

        Executable ILinker<Win64ObjectFile, Executable>.Link(IEnumerable<StaticLibrary<Win64ObjectFile>> objs)
        {
            return Link<Win64ObjectFile, Executable>(objs, x => new Executable(x, CConfig.ExportedFunctions), Config.OutputType.EXE);
        }

        DynamicLinkLibrary ILinker<Win64ObjectFile, DynamicLinkLibrary>.Link(IEnumerable<StaticLibrary<Win64ObjectFile>> objs)
        {
            return Link<Win64ObjectFile, DynamicLinkLibrary>(objs, x => new DynamicLinkLibrary(x, CConfig.ExportedFunctions), Config.OutputType.DLL);
        }

        DynamicLinkLibrary ILinker<Win32ObjectFile, DynamicLinkLibrary>.Link(IEnumerable<StaticLibrary<Win32ObjectFile>> objs)
        {
            return Link<Win32ObjectFile, DynamicLinkLibrary>(objs, x => new DynamicLinkLibrary(x, CConfig.ExportedFunctions), Config.OutputType.DLL);
        }
    }
}
