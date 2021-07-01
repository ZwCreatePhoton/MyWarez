using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


// Good place for extension methods
// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods

namespace MyWarez.Base
{
    public class SamplesOutput : FileOutput
    {
        public static string ServerOutputDirectoryName = "Samples";

        public SamplesOutput(string baseDirectory="")
            : base(Path.Join(baseDirectory, ServerOutputDirectoryName))
        { }
    }
}
