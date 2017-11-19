using System;
using System.Collections.Generic;
using System.Data;

namespace AntiVirusLib.Models
{
    public class FileModel
    {
        public string Name { get; set; } = "Unknown";
        public string Md5Hash { get; set; }
        public string Sha1Hash { get; set; }
        public string Sha256Hash { get; set; }
        public List<string> MatchedSignatures { get; set; }
        public DateTime ScanTime { get; set; }
        public string VirusTotalReportJson { get; set; }
    }
}