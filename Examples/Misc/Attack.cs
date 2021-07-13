using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MyWarez.Base;
using MyWarez.Core;

namespace Misc
{
    public static partial class Attack
    {
        public static void GenerateAll()
        {
            MethodInfo[] methodInfos = typeof(Attack).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var methodInfo in methodInfos)
            {
                if (methodInfo.ReturnType == typeof(IAttack))
                {
                    methodInfo.Invoke(null, null);
                }
            }
        }
    }
}