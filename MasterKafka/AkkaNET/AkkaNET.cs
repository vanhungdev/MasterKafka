using System;
using System.Threading.Tasks;
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


            int[] numbers = new int[10];

            // Khởi tạo mảng với các giá trị từ 0 đến 9
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = i;
            }

            // Sử dụng Parallel.For để tăng giá trị mỗi phần tử của mảng lên 1
            Parallel.For(0, numbers.Length, i =>
            {
                Console.WriteLine($"Thread {Task.CurrentId} is processing index {i}");
                numbers[i]++;
            });




        }
    }
}

