using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaccination.Domain.Entities;

namespace Vaccination.Infrastructure.Context
{
    public class VaccinationContext : IdentityDbContext<User>
    {
        public VaccinationContext(DbContextOptions<VaccinationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("Users");
            });

            modelBuilder.Entity<User>(b =>
            {
                b.Property(u => u.Email).HasMaxLength(128);
                b.Property(u => u.NormalizedEmail).HasMaxLength(128);
            });

            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        }
    }
}