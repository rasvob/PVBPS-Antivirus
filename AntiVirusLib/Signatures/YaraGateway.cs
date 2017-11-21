using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            string performScan = PerformScan(filePath, yarFilePath);
            var lines = performScan.Split('\n').Where(t => t.Any()).Select(t => t.Replace("\r", "")).Select(t =>
            {
                string[] split = t.Split(' ');
                return split[0];
            });
            return lines;
        }

        private string PerformScan(string payload, string yarFile)
        {
            Process p = new Process
            {
                StartInfo =
                {
                    FileName = YaraPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = $"-w {yarFile} {payload}",
                    CreateNoWindow = true
                }
            };

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }
    }
}