using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArcTrigger.Core.Entities;

namespace ArcTrigger.Domain.IRepositories
{
    public interface IOrdersRepository
    {
        // -------- Orders --------
        Task<Order> GetOrderByGuidAsync(Guid orderId, CancellationToken ct = default);
        Task<Order> GetOrderByIdAsync(int id, CancellationToken ct = default);
        Task<List<Order>> GetAllOrdersAsync(CancellationToken ct = default);
        Task<List<Order>> GetOrdersByUserAsync(Guid userId, CancellationToken ct = default);
        Task<List<Order>> GetOrdersBySymbolAsync(string symbol, CancellationToken ct = default);

        Task AddOrderAsync(Order order, CancellationToken ct = default);
        Task AddOrdersAsync(IEnumerable<Order> orders, CancellationToken ct = default);

        Task RemoveOrderAsync(Order order, CancellationToken ct = default);

        // -------- Previous Orders (History) --------
        Task<PreviousOrder> GetPreviousOrderByGuidAsync(Guid previousOrderId, CancellationToken ct = default);
        Task<PreviousOrder> GetPreviousOrderByIdAsync(int id, CancellationToken ct = default);
        Task<List<PreviousOrder>> GetAllPreviousOrdersAsync(CancellationToken ct = default);
        Task<List<PreviousOrder>> GetPreviousOrdersByUserAsync(Guid userId, CancellationToken ct = default);
        Task<List<PreviousOrder>> GetPreviousOrdersBySymbolAsync(string symbol, CancellationToken ct = default);

        Task AddPreviousOrderAsync(PreviousOrder previousOrder, CancellationToken ct = default);
        Task AddPreviousOrdersAsync(IEnumerable<PreviousOrder> previousOrders, CancellationToken ct = default);
        
      
        Task RemovePreviousOrderAsync(PreviousOrder previousOrder, CancellationToken ct = default);

        // -------- Persistence --------
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
