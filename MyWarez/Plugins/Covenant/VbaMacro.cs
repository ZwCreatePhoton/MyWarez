
using Covenant.Models.Launchers;
using MyWarez.Core;

namespace MyWarez.Plugins.Covenant
{
    public sealed class CovenantVbaMacro : CovenantPayload, IVbaMacro
    {
        public CovenantVbaMacro(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public PayloadType Type { get; } = PayloadType.VbaMacro;
        protected override Launcher Launcher { get; } = new VBAMacroLauncher() {  };
    }
}