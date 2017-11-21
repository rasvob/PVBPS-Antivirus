using System.IO;

namespace AntiVirusLib.FileInfo
{
    public class FileTypeChecker
    {
        public string FileName { get; set; }

        public FileTypeChecker(string fileName)
        {
            FileName = fileName;
        }

        //TODO: Byte validation
        public bool IsValid()
        {
            return SimpleExtCheck();
        }

        private bool SimpleExtCheck()
        {
            string extension = Path.GetExtension(FileName).ToLower();
            return extension == "exe" || extension == "dll";
        }
    }
}