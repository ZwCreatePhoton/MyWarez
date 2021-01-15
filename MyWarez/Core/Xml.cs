using System;
using System.Collections.Generic;
using System.Text;

namespace MyWarez.Core
{

    public interface IXml : IPlainTextFile
    {

    }

    public class Xml : IXml
    {
        public Xml(string sourceCode)
        {
            Text = sourceCode;
        }
        public PayloadType Type { get; } = PayloadType.Xml;
        public string Text { get; }
    }
}
