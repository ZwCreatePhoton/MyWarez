using System;
using System.IO;

namespace MyWarez.Core
{
    public static class Constants
    {
        public static readonly string WorkingDirectory = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
        public static readonly string TempDirectory = WorkingDirectory + "Temp" + Path.DirectorySeparatorChar;
        public static readonly string ResourceDirectory = WorkingDirectory + "Resources" + Path.DirectorySeparatorChar;
        public static readonly string CoreResourceDirectory = ResourceDirectory + "Core" + Path.DirectorySeparatorChar;
        public static readonly string BaseResourceDirectory = ResourceDirectory + "Base" + Path.DirectorySeparatorChar;
        public static readonly string PluginsResourceDirectory = ResourceDirectory + "Plugins" + Path.DirectorySeparatorChar;
        public static readonly string PayloadsResourceDirectory = ResourceDirectory + "Payloads" + Path.DirectorySeparatorChar;
        public static readonly string OutputDirectory = WorkingDirectory + "Output" + Path.DirectorySeparatorChar;
    }
}
