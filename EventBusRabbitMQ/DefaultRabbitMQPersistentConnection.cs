using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;

namespace EventBusRabbitMQ
{
    /// <summary>
    /// 
    /// </summary>s
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        //Polly kütüphanesi kullanıcaz.
        // Connection başlatabilmek için
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;

        //tekrar connection denenmesi
        private readonly int _retryCount;

        //Logger
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;

        //dispose patern için gerekli
        private bool _disposed;

        public DefaultRabbitMQPersistentConnection(
            IConnectionFactory connectionFactory,
            int retryCount,
            ILogger<DefaultRabbitMQPersistentConnection> logger)
        {
            _connectionFactory = connectionFactory;
            _retryCount = retryCount;
            _logger = logger;
        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }
         
        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            //policy üzerinden connection oluşturma
            //Burada 1 kere deneyip hata aldığında durmasın. Tekrar tekrar denesin.connect olsun. Bu yüzden PollyFramework kullanıyoruz.
            //RetryPolicy tanımladık. SocketException ve BrokerUnreachableException -> RabbitMQ'nun messageBroker'a erişemediğinde fırlattığı hata mesajları
            //Bu hataları aldığında benim verdiğim koşullarda bekle ve yeniden dene 
            //_retryCount
            //kaçıncı denemesinde - denemex saniye 1x2 2sn 2x2 4 sn bekle retry denemesi zaman kazandırma

            var policy = RetryPolicy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                });

            //Startup
            //Connection oluşturması
            policy.Execute(() =>
            {
                _connection = _connectionFactory.CreateConnection();
            });

            //işlemler başarılı
            if (IsConnected)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;

                _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                return true;
            }
            else
            {
                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                return false;
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) 
                return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }

        //QueuManagement işlemleri
        public IModel CreateModel()
        {
            //connect olup olmadığı
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            //Connection başarılı ise CreateModel ile IModel tipinde geriye bir nesne döndürdük.Queu işlemlerini yapabilmek için.
            return _connection.CreateModel();
        }

        //connectionlar dispose patern ile olması sağlıklı olacaktır.s
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}
