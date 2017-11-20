using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using AntiVirusLib.Database;
using AntiVirusLib.Signatures;

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
        public DateTime ScanTime { get; set; }
        public bool IsClean { get; set; } = true;
        public VirusTotalReport VirusTotalReport { get; set; }

        public XElement ToXml()
        {
            XElement ele = new XElement("malware");

            XElement name = new XElement("name") {Value = Name};
            XElement md5 = new XElement("md5") {Value = Md5Hash};
            XElement sha1 = new XElement("md5") {Value = Sha1Hash};
            XElement sha256 = new XElement("md5") {Value = Sha256Hash};
            XElement scanTime = new XElement("scanned") {Value = ScanTime.ToString("O")};
            XElement clean = new XElement("clean") {Value = IsClean.ToString()};

            ele.Add(name);
            ele.Add(md5);
            ele.Add(sha1);
            ele.Add(sha256);
            ele.Add(sha256);
            ele.Add(scanTime);
            ele.Add(clean);

            if (VirusTotalReport.ScanId != null)
            {
                XElement xVirus = VirusTotalReport.ToXml();
                ele.Add(xVirus);
            }

            if (MatchedSignatures.Any())
            {
                XElement xVirus = new XElement("yara");

                IEnumerable<XElement> elements = MatchedSignatures.Select(t => new XElement("name") { Value = t });
                xVirus.Add(elements);

                ele.Add(xVirus);
            }
            
            return ele;
        }

        public FileModel()
        {
            MatchedSignatures = new List<string>();
            VirusTotalReport = new VirusTotalReport();
        }

        public void FromXml(XElement element)
        {
            throw new NotImplementedException();
        }

        public void ComputeHashes()
        {
            var hash = new HashCreator();
            Md5Hash = hash.CreateMd5Hash(FilePath);
            Sha1Hash = hash.CreateSha1Hash(FilePath);
            Sha256Hash = hash.CreateSha256Hash(FilePath);
        }

        public void RefreshModel(FileModel model)
        {
            Name = model.Name;

            if (model.MatchedSignatures.Any())
            {
                MatchedSignatures.Clear();
                MatchedSignatures.AddRange(model.MatchedSignatures);
            }
            
            ScanTime = model.ScanTime;
            IsClean = model.IsClean;

            if (VirusTotalReport.ScanId != null)
            {
                VirusTotalReport = model.VirusTotalReport;
            }
        }
    }
}