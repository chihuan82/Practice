using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YoubikeStation
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var uri = "https://tcgbusfs.blob.core.windows.net/dotapp/youbike/v2/youbike_immediate.json";
            var youbikeService = new YoubikeService(new HttpClient(),uri);
            youbikeService.setTimeout(10);

            var targetList = youbikeService.GetStringList();

            //非執行緒封鎖的方法
            await youbikeService.GetYoubikeStationAsync(targetList);
            //await與GetAwaiter混用
            youbikeService.GetYoubikeStation(targetList);
            //全部給他封鎖起來
            youbikeService.GetYoubikeStationBlock(targetList);

            Console.Read();
        }
    }
}
