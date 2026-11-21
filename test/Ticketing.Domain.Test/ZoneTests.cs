using Ticketing.Domain.Common;
using Ticketing.Domain.Entities;

namespace Ticketing.Domain.Test
{
    public class ZoneTests
    {
        [Fact]
        public void AddSeat_WithZeroNumber_ThrowsDomainException()
        {
            // Arrange
            var evt = new Event("Game 1", DateTime.UtcNow.AddDays(10));
            var zone = evt.AddZone("General", 100m, 10);

            // Act + Assert
            Assert.Throws<DomainException>(() => zone.AddSeat("A", 0));
        }

        [Fact]
        public void AddSeats_With28Rows_GeneratesRowABForLastSeat()
        {
            // Arrange
            var evt = new Event("Petroleros vs. Solares", DateTime.UtcNow.AddDays(15));
            var zone = evt.AddZone("Preferente", 85m, 28);

            // Act
            var seats = zone.AddSeats(28, 1);

            // Assert
            Assert.Equal("AB", seats.Last().Row);
            Assert.Equal(1, seats.Last().Number);
        }

        [Fact]
        public void AddSeat_WhenSeatAlreadyExists_ThrowsDomainException()
        {
            // Arrange
            var evt = new Event("Game 1", DateTime.UtcNow.AddDays(10));
            var zone = evt.AddZone("General", 100m, 10);

            // Act
            zone.AddSeat("A", 1);

            // Assert
            Assert.Throws<DomainException>(() => zone.AddSeat("A", 1));
        }

        [Fact]
        public void AddSeats_WhenCapacityIsExceeded_ThrowsDomainException()
        {
            // Arrange
            var evt = new Event("Petroleros vs. Solares", DateTime.UtcNow.AddDays(15));
            var zone = evt.AddZone("Preferente", 85m, 10);

            // Act + Assert
            Assert.Throws<DomainException>(() => zone.AddSeats(11, 1));
        }
    }
}
