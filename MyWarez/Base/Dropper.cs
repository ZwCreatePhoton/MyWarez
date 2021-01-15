/*using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Tonsil.Files;

namespace MyWarez.Payloads
{ 
    public abstract class Dropper : Payload, IFilePayload
    {
        public Dropper(ProcessList processList, Tonsil.Files.FilePath droppedFilePath)
        {
            ProcessList = processList;
            DroppedFilePath = droppedFilePath;
        }

        protected ProcessList ProcessList { get; }
        public FilePath DroppedFilePath { get; }

        public override string FullDescription => Description + " ; " + ProcessList.FullDescription;
        protected virtual string Template => @"{0}{1}";
        public virtual string Text
        {
            get
            {
                string dropFileString = "";
                if (ProcessList.FilePayload != null)
                {
                    dropFileString += DropFileString(((IFilePayload)ProcessList.FilePayload).Bytes);
                    dropFileString += NewLine;
                }
                string executeProcessesString = ExecuteProcessesString(ProcessList.Processes.Select(p => p.CmdLine.ToString()).ToList());
                return string.Format(Template, dropFileString, executeProcessesString);
            }
        }
        public abstract bool IsCompatible(FileType fileType);
        public abstract string DropFileString(byte[] bytes);
        public abstract string ExecuteProcessString(string cmdline);
        public virtual string ExecuteProcessesString(IEnumerable<string> cmdlines) => string.Join(NewLine, cmdlines.Select(c => ExecuteProcessString(c)).ToList()) + NewLine;
        public virtual byte[] Bytes => Encoding.ASCII.GetBytes(Text);
        protected virtual string EscapeCmdline(string script) => script;
        protected virtual string NewLine { get; } = "\r\n";
    }
}
*/