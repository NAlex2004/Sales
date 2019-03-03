using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Converters;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Playground
{
    public class HookResponse
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Download_Url { get; set; }
        public string Content { get; set; }
    }

    class Program
    {
        // не забыть хрень в конфиге!
        static void GetFileContent_Example()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            string url = "https://api.github.com/repos/NAlex2004/SalesData/contents/Manager_1/AlNaz_18121980.json";

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json,text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");

                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                    //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "NAlex2004");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", "..token here..");

                    string responseBody = httpClient.GetStringAsync(url).GetAwaiter().GetResult();
                    
                    Console.WriteLine(responseBody);
                    var responseObject = JsonConvert.DeserializeObject<HookResponse>(responseBody);
                    var bytes = Convert.FromBase64String(responseObject.Content);
                    // содержимое файла. все еще в json, но там уже дело за малым
                    string content = Encoding.UTF8.GetString(bytes);
                    Console.WriteLine();
                    Console.WriteLine(content);
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nException Caught!");
                    while (e != null)
                    {
                        Console.WriteLine("Message :{0} ", e.Message);
                        Console.WriteLine();
                        e = e.InnerException;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            GetFileContent_Example();
            Console.ReadKey();
        }
    }
}
