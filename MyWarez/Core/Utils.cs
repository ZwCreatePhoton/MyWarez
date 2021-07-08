using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MyWarez.Core
{
    // Can implement some of these as Extension Methods

    public static class Utils
    {
        private readonly static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void RandomizeFilePath(Tonsil.Files.FilePath filePath)
        {
            string dirSepChar;
            if (filePath.FilePathType == Tonsil.Files.FilePathType.SMB)
                dirSepChar = @"\";
            else
                dirSepChar = "/";
            filePath.Directory = dirSepChar + RandomString(5) + dirSepChar;
            FileInfo fi = new FileInfo(filePath.Filename);
            filePath.Filename = RandomString(5) + fi.Extension;
            if (filePath.FilePathType == Tonsil.Files.FilePathType.SMB)
                filePath.Directory = filePath.Directory.Replace("/", @"/");
        }

        public static byte[] XorEncryptDecrypt(byte[] input, string key)
        {
            var output = new byte[input.Length];

            for (int c = 0; c < input.Length; c++)
                output[c] =  (byte)(input[c] ^ (byte)key[c % key.Length]);

            return output;
        }

        public static byte[] ShiftEncrypt(byte[] input, byte key)
        {
            var output = new byte[input.Length];

            for (int c = 0; c < input.Length; c++)
                output[c] = (byte)((byte)(input[c]) - (byte)(key));

            return output;
        }

        public static byte[] ShiftDecrypt(byte[] input, byte key)
        {
            var output = new byte[input.Length];

            for (int c = 0; c < input.Length; c++)
                output[c] = (byte)((byte)(input[c]) + (byte)key);

            return output;
        }

        public static IEnumerable<Tonsil.Files.File> GetAllFilesRead(Tonsil.Processes.Process process, bool remoteOnly = true)
        {
            var filesRead = new List<Tonsil.Files.File>();

            foreach (var file in process.FilesRead)
            {
                if (!remoteOnly || typeof(Tonsil.Files.RemoteFilePath).IsInstanceOfType(file.FilePath))
                {
                    filesRead.Add(file);
                }
            }

            foreach (var childProcess in process.ChildProcesses)
            {
                var files = GetAllFilesRead(childProcess);
                foreach (var file in files)
                {
                    if (!remoteOnly || typeof(Tonsil.Files.RemoteFilePath).IsInstanceOfType(file.FilePath))
                    {
                        filesRead.Add(file);
                    }
                }
            }

            return filesRead;
        }
        public static IEnumerable<Tonsil.Files.File> GetAllFilesRead(IEnumerable<Tonsil.Processes.Process> processes, bool remoteOnly = true)
        {
            var filesRead = new List<Tonsil.Files.File>();

            foreach (var process in processes)
            {
                var files = GetAllFilesRead(process, remoteOnly);
                foreach (var file in files)
                {
                    filesRead.Add(file);
                }
            }

            return filesRead;
        }
        public static void ClearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Couldn't delete " + fi.FullName);
                }
            }
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }
        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
        public static void CopyFilesRecursively(string source, string target)
        {
            CopyFilesRecursively(new DirectoryInfo(source), new DirectoryInfo(target));
        }
        public static string ByteArrayToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
        // TODO: Abstract away shellcode encoding so that plugins can extend the available encoders
        //          and so that the MyWarez.Core namespace does not depend on msfvenom
        private static byte[] Msfvenom(byte[] shellcode, ShellcodeArch arch, string format, byte[] badChars = null)
        {
            string archString = arch.ToString();
            var inputFilename = RandomString(5) + ".bin";
            var outputFilename = inputFilename + ".encoded";
            var encodeCommandFormatStr = "cat {0} | msfvenom --arch {1} --platform windows -p - -f {2} -o {3}";
            var encodeCommand = string.Format(encodeCommandFormatStr, inputFilename, archString, format, outputFilename);
            if (badChars != null)
            {
                var badCharString = @"\x" + BitConverter.ToString(badChars).Replace("-", @"\x");
                encodeCommand += " -b '" + badCharString + "'";
            }
            var cwd = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Constants.TempDirectory);
            File.WriteAllBytes(inputFilename, shellcode);
            var proc = Process.Start("powershell", string.Format(@"-c ""{0}""", encodeCommand));
            proc.WaitForExit();
            var outputBytes = File.ReadAllBytes(outputFilename);
            Directory.SetCurrentDirectory(cwd);
            return outputBytes;
        }
        public static byte[] EncodeShellcode(byte[] shellcode, ShellcodeArch arch, byte[] badChars = null)
        {
            return Msfvenom(shellcode, arch, "raw", badChars);
        }
        public static string TransformShellcode(byte[] shellcode, ShellcodeArch arch, string format)
        {
            return Encoding.ASCII.GetString(Msfvenom(shellcode, arch, format));
        }

        public static string StringToCArrary(string s, bool wide = false)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            foreach (char c in s)
            {
                if (wide)
                    builder.Append("L");
                builder.Append("'");
                switch (c)
                {
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
                builder.Append("'");
                builder.Append(",");
            }
            builder.Append("0");
            builder.Append("}");
            return builder.ToString();
        }

        public static string BytesToCArray(byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            foreach (var b in data)
                builder.Append(((int)b).ToString() + ",");
            if (data.Length > 0)
                builder.Length--;
            builder.Append("}");
            return builder.ToString();
        }
        public static string BytesToJavaScriptArray(byte[] data)
        {
            return "[" + string.Join(",", data.Select(b => ((int)b).ToString()).ToList()) + "]";
        }

        // Move to Host class ?
        public static void InitHosts(string hostsYaml)
        {
            // Setup the input
            var input = new StringReader(hostsYaml);

            // Load the stream
            var yaml = new YamlStream();
            yaml.Load(input);

            // Examine the stream
            var mapping =
                (YamlMappingNode)yaml.Documents[0].RootNode;

            var items = (YamlSequenceNode)mapping.Children[new YamlScalarNode("hosts")];
            foreach (YamlMappingNode item in items)
            {
                new Host(
                    item.Children[new YamlScalarNode("id")].ToString(),
                    item.Children[new YamlScalarNode("hostname")].ToString(),
                    item.Children[new YamlScalarNode("ip")].ToString()
                    );
            }
        }
    }
}
