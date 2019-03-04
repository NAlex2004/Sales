using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.Storage.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections;
using Sales.Storage.DTO;
using Sales.SaleSource.Github;
using System.IO;

namespace Sales.SaleSource
{
    public class GithubSaleFileHandler : SaleFileHandlerBase
    {
        protected string token;

        public GithubSaleFileHandler(ISalesDataManager salesDataManager, string token) : base(salesDataManager)
        {
            this.token = token;
        }

        protected async virtual Task<SaleDataDto> GetSalesFromGithub(string url)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

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
                        return null;
                    }

                    bool IsAllDataCorrect = salesFromFile.Count > 0 && !salesFromFile.Any(s => s == null || !s.HasValidData());

                    if (IsAllDataCorrect)
                    {                        
                        SaleDataDto saleData = new SaleDataDto()
                        {
                            Sales = salesFromFile
                        };

                        return saleData;
                    }

                }
                catch (Exception e)
                {
                }

                return null;
            }
        }

        public override async Task HandleSaleFileAsync(string location)
        {
            SaleDataDto saleData = await GetSalesFromGithub(location);
            Uri uri = new Uri(location);
            string fileName = Path.GetFileName(uri.LocalPath);
            SaleManagementResult result;

            if (saleData != null)
            {
                saleData.SourceFileName = fileName;
                result = await salesDataManager.AddOrUpdateSaleDataAsync(saleData);
                if (result.Succeeded)
                {
                    await salesDataManager.RemoveErrorAsync(new SaleManagementResult() { FileName = saleData.SourceFileName });
                    return;
                }                
            }
            else
            {
                result = new SaleManagementResult()
                {
                    Succeeded = false,
                    ErrorMessage = "[HandleSaleFileAsync]: saleData is null"
                };
            }

            result.FileName = fileName;

            await salesDataManager.AddErrorAsync(result);            
        }

        
    }
}
