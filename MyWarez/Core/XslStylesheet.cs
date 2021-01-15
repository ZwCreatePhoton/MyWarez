namespace MyWarez.Core
{
    public interface IXslStylesheet : IPlainTextFile
    {

    }

    public class XslStylesheet : IXslStylesheet
    {
        public XslStylesheet() { }
        public XslStylesheet(string sourceCode)
        {
            Text = sourceCode;
        }
        public string Text { get; }
    }
}