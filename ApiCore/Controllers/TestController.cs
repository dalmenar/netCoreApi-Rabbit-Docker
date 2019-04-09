using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCore.Controllers
{
    public class TestController :  Controller
    {
        public TestController()
        {

        }
        // GET api/values
        [HttpGet]
        [Route("apiTest/suma/{cantidad}")]
        public bool Suma(int cantidad)
        {
            ConnectionFactory factory = new ConnectionFactory()
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

                var body = Encoding.UTF8.GetBytes(cantidad.ToString());

                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                

            }
            return true;
        }
    }
}
