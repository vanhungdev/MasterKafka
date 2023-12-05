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
            return Ok("No thing. . . ");
        }
    }
}
