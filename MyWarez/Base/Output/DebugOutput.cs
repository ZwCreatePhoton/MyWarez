using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace MyWarez.Base
{
    public class DebugOutput : FileOutput
    {
        public static string OutputDirectoryName = "Debug";

        public DebugOutput(string baseDirectory = "")
            : base(Path.Join(baseDirectory, OutputDirectoryName))
        { }
    }
}
