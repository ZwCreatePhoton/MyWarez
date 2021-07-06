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
        public WordDocument()
        {

        }
        public WordDocument(byte[] bytes)
            : this(bytes, "doc")
        {}
        public WordDocument(byte[] bytes, string type)
        {
            Bytes = bytes;
            Type = type;
        }

        public byte[] Bytes { get; }
        public string Type { get; }
    }

    public class MacroWordDocument : WordDocument, IMacroWordDocument
    {
        public MacroWordDocument()
            : base()
        { }
        public MacroWordDocument(byte[] bytes)
            : this(bytes, "docm")
        { }
        public MacroWordDocument(byte[] bytes, string type)
            : base(bytes, type)
        { }
    }
}