using System.Text.RegularExpressions;
using System.Net.Mail;
using Ticketing.Domain.Common;
using Ticketing.Domain.Enums;

namespace Ticketing.Domain.Entities
{
    public class User : AuditableEntity
    {
        public string Name { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public UserRole Role { get; private set; }

        private User() { }

        public User(string name, string email, string passwordHash, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("User name is required.");
            if (string.IsNullOrWhiteSpace(email)) throw new DomainException("The user email is required.");
            if (!IsValidEmail(email)) throw new DomainException("Invalid email.");
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("The user password is required.");

            Name = name.Trim();
            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            Role = role;
        }

        public void AssignRole(UserRole role)
        {
            if (Role == role) throw new DomainException("The same role cannot be assigned.");

            Role = role;
        }

        private static bool IsValidEmail(string email)
        {
            try { return new MailAddress(email).Address == email; }
            catch (FormatException) { return false; }
        }
    }
}
