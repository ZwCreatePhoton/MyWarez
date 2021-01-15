using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Linq;

namespace MyWarez.Base
{
    // Contract: Load a DLL
    public interface ILoadDll : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "LoadDll";
    }
}
