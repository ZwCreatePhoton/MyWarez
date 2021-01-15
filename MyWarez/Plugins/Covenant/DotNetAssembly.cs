using Covenant.Models.Launchers;
using MyWarez.Core;
using MyWarez.Plugins.Covenant;
using System;

namespace MyWarez.Plugins.Covenant
{
    public abstract class CovenantDotNetAssembly : CovenantPayload, IDotNetAssembly
    {
        public CovenantDotNetAssembly(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public PayloadType Type { get; } = PayloadType.DotNetAssembly;
        public virtual bool ImplementsInstaller => false;
        public virtual bool HasEntryPoint => false;
        PortableExecutableType IPortableExecutable.Type => PortableExecutableType.Dotnet;
    }
}

namespace MyWarez.Plugins.Covenant
{
    public class CovenantConsoleDotNetAssembly : CovenantDotNetAssembly
    {
        public CovenantConsoleDotNetAssembly(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override byte[] Bytes
        {
            get
            {
                CovenantImplant.Compile();
                var text = CovenantImplant.Launcher.Base64ILByteString;
                return Convert.FromBase64String(text);
            }
        }
        protected override Launcher Launcher => new BinaryLauncher();
        public override bool HasEntryPoint => true;
    }
}

namespace MyWarez.Base
{
    public class CovenantInstallerDotNetAssembly : CovenantDotNetAssembly
    {
        public CovenantInstallerDotNetAssembly(CovenantImplant covenantImplant) : base(covenantImplant) { }
        public override byte[] Bytes
        {
            get
            {
                CovenantImplant.Compile();
                var text = CovenantImplant.Launcher.Base64ILByteString;
                return Convert.FromBase64String(text);
            }
        }
        protected override Launcher Launcher => new InstallUtilLauncher();
        public override bool ImplementsInstaller => true;
    }
}