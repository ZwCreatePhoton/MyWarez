namespace MyWarez.Core
{
    public enum ShellcodeArch
    {
        x86,
        x64,
        x84, // Compatiable with both x86 & x64  
        ARM,
        ARM64
    }
    public interface IShellcode
    {
        public byte[] Bytes { get; }
        public ShellcodeArch Arch { get; }
    }

    public interface IShellcodeX86 : IShellcode { }
    public interface IShellcodeX64 : IShellcode { }
    public interface IShellcodeX84 : IShellcodeX86, IShellcodeX64{ }
    public interface IShellcodeARM : IShellcode { }
    public interface IShellcodeARM64 : IShellcode { }

    public interface IShellcodeable { }

    public abstract class Shellcode : IShellcode
    {
        public Shellcode() { }
        public Shellcode(byte[] bytes, ShellcodeArch arch)
        {
            Bytes = bytes;
            Arch = arch;
        }
        public byte[] Bytes { get; }
        public ShellcodeArch Arch { get; }
    }

    public class ShellcodeX86 : Shellcode, IShellcodeX86
    {
        public ShellcodeX86(byte[] bytes) : base(bytes, ShellcodeArch.x86)
        {}
    }

    public class ShellcodeX64 : Shellcode, IShellcodeX64
    {
        public ShellcodeX64(byte[] bytes) : base(bytes, ShellcodeArch.x64)
        { }
    }

    public class ShellcodeX84 : Shellcode, IShellcodeX64
    {
        public ShellcodeX84(byte[] bytes) : base(bytes, ShellcodeArch.x84)
        { }
    }

    public class ShellcodeARM : Shellcode, IShellcodeARM
    {
        public ShellcodeARM(byte[] bytes) : base(bytes, ShellcodeArch.ARM)
        { }
    }

    public class ShellcodeARM64 : Shellcode, IShellcodeARM64
    {
        public ShellcodeARM64(byte[] bytes) : base(bytes, ShellcodeArch.ARM64)
        { }
    }
}
