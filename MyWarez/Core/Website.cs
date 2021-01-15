using System.Collections.Generic;
using System.Linq;

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
