namespace MyWarez.Core
{
    public interface IScriptComponent : IPlainTextFile
    {
    }

    public class ScriptComponent : IPayload, IScriptComponent
    {
        public ScriptComponent(string sourceCode)
        {
            Text = sourceCode;
        }
        public PayloadType Type { get; } = PayloadType.ScriptComponent;
        public string Text { get; }
    }
}