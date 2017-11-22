﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AntiVirusLib.Database;
using Newtonsoft.Json.Linq;

namespace AntiVirusLib.Models
{
    public class VirusTotalReport
    {
        public string VirusTotalReportJson { get; set; }
        public string ScanId { get; set; }
        public int Positives { get; set; }
        public int Total { get; set; }
        public string ScanDate { get; set; }
        public int ResponseCode { get; set; }
        public List<ScanModel> Scans { get; set; }

        public VirusTotalReport()
        {
            Scans = new List<ScanModel>();
        }

        public void LoadFromJson(string json)
        {
            JObject root = JObject.Parse(json);
            ResponseCode = root["response_code"].Value<int>();

            if (ResponseCode == 1)
            {
                VirusTotalReportJson = json;
                ScanId = root["scan_id"].Value<string>();
                Positives = root["positives"].Value<int>();
                Total = root["total"].Value<int>();
                ScanDate = root["scan_date"].Value<string>();

                var arrSrc = root["scans"].Children();
                List<ScanModel> scanModels = arrSrc.Select(o =>
                {
                    var c = o.Children().FirstOrDefault();

                    var r = new ScanModel()
                    {
                        Detected = c["detected"].Value<bool>(),
                        Result = c["result"].Value<string>(),
                        AvName = o.Path
                       
                    };

                    return r;
                }).ToList();

                Scans.Clear();
                Scans.AddRange(scanModels);
            }
        }
    }
}