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
        Task HandleMessage(string message, string topic, CancellationToken cancellationToken);

        /// <summary>
        /// Handle message if success
        /// </summary>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task HandleSuccess(string message, string topic);

        /// <summary>
        /// Handle message faild
        /// </summary>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task HandleFailure(string message, string topic, Exception exception);
    }
}
