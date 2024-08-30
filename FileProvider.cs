using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EdgarC
{

    public interface IFileProvider
    {
        string GetTextFile(string path);
    }

    public class FileProvider : IFileProvider
    {
        public string GetTextFile(string path)
        {
            string content = null;

            try
            {
                //var wc = new WebClient();
                var url = new Uri(path);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;



                /*
                GET /Archives/edgar/full-index/2024/QTR3/master.idx HTTP/2
                Host: www.sec.gov
                User-Agent: Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:129.0) Gecko/20100101 Firefox/129.0
                Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/png,image/svg+xml,    /*;q=0.8
                Accept-Language: en-US,en;q=0.5
                Accept-Encoding: gzip, deflate, br, zstd
                DNT: 1
                Sec-GPC: 1
                Connection: keep-alive
                Upgrade-Insecure-Requests: 1
                Sec-Fetch-Dest: document
                Sec-Fetch-Mode: navigate
                Sec-Fetch-Site: none
                Sec-Fetch-User: ?1
                Priority: u=0, i
                */





                var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:129.0) Gecko/20100101 Firefox/129.0");
                httpClient.DefaultRequestHeaders.Accept.ParseAdd(" text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/png,image/svg+xml,*/*;q=0.8");
                httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.5");
                httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br, zstd");

                httpClient.DefaultRequestHeaders.Add("DNT", "1");
                httpClient.DefaultRequestHeaders.Add("Sec-GPC", "1");
                httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "none");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                httpClient.DefaultRequestHeaders.Add("Priority", "u=0, i");




                //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");


                //var response = await httpClient.GetAsync("https://www.sec.gov/some-uri");

                var response = httpClient.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    //content = response.Content.ReadAsStringAsync().Result;

                    var contentEncoding = response.Content.Headers.ContentEncoding.ToString();
                    Stream responseStream = response.Content.ReadAsStreamAsync().Result;


                    if (contentEncoding.Contains("gzip"))
                    {
                        using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                        using (var reader = new StreamReader(decompressedStream))
                        {
                            content = reader.ReadToEndAsync().Result;
                        }
                    }
                    else if (contentEncoding.Contains("deflate"))
                    {
                        using (var decompressedStream = new DeflateStream(responseStream, CompressionMode.Decompress))
                        using (var reader = new StreamReader(decompressedStream))
                        {
                            content = reader.ReadToEndAsync().Result;
                        }
                    }
                    else
                    {
                        // If no compression, read the stream directly
                        using (var reader = new StreamReader(responseStream))
                        {
                            content = reader.ReadToEndAsync().Result;
                        }
                    }
                }
                else
                {
                    throw new Exception($"File get failed: {response.StatusCode}: {response.RequestMessage}, {response.ReasonPhrase}");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetFormList: {ex.ToString()}");
            }

            return content;
        }
    }
}