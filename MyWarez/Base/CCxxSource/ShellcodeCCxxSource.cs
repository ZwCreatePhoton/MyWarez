using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MyWarez.Base
{
    // C code that can be compiled into shellcode
    // Requires the developer to be meticulous in how they write, compile, and link the code to ensure the compiler+linker will output shellcode/PiC
    public interface IShellcodeCCxxSource : ICCxxSource
    {
        // return a variant of ICCxxSource.SourceFiles (for example, change imported WinAPI calls to dymically imported WinAPI calls)
        public new IEnumerable<CCxxSourceFile> SourceFiles { get; }
    }
    public interface IShellcodeCCxxSourceIParameterlessCFunction : IShellcodeCCxxSource, IParameterlessCFunction, ICCxxSourceIParameterlessCFunction, IShellcodeParameterlessCFunction { }
    public interface IShellcodeCCxxSourceICFunction : IShellcodeCCxxSource, ICFunction, ICCxxSourceICFunction, IShellcodeCFunction { }


    public class ShellcodeCCxxSource : CCxxSource, IShellcodeCCxxSource
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "CCxxSource", nameof(ShellcodeCCxxSource));
        private static readonly string HeadersPlaceholder = "//HEADERS";
        private static readonly string DeclarationsPlaceholder = "//DECLARATIONS";
        private static readonly string InitializationsPlaceholder = "//INITIALIZATIONS";

        private static readonly string FuncDeclarationImplementionFunctionsRegex = @"([\.\w]+)!(\w+)"; // Capture Groups: ( dll name , function name)

        public ShellcodeCCxxSource(ICCxxSource source) : base(source) { }

        public ShellcodeCCxxSource(CCxxSourceFile sourceFile, string entryPointSymbolName = null) : this(new List<CCxxSourceFile>() { sourceFile }, entryPointSymbolName) { }

        public ShellcodeCCxxSource(IEnumerable<CCxxSourceFile> sourceFiles, string entryPointSymbolName = null) : base(sourceFiles, entryPointSymbolName) { }


        IEnumerable<CCxxSourceFile> IShellcodeCCxxSource.SourceFiles
        {
            get
            {
                // TODO: populate DynamicallyLoadedFunctionSignatures.h with more signatures ...

                // TODO: variant that doesn't walk the PEB and/or IATs in the target process (detectable behavior by EMET)
                //          Can be acomplished by inserting placeholder addresses for winapi functions.
                //          The process injecting this shellcode will then be responsible with patching the shellcode with the correct addresses at runtime
                //          Virtual addresses of DLLs accross processes will be the same (assuming no conflicts with loaded non-OS DLLs) as long as at least 1 (nonwritable) copy of the DLL is maintained in memory between patch time and shellcode time
                //              Would still need to load nonLoaded DLLs wilth LoadLibrary in the target process
                // TODO: Variant that can selectivey avoid the need to call LoadLibrary if the user knows that the target DLL will already be loaded in the target address space

                var allDynamicallyLoadedFunctions = DynamicallyLoadedFunctions();

                var sourceFiles = ((ICCxxSource)this).SourceFiles;

                foreach (var sourceFile in sourceFiles)
                {
                    var relevantDLFunctions = allDynamicallyLoadedFunctions.Where(x => sourceFile.Source.Contains(x.Item2)).ToList();
                    if (relevantDLFunctions.Count == 0)
                        continue;

                    // Swap function calls with the new ones
                    // Are there any Windows API function names that are substrings of other Windows API function names?
                    foreach (var x in relevantDLFunctions)
                        sourceFile.Source = sourceFile.Source.Replace(x.Item2+"(", "My" + x.Item2+"(");

                    // Declarations
                    var declarations = new List<string>();
                    var dontLoadLibraryDLLs = new[] { "ntdll.dll", "kernelbase.dll", "kernel32.dll" };
                    var modulesToBeLoadedList = relevantDLFunctions.Where(x => !dontLoadLibraryDLLs.Contains(x.Item1)).Select(x => x.Item1).ToList();
                    var modulesToBeLoaded = new HashSet<string>(modulesToBeLoadedList);
                    if (modulesToBeLoaded.Count > 0 && !relevantDLFunctions.Contains(("kernel32.dll", "LoadLibraryA")))
                        relevantDLFunctions.Add(("kernel32.dll", "LoadLibraryA"));
                    var dlFunctionDeclarations = relevantDLFunctions.Select(x => $"Func{x.Item2} My{x.Item2};").ToList();
                    declarations.AddRange(dlFunctionDeclarations);
                    var declarationsString = string.Join("\r\n", declarations.ToArray());
                    sourceFile.Source = sourceFile.Source.Replace(DeclarationsPlaceholder, declarationsString);

                    // Initializations
                    var initializations = new List<string>();
                    var moduleStringInitializations = modulesToBeLoaded.Select(x => $"char String{x.Replace(".", "_")}[] = {Utils.StringToCArrary(x)};");
                    initializations.AddRange(moduleStringInitializations);
                    if (relevantDLFunctions.Any(x => x.Item2 == "LoadLibraryA"))
                    {
                        initializations.Add($"MyLoadLibraryA = (FuncLoadLibraryA) GetProcAddressWithHash(0x{Rot13Hash("kernel32.dll", "LoadLibraryA")});");
                        foreach (var module in modulesToBeLoaded)
                            initializations.Add($"MyLoadLibraryA((LPCSTR) String{module.Replace(".", "_")});");
                    }                    
                    initializations.AddRange(relevantDLFunctions.Where(x => x.Item2 != "LoadLibraryA").Select(x => $"My{x.Item2} = (Func{x.Item2}) GetProcAddressWithHash(0x{Rot13Hash(x.Item1, x.Item2)});"));
                    var initializationsString = string.Join("\r\n", initializations.ToArray());
                    sourceFile.Source = sourceFile.Source.Replace(InitializationsPlaceholder, initializationsString);

                    // Headers
                    var headers = new[]
                    {
                        "#include \"DynamicallyLoadedFunctionSignatures.h\"", // needs to be first due to Winsock2.h
                        "#include \"GetProcAddressWithHash.h\"",
                    };
                    var headersString = string.Join("\r\n", headers);
                    sourceFile.Source = sourceFile.Source.Replace(HeadersPlaceholder, headersString);
                }

                return SourceDirectoryToSourceFiles(ResourceDirectory, additionalSources: new[] { new CCxxSource(sourceFiles) } );
            }
        }

        private static string Rot13Hash(string module, string function)
        {
            static uint Rotr32(uint value, byte shift)
            {
                return (value >> shift) | (value << (32 - shift));
            }

            static uint Rot13(string input)
            {
                uint hash = 0;
                var inputBytes = Encoding.ASCII.GetBytes(input);
                foreach (var c in inputBytes)
                    hash = Rotr32(hash, 13) + c;
                return hash;
            }

            static string unicode(string input, bool uppercase = true)
            {
                string output = "";
                if (uppercase)
                    input = input.ToUpper();
                foreach (var c in input)
                {
                    output += c;
                    output += '\x00';
                }
                return output;
            }

            return (Rot13(unicode(module + '\x00')) + Rot13(function + '\x00')).ToString("X");
        }

        // (module name, function name)
        // e.g. (kernel32.dll, CreateProcessA)
        private static IEnumerable<(string,string)> DynamicallyLoadedFunctions()
        {
            var implementedFunctions = new List<(string, string)>() { };
            Regex regex = new Regex(FuncDeclarationImplementionFunctionsRegex);
            Match match = regex.Match(File.ReadAllText(Path.Join(ResourceDirectory, "DynamicallyLoadedFunctionSignatures.h")));
            while (match.Success)
            {
                var dllName = match.Groups[1].Value.ToLower();
                var functionName = match.Groups[2].Value;
                implementedFunctions.Add((dllName, functionName));
                match = match.NextMatch();
            }
            return implementedFunctions;
        }
    }


    public class ShellcodeCCxxSourceParameterlessCFunction : ShellcodeCCxxSource, IShellcodeCCxxSourceIParameterlessCFunction
    {
        public ShellcodeCCxxSourceParameterlessCFunction(ICCxxSource source)
            : base(source)
        { }

        public ShellcodeCCxxSourceParameterlessCFunction(IEnumerable<CCxxSourceFile> sourceFiles, string entryPointSymbolName = null)
            : base(sourceFiles, entryPointSymbolName)
        { }


        string ICFunction.Name => EntryPointSymbolName;

        IEnumerable<string> ICFunction.ParameterTypes => new string[] { };
    }
}
