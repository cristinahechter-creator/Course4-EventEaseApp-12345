using System;
using System.Collections.Generic;
using EventEaseApp.Models;

namespace EventEaseApp.Data
{
    public interface IEventService
    {
        IReadOnlyList<EventItem> GetEvents();
        EventItem CreateEvent(string name, DateTime date, string location);
        void AddEvent(EventItem eventItem);
        bool DeleteEvent(Guid eventId);
        bool CompleteEvent(Guid eventId);
        void UpdateEvent(EventItem updatedEvent);
        bool RegisterAttendance(Guid eventId);
        System.Threading.Tasks.Task LoadAsync();
        event Action? EventsChanged;
    }
}
