using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MasterKafka.Kafka.Consumer
{
    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly Func<string, Task> _messageHandler;
        private readonly ConsumerConfig _kafkaConfig;
        private readonly int _instance;

        public KafkaConsumer(Func<string, Task> messageHandler, ConsumerConfig kafkaConfig, int instance)
        {
            _messageHandler = messageHandler;
            _kafkaConfig = kafkaConfig;
            _instance = instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public Task StartConsuming(string topic, CancellationToken stoppingToken)
        {
            List<IConsumer<Ignore, string>> consumers = CreateInstanceConsumers(_kafkaConfig, topic, _instance);

            int threadCount = 0;
            Parallel.ForEach(consumers, new ParallelOptions { MaxDegreeOfParallelism = _instance }, consumer =>
            {
                int threadId = Interlocked.Increment(ref threadCount);
                Console.WriteLine($"Start thread #{threadId}");
                ConsumePartition(consumer, stoppingToken, topic, threadId);
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="stoppingToken"></param>
        /// <param name="topic"></param>
        void ConsumePartition(IConsumer<Ignore, string> consumer, CancellationToken stoppingToken, string topic, int threadId)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // Đọc một batch message từ Kafka vào list
                    var batch = ReadMessageBatchFromKafka(consumer, threadId);
                    //Console.WriteLine($"Batch {JsonConvert.SerializeObject(batch)}");
                    //Console.WriteLine($"Batch Count {batch.Count()} -- Topic: {topic}");
                    //Console.WriteLine($"DateTime: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
                    //Console.WriteLine($"------------------------------------------");

                    Parallel.ForEach(batch, new ParallelOptions { MaxDegreeOfParallelism = 10 },
                      msg =>
                      {
                          try
                          {
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="topics"></param>
        /// <returns></returns>
        List<IConsumer<Ignore, string>> CreateInstanceConsumers(ConsumerConfig config, string topics, int consumerInstance)
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
        List<string> ReadMessageBatchFromKafka(IConsumer<Ignore, string> consumer, int threadId)
        {
            const int MAX_MESSAGES = 30;
            List<string> batch = new List<string>();

            for (int i = 0; i < MAX_MESSAGES; i++)
            {
                try
                {
                    // Lấy message và add vào batch
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(10)); // Chờ 3s nếu k có message mới thì trả về null || 100*30 =3000 = 0,3
                    if (result != null && result.Message != null && !string.IsNullOrEmpty(result.Message.Value))
                    {
                        batch.Add(result.Message.Value);    
                        consumer.Commit(result); // Commit offset
                        var offset = result.Offset;
                        var partition = result.Partition;
                        Console.WriteLine($"ThreadId: {threadId } || Consumer offset: {offset} || partition: {partition} -- {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}");
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
