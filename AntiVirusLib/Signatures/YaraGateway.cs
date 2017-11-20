using System.Collections.Generic;
using System.Diagnostics;

namespace AntiVirusLib.Signatures
{
    public class YaraGateway
    {
        public string YaraPath { get; set; }

        public YaraGateway(string yaraPath)
        {
            YaraPath = yaraPath;
        }

        public IEnumerable<string> ScanFile(string filePath, string yarFilePath)
        {
            string command = filePath + " -w " + yarFilePath + " " + filePath;
            string performScan = PerformScan(command);
            return null;
        }

        private string PerformScan(string payload)
        {
            Process p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = payload
                }
            };
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }
    }
}