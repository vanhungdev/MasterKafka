using Confluent.Kafka;

namespace MasterKafka.Kafka
{
    public class KafkaConfiguration
    {
        public static ConsumerConfig ConsumerConfig { get; }
        public static readonly string Topic = "events1";

        static KafkaConfiguration()
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "34.171.40.194:9092",
                GroupId = null,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            ConsumerConfig = consumerConfig;
        }
    }
}
