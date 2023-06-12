using EventBusRabbitMQ.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusRabbitMQ.Events
{
    //Sourcing service EventBus'a bir tane event bırakıcak.
    //OrderService queue dinlicek. Gelen event'i alacak ve siparişe dönüştürecek.
    public class OrderCreateEvent : IEvent
    {
        public string Id { get; set; }
        public string AuctionId { get; set; } //açık artırma
        public string ProductId { get; set; }
        public string SellerUserName { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Quantity { get; set; }
    }
}
