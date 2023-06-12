using AutoMapper;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Core;
using EventBusRabbitMQ.Events;
using MediatR;
using Newtonsoft.Json;
using Ordering.Application.Commands.OrderCreate;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESourcing.Order.Consumers
{
    public class EventBusOrderCreateConsumer
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public EventBusOrderCreateConsumer(IRabbitMQPersistentConnection persistentConnection, IMediator mediator, IMapper mapper)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public void Consume()
        {
            //rabbitmq connection kontrolü
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();
            channel.QueueDeclare(queue: EventBusConstants.OrderCreateQueue, 
                                 durable: false, 
                                 exclusive: false,
                                 autoDelete: false, 
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            //custom event metot
            consumer.Received += ReceivedEvent;

            channel.BasicConsume(queue: EventBusConstants.OrderCreateQueue, 
                                 autoAck: true, 
                                 consumer: consumer);
        }

        //Düşen mesajlar için yapılacak işlemler
        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            //mesaja ulaşıyoruz. e.body.span ile.
            var message = Encoding.UTF8.GetString(e.Body.Span);

            //OrderCreateEvent modeli göndermiştik.
            var @event = JsonConvert.DeserializeObject<OrderCreateEvent>(message);

            if(e.RoutingKey == EventBusConstants.OrderCreateQueue)
            {
                var command = _mapper.Map<OrderCreateCommand>(@event);

                command.CreatedAt = DateTime.Now;
                command.TotalPrice = @event.Quantity * @event.Price; //her bir ürün fiyat * toplam ürün
                command.UnitPrice = @event.Price;

                var result = await _mediator.Send(command);
            }
        }

        //connection dispose ediyoruz.
        public void Disconnect()
        {
            _persistentConnection.Dispose();
        }
    }
}
