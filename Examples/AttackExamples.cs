using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MyWarez.Base;
using MyWarez.Core;

namespace Examples
{
    public static partial class AttackExamples
    {
        public static void GenerateAll()
        {
            MethodInfo[] methodInfos = typeof(AttackExamples).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
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