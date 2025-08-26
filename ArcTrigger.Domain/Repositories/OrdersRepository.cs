using ArcTrigger.Core.Entities;
using ArcTrigger.Domain.Contexts;
using ArcTrigger.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace ArcTrigger.Domain.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {

        private TriggerContext _triggerContext;
        public OrdersRepository()
        {
            _triggerContext = new TriggerContext();
            _triggerContext.Database.EnsureCreated();
        }
         

        async Task IOrdersRepository.AddOrderAsync(Order order, CancellationToken ct)
        {
            await _triggerContext.Orders.AddAsync(order, ct);
        }

        async Task IOrdersRepository.AddOrdersAsync(IEnumerable<Order> orders, CancellationToken ct)
        {
            await _triggerContext.Orders.AddRangeAsync(orders, ct);
        }

        async Task IOrdersRepository.AddPreviousOrderAsync(PreviousOrder previousOrder, CancellationToken ct)
        {
            await _triggerContext.PreviousOrders.AddAsync(previousOrder, ct);
        }

        async Task IOrdersRepository.AddPreviousOrdersAsync(IEnumerable<PreviousOrder> previousOrders, CancellationToken ct)
        {
            await _triggerContext.PreviousOrders.AddRangeAsync(previousOrders, ct);
        }

        async Task<List<Order>> IOrdersRepository.GetAllOrdersAsync(CancellationToken ct)
        {
            return await _triggerContext.Orders.ToListAsync();
        }

        async Task<List<PreviousOrder>> IOrdersRepository.GetAllPreviousOrdersAsync(CancellationToken ct)
        {
            return await _triggerContext.PreviousOrders.ToListAsync();
        }

        async Task<Order> IOrdersRepository.GetOrderByGuidAsync(Guid orderId, CancellationToken ct)
        {
            return await _triggerContext.Orders.Where(x=>x.Id == orderId).FirstOrDefaultAsync();
        }

        async Task<Order> IOrdersRepository.GetOrderByIdAsync(int id, CancellationToken ct)
        {
            return await _triggerContext.Orders.Where(x => x.intid == id).FirstOrDefaultAsync();
        }

        async Task<List<Order>> IOrdersRepository.GetOrdersBySymbolAsync(string symbol, CancellationToken ct)
        {
            return await _triggerContext.Orders.Where(x => x.Symbol == symbol).ToListAsync();//08502226429
        }

        async Task<List<Order>> IOrdersRepository.GetOrdersByUserAsync(Guid userId, CancellationToken ct)
        {
            return await _triggerContext.Orders.Where(x => x.UserId == userId).ToListAsync();
        }

        async Task<PreviousOrder> IOrdersRepository.GetPreviousOrderByGuidAsync(Guid previousOrderId, CancellationToken ct)
        {
            return await _triggerContext.PreviousOrders.Where(x => x.UserId == previousOrderId).FirstOrDefaultAsync();
        }

        async Task<PreviousOrder> IOrdersRepository.GetPreviousOrderByIdAsync(int id, CancellationToken ct)
        {
            return await _triggerContext.PreviousOrders.Where(x => x.Intid == id).FirstOrDefaultAsync();
        }

        async Task<List<PreviousOrder>> IOrdersRepository.GetPreviousOrdersBySymbolAsync(string symbol, CancellationToken ct)
        {
            return await _triggerContext.PreviousOrders.Where(x =>x.Symbol == symbol).ToListAsync();
        }

        async Task<List<PreviousOrder>> IOrdersRepository.GetPreviousOrdersByUserAsync(Guid userId, CancellationToken ct)
        {
            return await _triggerContext.PreviousOrders.Where(x => x.UserId == userId).ToListAsync();
        }

        async Task IOrdersRepository.RemoveOrderAsync(Order order, CancellationToken ct)
        {
            
            await Task.Run(() => _triggerContext.Orders.Remove(order), ct);

        }

        async Task IOrdersRepository.RemovePreviousOrderAsync(PreviousOrder previousOrder, CancellationToken ct)
        {
            await Task.Run(() => _triggerContext.PreviousOrders.Remove(previousOrder), ct);
        }

        async Task IOrdersRepository.SaveChangesAsync(CancellationToken ct)
        {
            int retryCount = 3; // Number of retry attempts
            bool saveFailed;

            do
            {
                saveFailed = false;
                try
                {
                    await _triggerContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    if (retryCount-- <= 0)
                        throw; // Re-throw if we've exhausted retries

                    // Refresh all affected entries
                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();

                        // For updates, you might want to preserve some changes
                        if (entry.State == EntityState.Modified)
                        {
                            var currentValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            // Here you can implement your merge strategy
                            foreach (var property in currentValues.Properties)
                            {
                                var currentValue = currentValues[property];
                                var databaseValue = databaseValues[property];

                                // Example: Keep current value if it was modified
                                if (!Equals(currentValue, entry.OriginalValues[property]))
                                {
                                    databaseValues[property] = currentValue;
                                }
                            }

                            entry.OriginalValues.SetValues(databaseValues);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log other exceptions (DbUpdateException, etc.)
                    // Consider using a logging framework here
                    Console.WriteLine($"Save changes failed: {ex.Message}");
                    throw;
                }
            } while (saveFailed && retryCount > 0);
        }
    }
}
