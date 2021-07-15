using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MyWarez.Base
{
    public class WsgiDAVServerOutput : HttpServerOutput
    {
        public WsgiDAVServerOutput(Host host, int port=80, string name="WsgiDAV", string baseDirectory = "", bool ssl=false, byte[] crt=null, byte[] key=null, bool dirBrowser=false)
            : base(host, port, name, baseDirectory: baseDirectory)
        {
            DirBrowser = dirBrowser;
            SSL = ssl;
            if (ssl && (crt is null || key is null))
                throw new ArgumentException("Certificate and key must not be null");
            Crt = crt;
            Key = key;
            Add(Path.Join("..", "launch.sh"), LaunchScriptSh);
            Add(Path.Join("..", "config.yaml"), Config);
        }

        private bool DirBrowser { get; }
        private bool SSL { get; }
        private byte[] Crt { get; }
        private byte[] Key { get; }

        private string LaunchScriptSh {
            get => $@"
SCRIPT_DIR=""$(cd ""$( dirname ""${{BASH_SOURCE[0]}}"" )"" &> /dev/null && pwd )""
(cd ""$SCRIPT_DIR""; exec wsgidav -c config.yaml)
".Replace("\r\n", "\n");
        }

        private string Config
        {
            get
            {
                var header = @"
# WsgiDAV configuration file
";
                var serverConfig = $@"
# ============================================================================
# SERVER OPTIONS

server: ""cheroot""

host: 0.0.0.0
port: {Port}

# ============================================================================
";
                var sslConfig = $@"
# ----------------------------------------------------------------------------
# SSL Support

{(SSL ? "":"#")}ssl_certificate: ""./localhost.crt""
{(SSL ? "" : "#")}ssl_private_key: ""./localhost.key""
{(SSL ? "" : "#")}ssl_certificate_chain: null

# ----------------------------------------------------------------------------
";
                var sharesConfig = $@"
# ==============================================================================
# SHARES



#: Application root, applied before provider mapping shares,
#: e.g. <mount_path>/<share_name>/<res_path>
mount_path: null

provider_mapping:
    ""/"": ""wwwroot""

# ==============================================================================
";
                var authenticationConfig = $@"
# ==============================================================================
# AUTHENTICATION
http_authenticator:
    #: Allow basic authentication
    accept_basic: true
    #: Allow digest authentication
    accept_digest: true
    #: true (default digest) or false (default basic)
    default_to_digest: true
    #: Header field that will be accepted as authorized user.
    #: Including quotes, for example: trusted_auth_header = ""REMOTE_USER""
    trusted_auth_header: null
    #: Domain controller that is used to resolve realms and authorization.
    #: Default null: which uses SimpleDomainController and the
    #: `simple_dc.user_mapping` option below.
    #: (See http://wsgidav.readthedocs.io/en/latest/user_guide_configure.html
    #: for details.)
    domain_controller: null
    # domain_controller: wsgidav.dc.simple_dc.SimpleDomainController
    # domain_controller: wsgidav.dc.pam_dc.PAMDomainController
    # domain_controller: wsgidav.dc.nt_dc.NTDomainController


# Additional options for SimpleDomainController only:
simple_dc:
    # Access control per share.
    # These routes must match the provider mapping.
    # NOTE: Provider routes without a matching entry here, are inaccessible.
    user_mapping:
        # default (used for all shares that are not explicitly listed)
        ""*"": true


# Additional options for NTDomainController only:
nt_dc:
    preset_domain: null
    preset_server: null

# Additional options for PAMDomainController only:
pam_dc:
    service: ""login""
    encoding: ""utf-8""
    resetcreds: true


# ==============================================================================
";
                var dirBrowserConfig = $@"
# ----------------------------------------------------------------------------
# WsgiDavDirBrowser

dir_browser:
    enable: {DirBrowser}
    #: List of fnmatch patterns that will be hidden in the directory listing
    ignore:
        - "".DS_Store""  # macOS folder meta data
        - ""Thumbs.db""  # Windows image previews
        - ""._*""  # macOS hidden data files
    #: Display WsgiDAV icon in header
    icon: true
    #: Raw HTML code, appended as footer (true: use a default trailer)
    response_trailer: true

# ----------------------------------------------------------------------------
";

                var config = "";
                config += header;
                config += serverConfig;

                config += sslConfig;
                if (SSL)
                {
                    Add(Path.Join("..", "localhost.crt"), Crt);
                    Add(Path.Join("..", "localhost.key"), Key);
                }
                
                config += sharesConfig;
                config += authenticationConfig;
                config += dirBrowserConfig;

                config = config.Replace("\r\n", "\n");
                return config;
            }
        }

    }
}
