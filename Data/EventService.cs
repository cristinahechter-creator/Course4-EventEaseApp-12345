using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EventEaseApp.Models;
using Microsoft.JSInterop;

namespace EventEaseApp.Data
{
    public class EventService : IEventService
    {
        private readonly List<EventItem> _events = new();
        private readonly IJSRuntime _jsRuntime;
        private const string StorageKey = "eventease.events";

        public event Action? EventsChanged;

        public IReadOnlyList<EventItem> GetEvents() => _events.AsReadOnly();

        public EventService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }

        public EventItem CreateEvent(string name, DateTime date, string location)
        {
            var eventItem = new EventItem
            {
                Name = name,
                Date = date,
                Location = location,
            };

            AddEvent(eventItem);
            return eventItem;
        }

        public void AddEvent(EventItem eventItem)
        {
            if (eventItem is null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }

            if (string.IsNullOrWhiteSpace(eventItem.Name))
            {
                throw new ArgumentException("Event must have a name.", nameof(eventItem));
            }

            _events.Add(eventItem);
            NotifyStateChanged();
            _ = SaveAsync();
        }

        public bool DeleteEvent(Guid eventId)
        {
            var item = _events.FirstOrDefault(x => x.Id == eventId);
            if (item is null)
            {
                return false;
            }

            _events.Remove(item);
            NotifyStateChanged();
            _ = SaveAsync();
            return true;
        }

        public bool CompleteEvent(Guid eventId)
        {
            var item = _events.FirstOrDefault(x => x.Id == eventId);
            if (item is null)
            {
                return false;
            }

            item.MarkComplete();
            NotifyStateChanged();
            _ = SaveAsync();
            return true;
        }

        public void UpdateEvent(EventItem updatedEvent)
        {
            if (updatedEvent is null)
            {
                throw new ArgumentNullException(nameof(updatedEvent));
            }

            var existing = _events.FirstOrDefault(x => x.Id == updatedEvent.Id);
            if (existing is null)
            {
                return;
            }

            existing.Name = updatedEvent.Name;
            existing.Date = updatedEvent.Date;
            existing.Location = updatedEvent.Location;
            existing.IsCompleted = updatedEvent.IsCompleted;

            NotifyStateChanged();
            _ = SaveAsync();
        }

        public bool RegisterAttendance(Guid eventId)
        {
            var existing = _events.FirstOrDefault(x => x.Id == eventId);
            if (existing is null)
            {
                return false;
            }

            existing.RegisterAttendance();
            NotifyStateChanged();
            _ = SaveAsync();
            return true;
        }

        private void NotifyStateChanged() => EventsChanged?.Invoke();

        public async Task LoadAsync()
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var items = JsonSerializer.Deserialize<List<EventItem>>(json);
                    if (items != null)
                    {
                        _events.Clear();
                        _events.AddRange(items);
                        NotifyStateChanged();
                    }
                }
            }
            catch
            {
                // ignore local storage errors
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_events);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
            }
            catch
            {
                // ignore storage errors
            }
        }
    }
}
