namespace MyWarez.Core
{
    public interface IExcelDocument : IFile
    {

    }

    public interface IMacroExcelDocument : IExcelDocument, IMacroDocument { }


    public class ExcelDocument : IExcelDocument
    {
        public ExcelDocument()
        {

        }
        public ExcelDocument(byte[] bytes)
            : this(bytes, "xlsx")
        { }
        public ExcelDocument(byte[] bytes, string type)
        {
            Bytes = bytes;
            Type = type;
        }

        public byte[] Bytes { get; }
        public string Type { get; }
    }

    public class MacroExcelDocument : ExcelDocument, IMacroExcelDocument
    {
        public MacroExcelDocument()
            : base()
        { }
        public MacroExcelDocument(byte[] bytes)
            : this(bytes, "xlsm")
        { }
        public MacroExcelDocument(byte[] bytes, string type)
            : base(bytes, type)
        { }
    }
}