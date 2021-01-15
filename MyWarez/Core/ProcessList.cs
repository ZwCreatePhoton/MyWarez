using System.Collections.Generic;

namespace MyWarez.Core
{
    public interface IProcessList
    {
        public IEnumerable<Tonsil.Processes.Process> Processes { get; }
    }

    public class ProcessList : IPayload, IProcessList
    {
        public ProcessList(IEnumerable<Tonsil.Processes.Process> processes)
        {
            Processes = processes;
        }

        public PayloadType Type { get; } = PayloadType.ProcessList;
        public virtual IEnumerable<Tonsil.Processes.Process> Processes { get; }
    }
}
