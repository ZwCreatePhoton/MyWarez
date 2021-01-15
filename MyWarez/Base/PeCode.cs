using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MyWarez.Base
{
    public static class PeCode
    {
        public static string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, "PeCode");

        public static byte[] Extract(Executable exe)
        {
            using (new TemporaryContext())
            {
                Core.Utils.CopyFilesRecursively(ResourceDirectory, ".");
                File.WriteAllBytes("OUTPUT.exe", exe.Bytes);
                var arguments = "-ExecutionPolicy Bypass -f Out-Shellcode.ps1 OUTPUT.exe . OUTPUT.bin";
                Process.Start("powershell", arguments).WaitForExit();
                if (!File.Exists("OUTPUT.bin"))
                    throw new Exception("PeCode error. Out-Shellcode failed");
                return File.ReadAllBytes("OUTPUT.bin");
            }
        }
    }
}

/*
 * 
 * 
 * 
$MapContents = Get-Content $InputMapFile
#
#$TextSectionInfo = @($MapContents | Where-Object { $_ -match '\.text\W+CODE' })[0]
$TextSectionInfo = @($MapContents | Where-Object { $_ -match 'CODE' })[0]
# Possible fix for VS 2017, sufficient to match on just CODE in line.

$ShellcodeLength = [Int] "0x$(( $TextSectionInfo -split ' ' | Where-Object { $_ } )[1].TrimEnd('H'))" - 1

Write-Host "Shellcode length: 0x$(($ShellcodeLength + 1).ToString('X4'))"

[IO.File]::WriteAllBytes($OutputFile, $TextSection.RawData[0..$ShellcodeLength])

 */