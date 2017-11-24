using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            try
            {
                string rep = string.Empty;
                for (int j = 0; j < 2; j++)
                {
                    rep = await ReadReport(model.Sha256Hash);

                    if (rep != string.Empty)
                    {
                        break;
                    }

                    await Task.Delay(100);
                }

                model.VirusTotalReport.LoadFromJson(rep);
                return model.VirusTotalReport.ScanId != null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return false;
        }

        public async Task<bool> DeepScanFileSimple(FileModel model)
        {
            try
            {
                string res = await SendFile(model.FilePath);
                string rep = await ReadReport(model.Sha256Hash);

                if (rep == string.Empty)
                {
                    return false;
                }

                model.VirusTotalReport.LoadFromJson(rep);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return model.VirusTotalReport.ScanId != null;
        }

        public async Task<bool> DeepScanFile(FileModel model)
        {
            string res = await SendFile(model.FilePath);

            for (int i = 0; i < 3; i++)
            {
                string rep = await ReadReport(model.Sha256Hash);

                if (rep == string.Empty)
                {
                    continue;
                }

                try
                {
                    model.VirusTotalReport.LoadFromJson(rep);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                if (model.VirusTotalReport.ScanId != null)
                {
                    return true;
                }

                await Task.Delay(300);
            }
            
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

        private async Task<string> SendFile(string file)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["apikey"] = ApiKey;
            string queryString = query.ToString();
            Uri requestUri = new Uri(_scanUrl + queryString);

            using (var stream = File.OpenRead(file))
            {
                var requestContent = new MultipartFormDataContent();
                requestContent.Add(CreateApiPart());
                requestContent.Add(CreateFileContent(stream, file));


                HttpResponseMessage httpResponseMessage = await _client.PostAsync(requestUri, requestContent);
                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }

        private HttpContent CreateApiPart()
        {
            HttpContent content = new StringContent(ApiKey);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"apikey\""
            };

            return content;
        }

        private HttpContent CreateFileContent(Stream stream, string fileName)
        {
            StreamContent fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = "\"" + fileName + "\"",
                Size = stream.Length
            };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return fileContent;
        }
    }
}