using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace EventEaseApp.Data
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string StorageKey = "eventease.userSession";

        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public bool IsActive => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Email);
        public event Action? SessionChanged;

        public UserSessionService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        public async Task LoadAsync()
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", StorageKey);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var state = JsonSerializer.Deserialize<UserSessionState>(json);
                    if (state != null)
                    {
                        Name = state.Name ?? string.Empty;
                        Email = state.Email ?? string.Empty;
                        NotifyStateChanged();
                    }
                }
            }
            catch
            {
                // ignore storage errors
            }
        }

        public async Task SetUserAsync(string name, string email)
        {
            Name = name?.Trim() ?? string.Empty;
            Email = email?.Trim() ?? string.Empty;
            NotifyStateChanged();
            await SaveAsync();
        }

        public async Task ClearAsync()
        {
            Name = string.Empty;
            Email = string.Empty;
            NotifyStateChanged();
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            try
            {
                var state = new UserSessionState
                {
                    Name = Name,
                    Email = Email,
                };

                var json = JsonSerializer.Serialize(state);
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", StorageKey, json);
            }
            catch
            {
                // ignore storage errors
            }
        }

        private void NotifyStateChanged() => SessionChanged?.Invoke();

        private sealed class UserSessionState
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
        }
    }
}
