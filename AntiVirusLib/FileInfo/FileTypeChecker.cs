using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using PeNet;

namespace AntiVirusLib.FileInfo
{
    public class FileTypeChecker
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref Shfileinfo psfi, uint cbFileInfo, uint uFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct Shfileinfo
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const int Exetype = 0x00002000;

        public enum FileType
        {
            Invalid, Dos, Win32Console, WindowsExe
        }

        //TODO: Byte validation
        public bool IsValid(string file)
        {
            return ShFileInfoValidation(file);
        }

        private bool ShFileInfoValidation(string file)
        {
            IntPtr ptr = IntPtr.Zero;
            Shfileinfo info = new Shfileinfo();
            ptr = SHGetFileInfo(file, 128, ref info, (uint) Marshal.SizeOf(info), Exetype);

            int intParam = ptr.ToInt32();
            int loWord = intParam & 0xffff;
            int hiWord = intParam >> 16;

            FileType type = FileType.Invalid;

            if (intParam == 0)
            {
                return IsValidDll(file);
            }

            switch (hiWord)
            {
                case 0x0000 when loWord == 0x5a4d:
                    type = FileType.Dos;
                    break;
                case 0x0000 when loWord == 0x4550:
                    type = FileType.Win32Console;
                    break;
                default:
                    if ((hiWord != 0x0000) && (loWord == 0x454E || loWord == 0x4550 || loWord == 0x454C))
                    {
                        type = FileType.WindowsExe;
                    }
                    break;
            }

            return type != FileType.Invalid;
        }

        private bool IsValidDll(string file)
        {
            byte[] bytes = File.ReadAllBytes(file);

            try
            {
                if (bytes[0] != (byte)'M' || bytes[1] != (byte)'Z')
                {
                    return false;
                }

                byte[] elf_newArr = bytes.Where((b, i) => i >= 60 && i < 60 + 4).ToArray();
                Int32 elf_new = BitConverter.ToInt32(elf_newArr, 0);

                byte[] pe = bytes.Where((b, i) => i >= elf_new && i < elf_new + 4).ToArray();
                byte[] peCheck = new[] {(byte) 'P', (byte) 'E', (byte) 0, (byte) 0};

                bool validPe = true;

                for (int i = 0; i < pe.Length; i++)
                {
                    if (pe[i] != peCheck[i])
                    {
                        validPe = false;
                    }
                }

                if (!validPe)
                {
                    return false;
                }

                byte[] characteristcs = bytes.Where((b, i) => i >= elf_new + 22 && i < elf_new + 24).ToArray();
                int character = BitConverter.ToInt16(characteristcs, 0) & 0x2000;

                return character == 0x2000;
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.WriteLine(e.Message);
            }

            return false;
        }
    }
}