using System.Configuration;

namespace PVBPS_Antivirus.Config
{
    public class ConfigGateway
    {
        public string YaraPath => ConfigurationManager.AppSettings["YaraPath"];
        public string IndexRule => ConfigurationManager.AppSettings["YaraPath"];
        public string CustomRule => ConfigurationManager.AppSettings["YaraPath"];
        public string DbPath => ConfigurationManager.AppSettings["YaraPath"];
        public string QuarantinePath => ConfigurationManager.AppSettings["YaraPath"];
        public string ApiKey => ConfigurationManager.AppSettings["ApiKey"];
    }
}