/*using System;
using System.Collections.Generic;
using System.Text;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{
    public abstract class CovenantHtmlApplication : CovenantPayload
    {
        public CovenantHtmlApplication(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => fileType == Tonsil.Files.FileType.Hta;
        public override PayloadType Type { get; } = PayloadType.HtmlApplication;
    }
    public class CovenantJavaScriptHtmlApplication : CovenantHtmlApplication
    {
        public CovenantJavaScriptHtmlApplication(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override string Name { get; } = "CovenantJavaScriptHtmlApplication";
        public override string Description { get; } = "Covenant implant HTML Application with JavaScript file";
        protected override Launcher Launcher { get; } = new MshtaLauncher() { ScriptLanguage = ScriptingLanguage.JScript };
    }
    public class CovenantVBScriptHtmlApplication : CovenantHtmlApplication
    {
        public CovenantVBScriptHtmlApplication(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override string Name { get; } = "CovenantVBScriptHtmlApplication";
        public override string Description { get; } = "Covenant implant HTML Application with VBScript file";
        protected override Launcher Launcher { get; } = new MshtaLauncher() { ScriptLanguage = ScriptingLanguage.VBScript };
    }
}*/