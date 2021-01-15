using System;

namespace MyWarez.Core
{
    public class ExcelCell
    {
        public ExcelCell(string value = null, string name = null)
        {
            Value = value;
            Name = name;
        }
        public string Value { get; set; }
        public string Name { get; set; }
        public bool IsExpression => Value.StartsWith("=");
        public string Expression => IsExpression ? Value.Substring(1) : null;
        public bool IsData => !IsExpression;
        public string Data => IsData ? Value : null;

    }
    public class ExcelCellMatrix
    {
        public ExcelCellMatrix(ExcelCell[][] cells)
        {
            Cells = cells.Clone() as ExcelCell[][];
        }
        public ExcelCell[][] Cells { get; set; }
    }

    public class ExcelWorksheet : IPayload
    {
        //TODO: Make dynamic
        public ExcelWorksheet(ExcelCellMatrix cells, bool macroEnabled = false)
        {
            CellMatrix = cells;
            MacroEnabled = macroEnabled;
        }
        public ExcelCellMatrix CellMatrix { get; }
        public bool MacroEnabled { get; }
        public PayloadType Type { get; } = PayloadType.ExcelWorksheet;
        protected static string EscapeString(string str)
        {
            return str.Replace(@"""", @"""""");
        }
    }


    // TODO: inherit ExcelWorksheet?
    public class ExcelCsv : Csv
    {
        public ExcelCsv(ExcelWorksheet worksheet)
        {
            if (DataLossCheck(worksheet))
                throw new ArgumentException("Data loss check failed");
            Worksheet = worksheet;
        }
        public ExcelWorksheet Worksheet { get; }
        public override string Text
        {
            get
            {
                var content = "";
                for (int row = 0; row < Worksheet.CellMatrix.Cells.GetLength(0); row++)
                {
                    for (int col = 0; col < Worksheet.CellMatrix.Cells[row].Length; col++)
                    {
                        var cell = Worksheet.CellMatrix.Cells[row][col];
                        if (cell == null)
                            continue;
                        else
                        {
                            if (col != 0)
                                content += ",";
                            content += EscapeCellValue(cell.Value);
                        }
                    }
                    content += NewLine;
                }
                return content;
            }
        }
        private static string NewLine = "\r\n";
        private static string EscapeCellValue(string cellValue)
        {
            return cellValue;
        }
        private static bool DataLossCheck(ExcelWorksheet document)
        {
            if (document.MacroEnabled)
                return true;
            foreach (var row in document.CellMatrix.Cells)
                foreach (var cell in row)
                    if (cell != null && cell.Name != null)
                        return true;
            return false;
        }
    }

    public interface ITabDelimited : IPlainTextFile
    {

    }


    // TODO: inherit ExcelWorksheet?
    public class TabDelimited : IPayload, ITabDelimited
    {
        public TabDelimited(ExcelWorksheet document)
        {
            Document = document;
        }
        public PayloadType Type { get; } = PayloadType.Text;
        public ExcelWorksheet Document { get; }
        public virtual string Text
        {
            get
            {
                var content = "";
                for (int row = 0; row < Document.CellMatrix.Cells.GetLength(0); row++)
                {
                    for (int col = 0; col < Document.CellMatrix.Cells[row].Length; col++)
                    {
                        var cell = Document.CellMatrix.Cells[row][col];
                        if (cell == null)
                            content += " ";
                        else
                        {
                            if (cell.Value != null)
                                content += cell.Value.Replace("=", ""); // Cheating for now. Assuming all cells are formulas
                            else
                                content += " ";
                        }
                        content += "\t"; // todo: remove the last tab in each line
                    }
                    content += NewLine;
                }
                return content;
            }
        }

        protected static string NewLine = "\r\n";
    }

    public interface ISylk : IPlainTextFile
    {

    }

    // TODO: inherit ExcelWorksheet?
    public class Sylk : IPayload, ISylk
    {
        public Sylk(ExcelWorksheet document)
        {
            Document = document;
        }
        public PayloadType Type { get; } = PayloadType.Sylk;
        public ExcelWorksheet Document { get; }
        public string Text
        {
            get
            {
                var content = "";
                content += Header + NewLine;
                content += Options + (Options.Length != 0 ? NewLine : "");
                for (int row = 0; row < Document.CellMatrix.Cells.GetLength(0); row++)
                {
                    for (int col = 0; col < Document.CellMatrix.Cells[row].Length; col++)
                    {
                        var cell = Document.CellMatrix.Cells[row][col];
                        if (cell == null)
                            continue;
                        if (cell.Name != null)
                            content += string.Format("NN;N{0};ER{1}C{2}", cell.Name, row + 1, col + 1) + NewLine;
                        if (cell.Value != null)
                            content += string.Format("C;X{0};Y{1};{2}{3}", col + 1, row + 1, cell.IsExpression ? "E" : "K", cell.IsExpression ? cell.Expression : cell.Data) + NewLine;
                    }
                }
                content += Footer;
                return content;
            }
        }
        private static string Header = "ID;P";
        private static string Footer = "E";
        private string Options => "O" + (Document.MacroEnabled ? ";E" : "");
        protected static string NewLine = "\n";
    }
}
