namespace MyWarez.Core
{
    public interface IElf : IFile
    {

    }

    public class Elf : IPayload, IElf
    {
        public Elf(byte[] bytes)
        {

        }
        public PayloadType Type { get; } = PayloadType.Elf;

        public byte[] Bytes { get; }
    }
}
