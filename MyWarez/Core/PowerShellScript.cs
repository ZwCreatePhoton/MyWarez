namespace MyWarez.Core
{
    public interface IPowerShellScript : IPlainTextFile
    {

    }

    public class PowerShellScript : IPayload, IPowerShellScript
    {
        public PowerShellScript() { }

        public PowerShellScript(string script)
        {
            Text = script;
        }

        //private string Script { get; } 

        public PayloadType Type { get; } = PayloadType.PowerShellScript;

        public virtual string Text { get; }
    }
}