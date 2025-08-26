using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArcTrigger.Core.Entities;

namespace ArcTrigger.Domain.IRepositories
{
    public interface IStocksRepository
    {
        // Reads
        Task<Stock?> FindAsync(Guid stockId, CancellationToken ct = default);
        Task<Stock?> FindBySymbolAsync(string symbol, CancellationToken ct = default);
        Task<List<Stock>> GetAllAsync(CancellationToken ct = default);

        // Writes
        Task AddAsync(Stock stock, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<Stock> stocks, CancellationToken ct = default);
        Task RemoveAsync(Guid stockId, CancellationToken ct = default);
        Task RemoveAsync(Stock stock, CancellationToken ct = default);

        // Persistence
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
