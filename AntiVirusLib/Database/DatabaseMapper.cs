using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using AntiVirusLib.Models;

namespace AntiVirusLib.Database
{
    public class DatabaseMapper: IMapper<FileModel>
    {
        public string Source { get; set; }

        public DatabaseMapper(string source)
        {
            Source = source;
        }

        public IEnumerable<FileModel> ListAll()
        {
            XDocument doc = XDocument.Load(Source);
            IEnumerable<XElement> elements = doc.XPathSelectElements("malware/sample");
            IEnumerable<FileModel> res = elements.Select(t =>
            {
                var model = new FileModel();
                model.FromXml(t);
                return model;
            });
            return res;
        }

        public FileModel FindOne(string key)
        {
            XDocument doc = XDocument.Load(Source);
            IEnumerable<XElement> elements = doc.XPathSelectElements("malware/sample");
            XElement xElement = elements.FirstOrDefault(t => t.Element("sha256")?.Value == key);

            if (xElement is null)
            {
                return null;
            }

            FileModel model = new FileModel();
            model.FromXml(xElement);
            return model;
        }

        public void Save(FileModel model)
        {
            XDocument doc = XDocument.Load(Source);
            IEnumerable<XElement> elements = doc.XPathSelectElements("malware/sample");
            XElement xElement = elements.FirstOrDefault(t => t.Element("sha256")?.Value == model.Sha256Hash);

            if (xElement is null)
            {
                xElement = model.ToXml();
                doc.Element("malware")?.Add(xElement);
            }
            else
            {
                if (xElement.Element("name")?.Value is null)
                {
                    xElement.Add(new XElement("name") { Value = model.Name });
                }

                if (xElement.Element("md5")?.Value is null)
                {
                    xElement.Add(new XElement("md5") { Value = model.Md5Hash });
                }

                if (xElement.Element("sha1")?.Value is null)
                {
                    xElement.Add(new XElement("sha1") { Value = model.Sha1Hash });
                }

                if (xElement.Element("sha256")?.Value is null)
                {
                    xElement.Add(new XElement("sha256") { Value = model.Sha256Hash });
                }

                if (xElement.Element("scanned")?.Value is null)
                {
                    xElement.Add(new XElement("scanned") { Value = model.ScanTime.ToString("O") });
                }
            }
            
            doc.Save(Source);
        }
    }
}