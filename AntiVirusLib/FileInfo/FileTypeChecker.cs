using System;
using System.Diagnostics;
using System.IO;
using PeNet;

namespace AntiVirusLib.FileInfo
{
    public class FileTypeChecker
    {
        //TODO: Byte validation
        public bool IsValid(string file)
        {
            try
            {
                bool isPeFile = PeFile.IsPEFile(file);
                return isPeFile;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }
    }
}