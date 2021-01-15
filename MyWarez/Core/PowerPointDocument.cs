namespace MyWarez.Core
{
    public interface IPowerPointDocument : IFile
    {

    }

    public interface IMacroPowerPointDocument : IPowerPointDocument, IMacroDocument { }


    public class PowerPointDocument : IPowerPointDocument
    {
        public PowerPointDocument() { }
        public PowerPointDocument(byte[] bytes)
        {
            Bytes = bytes;
        }
        public byte[] Bytes { get; }
    }
    public class MacroPowerPointDocument : PowerPointDocument, IMacroPowerPointDocument
    {
        public MacroPowerPointDocument() { }
        public MacroPowerPointDocument(byte[] bytes) : base(bytes)
        { }
    }
}