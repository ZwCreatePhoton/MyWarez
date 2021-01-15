/*using System;
using System.Collections.Generic;
using System.Text;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{
    public class CovenantMSBuildProject : CovenantPayload
    {
        public CovenantMSBuildProject(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => fileType == Tonsil.Files.FileType.MsbuildProject;
        public override PayloadType Type { get; } = PayloadType.MSBuildProject;
        public override string Name { get; } = "CovenantMSBuildProject";
        public override string Description { get; } = "Covenant implant MSBuild Project file";
        protected override Launcher Launcher { get; } = new MSBuildLauncher();
    }
}*/