using Confluent.Kafka;
using MasterKafka.Kafka.Producer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
                BootstrapServers = "34.171.40.194:9092"
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
    }

    public class KafkaProducer{
        public List<string> Topics { get; set; }
        public int TotalMessage { get; set; }
    }
}
