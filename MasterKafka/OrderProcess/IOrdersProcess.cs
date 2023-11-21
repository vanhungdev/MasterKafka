using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterKafka.OrderProcess
{
    public interface IOrdersProcess
    {
        public void CreateOrderProcess(string message, string topic);
    }
}
