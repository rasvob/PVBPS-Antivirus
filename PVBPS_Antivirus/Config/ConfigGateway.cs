using System.Configuration;

namespace PVBPS_Antivirus.Config
{
    public class ConfigGateway
    {
        public string YaraPath => ConfigurationManager.AppSettings["YaraPath"];
        public string IndexRule => ConfigurationManager.AppSettings["IndexRule"];
        public string CustomRule => ConfigurationManager.AppSettings["CustomRule"];
        public string DbPath => ConfigurationManager.AppSettings["DbPath"];
        public string QuarantinePath => ConfigurationManager.AppSettings["QuarantinePath"];
        public string ApiKey => ConfigurationManager.AppSettings["ApiKey"];
        public string StringsPath => ConfigurationManager.AppSettings["StringsPath"];
    }
}