using TuyenDungCoreApp.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace TuyenDungCoreApp.Services
{
    public class RedisService
    {
        private readonly IDatabase _redisDb;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        // Tăng số lượng thiết bị online
        public async Task IncrementDeviceCountAsync()
        {
            await _redisDb.StringIncrementAsync("online_device_count");
        }

        // Giảm số lượng thiết bị online
        public async Task DecrementDeviceCountAsync()
        {
            await _redisDb.StringDecrementAsync("online_device_count");
        }

        // Lấy số lượng thiết bị online
        public async Task<int> GetOnlineDeviceCountAsync()
        {
            var count = await _redisDb.StringGetAsync("online_device_count");
            return count.IsNullOrEmpty ? 0 : (int)count;
        }
    }
}
