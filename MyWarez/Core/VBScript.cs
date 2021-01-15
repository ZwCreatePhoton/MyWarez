namespace MyWarez.Core
{
    public interface IVBScript : IPlainTextFile
    {

    }

    public class VBScript : IPayload, IVBScript
    {
        public VBScript() { }
        public VBScript(string sourceCode)
        {
            Text = sourceCode;
        }
        public PayloadType Type { get; } = PayloadType.VBScript;
        public string Text { get; }
    }
}