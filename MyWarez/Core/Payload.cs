namespace MyWarez.Core
{
    public interface IPayload
    {
        public PayloadType Type => PayloadType.UNKNOWN;
    }

    public enum PayloadType
    {
        UNKNOWN,
        DotNetAssembly,
        MSBuildProject,
        PowerShellScript,
        HtmlApplication,
        ScriptComponent,
        XslStylesheet,
        JavaScript,
        VBScript,
        VbaMacro,
        ProcessList,
        Elf,
        Executable,
        DynamicLinkLibrary,
        Html,
        Sylk,
        Xlm,
        Csv,
        ExcelWorksheet,
        Shellcode,
        Batch,
        Cpp,
        Rtf,
        Text,
        Yasm,
        Website,
        HtmlmthWebsite,
        StaticLibrary,
        CCxxSource,
        VisualStudioSolution,
        Xml,
        CSharp
    }
}
