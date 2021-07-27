using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyWarez.Core;


namespace MyWarez.Plugins.Htmlmth
{
    public class HtmlmthBaseline
    {
        public HtmlmthBaseline(string host, string path, string filepath, byte[] bytes)
        {
            Host = host;
            Path = path;
            Filepath = filepath;
            Bytes = bytes;
        }
        public string Host { get; set; }
        public string Path { get; set; }
        public string Filepath { get; set; }
        public byte[] Bytes { get; set; }
    }

    public class HtmlmthCase
    {
        public HtmlmthCase(string host, string path, string casename, IEnumerable<string> caseImplementations)
        {
            Host = host;
            Path = path;
            Casename = casename;
            CaseImplementations = caseImplementations;
        }
        public string Host { get; set; }
        public string Path { get; set; }
        public string Casename { get; set; }
        public IEnumerable<string> CaseImplementations { get; set; }
    }

    public class HtmlmthWebsite : IPayload
    {
        public HtmlmthWebsite(WebsiteResource websiteResources, string host = null, IEnumerable<string> evasions = null)
            : this(new[] { websiteResources }, host, evasions)
        { }

        public HtmlmthWebsite(IEnumerable<WebsiteResource> websiteResources, string host=null, IEnumerable<string> evasions = null)
            : this(new Website(websiteResources), host, evasions)
        { }

        public HtmlmthWebsite(Website website, string host = null, IEnumerable<string> evasions = null)
        {
            Baselines = WebsiteResourcesToBaselines(website.Resources, host);
            SetEvasions(evasions);
        }

        private static HtmlmthBaseline WebsiteResourceToBaseline(WebsiteResource resource, string host = null)
        {
            string ext = Path.GetExtension(resource.Path);
            if (ext == "") ext = ".html";
            HtmlmthBaseline baseline = new HtmlmthBaseline(host, resource.Path, Utils.RandomString(10) + ext, resource.Bytes);
            return baseline;
        }

        private static List<HtmlmthBaseline> WebsiteResourcesToBaselines(IEnumerable<WebsiteResource> resources, string host = null)
        {
            List<HtmlmthBaseline> baselines = new List<HtmlmthBaseline>();
            foreach (var resource in resources)
                baselines.Add(WebsiteResourceToBaseline(resource, host));
            return baselines;
        }

        public List<HtmlmthBaseline> Baselines = new List<HtmlmthBaseline>();

        public List<HtmlmthCase> Cases = new List<HtmlmthCase>();

        public void SetEvasions(IEnumerable<string> evasions)
        {
            Cases.Clear();
            foreach (var baseline in Baselines)
                SetEvasions(baseline, evasions);
        }

        public void SetEvasions(HtmlmthBaseline baseline, IEnumerable<string> evasions)
        {
            var c = new HtmlmthCase(baseline.Host, baseline.Path, Utils.RandomString(10), evasions);
            Cases.Add(c);
        }


        public void AddResource(WebsiteResource resource, string host = null, IEnumerable<string> evasions = null)
        {
            HtmlmthBaseline baseline = WebsiteResourceToBaseline(resource, host);
            Baselines.Add(baseline);
            if (evasions != null)
                SetEvasions(baseline, evasions);
        }

        public PayloadType Type => PayloadType.HtmlmthWebsite;
    }
}
