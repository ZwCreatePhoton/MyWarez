
using Covenant.Models.Launchers;
using MyWarez.Core;

namespace MyWarez.Plugins.Covenant
{
    public sealed class CovenantJavaScript : CovenantPayload, IJavaScript
    {
        public CovenantJavaScript(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public PayloadType Type { get; } = PayloadType.JavaScript;
        protected override Launcher Launcher { get; } = new WscriptLauncher() { ScriptLanguage = ScriptingLanguage.JScript };
    }
}