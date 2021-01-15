namespace MyWarez.Core
{
    public interface IAccessDocument : IFile
    {

    }

    public interface IMacroAccessDocument : IAccessDocument, IMacroDocument { }

    public class AccessDocument : IAccessDocument
    {
        public AccessDocument() { }
        public AccessDocument(byte[] bytes)
        {
            Bytes = bytes;
        }
        public byte[] Bytes { get; }
    }
    public class MacroAccessDocument : AccessDocument, IMacroAccessDocument
    {
        public MacroAccessDocument() { }
        public MacroAccessDocument(byte[] bytes) : base(bytes)
        { }
    }
}