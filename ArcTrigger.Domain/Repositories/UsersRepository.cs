using ArcTrigger.Core.Entities;
using ArcTrigger.Domain.Contexts;
using ArcTrigger.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcTrigger.Domain.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private TriggerContext _triggerContext; 
        public UsersRepository() 
        {
            _triggerContext = new TriggerContext();
            _triggerContext.Database.EnsureCreated();
        }
        async Task IUsersRepository.AddUserAsync(User user, CancellationToken ct)
        {
            await _triggerContext.Users.AddAsync(user, ct);
        }

        async Task<bool> IUsersRepository.EmailExistsAsync(string email, CancellationToken ct)
        {
            return await _triggerContext.Users.Where(x => x.Email == email).AnyAsync();
        }



        async Task<User?> IUsersRepository.FindAsync(Guid userId, CancellationToken ct)
        {
            return await _triggerContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
        }

        async Task<User?> IUsersRepository.FindByEmailAsync(string email, CancellationToken ct)
        {
            return await _triggerContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        async Task<List<User>> IUsersRepository.GetUsersAsync()
        {
            return await _triggerContext.Users.ToListAsync();
        }

        async Task IUsersRepository.RemoveUserAsync(Guid userId, CancellationToken ct)
        {
            var user = await _triggerContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
             _triggerContext.Users.Remove(user);
        
        }

        async Task IUsersRepository.RemoveUserAsync(User user, CancellationToken ct)
        {
            await Task.Run(() => _triggerContext.Users.Remove(user), ct);
        }

        async Task IUsersRepository.SaveChangesAsync(CancellationToken ct)
        {
            await _triggerContext.SaveChangesAsync(ct);
        }
    }
}
