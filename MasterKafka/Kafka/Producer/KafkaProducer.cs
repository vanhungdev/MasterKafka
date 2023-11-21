using Confluent.Kafka;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System;

namespace MasterKafka.Kafka.Producer
{
    public class KafkaProducer : IKafkaProducer
    {
        public async Task<bool> ProducePushMessage(string topic, ProducerConfig config, object objRequest)
        {
            var log = new StringBuilder();
            using var producer = new ProducerBuilder<Null, string>(config).Build();
            try
            {
                var jsonObj = JsonConvert.SerializeObject(objRequest);
                var message = new Message<Null, string> { Value = jsonObj };
                var result = await producer.ProduceAsync(topic, message);

                log.AppendLine($"Input: {jsonObj}");
                log.AppendLine($"Delivered: {JsonConvert.SerializeObject(result.Value)} to: {result.TopicPartitionOffset}");
                return true;
            }
            catch (ProduceException<Null, string> e)
            {
                log.AppendLine($"Delivery failed: {e.Error.Reason}");
                return false;
            }
            finally
            {
                Console.WriteLine(log);
                //LoggingHelper.SetLogStep(log.ToString());
            }
        }
    }
}
