﻿using System.Linq.Expressions;

namespace Vaccination.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T> FindAll();

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

        void Create(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}