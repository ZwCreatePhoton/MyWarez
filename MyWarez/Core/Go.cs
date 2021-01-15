namespace MyWarez.Core
{
    public interface IGoScript : IPlainTextFile
    {

    }

    public class GoScript : IGoScript
    {
        public GoScript() { }
        public GoScript(string sourceCode)
        {
            Text = sourceCode;
        }
        public string Text { get; }
    }

    // TODO: class that analogous to CCxxSource (Multi-file Go)
}