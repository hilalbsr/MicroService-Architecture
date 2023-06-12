using System;

namespace EventBusRabbitMQ.Events.Interfaces
{

    public abstract class IEvent
    {
        public IEvent()
        {
            RequestId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        //Her event oluştuğunda unique bir guid üzerinden track edicez.
        public Guid RequestId { get; private init; }

        //ne zaman oluşturulduğu
        public DateTime CreationDate { get; private init; }
    }
}
