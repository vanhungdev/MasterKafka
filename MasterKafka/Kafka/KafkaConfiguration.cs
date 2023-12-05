using Confluent.Kafka;

namespace MasterKafka.Kafka
{
    public class KafkaConfiguration
    {
        public static ConsumerConfig ConsumerConfig { get; }

        static KafkaConfiguration()
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "34.171.40.194:9092",
                GroupId = "spf-group-0111",
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            ConsumerConfig = consumerConfig;
        }
    }
}
