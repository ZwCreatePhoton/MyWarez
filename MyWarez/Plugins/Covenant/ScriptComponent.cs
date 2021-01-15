/*using System;
using System.Collections.Generic;
using System.Text;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{
    public abstract class CovenantScriptComponent : CovenantPayload
    {
        public CovenantScriptComponent(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => false;
        public override PayloadType Type { get; } = PayloadType.ScriptComponent;
    }
    public class CovenantJavaScriptScriptComponent : CovenantScriptComponent
    {
        public CovenantJavaScriptScriptComponent(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override string Name { get; } = "CovenantJavaScriptScriptComponent";
        public override string Description { get; } = "Covenant implant Script Component with JavaScript file";
        protected override Launcher Launcher { get; } = new Regsvr32Launcher() { ScriptLanguage = ScriptingLanguage.JScript };
    }
    public class CovenantVBScriptScriptComponent : CovenantScriptComponent
    {
        public CovenantVBScriptScriptComponent(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override string Name { get; } = "CovenantVBScriptScriptComponent";
        public override string Description { get; } = "Covenant implant Script Component with VBScript file";
        protected override Launcher Launcher { get; } = new Regsvr32Launcher() { ScriptLanguage = ScriptingLanguage.VBScript };
    }
}*/