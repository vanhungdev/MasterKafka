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
                BootstrapServers = "localhost:9092",
                GroupId = "spf-group-01",
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            ConsumerConfig = consumerConfig;
        }
    }
}
