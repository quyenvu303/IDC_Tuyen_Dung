using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TuyenDungCoreApp.Services
{
    public static class HttpContextHelper
    {
        public static async Task<string> GetPublicIpAsync()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Truy vấn đến dịch vụ bên thứ ba để lấy IP
                    string publicIp = await httpClient.GetStringAsync("https://api.ipify.org");
                    return publicIp.Trim();
                }
                catch
                {
                    return "Unable to fetch public IP";
                }
            }
        }
    }
}
