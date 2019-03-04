using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sales.SaleSource.Github;
using Sales.Storage.DTO;

namespace Sales.SaleSource
{
    public class GithubSaleDataSource : ISaleDataSource
    {
        string url;
        string token;

        public GithubSaleDataSource(string url, string githubRepoToken)
        {
            this.url = url;
            token = githubRepoToken;
        }

        public async Task<SaleDataDto> GetSaleDataAsync()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Uri uri = new Uri(url);
            string fileName = Path.GetFileName(uri.LocalPath);
            SaleDataDto saleData = new SaleDataDto()
            {                
                SourceFileName = fileName
            };

            Debug.WriteLine($"[GetSaleDataAsync]: {fileName}");
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "NAlex2004");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);

                    string responseBody = await httpClient.GetStringAsync(url);
                    List<SaleDto> salesFromFile = await Task.Run(() =>
                    {
                        var responseObject = JsonConvert.DeserializeObject<GithubFileResponse>(responseBody);
                        var bytes = Convert.FromBase64String(responseObject.Content);
                        string content = Encoding.UTF8.GetString(bytes);
                        List<SaleDto> sales = JsonConvert.DeserializeObject<List<SaleDto>>(content);
                        return sales;
                    });

                    // If we'd like to take any correct data from file, skipping incorrect
                    //salesFromFile = salesFromFile.Where(s => s != null && s.HasValidData()).Select(s => s).ToList();

                    // Let's assume, all data must be correct
                    if (salesFromFile == null)
                    {
                        return saleData;
                    }

                    bool IsAllDataCorrect = salesFromFile.Count > 0 && !salesFromFile.Any(s => s == null || !s.HasValidData());

                    if (IsAllDataCorrect)
                    {
                        Debug.WriteLine($"[GetSaleDataAsync]: CORRECT {fileName}");
                        saleData.Sales = salesFromFile;
                        return saleData;
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine($"[GetSaleDataAsync]: ERROR: {e.Message}");
                }

                return saleData;
            }
        }
    }
}
