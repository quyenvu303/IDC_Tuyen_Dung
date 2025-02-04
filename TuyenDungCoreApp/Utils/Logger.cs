using System.IO;
using System;
using Microsoft.Extensions.Configuration;

namespace TuyenDungCoreApp.Utils
{
    public class Logger
    {
        private static IConfiguration _configuration;
        public static void Configure(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // <summary>
        /// Ghi log lỗi vào file
        /// </summary>
        /// <param name="ex">Exception cần ghi log</param>
        /// <param name="logType">Loại log (Error, Info, Warning, ...)</param>
        /// <returns>Thông tin log dưới dạng object</returns>
        public static object LogError(Exception ex, string logType = "Error")
        {
            try
            {
                // Đường dẫn thư mục chứa file log
                string logDirectory = _configuration["Logging:LogDirectory"];// "D://TuyenDungIDC/logs";

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Đường dẫn file log theo ngày
                string logFilePath = Path.Combine(logDirectory, $"log_{DateTime.Now:yyyy-MM-dd}.txt");

                // Nội dung log
                string logContent = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logType}] - {ex.Message}\n{ex.StackTrace}\n";

                // Ghi log vào file
                System.IO.File.AppendAllText(logFilePath, logContent);

                // Trả về thông tin log dưới dạng object
                return new
                {
                    Error = true,
                    Message = ex.Message,
                    LogType = logType,
                    Timestamp = DateTime.Now
                };
            }
            catch (Exception logEx)
            {
                // Nếu ghi log thất bại, trả về lỗi ghi log
                return new
                {
                    Error = true,
                    Message = "Failed to write log",
                    Exception = logEx.Message
                };
            }
        }
    }
}
