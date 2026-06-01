using System;
using System.Threading.Tasks;

namespace EventEaseApp.Data
{
    public interface IUserSessionService
    {
        string Name { get; }
        string Email { get; }
        bool IsActive { get; }
        event Action? SessionChanged;
        Task LoadAsync();
        Task SetUserAsync(string name, string email);
        Task ClearAsync();
    }
}
