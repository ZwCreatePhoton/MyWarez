/*using System;
using System.Collections.Generic;
using System.Text;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{
    public class CovenantVBScript : CovenantPayload
    {
        public CovenantVBScript(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => fileType == Tonsil.Files.FileType.VBScript;
        public override PayloadType Type { get; } = PayloadType.VBScript;
        public override string Name { get; } = "CovenantVBScript";
        public override string Description { get; } = "Covenant implant VBScript file";
        protected override Launcher Launcher { get; } = new WscriptLauncher() { ScriptLanguage = ScriptingLanguage.VBScript };
    }
}*/