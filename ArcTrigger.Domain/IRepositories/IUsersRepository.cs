using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArcTrigger.Core.Entities;

namespace ArcTrigger.Domain.IRepositories
{
    public interface IUsersRepository
    {
        // Reads
        Task<User?> FindAsync(Guid userId, CancellationToken ct = default);
        Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);

        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task<List<User>> GetUsersAsync();
       
        // Writes (EF Core tracking handles updates)
        Task AddUserAsync(User user, CancellationToken ct = default);
        Task RemoveUserAsync(Guid userId, CancellationToken ct = default);
        Task RemoveUserAsync(User user, CancellationToken ct = default);

        // Persistence
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
