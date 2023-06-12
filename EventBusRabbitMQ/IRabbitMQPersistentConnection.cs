using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusRabbitMQ
{
    /// <summary>
    /// Connection işlemleri
    /// IDisposable - connection sınıfları bu şekilde olmalı
    /// </summary>
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        /// <summary>
        /// Connect olup olmadığı
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connection başlatacak
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        /// <summary>
        /// Que management işlemleri
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}
