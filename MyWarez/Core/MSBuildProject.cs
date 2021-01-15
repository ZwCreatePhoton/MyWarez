namespace MyWarez.Core
{
    public interface IMSBuildProject : IXml
    {
    }

    public class MSBuildProject : IPayload, IMSBuildProject
    {
        public MSBuildProject(string sourceCode)
        {
            Text = sourceCode;
        }
        public PayloadType Type { get; } = PayloadType.MSBuildProject;
        public string Text { get; }
    }
}