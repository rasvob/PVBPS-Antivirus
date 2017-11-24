using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using AntiVirusLib.Database;
using AntiVirusLib.Signatures;
using PeNet;

namespace AntiVirusLib.Models
{
    public class FileModel: IXmlDatabaseProvider
    {
        public string Name { get; set; } = "Unknown";
        public string FilePath { get; set; }
        public string Md5Hash { get; set; }
        public string Sha1Hash { get; set; }
        public string Sha256Hash { get; set; }
        public List<string> MatchedSignatures { get; set; }
        public List<string> Urls { get; set; }
        public DateTime ScanTime { get; set; }
        public bool IsClean { get; set; } = true;
        public VirusTotalReport VirusTotalReport { get; set; }
        public FileVersionInfo FileVersionInfo { get; set; }
        public string FileSize { get; set; }

        public XElement ToXml()
        {
            XElement ele = new XElement("sample");
            XElement name = new XElement("name") {Value = Name};
            XElement md5 = new XElement("md5") {Value = Md5Hash};
            XElement sha1 = new XElement("sha1") {Value = Sha1Hash};
            XElement sha256 = new XElement("sha256") {Value = Sha256Hash};
            XElement scanTime = new XElement("scanned") {Value = ScanTime.ToString("O")};

            ele.Add(name);
            ele.Add(md5);
            ele.Add(sha1);
            ele.Add(sha256);
            ele.Add(scanTime);

            return ele;
        }

        public FileModel()
        {
            MatchedSignatures = new List<string>();
            Urls = new List<string>();
            VirusTotalReport = new VirusTotalReport();
        }

        public void FromXml(XElement element)
        {
            string name = element.Element("name")?.Value;
            string scanned = element.Element("scanned")?.Value;
            string sha1 = element.Element("sha1")?.Value;
            string sha256 = element.Element("sha256")?.Value;
            string md5 = element.Element("md5")?.Value;

            if (name != null)
            {
                Name = name;
            }

            if (scanned != null)
            {
                ScanTime = DateTime.Parse(scanned);
            }

            if (sha1 != null)
            {
                Sha1Hash = sha1;
            }

            if (sha256 != null)
            {
                Sha256Hash = sha256;
            }

            if (md5 != null)
            {
                Md5Hash = md5;
            }

            IsClean = false;
        }

        public void ComputeHashes()
        {
            var hash = new HashCreator();
            Md5Hash = hash.CreateMd5Hash(FilePath);
            Sha1Hash = hash.CreateSha1Hash(FilePath);
            Sha256Hash = hash.CreateSha256Hash(FilePath);
        }

        public void CreateFileInfo()
        {
            FileVersionInfo = FileVersionInfo.GetVersionInfo(FilePath);
            System.IO.FileInfo info = new System.IO.FileInfo(FilePath);
            long length = info.Length;
            FileSize = $"{(length / (Math.Pow(2, 20))):F3} MiB";
        }

        public void RefreshModel(FileModel model)
        {
            Name = model.Name;
            ScanTime = model.ScanTime;
            IsClean = model.IsClean;
        }
    }
}