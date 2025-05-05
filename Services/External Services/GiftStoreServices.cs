using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Services.Dtos;

namespace Services.External_Services
{
    public class GiftStoreServices
    {
        public async Task<List<GiftDto>> GetAllGiftsItems()
        {
            const string url = "https://api.restful-api.dev/objects";
            Log.Information("Conectando a {Url}", url);

            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<GiftDto>>(body)!;
        }
    }
}
