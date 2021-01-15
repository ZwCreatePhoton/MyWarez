namespace MyWarez.Core
{
    public interface IPythonScript : IPlainTextFile
    {

    }

    public class PythonScript : IPythonScript
    {
        public PythonScript() { }
        public PythonScript(string sourceCode)
        {
            Text = sourceCode;
        }
        public string Text { get; }
    }

    // TODO: class that analogous to CCxxSource (Multi-file Python)
}