using EventBusRabbitMQ.Events.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Text;

namespace EventBusRabbitMQ.Producer
{
    //bir tane event üretip bunu queue ya bırakmak
    public class EventBusRabbitMQProducer
    {
        //producer üzerinden event gönderebilmek için rabitmq ya bağlanmalıyız.
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQProducer> _logger;

        //connection polly framework denemesayısı
        private readonly int _retryCount;

        public EventBusRabbitMQProducer(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQProducer> logger, int retryCount = 5)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;
            _retryCount = retryCount;
        }

        //şu queue'ya şu eventi bırak 
        //Producer
        public void Publish(string queueName, IEvent @event)
        {
            //connection durumu
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            //RetryPolicy -- execute işlemi
            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.RequestId, $"{time.TotalSeconds:n1}", ex.Message);
            });

            //publish işlemleri
            using(var channel = _persistentConnection.CreateModel())
            {
                //durable:Eğer false ise inmemory. Kuyruk bilgileri restart olursa kuyruktan siliniyor.True olursa fiziksel olarak arkada rabitmq sunucu içerisine laydedbiliyor ve restart olunca silinmiyor.
                //exclusive: Kuyruğun tek bir connection olmasını sağlıyor. Tek bir consumer burayı connect edebilir. Consumer silindiği connection kapandığında o kutruğun silindiğini belirtiyor. False defaultu
                //autoDelete:Kuyruk en az 1 consumera sahip ise son subscribe ortadan kalktığında otomatik olarak kuyruk silinecek. En az 1 connectiona sahip olması lazım. 
                //arguments:Brokera özgğ bazı parametreler içerir.
                channel.QueueDeclare(queueName, 
                                     durable: false, 
                                     exclusive: false, 
                                     autoDelete: false, 
                                     arguments: null);

                var message = JsonConvert.SerializeObject(@event);

                //Kuyruğa byte olarak bırakabiliyoruz.
                var body = Encoding.UTF8.GetBytes(message);

                //policy üzerinden execute edicez.
                policy.Execute(() =>
                {
                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.DeliveryMode = 2;  

                    channel.ConfirmSelect();
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                    channel.WaitForConfirmsOrDie();

                    channel.BasicAcks += (sender, eventArgs) =>
                    {
                        Console.WriteLine("Sent RabbitMQ");
                        //implement ack handle
                    };
                });
            }
        }
    }
}
