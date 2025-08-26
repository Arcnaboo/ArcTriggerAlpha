using ArcTrigger.Core.Entities;
using ArcTrigger.Domain.Contexts;
using ArcTrigger.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;


using System.Data.Entity;

using EntityState = Microsoft.EntityFrameworkCore.EntityState;

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

         bool IUsersRepository.EmailExistsAsync(string email, CancellationToken ct)
        {
            return _triggerContext.Users.Where(x => x.Email == email).Any();
        }



        async Task<User?> IUsersRepository.FindAsync(Guid userId, CancellationToken ct)
        {
            return await _triggerContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
        }

        async Task<User?> IUsersRepository.FindByEmailAsync(string email, CancellationToken ct)
        {
            return await _triggerContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        async Task<IQueryable<User>> IUsersRepository.GetUsersAsync()
        {
            return (IQueryable<User>)await _triggerContext.Users.ToListAsync();
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
            //await _triggerContext.SaveChangesAsync(ct);
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
