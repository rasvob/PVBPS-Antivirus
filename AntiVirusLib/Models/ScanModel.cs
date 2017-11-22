using System.Xml.Linq;
using AntiVirusLib.Database;

namespace AntiVirusLib.Models
{
    public class ScanModel
    {
        public string AvName { get; set; }
        public bool Detected { get; set; }
        public string Result { get; set; }
    }
}