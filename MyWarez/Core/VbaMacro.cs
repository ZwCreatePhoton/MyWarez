
namespace MyWarez.Core
{
    public interface IVbaMacro : IPlainTextFile
    {

    }

    public class VbaMacro : IPayload, IVbaMacro
    {
        public VbaMacro() { }
        public VbaMacro(string sourceCode)
        {
            Text = sourceCode;
        }
        public PayloadType Type { get; } = PayloadType.VbaMacro;

        public string Text { get; }
    }
}
