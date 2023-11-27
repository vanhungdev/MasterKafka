using System;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.Streams.Kafka.Dsl;
using Akka.Streams.Kafka.Settings;
using Confluent.Kafka;

namespace MasterKafka.AkkaNET
{
    public class AkkaNET
	{
        public void StartConsuming()
        {
            var system = ActorSystem.Create("MySystem");
            var materializer = system.Materializer();

            var consumerSettings = ConsumerSettings<Null, string>.Create(system, null, null)
                .WithBootstrapServers("localhost:9092")
                .WithGroupId("my-group")
                .WithProperty("auto.offset.reset", "earliest");

            var kafkaSource = KafkaConsumer.PlainSource(consumerSettings, Subscriptions.Topics("my-topic"));

            var consoleSink = Sink.ForEach<string>(message => Console.WriteLine(message));

            //kafkaSource.To(consoleSink).Run(materializer);
        }
    }
}

