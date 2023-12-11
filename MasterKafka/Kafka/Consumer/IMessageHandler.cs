using System.Threading.Tasks;
using System;
using System.Threading;

namespace MasterKafka.Kafka.Consumer
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Handle message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task<bool> HandleMessage(string message, string topic, CancellationToken cancellationToken);

        /// <summary>
        /// Handle message if success | never throw exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task<bool> HandleSuccess(string message, string topic, CancellationToken cancellationToken);

        /// <summary>
        /// Handle message faild | never throw exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task<bool> HandleFailure(string message, string topic, Exception exception, CancellationToken cancellationToken);
    }
}
