using Esourcing.Sourcing.Entities;
using MongoDB.Driver;

namespace Esourcing.Sourcing.Data.Interface
{
    public interface ISourcingContext
    {
        IMongoCollection<Auction> Auctions { get; }
        IMongoCollection<Bid> Bids { get; }
    }
}
