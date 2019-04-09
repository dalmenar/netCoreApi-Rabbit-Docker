using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ConsoleCache
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection().AddMemoryCache()
               .BuildServiceProvider();

             // create a service provider from the service collection
             

            // resolve the dependency graph
            var cacheService = services.GetService<IMemoryCache>();



            Console.WriteLine("Hello World!");


            var factory = new ConnectionFactory()
            {
                HostName = "bulldog.rmq.cloudamqp.com",
                UserName = "pcjuohcb",
                Password = "qe7Fe_UP8Ym6DHrzxuzpGdKp9_cHVXuC",
                Port = 1883,
                VirtualHost = "pcjuohcb",
                Endpoint = new AmqpTcpEndpoint(new Uri("amqp://pcjuohcb:qe7Fe_UP8Ym6DHrzxuzpGdKp9_cHVXuC@bulldog.rmq.cloudamqp.com/pcjuohcb"))
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => {
                    Console.WriteLine("Ha llegado bien...");
                    var body = ea.Body;
                    var cantidadStr = Encoding.UTF8.GetString(body);
                    int cantidad;
                    bool canParse =  int.TryParse(cantidadStr,out cantidad);

                    if (canParse)
                    {
                        int micaja = 0;
                        bool isExist = cacheService.TryGetValue("micaja", out micaja);
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(5000));
                        int suma = micaja + cantidad;
                        Console.WriteLine("Ha llegado..." + cantidad);
                        Console.WriteLine("Llevamos..." + suma);
                        cacheService.Set("micaja", suma, cacheEntryOptions);
                    }
                };
                channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

                Console.WriteLine("Escuchando al conejo...");
                while (true) ;

            }
        }

        public static void WaitingMsg()
        {
            

        }
    }
}
