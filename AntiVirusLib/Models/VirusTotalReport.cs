using System.Collections.Generic;
using System.Xml.Linq;
using AntiVirusLib.Database;

namespace AntiVirusLib.Models
{
    public class VirusTotalReport: IXmlDatabaseProvider
    {
        public string VirusTotalReportJson { get; set; }
        public string ScanId { get; set; }
        public int Positives { get; set; }
        public int Total { get; set; }
        public string ScanDate { get; set; }
        public IEnumerable<ScanModel> Scans { get; set; }

        public VirusTotalReport()
        {
            Scans = new List<ScanModel>();
        }

        public XElement ToXml()
        {
            throw new System.NotImplementedException();
        }

        public void FromXml(XElement element)
        {
            throw new System.NotImplementedException();
        }
    }
}