using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AntiVirusLib.Signatures
{
    public class StringsGateway
    {
        private readonly string _exePath;

        public StringsGateway(string exePath)
        {
            _exePath = exePath;
        }

        public IEnumerable<string> ScanFile(string filePath)
        {
            string performScan = PerformScan(filePath);
            var lines = performScan.Split('\n').Where(t => t.Any()).Select(t => t.Replace("\r", ""));
            return lines;
        }

        private string PerformScan(string payload)
        {
            Process p = new Process
            {
                StartInfo =
                {
                    FileName = _exePath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = $"-nobanner \"{payload}\"",
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