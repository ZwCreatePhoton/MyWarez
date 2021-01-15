/*using System;
using System.Collections.Generic;
using System.Text;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{    
    public class ExecCmdlineVBScript : ExecCmdline
    {
        public ExecCmdlineVBScript(ProcessList processList) : base(processList) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => fileType == Tonsil.Files.FileType.VBScript;
        public override PayloadType Type { get; } = PayloadType.VBScript;
        public override string Name { get; } = "ExecCmdlineVBScript";
        public override string Description { get; } = "VBscript that executes cmdlines";
        protected override string EscapeCmdline(string script) => script.Replace(@"""", @"""""");
        protected override string TemplateExecCmdline { get; } = @"CreateObject(""WScript.Shell"").Run ""{0}"", 0, true" + "\n";
    }
}*/