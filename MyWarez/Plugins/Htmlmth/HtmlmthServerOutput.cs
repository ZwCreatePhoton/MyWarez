using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using MyWarez.Core;
using MyWarez.Base;


namespace MyWarez.Plugins.Htmlmth
{
    public class HtmlmthServerOutput : Output, IServerOutput
    {
        private static readonly string HtmlmthLibDirectory = Path.Join(Core.Constants.PluginsResourceDirectory, "Htmlmth", "htmlmth");

        public HtmlmthServerOutput(Host host, int port=80, string name="HTMLMTH_Server",
            string scriptEncodingServerHost = null, // Needed for evasions.html.encoded_script
            int? scriptEncodingServerPort = null // Needed for evasions.html.encoded_script
            )
        {
            Host = host;
            Port = port;
            Name = name;

            // Make sure a Windows server is running scripting_encoder_server.py
            ScriptEncodingServerHost = scriptEncodingServerHost;
            ScriptEncodingServerPort = scriptEncodingServerPort;
        }
        public Host Host { get; }
        public int Port { get; }
        public string Name { get; }
        public string ScriptEncodingServerHost { get; }
        public int? ScriptEncodingServerPort { get; }
        public string HtmlmthServerDirectory => Path.Join(Core.Constants.OutputDirectory, IServerOutput.ServerOutputDirectoryName, Host.HostId, Port.ToString() + "_" + Name);

        public void Add(HtmlmthWebsite website)
        {
            HtmlmthWebsites.Add(website);
        }

        private List<HtmlmthWebsite> HtmlmthWebsites = new List<HtmlmthWebsite>();

        // TODO: Obtain a mutex lock if MultiThreading is ever implemented...
        public override void Generate()
        {
            if (HtmlmthWebsites.Count == 0)
                return;

            string randID = Utils.RandomString(10);

            // copy htmlmth repo
            if (!Directory.Exists(Path.Join(HtmlmthServerDirectory, "htmlmth")))
            {
                Directory.CreateDirectory(Path.Join(HtmlmthServerDirectory, "htmlmth"));
                Utils.CopyFilesRecursively(HtmlmthLibDirectory, Path.Join(HtmlmthServerDirectory, "htmlmth"));
            }

            // Save the baseline files to disk
            var mainBaselineDir = Path.Join(HtmlmthServerDirectory, "baselines");
            var myBaselineDir = Path.Join(mainBaselineDir, randID);
            Directory.CreateDirectory(myBaselineDir);
            foreach (var baseline in Baselines)
                File.WriteAllText(Path.Join(myBaselineDir, baseline.Filepath), baseline.Data);

            // Save the baseline yaml to disk
            var myBaselineYamlPath = Path.Join(myBaselineDir, "baseline.yaml");
            File.WriteAllText(myBaselineYamlPath, BaselineYaml);

            // Append main baseline yaml on disk
            var mainBaselineYamlPath = Path.Join(mainBaselineDir, "baseline.yaml");
            if (!File.Exists(mainBaselineYamlPath))
                File.WriteAllText(mainBaselineYamlPath, "include:\r\n");
            File.AppendAllText(mainBaselineYamlPath, "  - \"" + randID + Path.DirectorySeparatorChar.ToString().Replace(@"\", @"\\") + "baseline.yaml" + "\"\r\n");

            // Save the case yaml to disk
            var mainCaseDir = Path.Join(HtmlmthServerDirectory, "cases");
            var myCaseDir = Path.Join(mainCaseDir, randID);
            Directory.CreateDirectory(myCaseDir);
            var myCaseYamlPath = Path.Join(myCaseDir, "case.yaml");
            File.WriteAllText(myCaseYamlPath, CaseYaml);

            // Append main case yaml on disk
            var mainCaseYamlPath = Path.Join(mainCaseDir, "case.yaml");
            if (!File.Exists(mainCaseYamlPath))
                File.WriteAllText(mainCaseYamlPath, "include:\n");
            File.AppendAllText(mainCaseYamlPath, "  - \"" + randID + Path.DirectorySeparatorChar.ToString().Replace(@"\", @"\\") + "case.yaml" + "\"\r\n");

            // Save / edit the case python script on disk
            var casePythonScriptPath = Path.Join(mainCaseDir, "case.py");
            if (!File.Exists(casePythonScriptPath))
            {
                File.WriteAllText(casePythonScriptPath, CasePythonCodeLeftTemplate);
                File.AppendAllText(casePythonScriptPath, CasePythonCodeRightTemplate);
            }
            var oldcases = File.ReadAllText(casePythonScriptPath).Replace(CasePythonCodeLeftTemplate, "").Replace(CasePythonCodeRightTemplate, "");

            var newcases = string.Join("", Cases.Select((c) => "    " + CasePythonString(c) + "\r\n"));
            File.WriteAllText(casePythonScriptPath, CasePythonCodeLeftTemplate + oldcases + newcases + CasePythonCodeRightTemplate);

            // Save the install & run scripts to disk
            var installBashScriptPath = Path.Join(HtmlmthServerDirectory, "install.sh");
            var runBashScriptPath = Path.Join(HtmlmthServerDirectory, "run.sh");
            if (!File.Exists(installBashScriptPath))
                File.WriteAllText(installBashScriptPath, installBashScript);
            if (!File.Exists(runBashScriptPath))
            {
                var bashScript = runBashScript;
                bashScript.Replace("80", Port.ToString());
                if (ScriptEncodingServerHost != null && ScriptEncodingServerPort.HasValue)
                {
                    bashScript.Replace("5000", ScriptEncodingServerPort.Value.ToString());
                    bashScript.Replace("127.0.0.1", ScriptEncodingServerHost.ToString());
                    var scriptingEncoderServerHost = Host.GetHostByHostName(ScriptEncodingServerHost) ?? new Host(ScriptEncodingServerHost, ScriptEncodingServerHost, null);
                    var scriptingEncoderServerOutput = new RemoteFileServerOutput(scriptingEncoderServerHost, ScriptEncodingServerPort.Value, "ScriptingEncoder_Server");
                    var scriptingEncoderServerScriptPath = Path.Join(HtmlmthLibDirectory, "scripting_encoder_server.py");
                    var scriptingEncoderServerScriptBytes = File.ReadAllBytes(scriptingEncoderServerScriptPath);
                    scriptingEncoderServerOutput.Add("scripting_encoder_server.py", scriptingEncoderServerScriptBytes);
                    scriptingEncoderServerOutput.Generate();
                }
                File.WriteAllText(runBashScriptPath, bashScript);
            }
        }

        private string CasePythonString(HtmlmthCase htmlmthCase)
        {
            if ((ScriptEncodingServerHost == null || ScriptEncodingServerPort == null) && htmlmthCase.CaseImplementations.Any(x => x.ToLower().Contains("evasions.html.encoded_script")))
                throw new Exception("ScriptingEncodingServerHost and ScriptingEncodingServerPort must be set if the evasion 'evasions.html.encoded_script' is used");
            return string.Format(PythonStringTemplate, htmlmthCase.Casename, string.Join(", ", htmlmthCase.CaseImplementations));
        }
        private static string PythonStringTemplate = "cases.append(TransformFunction(\"{0}\", None, {1}))";

        private static string installBashScript = @"
python2 -m pip install --ignore-installed -r htmlmth/requirements.txt
bash htmlmth/submodules/codetransform/setup.sh
".Replace("\r\n", "\n");

        private static string runBashScript = @"
export PYTHONPATH=""${PYTHONPATH}:..""
cd htmlmth
python2 EvasionHTTPServer.py -i 0.0.0.0 -p 80 -ipv 4 -sesh 127.0.0.1 -sesp 5000 -b ../baselines/baseline.yaml -c ../cases/case.py -tc ../cases/case.yaml
cd ..
".Replace("\r\n", "\n");

        private List<HtmlmthBaseline> Baselines
        {
            get
            {
                var baselines = new List<HtmlmthBaseline>();
                foreach (var htmlmthWebsite in HtmlmthWebsites)
                    baselines.AddRange(htmlmthWebsite.Baselines);
                return baselines;
            }
        }

        private List<HtmlmthCase> Cases
        {
            get
            {
                var cases = new List<HtmlmthCase>();
                foreach (var htmlmthWebsite in HtmlmthWebsites)
                    cases.AddRange(htmlmthWebsite.Cases);
                return cases;
            }
        }

        // TODO: Check for collisions
        private string BaselineYaml
        {
            get
            {
                string yaml = "";
                yaml += "baselines:\n";
                foreach (var baseline in Baselines)
                {
                    string bl = "  -\n";
                    if (baseline.Host != null)
                    {
                        bl += "    host: \"" + baseline.Host + "\"\n";
                    }
                    if (baseline.Path != null)
                    {
                        bl += "    path: \"" + baseline.Path + "\"\n";
                    }
                    if (baseline.Filepath != null)
                    {
                        bl += "    filepath: \"" + baseline.Filepath.Replace(@"\", @"\\") + "\"\n";
                    }
                    yaml += bl;
                }
                return yaml;
            }
        }

        private string CaseYaml
        {
            get
            {
                string yaml = "";
                yaml += "baselines:\n";
                foreach (var _case in Cases)
                {
                    string bl = "  -\n";
                    if (_case.Host != null)
                    {
                        bl += "    host: \"" + _case.Host + "\"\n";
                    }
                    if (_case.Path != null)
                    {
                        bl += "    path: \"" + _case.Path + "\"\n";
                    }
                    if (_case.Casename != null)
                    {
                        bl += "    casename: \"" + _case.Casename + "\"\n";
                    }
                    yaml += bl;
                }
                return yaml;
            }
        }

        private static string CasePythonCodeLeftTemplate = @"
from collections import OrderedDict

from htmlmth.utils import TransformFunction
import htmlmth.evasions.html
import htmlmth.evasions.http


# returns an OrderedDict of cases with (casenames, case) as items
def get_cases(long_descriptions=False):
    cases = []

    # A case is defined as an instance of TransformFunction.
    # Arguments are (casename, description, ...) where ... are instances of TransformFunction (predefined in package: htmlmth.evasions)
";

        private static string CasePythonCodeRightTemplate = @"    

    simple_index = len(cases)

    # description cleanup
    if not long_descriptions:
        TransformFunction.cleanup_descriptions(cases, simple_index)

    return OrderedDict([(c.name, c) for c in cases])

";
    }
}
