using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Linq;

namespace MyWarez.Base
{
    public class CCxxSourceParameterlessCFunction : CCxxSource, ICCxxSourceIParameterlessCFunction
    {
        public CCxxSourceParameterlessCFunction(ICCxxSource source, string functionName, IEnumerable<string> parameterTypes = null)
            : base(source)
        {
            FunctionName = functionName;
            ParameterTypeList = parameterTypes ?? new List<string>();
        }


        public string FunctionName { get; }
        public IEnumerable<string> ParameterTypeList { get; }
        string ICFunction.Name => FunctionName;

        IEnumerable<string> ICFunction.ParameterTypes => ParameterTypeList;
    }
}
