/*using System;
using System.Collections.Generic;
using System.Text;
using Tonsil.Files;

namespace MyWarez.Payloads
{ 
    public abstract class ExecCmdline : Payload, IFilePayload
    {
        public ExecCmdline(ProcessList processList) => ProcessList = processList;
        protected ProcessList ProcessList { get; }
        public override string FullDescription => Description + " ; " + ProcessList.FullDescription;
        public override IEnumerable<(Payload, File)> Dependencies =>ProcessList.Dependencies;
        public string Text => Encoding.ASCII.GetString(Bytes);
        public abstract bool IsCompatible(FileType fileType);
        public virtual byte[] Bytes
        {
            get
            {
                string script = "";
                script += TemplateHeader;
                foreach (var process in ProcessList.Processes)
                {
                    script += string.Format(TemplateExecCmdline, EscapeCmdline(process.CmdLine.ToString()));
                }
                script += TemplateFooter;
                return Encoding.ASCII.GetBytes(script);
            }
        }
        protected virtual string EscapeCmdline(string script) => script;
        protected virtual string TemplateHeader { get; } = "";
        protected virtual string TemplateExecCmdline { get; } = @"{0}" + "\n";
        protected virtual string TemplateFooter { get; } = "";
    }
}
*/