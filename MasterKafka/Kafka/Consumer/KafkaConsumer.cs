using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            // Tạm thời chưa ghi log
            using (var consumer = new ConsumerBuilder<Ignore, string>(_kafkaConfig).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        string messageValue = consumeResult.Message.Value;

                        if (!string.IsNullOrEmpty(messageValue))
                        {
                            await _messageHandler(messageValue);
                            consumer.Commit(consumeResult);
                        }
                        //await Task.Delay(TimeSpan.FromMilliseconds(100));
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

        public Task StopConsuming(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
