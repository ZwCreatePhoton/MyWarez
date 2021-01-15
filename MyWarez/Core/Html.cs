namespace MyWarez.Core
{
    public interface IHtml : IPlainTextFile
    {

    }

    public class Html : IPayload, IHtml
    {
        public Html(string sourceCode)
        {
            Text = sourceCode;
        }
        public PayloadType Type { get; } = PayloadType.Html;
        public string Text { get; }
    }
}