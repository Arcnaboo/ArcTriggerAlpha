using System;
using System.Threading;
using System.Threading.Tasks;
using ArcTrigger.Core.Entities;
using ArcTrigger.Domain.IRepositories;
using ArcTrigger.Domain.Repositories;

internal static class Program
{
    // C# 7.1+ required for async Main
    private static async Task Main()
    {
        Console.WriteLine("=== IUsersRepository explicit-impl test ===");
        Console.WriteLine($"Start: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}\n");

        // 1) interface-typed variable pointing to the concrete repo
        IUsersRepository usersRepo = new UsersRepository();

        // You can change these two lines as you wish; everything else stays interface-only:
        string email = "tester@example.com";
        string password = "P@ssw0rd!";

        var ct = CancellationToken.None;

        // Add a user via the interface (explicit implementation requires interface reference)
        var newUser = new User(email, password);
        await usersRepo.AddUserAsync(newUser, ct);
        await usersRepo.SaveChangesAsync();

        Console.WriteLine($"[ADD]   User added: Id={newUser.Id}, Email={newUser.Email}");

        // EmailExistsAsync
        bool exists = usersRepo.EmailExistsAsync(email, ct);
        Console.WriteLine($"[EXISTS] EmailExistsAsync(\"{email}\") => {exists}");

        // FindByEmailAsync
        var byEmail = await usersRepo.FindByEmailAsync(email, ct);
        Console.WriteLine(byEmail is null
            ? $"[FIND@MAIL] Not found: {email}"
            : $"[FIND@MAIL] Found: Id={byEmail.Id}, Email={byEmail.Email}");

        // FindAsync by Id
        var byId = await usersRepo.FindAsync(newUser.Id, ct);
        Console.WriteLine(byId is null
            ? $"[FIND@ID]   Not found: {newUser.Id}"
            : $"[FIND@ID]   Found: Id={byId.Id}, Email={byId.Email}");

        // GetUsersAsync
        var all = await usersRepo.GetUsersAsync();
        Console.WriteLine($"[LIST]  Total users: {all.Count}");
        foreach (var u in all)
            Console.WriteLine($"        -> {u.Id} | {u.Email}");

        // Remove via interface, then save
        await usersRepo.RemoveUserAsync(newUser, ct);
        await usersRepo.SaveChangesAsync();
        Console.WriteLine($"[REMOVE] Removed: {newUser.Id}");

        Console.WriteLine($"\nFinished: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
    }
}
