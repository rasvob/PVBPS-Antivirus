using System.Xml.Linq;

namespace AntiVirusLib.Database
{
    public interface IXmlDatabaseProvider
    {
        XElement ToXml();
        void FromXml(XElement element);
    }
}