namespace MyWarez.Core
{
    public interface IExcelDocument : IFile
    {

    }
    public interface IMacroExcelDocument : IExcelDocument, IMacroDocument { }

    public class ExcelDocument : IExcelDocument
    {
        public ExcelDocument() { }
        public ExcelDocument(byte[] bytes)
        {
            Bytes = bytes;
        }
        public byte[] Bytes { get; }
    }

    public class MacroExcelDocument : ExcelDocument, IMacroExcelDocument
    {
        public MacroExcelDocument() { }
        public MacroExcelDocument(byte[] bytes) : base(bytes)
        { }
    }
}