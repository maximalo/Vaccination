using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Vaccination.Domain.Interfaces;
using Vaccination.Infrastructure.Context;

namespace Vaccination.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly VaccinationContext context;

        public BaseRepository(VaccinationContext context)
        {
            this.context = context;
        }

        public IQueryable<T> FindAll()
        {
            return context.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return context.Set<T>()
                .Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }
    }
}