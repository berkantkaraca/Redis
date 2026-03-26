using StackExchange.Redis;

// Aynı Redis sunucusuna bağlanılır.
ConnectionMultiplexer connection = await ConnectionMultiplexer.ConnectAsync("localhost:1453");

// Dinleme işlemi için arayüz alınır.
ISubscriber subscriber = connection.GetSubscriber();

// "mychannel" kanalına abone olunur. 
// Kanal her mesaj aldığında lambda fonksiyonu tetiklenir ve ekrana yazdırır.
await subscriber.SubscribeAsync("mychannel", (channel, message) =>
{
    Console.WriteLine($"Gelen Mesaj: {message}");
});

Console.Read();
