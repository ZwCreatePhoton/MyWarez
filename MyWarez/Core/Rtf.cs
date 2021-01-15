namespace MyWarez.Core
{
    public interface IRtf : IFile
    {

    }

    public class Rtf : IPayload, IFile
    {
        public Rtf(byte[] bytes)
        {
            Bytes = bytes;
        }
        public PayloadType Type { get; } = PayloadType.Rtf;
        public byte[] Bytes { get; }
    }
}