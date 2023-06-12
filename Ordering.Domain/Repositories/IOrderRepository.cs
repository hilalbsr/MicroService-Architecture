using Ordering.Domain.Entities;
using Ordering.Domain.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.Domain.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersBySellerUserName(string userName);
    }
}
