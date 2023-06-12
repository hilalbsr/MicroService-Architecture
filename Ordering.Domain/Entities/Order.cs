using Ordering.Domain.Entities.Base;
using System;

namespace Ordering.Domain.Entities
{
    //siparişi işleyen entity
    public class Order : Entity
    {
        public string AuctionId { get; set; } //hangi ihaleden siparişe dönüşen 
        public string SellerUserName { get; set; } //hangi satıcı
        public string ProductId { get; set; } //hangi ürün
        public decimal UnitPrice { get; set; } //birim fiyat kazanılan fiyat
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
