using Ticketing.Domain.Common;
using Ticketing.Domain.Enums;

namespace Ticketing.Domain.Entities
{
    public class Event : AuditableEntity
    {
        private readonly List<Zone> _zones = new();

        public string Name { get; private set; } = null!;
        public DateTime StartAt { get; private set; }
        public EventStatus Status { get; private set; }
        public IReadOnlyCollection<Zone> Zones => _zones.AsReadOnly();

        private Event() { }

        public Event(string name, DateTime startAt)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("The event name is required.");
            if (startAt <= DateTime.UtcNow) throw new DomainException("The event date must be in the future.");
            
            Name = name.Trim();
            StartAt = startAt;
            Status = EventStatus.Draft;
        }

        public void Publish()
        {
            if (Status != EventStatus.Draft) throw new DomainException("Only a draft event can be published.");
            if (_zones.Count == 0) throw new DomainException("An event needs at least one zone to be published.");
            Status = EventStatus.Published;
        }

        public void Cancel()
        {
            if (Status == EventStatus.Cancelled) throw new DomainException("The event is already cancelled.");
            Status = EventStatus.Cancelled;
        }

        public Zone AddZone(string name, decimal price, int capacity)
        {
            if (Status == EventStatus.Cancelled) throw new DomainException("Cannot add zones to a cancelled event.");

            var zone = new Zone(Id, name, price, capacity);
            if (_zones.Any(z => z.Name.Equals(zone.Name, StringComparison.OrdinalIgnoreCase)))
                throw new DomainException("A zone with that name alredy exist in ths event.");
            _zones.Add(zone);
            return zone;
        }
    }
}
