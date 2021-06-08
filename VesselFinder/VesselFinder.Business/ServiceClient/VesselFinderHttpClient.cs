using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HtmlAgilityPack;
using VesselFinder.Business.Mapper;
using VesselFinder.Business.Models;

namespace VesselFinder.Business.ServiceClient
{
    public interface IVesselFinderHttpClient
    { 
        Task<VesselDetails> GetVesselDetailsAsync(string imo);
    }
    
    public class VesselFinderHttpClient : IVesselFinderHttpClient
    {
        private const string Uri = "https://www.vesselfinder.com";
        
        private readonly IDescriptionAttributeMapper<VesselDetails> _mapper;

        public VesselFinderHttpClient(IDescriptionAttributeMapper<VesselDetails> mapper)
        {
            _mapper = mapper;
        }
        private VesselDetails Map(Dictionary<string, string> items)
        {
            var vesselDetails = new VesselDetails();
            
            items.ToList().ForEach(item => _mapper.Map(vesselDetails, item.Key, item.Value));

            return vesselDetails;
        }
        
        private static async Task<string> GetInternalAsync(string uri)
        {
            using var client = new HttpClient();
            
            CreateHeader(client);

            return await DecryptResponse(await client.GetAsync(uri));
        }

        private static async Task<string> DecryptResponse(HttpResponseMessage response)
        {
            await using var brotliStream = new Brotli.BrotliStream(await response.Content.ReadAsStreamAsync(),
                CompressionMode.Decompress, true);
            var streamReader = new StreamReader(brotliStream);
            return await streamReader.ReadToEndAsync();
        }

        private static void CreateHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.Add("Accept-Encoding", "br");
            client.DefaultRequestHeaders.Add("user-agent", "PostmanRuntime/7.28");
            client.DefaultRequestHeaders.ConnectionClose = false;
        }
        
        private static async Task<HtmlAttribute> GetHomePageAsync(string imo)
        {
            var uri = $"{Uri}/vessels?name={imo}";
            var response = await GetInternalAsync(uri);
            var htmlDocument = new HtmlDocument();
            
            htmlDocument.LoadHtml(response);
            var links = htmlDocument.DocumentNode.SelectNodes("//a[contains(concat(' ', @class, ' '), 'ship-link')]");

            return links.FirstOrDefault()?.Attributes["href"];
        }

        private async Task<VesselDetails> GetDetailPageAsync(HtmlAttribute nextLink)
        {
            var dictionary = new Dictionary<string, string>();
            var response = await GetInternalAsync($"{Uri}{nextLink?.Value}");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);
            var links = htmlDocument.DocumentNode.SelectNodes("//table[contains(concat(' ', @class, ' '), 'tparams')]//tr");

            try
            {
                foreach (var link in links)
                {
                    var previous = string.Empty;
                    foreach (var node in link.SelectNodes("//td"))
                    {
                        var key = node.InnerText;
                        if (string.IsNullOrWhiteSpace(key) || !node.HasAttributes) continue;
                        if (node.Attributes["class"].Value.StartsWith("n") && !dictionary.ContainsKey(key))
                        {
                            dictionary.Add(key, "-");
                            previous = key;
                        }

                        if (node.Attributes["class"].Value.StartsWith("v"))
                        {
                            dictionary[previous] = node.InnerText;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Map(dictionary);
        }

        public async Task<VesselDetails> GetVesselDetailsAsync(string imo)
        {
            var nextPage = await GetHomePageAsync(imo);
            var vesselDetails = await GetDetailPageAsync(nextPage);

            return vesselDetails;
        }
    }
}