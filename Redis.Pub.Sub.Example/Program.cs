using StackExchange.Redis;

// Redis sunucusuna bağlantı açılır.
ConnectionMultiplexer connection = await ConnectionMultiplexer.ConnectAsync("localhost:1453");

// Pub/Sub işlemlerini yönetmek için 'Subscriber' arayüzü alınır.
ISubscriber subscriber = connection.GetSubscriber();

while (true)
{
    Console.Write("Mesaj : ");
    string message = Console.ReadLine();

    // "mychannel" isimli kanala girilen mesajı asenkron olarak fırlatır.
    await subscriber.PublishAsync("mychannel", message);
}
