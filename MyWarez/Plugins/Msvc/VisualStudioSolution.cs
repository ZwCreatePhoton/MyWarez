using MyWarez.Core;
using System.Collections.Generic;
using System.IO;


namespace MyWarez.Plugins.Msvc
{
    public interface IVisualStudioSolution
    {
        bool BuildPending { get; set; }
        string OutputDirectory { get; set; }
        IEnumerable<string> OutputFiles => Directory.GetFiles(OutputDirectory);
    }

    public interface ISingleOutputVisualStudioSolution : IVisualStudioSolution, IFile
    {
        string OutputFilename { get; }

        byte[] IFile.Bytes => File.ReadAllBytes(System.IO.Path.Join(OutputDirectory, OutputFilename));
    }

    public class VisualStudioSolution : IPayload, IVisualStudioSolution
    {
        public VisualStudioSolution(string solutionDirectory)
        {
            SolutionDirectory = solutionDirectory;
        }

        public PayloadType Type => PayloadType.VisualStudioSolution;

        public string SolutionDirectory { get; }

        public bool BuildPending { get; set; }
        public string OutputDirectory { get; set; }

        // Add compile.bat stuff here
        // All the LPE projects (Visual studio soultions) should implement this since they are too complex for CCxxSource
    }
}
