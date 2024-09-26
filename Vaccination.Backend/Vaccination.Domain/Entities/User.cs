using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Vaccination.Domain.Shared;

namespace Vaccination.Domain.Entities
{
    public class User : IdentityUser, IAuditableEntity
    {
        [MaxLength(150)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string LastName { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Nationality { get; set; }

        public string? Address { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(13)]
        public string? SocialSecurityNumber { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? ModifiedOnUtc { get; set; }

        public bool IsDeleted { get; set; }
    }
}