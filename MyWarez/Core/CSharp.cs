using System.Text.RegularExpressions;

namespace MyWarez.Core
{
    public interface ICSharp : IPlainTextFile
    {
    }

    public class CSharp : IPayload, ICSharp
    {
        public CSharp(string sourceCode)
        {
            Text = sourceCode;
        }
        public PayloadType Type { get; } = PayloadType.CSharp;
        public string Text { get; private set; }
        public void StripComments()
        {
            var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
            Text = Regex.Replace(Text, re, "$1");
        }
    }
}