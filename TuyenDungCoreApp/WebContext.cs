using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace TuyenDungCoreApp
{
    public static class WebContext
    {
        private static IHttpContextAccessor m_httpContextAccessor;
        private static IConfigurationSection m_appSettings;

        public static void Configure(IHttpContextAccessor httpContextAccessor, IConfigurationSection appSettings)
        {
            m_httpContextAccessor = httpContextAccessor;
            m_appSettings = appSettings;
        }
        public static string UrlStatic
        {
            get
            {
                var request = m_httpContextAccessor.HttpContext?.Request;

                if (request == null)
                {
                    return string.Empty; // Hoặc trả về giá trị mặc định nếu request không tồn tại
                }

                // Lấy domain từ Request
                var domain = $"{request.Scheme}://{request.Host.Value}";

                return domain;
            }
        }

    }
}
