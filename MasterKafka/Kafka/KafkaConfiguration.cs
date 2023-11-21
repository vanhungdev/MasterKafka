using Confluent.Kafka;

namespace MasterKafka.Kafka
{
    public class KafkaConfiguration
    {
        public static ConsumerConfig Config { get; }
        public static readonly string Topic = "events1";

        static KafkaConfiguration()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "spf-group-01",
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            Config = config;
        }
    }
}
