using Covenant.Models.Launchers;
using MyWarez.Core;
using System.Text;

namespace MyWarez.Plugins.Covenant
{
    public abstract class CovenantPayload : IPayload
    {
        public CovenantPayload(CovenantImplant covenantImplant)
        {
            CovenantImplant = covenantImplant;
            CovenantImplant.SetLauncher(Launcher);
        }
        protected CovenantImplant CovenantImplant { get; }
        public virtual byte[] Bytes
        {
            get
            {
                CovenantImplant.Compile();
                var text = ((DiskLauncher)CovenantImplant.Launcher).DiskCode;
                return Encoding.ASCII.GetBytes(text);
            }
        }

        public virtual string Text => Encoding.ASCII.GetString(Bytes);

        protected abstract Launcher Launcher { get; }
    }
}
