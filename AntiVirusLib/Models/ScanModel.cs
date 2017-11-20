using System.Xml.Linq;
using AntiVirusLib.Database;

namespace AntiVirusLib.Models
{
    public class ScanModel: IXmlDatabaseProvider
    {
        public string AvName { get; set; }
        public bool Detected { get; set; }
        public string Result { get; set; }

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