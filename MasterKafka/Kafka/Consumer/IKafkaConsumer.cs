using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MasterKafka.Kafka.Consumer
{
    public interface IKafkaConsumer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="stoppingToken"></param>
        /// <param name="consumerInstance"></param>
        /// <returns></returns>
        Task StartConsuming(string topic, CancellationToken stoppingToken);

        /// <summary>
        /// Stop all
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        Task StopConsuming(CancellationToken stoppingToken);
    }
}
