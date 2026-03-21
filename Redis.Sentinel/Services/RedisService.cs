using StackExchange.Redis;
using System.Net;

namespace Redis.Sentinel.Services
{
    public class RedisService
    {
        static ConfigurationOptions sentinelOptions => new()
        {
            EndPoints =
            {
                // Sentinel hostları
                { "localhost", 6383 },
                { "localhost", 6384 },
                { "localhost", 6385 }
            },
            CommandMap = CommandMap.Sentinel, // CommandMap, Redis komutlarının nasıl işleneceğini ve hangi sunuculara yönlendirileceğini tanımlayan bir yapıdır.
            AbortOnConnectFail = false // Bağlantı başarısız olduğunda istisna atılmasını engeller
        };

        static ConfigurationOptions masterOptions => new()
        {
            AbortOnConnectFail = false
        };

        static public async Task<IDatabase> RedisMasterDatabase()
        {
            //Sentinel sunucularına bağlanarak master sunucusunun adresini alıyoruz
            ConnectionMultiplexer sentinelConnection = await ConnectionMultiplexer.SentinelConnectAsync(sentinelOptions);

            EndPoint masterEndPoint = null;

            // Sentinel sunucularını dolaşarak master sunucusunun adresini buluyoruz
            foreach (EndPoint endpoint in sentinelConnection.GetEndPoints())
            {
                IServer server = sentinelConnection.GetServer(endpoint);

                if (!server.IsConnected)
                    continue;

                masterEndPoint = await server.SentinelGetMasterAddressByNameAsync("mymaster"); //sentinal.config dosyasında tanımladığımız master adı
                break;
            }

            //masterEndPoint'de docker container'larının IP adresleri bulunuyor. Bu IP adreslerini localhost:port şeklinde değiştirmemiz gerekiyor.
            var localMasterIP = masterEndPoint.ToString() switch
            {
                "172.18.0.2:6379" => "localhost:6379",
                "172.18.0.3:6379" => "localhost:6380",
                "172.18.0.4:6379" => "localhost:6381",
                "172.18.0.5:6379" => "localhost:6382",
            };

            ConnectionMultiplexer masterConnection = await ConnectionMultiplexer.ConnectAsync(localMasterIP);
            IDatabase database = masterConnection.GetDatabase();

            return database;
        }
    }
}
