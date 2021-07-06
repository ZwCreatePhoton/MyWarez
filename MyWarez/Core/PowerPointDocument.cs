namespace MyWarez.Core
{
    public interface IPowerPointDocument : IFile
    {

    }

    public interface IMacroPowerPointDocument : IPowerPointDocument, IMacroDocument { }


    public class PowerPointDocument : IPowerPointDocument
    {
        public PowerPointDocument()
        {

        }
        public PowerPointDocument(byte[] bytes)
            : this(bytes, "pptx")
        { }
        public PowerPointDocument(byte[] bytes, string type)
        {
            Bytes = bytes;
            Type = type;
        }

        public byte[] Bytes { get; }
        public string Type { get; }
    }

    public class MacroPowerPointDocument : PowerPointDocument, IMacroPowerPointDocument
    {
        public MacroPowerPointDocument()
            : base()
        { }
        public MacroPowerPointDocument(byte[] bytes)
            : this(bytes, "pptm")
        { }
        public MacroPowerPointDocument(byte[] bytes, string type)
            : base(bytes, type)
        { }
    }
}