using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccessLayer.Repository
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> GetAllList();
        T GetById(int id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(T obj);
        void HardDelete(T obj);
        void HardDeleteRange(List<T> obj);
        void AddRange(List<T> obj);
        void UpdateRange(List<T> obj);
        List<T> Where(Expression<Func<T, bool>> predicate, params string[] navigationProperties);
        void DeleteRange(IEnumerable<T> List);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        T First(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);

    }
}
