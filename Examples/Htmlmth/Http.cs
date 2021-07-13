using MyWarez.Core;
using MyWarez.Plugins.Htmlmth;
using System.Collections.Generic;

namespace Examples
{
    public static class Http
    {
        private static MyWarez.Core.IAttack Sample1()
        {
            var NAME = "http-000";
            var HOSTNAME = "Evasion084182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.http.null",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample2()
        {
            var NAME = "http-011";
            var HOSTNAME = "Evasion085182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.http.contentencoding_deflate",
                "htmlmth.evasions.http.encode_deflate_compression_none",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample3()
        {
            var NAME = "http-012";
            var HOSTNAME = "Evasion086182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.http.contentencoding_deflate",
                "htmlmth.evasions.http.encode_deflate_compression_min",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample4()
        {
            var NAME = "http-013";
            var HOSTNAME = "Evasion087182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.http.contentencoding_deflate",
                "htmlmth.evasions.http.encode_deflate_compression_some",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample5()
        {
            var NAME = "http-014";
            var HOSTNAME = "Evasion088182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.http.contentencoding_deflate",
                "htmlmth.evasions.http.encode_deflate_compression_max",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample6()
        {
            var NAME = "http-015";
            var HOSTNAME = "Evasion089182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.http.transferencoding_chunked",
                "htmlmth.evasions.http.encode_chunked_varysize.parameterize(min_chunksize=16, max_chunksize=64)",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }

        private static MyWarez.Core.IAttack Sample7()
        {
            var NAME = "http-016";
            var HOSTNAME = "Evasion090182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.http.transferencoding_chunked",
                "htmlmth.evasions.http.encode_chunked_varysize_leadingzeros.parameterize(min_chunksize=16, max_chunksize=64, leadingzeros=15)",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }

        private static MyWarez.Core.IAttack Sample8()
        {
            var NAME = "http-017";
            var HOSTNAME = "Evasion091182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.http.status_code_4xx.parameterize(statuscode=414)",

            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }

        private static MyWarez.Core.IAttack Sample9()
        {
            var NAME = "http-501";
            var HOSTNAME = "Evasion092182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.http.status_code_4xx.parameterize(statuscode=414)",
                "htmlmth.evasions.http.contentencoding_deflate",
                "htmlmth.evasions.http.encode_deflate_compression_max",
                "htmlmth.evasions.http.transferencoding_chunked",
                "htmlmth.evasions.http.encode_chunked_varysize_leadingzeros.parameterize(min_chunksize=16, max_chunksize=64, leadingzeros=15)",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
    }
}