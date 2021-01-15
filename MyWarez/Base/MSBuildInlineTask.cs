using MyWarez.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MyWarez.Base
{
    // TODO: RoslynCodeTaskFactoryInlineTask (https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-roslyncodetaskfactory?view=vs-2019)

    public class MSBuildCodeTaskFactoryInlineTask : MSBuildProject
    {
        private static readonly string ResourceDirectory = Path.Join(Core.Constants.BaseResourceDirectory, nameof(MSBuildCodeTaskFactoryInlineTask));
        private static readonly string ResourceFilePath = Path.Join(ResourceDirectory, "HelloWorld.csproj");
        private static readonly string CSharpPlaceholder = "//CODEHERE";
        private static readonly string MethodFullNamePlaceholder = "SomeNamespace.SomeClass.SomeMethod";

        public MSBuildCodeTaskFactoryInlineTask(ICSharp cSharp, string staticMethodFullName) // fully qualified name for a static method defined and implemented in cSharp
            : base(CreateSource(cSharp, staticMethodFullName))
        { }

        public static string CreateSource(ICSharp cSharp, string staticMethodFullName)
        {
            var csProj = File.ReadAllText(ResourceFilePath);
            csProj = csProj.Replace(CSharpPlaceholder, cSharp.Text);
            csProj = csProj.Replace(MethodFullNamePlaceholder, staticMethodFullName);
            return csProj;
        }
    }
}
