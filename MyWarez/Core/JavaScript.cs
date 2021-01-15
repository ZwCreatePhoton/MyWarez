namespace MyWarez.Core
{
    public interface IJavaScript : IPlainTextFile
    {
    }

    public class JavaScript : IPayload, IJavaScript
    {
        public JavaScript(string sourceCode)
        {
            Text = sourceCode;
        }
        public PayloadType Type { get; } = PayloadType.JavaScript;
        public string Text { get; }
    }
}