using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Linq;

namespace MyWarez.Base
{
    // Contract: Call a series of parameterless functions
    public interface ISequentialFunctionCall : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "SequentialFunctionCall";
    }

    public class SequentialFunctionCallCCxxSource : CCxxSource, ISequentialFunctionCall
    {
        private static readonly string FunctionNamePlaceholder = "SequentialFunctionCall";

        // Merges only the elements that implement ICCxxSourceIParameterlessCFunction
        public SequentialFunctionCallCCxxSource(IEnumerable<IParameterlessCFunction> functionSources, bool mergeCCxxSources = true)
            : base(MergeSourceFiles(
                SequentialFunctionCallCCxxSource.CreateSource(functionSources),
                functionSources.Where(x => mergeCCxxSources && x is ICCxxSourceIParameterlessCFunction).Cast<ICCxxSource>())
                )
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.Name => ((ISequentialFunctionCall)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource(IEnumerable<IParameterlessCFunction> functions)
        {
            string sourcecode = "";
            foreach (var function in functions)
                sourcecode += function.Signature + "\r\n";
            sourcecode += "void " + "SequentialFunctionCall" + "(void){\r\n";
            foreach (var function in functions)
                sourcecode += function.Name + "();\r\n";
            sourcecode += "}\r\n";
            return new CCxxSource(new List<CCxxSourceFile>() { new CCxxSourceFile(sourcecode, Utils.RandomString(10) + ".c")});
        }
    }
    public class SequentialFunctionCallShellcodeCCxxSource : ShellcodeCCxxSource, ISequentialFunctionCall, IShellcodeCCxxSourceIParameterlessCFunction, IShellcodeParameterlessCFunction
    {
        private static readonly string FunctionNamePlaceholder = "SequentialFunctionCall";

        // Merges only the elements that implement IShellcodeCCxxSourceIParameterlessCFunction
        public SequentialFunctionCallShellcodeCCxxSource(IEnumerable<IShellcodeParameterlessCFunction> functionSources, bool mergeCCxxSources = true)
            : base(MergeSourceFiles(
                SequentialFunctionCallCCxxSource.CreateSource(functionSources),
                functionSources.Where(x => mergeCCxxSources && x is IShellcodeCCxxSourceIParameterlessCFunction).Cast<IShellcodeCCxxSource>())
                )
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }
        string ICFunction.Name => ((ISequentialFunctionCall)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;
    }
}
