using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using MyWarez.Core;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace MyWarez.Plugins.PyInstaller
{
    // TODO: Incorporate all of PyInstaller's features https://pyinstaller.readthedocs.io/en/stable/usage.html

    public static class PyInstaller
    {
        public static Executable BuildExeX64(IPythonScript script, bool windowed = false, string additionalPyInstallerArguments = "")
        {
            // Requires Windows,
            // Requires a 64-bit install of python

            throw new NotImplementedException();
        }

        public static Executable BuildExeX86(IPythonScript script, bool windowed = false, string additionalPyInstallerArguments = "")
        {
            // TODO: Verify that we are running on Windows and that 32-bit python is available (PyInstaller output is dependent on the OS & Architecture)

            using (new TemporaryContext())
            {
                var scriptText = script.Text;
                scriptText += "\r\n#" + Utils.RandomString(10); // non-functional change that will result in the compiled exe having a varying hash
                File.WriteAllText("input.py", scriptText);
                Process.Start("pyinstaller", "--onefile " + (windowed ? " --windowed " : "") + additionalPyInstallerArguments + " input.py").WaitForExit();
                var exeBytes = File.ReadAllBytes(Path.Join("dist", "input.exe"));
                return new Executable(exeBytes);
            }
        }
    }
}
