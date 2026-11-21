using Ticketing.Domain.Common;

namespace Ticketing.Domain.Entities
{
    public class Zone : AuditableEntity
    {
        private readonly List<Seat> _seats = new();

        public string Name { get; private set; } = null!;
        public decimal Price { get; private set; }
        public int Capacity { get; private set; }
        public Guid EventId { get; private set; }
        public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();

        private Zone() { }

        internal Zone(Guid eventId, string name, decimal price, int capacity)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("The Zone name is required.");
            if (price < 0) throw new DomainException("The zone price cannot be negative.");
            if (capacity <= 0) throw new DomainException("The zone capacity must be greater than zero.");

            EventId = eventId;
            Name = name.Trim();
            Price = price;
            Capacity = capacity;
        }

        public Seat AddSeat(string row, int number)
        {
            if (Seats.Count >= Capacity)
                throw new DomainException("Cannot add more seat: the zone is at full capacity.");

            var seat = new Seat(Id, row, number);
            if (_seats.Any(s => s.Row.Equals(seat.Row, StringComparison.OrdinalIgnoreCase) && s.Number == seat.Number))
                throw new DomainException($"Seat {seat.Number} alredy exist in row {seat.Row}.");

            _seats.Add(seat);
            return seat;
        }

        public IReadOnlyList<Seat> AddSeats(int rows, int seatsPerRow)
        {
            if (rows <= 0 || seatsPerRow <= 0) throw new DomainException("Rows and seats per row must be greater than zero.");
            
            var total = rows * seatsPerRow;
            if (_seats.Count + total > Capacity) throw new DomainException($"Cannot add {total} seats: {_seats.Count} of {Capacity} are alredy used.");

            var existing = _seats.Select(s => (Row: s.Row.ToUpperInvariant(), s.Number)).ToHashSet();
            var newSeats = new List<Seat>();

            for (var r = 1; r <= rows; r++)
            {
                var rowName = GetRowName(r);
                for (var n = 1; n <= seatsPerRow; n++)
                {
                    if (existing.Contains((rowName, n)))
                        throw new DomainException($"Seat {n} already exists in row {rowName}.");
                    newSeats.Add(new Seat(Id, rowName, n));
                }
            }

            _seats.AddRange(newSeats);
            return newSeats;
        }

        private static string GetRowName(int rowNumber)
        {
            var name = "";
            while (rowNumber > 0)
            {
                var modulo = (rowNumber - 1) % 26;
                name = (char)('A' + modulo) + name;
                rowNumber = (rowNumber - 1) / 26;
            }
            return name;
        }
    }
}
