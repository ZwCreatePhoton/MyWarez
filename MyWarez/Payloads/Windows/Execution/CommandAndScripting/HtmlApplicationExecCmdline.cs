/*using System;
using System.Collections.Generic;
using System.Text;

namespace MyWarez.Payloads
{
    public abstract class ExecCmdlineHtmlApplication : ExecCmdline
    {
        public ExecCmdlineHtmlApplication(ProcessList processList, ExecCmdline execCmdlineScript, string scriptName) : base(processList)
        {
            ExecCmdlineScript = execCmdlineScript;
            ScriptName = scriptName;
        }

        public override bool IsCompatible(Tonsil.Files.FileType fileType) => fileType == Tonsil.Files.FileType.Hta;
        public override PayloadType Type { get; } = PayloadType.HtmlApplication;
        private string ScriptName { get; }
        public override string Name => string.Format("ExecCmdline{0}HtmlApplication", ScriptName);
        public override string Description => string.Format("HTML Application with {0} that executes cmdlines", ScriptName);
        protected override string TemplateHeader => string.Format(@"<script LANGUAGE=""{0}"">", ScriptName) + "\n";
        protected override string TemplateExecCmdline => ExecCmdlineScript.Text;
        protected override string TemplateFooter { get; } = "</script>" + "\n";
        private ExecCmdline ExecCmdlineScript { get; }
    }
    public class ExecCmdlineJavaScriptHtmlApplication : ExecCmdlineHtmlApplication
    {
        public ExecCmdlineJavaScriptHtmlApplication(ProcessList processList) : base(processList, new ExecCmdlineJavaScript(processList), ScriptName){ }
        private static readonly string ScriptName = "JavaScript";
    }
    public class ExecCmdlineVBScriptHtmlApplication : ExecCmdlineHtmlApplication
    {
        public ExecCmdlineVBScriptHtmlApplication(ProcessList processList) : base(processList, new ExecCmdlineVBScript(processList), ScriptName) { }
        private static readonly string ScriptName = "VBScript";
    }
}
*/