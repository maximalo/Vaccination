using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;
using Vaccination.Domain.Shared;

namespace Vaccination.Infrastructure.Interceptors
{
    public sealed class UpdateAuditableEntitiesInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateAuditableEntitiesInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            DbContext? dbContext = eventData.Context;

            if (dbContext is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            IEnumerable<EntityEntry<IAuditableEntity>> entries = dbContext.ChangeTracker.Entries<IAuditableEntity>();

            foreach (EntityEntry<IAuditableEntity> entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property(x => x.CreatedOnUtc).CurrentValue = DateTime.UtcNow;

                    if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
                    {
                        entityEntry.Property(x => x.CreatedBy).CurrentValue = new Guid(_httpContextAccessor.HttpContext.User.FindFirstValue("userid"));
                    }
                    else
                    {
                        entityEntry.Property(x => x.CreatedBy).CurrentValue = Guid.Empty;
                    }
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Property(x => x.ModifiedOnUtc).CurrentValue = DateTime.UtcNow;
                    if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
                    {
                        entityEntry.Property(x => x.ModifiedBy).CurrentValue = new Guid(_httpContextAccessor.HttpContext.User.FindFirstValue("userid"));
                    }
                    else
                    {
                        entityEntry.Property(x => x.ModifiedBy).CurrentValue = Guid.Empty;
                    }
                }

                if (entityEntry.State == EntityState.Deleted)
                {
                    entityEntry.Property(x => x.ModifiedOnUtc).CurrentValue = DateTime.UtcNow;
                    if (_httpContextAccessor.HttpContext?.User != null)
                    {
                        entityEntry.Property(x => x.ModifiedBy).CurrentValue = new Guid(_httpContextAccessor.HttpContext.User.FindFirstValue("userid"));
                    }
                    else
                    {
                        entityEntry.Property(x => x.ModifiedBy).CurrentValue = Guid.Empty;
                    }
                    entityEntry.Property(x => x.IsDeleted).CurrentValue = true;
                    entityEntry.State = EntityState.Modified;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}