using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Core;
using System.Linq;

namespace MyWarez.Base
{
    // Contract: If a conditional expression is true, execute a parameterless function, else execute another parameterless function
    public interface IIfElseFunctionCall : ICCxxSourceIParameterlessCFunction
    {
        public new string Name => "IfElseFunctionCall";
    }

    public class IfElseFunctionCallCCxxSource : CCxxSource, IIfElseFunctionCall
    {
        private static readonly string FunctionNamePlaceholder = "IfElseFunctionCall";

        // Merges only the elements that implement ICCxxSourceIParameterlessCFunction
        public IfElseFunctionCallCCxxSource(
            string conditionalExpression, // format string representing a conditional expression in C: "{0} && (!{1} || {2} == {3})"
            IEnumerable<IParameterlessCFunction> conditionalExpressionFunctionSources, // The arguments for the format string conditionalExpression
            IParameterlessCFunction trueCaseFunction = null, // call this function if the conditional is true
            IParameterlessCFunction falseCaseFunction = null, // call this function if the conditional is false
            bool mergeCCxxSources = true)
            : base(MergeSourceFiles(
                IfElseFunctionCallCCxxSource.CreateSource(conditionalExpression, conditionalExpressionFunctionSources, trueCaseFunction, falseCaseFunction),
                conditionalExpressionFunctionSources.Append(trueCaseFunction).Append(falseCaseFunction).Where(x => mergeCCxxSources && x is ICCxxSourceIParameterlessCFunction).Cast<ICCxxSource>())
                )
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }

        string ICFunction.Name => ((IIfElseFunctionCall)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;

        public static ICCxxSource CreateSource(
            string conditionalExpression,
            IEnumerable<IParameterlessCFunction> conditionalExpressionFunctionSources,
            IParameterlessCFunction trueCaseFunction,
            IParameterlessCFunction falseCaseFunction)
        {
            for (int i =0; i < conditionalExpressionFunctionSources.Count(); i++ )
            {
                if (!conditionalExpression.Contains("{" + i + "}"))
                    throw new ArgumentException("conditionalExpression missing an argument");
            }
            string sourcecode = "";
            sourcecode += "#include <Windows.h>\r\n";
            foreach (var function in conditionalExpressionFunctionSources)
                sourcecode += function.Signature + "\r\n";
            if (trueCaseFunction != null)
                sourcecode += trueCaseFunction.Signature + "\r\n";
            if (falseCaseFunction != null)
                sourcecode += falseCaseFunction.Signature + "\r\n";
            sourcecode += "void " + "IfElseFunctionCall" + "(void){\r\n";
            sourcecode += "if(" + string.Format(conditionalExpression, conditionalExpressionFunctionSources.Select(x => x.Name+"()").ToArray()) + ") {\r\n";
            if (trueCaseFunction != null)
                sourcecode += "\t" + trueCaseFunction.Name + "();" + "\r\n";
            sourcecode += "}\r\n";
            sourcecode += "else {\r\n";
            if (falseCaseFunction != null)
                sourcecode += "\t" + falseCaseFunction.Name + "();" + "\r\n";
            sourcecode += "}\r\n";
            sourcecode += "}\r\n";
            return new CCxxSource(new List<CCxxSourceFile>() { new CCxxSourceFile(sourcecode, Utils.RandomString(10) + ".c")});
        }
    }
    public class IfElseFunctionCallShellcodeCCxxSource : ShellcodeCCxxSource, IIfElseFunctionCall, IShellcodeCCxxSourceIParameterlessCFunction, IShellcodeParameterlessCFunction
    {
        private static readonly string FunctionNamePlaceholder = "IfElseFunctionCall";

        // Merges only the elements that implement IShellcodeCCxxSourceIParameterlessCFunction
        public IfElseFunctionCallShellcodeCCxxSource(
            string conditionalExpression, // format string representing a conditional expression in C: "{0} && (!{1} || {2} == {3})"
            IEnumerable<IShellcodeParameterlessCFunction> conditionalExpressionFunctionSources, // The arguments for the format string conditionalExpression
            IShellcodeParameterlessCFunction trueCaseFunction, // call this function if the conditional is true
            IShellcodeParameterlessCFunction falseCaseFunction, // call this function if the conditional is false
            bool mergeCCxxSources = true)
            : base(MergeSourceFiles(
                IfElseFunctionCallCCxxSource.CreateSource(conditionalExpression, conditionalExpressionFunctionSources, trueCaseFunction, falseCaseFunction),
                conditionalExpressionFunctionSources.Append(trueCaseFunction).Append(falseCaseFunction).Where(x => mergeCCxxSources && x is IShellcodeCCxxSourceIParameterlessCFunction).Cast<IShellcodeCCxxSource>())
                )
        {
            FindAndReplace(SourceFiles, FunctionNamePlaceholder, ((ICFunction)this).Name);
        }
        string ICFunction.Name => ((IIfElseFunctionCall)this).Name + GetHashCode();
        public IEnumerable<string> ParameterTypes => null;
    }
}
