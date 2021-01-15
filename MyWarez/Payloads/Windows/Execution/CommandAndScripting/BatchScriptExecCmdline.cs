/*using System;
using System.Collections.Generic;
using System.Text;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{    
    public class ExecCmdlineBatchScript : ExecCmdline
    {
        public ExecCmdlineBatchScript(ProcessList processList) : base(processList) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => false;
        public override PayloadType Type { get; } = PayloadType.Batch;
        public override string Name { get; } = "ExecCmdlineBatchScript";
        public override string Description { get; } = "Batch script that executes cmdlines";
    }   
}*/