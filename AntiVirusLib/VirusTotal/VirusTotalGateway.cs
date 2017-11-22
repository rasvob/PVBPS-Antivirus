using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using AntiVirusLib.Models;

namespace AntiVirusLib.VirusTotal
{
    public class VirusTotalGateway
    {
        public string ApiKey { get; set; }
        private readonly HttpClient _client;

        private readonly string _reportUrl = @"https://www.virustotal.com/vtapi/v2/file/report?";
        private readonly string _scanUrl = @"https://www.virustotal.com/vtapi/v2/file/scan?";

        public VirusTotalGateway(string apiKey)
        {
            ApiKey = apiKey;
            _client = new HttpClient();
        }

        public async Task<bool> FastScanFile(FileModel model)
        {
            string res = await ReadReport(model.Sha256Hash);
            model.VirusTotalReport.LoadFromJson(res);
            return false;
        }

        public bool DeepScanFile(FileModel model)
        {
            return false;
        }

        private async Task<string> ReadReport(string sha256)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = ApiKey;
            query["resource"] = sha256;
            string queryString = query.ToString();
            Uri requestUri = new Uri(_reportUrl + queryString);
            string res = await _client.GetStringAsync(requestUri);
            return res;
        }
    }
}