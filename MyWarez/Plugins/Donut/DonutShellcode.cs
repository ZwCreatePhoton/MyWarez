using System;
using System.IO;

using DonutCS;
using DonutCS.Structs;

using MyWarez.Core;

namespace MyWarez.Plugins.Donut
{
    public abstract class DonutShellcode : IShellcodeX84
    {
        public DonutShellcode(byte[] input, string fileExtension, int bypass = 3, string className = null, string method = null, string args = null, string runtime = null)
        {
            Input = input;
            FileExtension = fileExtension;

            var config = new DonutConfig();
            config.Arch = global::Constants.DONUT_ARCH_X84;
            config.Bypass = bypass;
            if (className != null)
                config.Class = className;
            if (method != null)
                config.Method = method;
            if (args != null)
                config.Args = args;
            if (runtime != null)
                if (runtime != global::Constants.DONUT_RUNTIME_NET2 && runtime != global::Constants.DONUT_RUNTIME_NET4)
                    throw new ArgumentException();
                config.Runtime = runtime;
            Config = config;
        }
        public DonutShellcode(DonutConfig config) => Config = config;
        public byte[] Bytes
        {
            get
            {
                if (CachedBytes == null)
                {
                    using (new TemporaryContext())
                    {
                        var config = Config;
                        config.InputFile = $"input{FileExtension}";
                        config.Payload = "output.bin";
                        File.WriteAllBytes(config.InputFile, Input);
                        int ret = Generator.Donut_Create(ref config);
                        if (ret != global::Constants.DONUT_ERROR_SUCCESS)
                            throw new Exception(Helper.GetError(ret));
                        var shellcode = File.ReadAllBytes(config.Payload + "n"); // Don't know why a "n" gets appended ...
                        CachedBytes = shellcode;
                    }
                }
                return CachedBytes;
            }
        }

        private byte[] CachedBytes = null; 

        private byte[] Input { get; }
        protected string FileExtension { get; }
        private DonutConfig Config { get; }

        public ShellcodeArch Arch => ShellcodeArch.x84;
    }

    public sealed class ExecutableDonutShellcode : DonutShellcode
    {
        public ExecutableDonutShellcode(IExecutable exe, int bypass = global::Constants.DONUT_BYPASS_CONTINUE)
            : base(exe.Bytes, ".exe", bypass: bypass) { }
    }

    public sealed class DynamicLinkLibraryDonutShellcode : DonutShellcode
    {
        public DynamicLinkLibraryDonutShellcode(IDynamicLinkLibrary dll, string function = null, string args = null, int bypass = global::Constants.DONUT_BYPASS_CONTINUE)
            : base(dll.Bytes, ".dll", bypass: bypass, method: function, args: args) { }
    }

    public sealed class DotNetAssemblyDonutShellcode : DonutShellcode
    {
        public DotNetAssemblyDonutShellcode(IDotNetAssembly assembly, string className = null, string method = null, string args = null, string runtime = null, int bypass = global::Constants.DONUT_BYPASS_CONTINUE)
            : base(assembly.Bytes, ".exe", bypass: bypass, className: className, method: method, args: args, runtime: runtime) { }
    }

    public sealed class JavaScriptDonutShellcode : DonutShellcode
    {
        public JavaScriptDonutShellcode(IJavaScript javascript, int bypass = global::Constants.DONUT_BYPASS_CONTINUE)
            : base(javascript.Bytes, ".js", bypass: bypass) { }
    }

    public sealed class VBScriptDonutShellcode : DonutShellcode
    {
        public VBScriptDonutShellcode(IVBScript vbscript, int bypass = global::Constants.DONUT_BYPASS_CONTINUE)
            : base(vbscript.Bytes, ".vbs", bypass: bypass) { }
    }

    public sealed class XslStylesheetDonutShellcode : DonutShellcode
    {
        public XslStylesheetDonutShellcode(IXslStylesheet xslstylesheet, int bypass = global::Constants.DONUT_BYPASS_CONTINUE)
            : base(xslstylesheet.Bytes, ".xsl", bypass: bypass) { }
    }
}
