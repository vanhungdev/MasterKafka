using Confluent.Kafka;
using MasterKafka.Kafka;
using MasterKafka.Kafka.Producer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterKafka.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {



        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IKafkaProducer _messageBroker;
        public WeatherForecastController(IKafkaProducer messageBroker)
        {
            _messageBroker = messageBroker;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var config1 = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };

            Task task1 = Task.Run(async () =>
            {
                for (int i = 0; i < 5000; i++)
                {
                    var messageBroker1 = await _messageBroker.ProducePushMessage("events1", config1, new Message<Null, string> { Value = $" topic: events1 --- message: {i}" });
                }
            });

            Task task2 = Task.Run(async () =>
            {
                for (int j = 0; j < 5000; j++)
                {
                    var messageBroker2 = await _messageBroker.ProducePushMessage("events2", config1, new Message<Null, string> { Value = $" topic: events2 --- message: {j}" });
                }
            });

            await Task.WhenAll(task1, task2);


            return Ok("In process. . . ");
        }
    }
}
