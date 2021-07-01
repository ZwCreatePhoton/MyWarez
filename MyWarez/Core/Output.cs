using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;


namespace MyWarez.Core
{
    public interface IOutput : IGeneratable
    { }

    public abstract class Output : IOutput
    {
        public abstract void Generate();
    }

    public abstract class FileOutput : Output
    {
        public FileOutput(
            string relativeDirectory // The path on the local filesystem where the files will be stored relative to the output directory
            )
        {
            RootDirectory = Path.Join(Core.Constants.OutputDirectory, relativeDirectory);
        }

        public string RootDirectory { get; }

        public override void Generate()
        {
            foreach (var file in Files)
            {
                var filepath = Path.Join(RootDirectory, file.Item1);
                FileInfo fileInfo = new FileInfo(filepath);
                Directory.CreateDirectory(fileInfo.Directory.FullName);
                File.WriteAllBytes(fileInfo.FullName, file.Item2.ToArray());
            }
        }

        public bool Contains(string relativeFilepath)
        {
            return Files.Any(f => f.Item1 == relativeFilepath);
        }

        public void Add(string relativeFilepath, IEnumerable<byte> data)
        {
            if (Contains(relativeFilepath))
                throw new ArgumentException("Duplicate filepath");
            Files.Add((relativeFilepath, data));
        }

        public void Add(string relativeFilepath, string data)
        {
            Add(relativeFilepath, Encoding.ASCII.GetBytes(data));
        }

        public void Add(string relativeFilepath, IFile file)
        {
            Add(relativeFilepath, file.Bytes);
        }

        private List<(string, IEnumerable<byte>)> Files = new List<(string, IEnumerable<byte>)>();
    }
}
