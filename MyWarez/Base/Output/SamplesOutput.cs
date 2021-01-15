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
        public SamplesOutput()
            : base(Path.Join(Core.Constants.OutputDirectory, "Samples"))
        { }
    }
}
