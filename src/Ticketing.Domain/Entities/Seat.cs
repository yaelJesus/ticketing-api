using Ticketing.Domain.Common;
using Ticketing.Domain.Enums;

namespace Ticketing.Domain.Entities
{
    public class Seat : AuditableEntity
    {
        public string Row { get; private set; } = null!;
        public int Number { get; private set; }
        public SeatStatus Status { get; private set; } = SeatStatus.Available;
        public DateTime? ReservedUntil { get; private set; }
        public Guid ZoneId { get; private set; }

        private Seat() { }

        internal Seat(Guid zoneId, string row, int number)
        {
            if (string.IsNullOrWhiteSpace(row)) throw new DomainException("The seat row is required.");
            if (number <= 0) throw new DomainException("The seat number must be greater than zero.");

            ZoneId = zoneId;
            Row = row.Trim();
            Number = number;
        }

        public void Reserve(DateTime nowUtc, TimeSpan holdTime)
        {
            var isAvailable = Status == SeatStatus.Available;
            var reservationExpired = Status == SeatStatus.Reserved && ReservedUntil <= nowUtc;

            if (!isAvailable && !reservationExpired) throw new DomainException("Only an available seat can be reserved.");

            Status = SeatStatus.Reserved;
            ReservedUntil = nowUtc.Add(holdTime);
        }

        public void Sell(DateTime nowUtc)
        {
            if (Status != SeatStatus.Reserved || ReservedUntil is null || ReservedUntil <= nowUtc)
                throw new DomainException("Only a seat with active reservation can be sold.");

            Status = SeatStatus.Sold;
            ReservedUntil = null;
        }

        public void Release()
        {
            if (Status == SeatStatus.Sold) throw new DomainException("A sold seat cannot be released.");

            Status = SeatStatus.Available;
            ReservedUntil = null;
        }
    }
}
