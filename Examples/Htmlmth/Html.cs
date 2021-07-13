using System.Collections.Generic;

namespace Examples
{
    public static class Html
    {
        private static MyWarez.Core.IAttack Sample1()
        {
            var NAME = "html-000";
            var HOSTNAME = "Evasion077182.com";
            var EVASIONS = new[]{
                "htmlmth.evasions.html.null",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample2()
        {
            var NAME = "html-005";
            var HOSTNAME = "Evasion078182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.html.move_body_to_nested_div_with_children.parameterize(N=500, M=500000)",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample3()
        {
            var NAME = "html-009";
            var HOSTNAME = "Evasion079182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.html.insert_slash_after_opening_tag_names",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample4()
        {
            var NAME = "html-016";
            var HOSTNAME = "Evasion080182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.html.entity_encoding_attributes_dec",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample5()
        {
            var NAME = "html-315";
            var HOSTNAME = "Evasion081182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.html.bom_declared_utf_16be_encoded_as_utf_16_be",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
        private static MyWarez.Core.IAttack Sample6()
        {
            var NAME = "html-551";
            var HOSTNAME = "Evasion082182.com";
            var EVASIONS = new[] {
                "htmlmth.evasions.html.entity_encoding_attributes_dec",
                "htmlmth.evasions.html.external_resource_internal_script",
                "htmlmth.evasions.html.insert_slash_after_opening_tag_names",
                "htmlmth.evasions.html.bom_declared_utf_16be_encoded_as_utf_16_be",
            };
            var attack = Evasion.Create(NAME, HOSTNAME, EVASIONS);
            attack.Generate();
            return attack;
        }
    }
}