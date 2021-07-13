using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MyWarez.Base;
using MyWarez.Core;

namespace Examples
{
    public static partial class Attacks
    {
        public static void GenerateAll()
        {
            List<MethodInfo> methodInfos = new List<MethodInfo>();
            methodInfos.AddRange(typeof(Http).GetMethods(BindingFlags.NonPublic | BindingFlags.Static));
            methodInfos.AddRange(typeof(Html).GetMethods(BindingFlags.NonPublic | BindingFlags.Static));
            methodInfos.AddRange(typeof(Combo).GetMethods(BindingFlags.NonPublic | BindingFlags.Static));
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