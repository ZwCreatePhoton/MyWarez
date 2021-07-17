using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MyWarez.Core
{
    public class WebsiteResource
    {
        public WebsiteResource(string data, string path)
        {
            this.Data = data;
            this.Path = path;
        }

        public string Data { get; }
        public string Path { get; }

        public static IEnumerable<WebsiteResource> SourceDirectoryToWebsiteResources(string sourceDirectory, IEnumerable<string> excludeFiles = null, IEnumerable<WebsiteResource> additionalResources = null)
        {
            excludeFiles ??= new List<string>();
            additionalResources ??= new List<WebsiteResource>();
            List<WebsiteResource> files = new List<WebsiteResource>();
            files.AddRange(Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories).ToList().Where(file => !excludeFiles.Contains(System.IO.Path.GetRelativePath(sourceDirectory, file))).Select((file) => new WebsiteResource(File.ReadAllText(file), System.IO.Path.GetRelativePath(sourceDirectory, file))));
            foreach (var source in additionalResources)
                files.Add(source);
            return files;
        }
    }

    public class Website : IPayload
    {
        public Website(IEnumerable<WebsiteResource> websiteResources, string entrypoint = null)
        {
            Resources = websiteResources;
            Entrypoint = entrypoint;
        }

        public Website(IPlainTextFile file) : this(file.Text) { }

        public Website(string fileContent) : this(new List<WebsiteResource>() { new WebsiteResource(fileContent, "/") }, "/") { }

        public string Entrypoint { get; }

        public IEnumerable<WebsiteResource> Resources { get; }

        public PayloadType Type => PayloadType.Website;
    }

    public class OnePageWebsite : Website, IHtml
    {
        public OnePageWebsite(string content) : base(content) { }

        public string Text => Resources.First().Data;
    }
}
