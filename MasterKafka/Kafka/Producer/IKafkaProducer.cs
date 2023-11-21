using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterKafka.Kafka.Producer
{
    public interface IKafkaProducer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="objRequest"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task<bool> ProducePushMessage(string topic, ProducerConfig config, object objRequest);
    }
}
