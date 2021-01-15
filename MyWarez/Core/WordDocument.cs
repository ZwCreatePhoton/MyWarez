namespace MyWarez.Core
{
    public interface IMacroDocument : IFile
    {

    }

    public interface IWordDocument : IFile
    {

    }

    public interface IMacroWordDocument : IWordDocument, IMacroDocument { }


    public class WordDocument : IWordDocument
    {
        public WordDocument() { }
        public WordDocument(byte[] bytes)
        {
            Bytes = bytes;
        }
        public byte[] Bytes { get; }
    }

    public class MacroWordDocument : WordDocument, IMacroWordDocument
    {
        public MacroWordDocument() { }
        public MacroWordDocument(byte[] bytes) : base(bytes)
        { }
    }
}