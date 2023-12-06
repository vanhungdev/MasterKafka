using System.Threading.Tasks;
using System;
using System.Threading;

namespace MasterKafka.Kafka.Consumer
{
    public interface IMessageHandler
    {
        /// <summary>
        /// hàm xử lý message nếu thất bại
        /// </summary>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task HandleFailure(string message, string topic, Exception exception);

        /// <summary>
        /// Hàm xử lý message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task HandleMessage(string message, string topic);

        /// <summary>
        /// Hàm xử lý nếu message thành công
        /// </summary>
        /// <param name="message"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task HandleSuccess(string message, string topic);
    }
}
