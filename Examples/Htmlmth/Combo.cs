namespace Examples
{
    public static class Combo
    {
        private static MyWarez.Core.IAttack Sample1()
        {
            var NAME = "combo-000";
            var HOSTNAME = "Evasion075182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.http.null",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample2()
        {
            var NAME = "combo-011";
            var HOSTNAME = "Evasion076182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.html.entity_encoding_attributes_dec",
                "htmlmth.evasions.html.insert_slash_after_opening_tag_names",
                "htmlmth.evasions.html.bom_declared_utf_16be_encoded_as_utf_16_be",
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