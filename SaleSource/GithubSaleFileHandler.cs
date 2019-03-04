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
        public GithubSaleFileHandler(ISalesDataManager salesDataManager) : base(salesDataManager)
        {
        }

        protected async virtual Task<SaleDataDto> GetSalesFromGithub(string url)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "NAlex2004");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", "..token here..");

                    string responseBody = await httpClient.GetStringAsync(url);
                    List<SaleDto> salesFromFile = await Task.Run(() =>
                        {
                            var responseObject = JsonConvert.DeserializeObject<GithubFileResponse>(responseBody);
                            var bytes = Convert.FromBase64String(responseObject.Content);
                            string content = Encoding.UTF8.GetString(bytes);
                            List<SaleDto> sales = JsonConvert.DeserializeObject<List<SaleDto>>(content);
                            return sales;
                        });

                    if (salesFromFile != null && salesFromFile.Count > 0)
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

            if (saleData != null)
            {
                saleData.SourceFileName = fileName;
                bool success = await salesDataManager.AddOrUpdateSaleDataAsync(saleData);
                if (success)
                {
                    await salesDataManager.RemoveErrorAsync(saleData);
                    return;
                }                
            }
            else
            {
                saleData = new SaleDataDto()
                {
                    SourceFileName = fileName
                };
            }
            
            await salesDataManager.AddErrorAsync(saleData);            
        }

        
    }
}
