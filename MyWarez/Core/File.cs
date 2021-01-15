using System.Text;

namespace MyWarez.Core
{
    public interface IFile
    {
        public byte[] Bytes { get; }
        public string Extension => null;
    }

    public interface IPlainTextFile : IFile
    {
        public string Text { get; }
        byte[] IFile.Bytes => Encoding.ASCII.GetBytes(Text);
    }
}
