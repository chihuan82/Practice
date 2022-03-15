using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

namespace YoubikeStation
{
    public class YoubikeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _uri;

        public YoubikeService(HttpClient httpClient,string uri)
        {
            this._httpClient = httpClient;
            this._uri = uri;
        }

        public void setTimeout(int timeout)
        {
            this._httpClient.Timeout=TimeSpan.FromSeconds(timeout);
        }

        public async Task<IEnumerable<YoubikeEntity>> GetYoubikeStationAsync(string[] request)
        {
            var taskList = request.Select(target => this.GetYoubikeStationBySNOAsync(target));

            return await Task.WhenAll(taskList);
        }

        public IEnumerable<YoubikeEntity> GetYoubikeStation(string[] request)
        {
            var taskList = request.Select(target => Task.Run(() => { return this.GetYoubikeStationBySNOAsync(target).GetAwaiter().GetResult(); })).ToArray();
            return Task.WhenAll(taskList).GetAwaiter().GetResult();
        }

        public IEnumerable<YoubikeEntity> GetYoubikeStationBlock(string[] request)
        {
            var result = request.Select(target => this.GetYoubikeStationBySNO(target)).ToList();
            return result;
        }

        private async Task<YoubikeEntity> GetYoubikeStationBySNOAsync(string target)
        {

            try
            {
                Console.WriteLine($"sno:{target} GetAsync Start");
                var response = await this._httpClient.GetAsync(this._uri);
                Console.WriteLine($"sno:{target} GetAsync end");
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<YoubikeEntity>>(responseString).FirstOrDefault(x => x.sno == target);

                Console.WriteLine($"sno:{target} {data.sna}");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"sno:{target} fault message:{ex.Message}");
            }

            return null;
        }

        private YoubikeEntity GetYoubikeStationBySNO(string target)
        {
            Console.WriteLine($"sno:{target} GetAsync Start");
            var response =  this._httpClient.GetAsync(this._uri).GetAwaiter().GetResult();
            Console.WriteLine($"sno:{target} GetAsync end");
            var responseString =  response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var data = JsonConvert.DeserializeObject<List<YoubikeEntity>>(responseString).FirstOrDefault(x => x.sno == target);

            Console.WriteLine($"sno:{target} {data.sna}");
            return data;
        }

        public string[] GetStringList() 
        {
            var response = this._httpClient.GetAsync(this._uri).GetAwaiter().GetResult();
            var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<List<YoubikeEntity>>(responseString).Select(x=>x.sno).Distinct().Take(30).ToArray();
        }

    }
}
