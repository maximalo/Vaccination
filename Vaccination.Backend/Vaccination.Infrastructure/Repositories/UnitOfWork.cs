using Vaccination.Domain.Interfaces;
using Vaccination.Infrastructure.Context;

namespace Vaccination.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VaccinationContext context;

        public UnitOfWork(VaccinationContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}