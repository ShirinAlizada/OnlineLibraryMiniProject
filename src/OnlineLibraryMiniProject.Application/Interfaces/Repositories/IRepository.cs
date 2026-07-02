using OnlineLibraryMiniProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OnlineLibraryMiniProject.Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        void Add(T entity);

        void Delete(T entity);

        void Update(T entity);

        bool Any(Expression<Func<T, bool>> predicate);

        T? GetById(int id);

        List<T> GetAll();

        void SaveChanges();
    }
}
