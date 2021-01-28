using System;
using System.IO;

using MyWarez.Base;
using MyWarez.Core;
using MyWarez.Payloads;
using MyWarez.Plugins.Yasm;
using MyWarez.Plugins.Msvc;
using MyWarez.Plugins.PyInstaller;
using MyWarez.Plugins.GoBuild;
using MyWarez.Plugins.Donut;

namespace Examples
{
    public static partial class AttackExamples
    {
        // PyInstaller Example
        private static IAttack PyInstallerExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "PyInstallerExample";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            var resourceFilePath = Path.Join(MyWarez.Core.Constants.ResourceDirectory, "calc.py");
            var resourceText = File.ReadAllText(resourceFilePath);
            // Wrapper around single file python
            var pythonScript = new PythonScript(resourceText);
            // Nonfunctional changes are made to the script so that the hash is different with every build
            var pythonExe = PyInstaller.BuildExeX86(pythonScript);
            samplesOutput.Add("pythonCalc.exe", pythonExe);

            attack.Generate();
            return attack;
        }

        // Go Build Example
        private static IAttack GoBuildExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "GoBuildExample";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            var resourceFilePath = Path.Join(MyWarez.Core.Constants.ResourceDirectory, "calc.go");
            var resourceText = File.ReadAllText(resourceFilePath);
            // Wrapper around single file go
            var goScript = new GoScript(resourceText);
            // Nonfunctional changes are made to the script so that the hash is different with every build
            var goExe64 = GoBuild.BuildExeX64(goScript);
            samplesOutput.Add("goCalc64.exe", goExe64);

            // Go build command errors out
            /*
            var goExe86 = GoBuild.BuildExeX86(goScript);
            samplesOutput.Add("goCalc86.exe", goExe86);

            var goDll64 = GoBuild.BuildDllX64(goScript, exportedFunctionName: "Pineapple");
            samplesOutput.Add("goCalc64.dll", goDll64);
            */

            attack.Generate();
            return attack;
        }

        // Donut (exe/dll -> shellcode) Example
        private static IAttack DonutExample()
        {
            static Executable ShellcodeToExe(IShellcode shellcode, TargetArch targetArch)
            {
                switch (targetArch)
                {
                    case TargetArch.x64:
                        {
                            var rawShellcodeAsm = new RawShellcodeYasmAssemblySource(shellcode, symbolName: "SheSellsShellCodesByTheSilkRoad");
                            var staticLibrary = ((IAssembler<YASM, Win64ObjectFile>)new MyWarez.Plugins.Yasm.Yasm()).Assemble(rawShellcodeAsm);
                            var entryPoint = ((ICFunction)rawShellcodeAsm).Name;
                            var linkerConfig = new MyWarez.Plugins.GoDevTool.Linker.Config(ENTRY: entryPoint);
                            return ((ILinker<Win64ObjectFile, Executable>)new MyWarez.Plugins.GoDevTool.Linker(linkerConfig)).Link(staticLibrary);
                        }
                    case TargetArch.x86:
                        {
                            var rawShellcodeAsm = new RawShellcodeYasmAssemblySource(shellcode, symbolName: "SheSellsShellCodesByTheSilkRoad");
                            var staticLibrary = ((IAssembler<YASM, Win32ObjectFile>)new MyWarez.Plugins.Yasm.Yasm()).Assemble(rawShellcodeAsm);
                            var entryPoint = ((ICFunction)rawShellcodeAsm).Name;
                            var linkerConfig = new MyWarez.Plugins.GoDevTool.Linker.Config(ENTRY: entryPoint);
                            var exe = ((ILinker<Win32ObjectFile, Executable>)new MyWarez.Plugins.GoDevTool.Linker(linkerConfig)).Link(staticLibrary);
                            return exe;
                        }
                    default:
                        throw new Exception();
                }
            }

            var samplesOutput = new SamplesOutput();
            var attackName = "DonutExample";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            // Donut Input
            // Executable
            var createProcessSource = new CreateProcess("notepad");
            var createProcessExeSource = new FunctionCallExeWinMainCCxxSource(createProcessSource);
            // 64bit
            var createProcessExeStaticLibrary64 = ((ICCxxCompiler<Win64ObjectFile>)new MyWarez.Plugins.Msvc.Compiler()).Compile(createProcessExeSource);
            var executable64 = ((ILinker<Win64ObjectFile, Executable>)new MyWarez.Plugins.Msvc.Linker()).Link(createProcessExeStaticLibrary64);
            // 32bit
            var createProcessExeStaticLibrary32 = ((ICCxxCompiler<Win32ObjectFile>)new MyWarez.Plugins.Msvc.Compiler()).Compile(createProcessExeSource);
            var executable32 = ((ILinker<Win32ObjectFile, Executable>)new MyWarez.Plugins.Msvc.Linker()).Link(createProcessExeStaticLibrary32);
            // DynamicLinkLibrary
            var exportedFunctionName = ((ICFunction)createProcessSource).Name;
            var createProcessDllSource = new SkeletonDllMainCCxxSource(createProcessSource, exportedFunctions: new[] { exportedFunctionName });
            // 64bit
            var createProcessDllStaticLibrary64 = ((ICCxxCompiler<Win64ObjectFile>)new MyWarez.Plugins.Msvc.Compiler()).Compile(createProcessDllSource);
            var dynamicLinkLibrary64 = ((ILinker<Win64ObjectFile, DynamicLinkLibrary>)new MyWarez.Plugins.Msvc.Linker(exportedFunctions: createProcessDllSource.ExportedFunctions)).Link(createProcessDllStaticLibrary64);
            // 32bit
            var createProcessDllStaticLibrary32 = ((ICCxxCompiler<Win32ObjectFile>)new MyWarez.Plugins.Msvc.Compiler()).Compile(createProcessDllSource);
            var dynamicLinkLibrary32 = ((ILinker<Win32ObjectFile, DynamicLinkLibrary>)new MyWarez.Plugins.Msvc.Linker(exportedFunctions: createProcessDllSource.ExportedFunctions)).Link(createProcessDllStaticLibrary32);

            // Generate Shellcodes
            var executable64Shellcode = new ExecutableDonutShellcode(executable64);
            var executable32Shellcode = new ExecutableDonutShellcode(executable32);
            // For DLLs, an exported function to execute should be specified
            var dynamicLinkLibrary64Shellcode = new DynamicLinkLibraryDonutShellcode(dynamicLinkLibrary64, function: exportedFunctionName);
            var dynamicLinkLibrary32Shellcode = new DynamicLinkLibraryDonutShellcode(dynamicLinkLibrary32, function: exportedFunctionName);

            // Shellcode test Exes
            // Architecture of the input needs to match the architeture of the target process
            // Input = 64-bit executable, Test process: 64-bit
            var executable64ShellcodeExe64 = ShellcodeToExe(executable64Shellcode, TargetArch.x64);
            samplesOutput.Add(nameof(executable64ShellcodeExe64) + ".exe", executable64ShellcodeExe64);
            // Input = 32-bit executable, Test process: 32-bit
            var executable32ShellcodeExe32 = ShellcodeToExe(executable32Shellcode, TargetArch.x86);
            samplesOutput.Add(nameof(executable32ShellcodeExe32) + ".exe", executable32ShellcodeExe32);

            // Input = 64-bit dynamicLinkLibrary, Test process: 64-bit
            var dynamicLinkLibrary64ShellcodeExe64 = ShellcodeToExe(dynamicLinkLibrary64Shellcode, TargetArch.x64);
            samplesOutput.Add(nameof(dynamicLinkLibrary64ShellcodeExe64) + ".exe", dynamicLinkLibrary64ShellcodeExe64);
            /*// Input = 32-bit dynamicLinkLibrary, Test process: 32-bit
            var dynamicLinkLibrary32ShellcodeExe32 = ShellcodeToExe(dynamicLinkLibrary32Shellcode, TargetArch.x86);
            samplesOutput.Add(nameof(dynamicLinkLibrary32ShellcodeExe32) + ".exe", dynamicLinkLibrary32ShellcodeExe32);
*/          // Don't know why this doesn't work

            attack.Generate();
            return attack;
        }
    }
}
