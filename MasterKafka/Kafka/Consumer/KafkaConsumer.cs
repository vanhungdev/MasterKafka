using Akka.Configuration;
using Akka.Streams.Dsl;
using Akka.Streams.Implementation.Fusing;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Akka.Streams.Kafka.Metadata;
using static Confluent.Kafka.ConfigPropertyNames;

namespace MasterKafka.Kafka.Consumer
{
    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly Func<string, Task> _messageHandler;
        private readonly ConsumerConfig _kafkaConfig;

        public KafkaConsumer(Func<string, Task> messageHandler, ConsumerConfig kafkaConfig)
        {
            _messageHandler = messageHandler;
            _kafkaConfig = kafkaConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public async Task StartConsuming(string topic, CancellationToken stoppingToken)
        {
            // Danh sách các topic cần subscribe
            string[] topics = { topic };

            // Tạo nhiều consumer instance
            List<IConsumer<Ignore, string>> consumers = CreateConsumers(_kafkaConfig, topics, 5);

            Parallel.ForEach(consumers, new ParallelOptions { MaxDegreeOfParallelism = 5 }, consumer =>
            {
                Console.WriteLine($"Start consum partition:");
                ConsumePartition(consumer, stoppingToken, topic);
            });

        }
        void ConsumePartition(IConsumer<Ignore, string> consumer, CancellationToken stoppingToken, string topic)
        {
            //Console.WriteLine($"Message from {partition}:");
            while (true)
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        // Đọc một batch message từ Kafka vào list
                        var batch = ReadMessageBatchFromKafka(consumer);
                        //Console.WriteLine($"Batch {JsonConvert.SerializeObject(batch)}");
                        //Console.WriteLine($"Batch Count {batch.Count()} -- Topic: {topic}");
                        //Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                        //Console.WriteLine($"------------------------------------------");

                        Parallel.ForEach(batch, new ParallelOptions { MaxDegreeOfParallelism = 10 },
                          msg =>
                          {
                              try
                              {
                                  //Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}  || Thread {Thread.CurrentThread.ManagedThreadId} starting processing message: {msg}");
                                  _messageHandler(msg);
                              }
                              catch (Exception ex)
                              {
                                  Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} | Exception handler: {ex.Message} | message: {msg} | Topic {topic}:");
                              }
                          });
                    }
                }
                catch (OperationCanceledException oe)
                {
                    string exceptionMessage = oe.Message;
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="topics"></param>
        /// <returns></returns>
        List<IConsumer<Ignore, string>> CreateConsumers(ConsumerConfig config, string[] topics, int consumerInstance)
        {
            var consumers = new List<IConsumer<Ignore, string>>();
            for (int i = 0; i < consumerInstance; i++)
            {
                var consumer = new ConsumerBuilder<Ignore, string>(config)
                                  .SetErrorHandler((_, e) => { Console.WriteLine($"Error: {e.Reason}"); }).Build();

                consumer.Subscribe(topics);
                consumers.Add(consumer);
            }
            return consumers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        List<string> ReadMessageBatchFromKafka(IConsumer<Ignore, string> consumer)
        {
            const int MAX_MESSAGES = 30;
            List<string> batch = new List<string>();

            for (int i = 0; i < MAX_MESSAGES; i++)
            {
                try
                {
                    // Lấy message và add vào batch
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(10)); // Chờ 3s nếu k có message mới thì trả về null || 100*30 =3000 = 3
                    if (result != null && result.Message != null && !string.IsNullOrEmpty(result.Message.Value))
                    {
                        batch.Add(result.Message.Value);    
                        consumer.Commit(result); // Commit offset
                        var offset = result.Offset;
                        var partition = result.Partition;
                        Console.WriteLine($"Consumer offset: {offset}                || partition: {partition}");
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")} | Method: ReadMessageBatchFromKafka | ConsumeException:  {ex.Message}");
                    break;
                }
            }
            //Console.WriteLine($"Done offset:");
            //Console.WriteLine($"batch offset count:{batch.Count()}");
            return batch;
        }
        public Task StopConsuming(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
