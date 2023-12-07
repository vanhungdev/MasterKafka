using Confluent.Kafka;
using Google.Protobuf.WellKnownTypes;
using MasterKafka.Kafka.Producer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MasterKafka.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KafkaProducerController : Controller
    {
        private readonly IKafkaProducer _messageBroker;
        public KafkaProducerController(IKafkaProducer messageBroker)
        {
            _messageBroker = messageBroker;
        }

        [HttpPost("api/push-message-test")]
        public IActionResult PushMessageTest(KafkaProducer input)
        {
            var config1 = new ProducerConfig
            {
                BootstrapServers = "34.27.189.200:9092"
            };

            Parallel.For(0, input.Topics.Count, i =>
            {
                var topic = input.Topics[i];
                var numMessages = input.TotalMessage;

                for (int j = 1; j <= numMessages; j++)
                {
                    var message = new Message<Null, string>
                    {
                        Value = $"message {j} for {topic}"
                    };

                    // Gọi hàm produce message theo từng topic
                    _messageBroker.ProducePushMessage(topic, config1, message, message.Value);
                }
            });
            return Ok("Đang xử lý. Vui lòng xem console hoặc Kafdrop.");
        }

        [HttpPost("api/TestAsync")]
        public async Task<IActionResult> TestAsync()
        {
            Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} -- start");

            int[] array = new int[1000];

            for (int i = 0; i < 1000; i++)
            {
                array[i] = i + 1;
            }

            Parallel.ForEach(array, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async (item, state) =>
            {
                Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} -- start paraller {item}");
                await Task1(item);
            });

            return Ok("Done.");
        }
        async Task<int> Task1(int value)
        {
            Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} -- start task 1... {value}");
            await Task.Delay(20000);
            Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} -- done task 1! {value}");
            return 1;
        }
        async Task<int> Task2(int value)
        {
            Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} -- start task 2... {value}");
            await Task.Delay(20000);
            Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} -- done task 2! {value}");
            return 1;
        }
    }

    public class KafkaProducer{
        public List<string> Topics { get; set; }
        public int TotalMessage { get; set; }
    }
}
