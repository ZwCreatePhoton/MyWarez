namespace MyWarez.Core
{
    public interface IAccessDocument : IFile
    {

    }

    public interface IMacroAccessDocument : IAccessDocument, IMacroDocument { }


    public class AccessDocument : IAccessDocument
    {
        public AccessDocument()
        {

        }
        public AccessDocument(byte[] bytes)
            : this(bytes, "accdb")
        { }
        public AccessDocument(byte[] bytes, string type)
        {
            Bytes = bytes;
            Type = type;
        }

        public byte[] Bytes { get; }
        public string Type { get; }
    }

    public class MacroAccessDocument : AccessDocument, IMacroAccessDocument
    {
        public MacroAccessDocument()
            : base()
        { }
        public MacroAccessDocument(byte[] bytes)
            : this(bytes, "accdb")
        { }
        public MacroAccessDocument(byte[] bytes, string type)
            : base(bytes, type)
        { }
    }
}