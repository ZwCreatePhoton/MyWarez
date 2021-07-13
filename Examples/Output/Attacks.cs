using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MyWarez.Base;
using MyWarez.Core;

namespace Output
{
    public static partial class Attacks
    {
        public static void GenerateAll()
        {
            MethodInfo[] methodInfos = typeof(Attacks).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
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