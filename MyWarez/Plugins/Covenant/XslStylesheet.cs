/*using System;
using System.Collections.Generic;
using System.Text;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{
    public abstract class CovenantXslStylesheet : CovenantPayload
    {
        public CovenantXslStylesheet(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => fileType == Tonsil.Files.FileType.Stylesheet;
        public override PayloadType Type { get; } = PayloadType.XslStylesheet;
    }
    public class CovenantXslStylesheetJavaScript : CovenantXslStylesheet
    {
        public CovenantXslStylesheetJavaScript(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override string Name { get; } = "CovenantXslStylesheetJavaScript";
        public override string Description { get; } = "Covenant implant XSL stylesheet with JavaScript file";
        protected override Launcher Launcher { get; } = new WmicLauncher() { ScriptLanguage = ScriptingLanguage.JScript };
    }
    public class CovenantVBScriptXslStylesheet : CovenantXslStylesheet
    {
        public CovenantVBScriptXslStylesheet(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override string Name { get; } = "CovenantVBScriptXslStylesheet";
        public override string Description { get; } = "Covenant implant XSL stylesheet with VBScript file";
        protected override Launcher Launcher { get; } = new WmicLauncher() { ScriptLanguage = ScriptingLanguage.VBScript };
    }

}*/