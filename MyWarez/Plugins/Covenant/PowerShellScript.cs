/*using System;
using System.Collections.Generic;
using System.Text;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{
    public class CovenantPowerShellScript : CovenantPayload
    {
        public CovenantPowerShellScript(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => fileType == Tonsil.Files.FileType.PowershellScript;
        public override PayloadType Type { get; } = PayloadType.PowerShellScript;
        public override string Name { get; } = "CovenantPowerShellScript";
        public override string Description { get; } = "Covenant implant PowerShell script file";
        public override byte[] Bytes
        {
            get
            {
                CovenantImplant.Compile();
                var text = ((PowerShellLauncher)CovenantImplant.Launcher).PowerShellCode;
                return Encoding.ASCII.GetBytes(text);
            }
        }
        protected override Launcher Launcher { get; } = new PowerShellLauncher();
    }
}*/