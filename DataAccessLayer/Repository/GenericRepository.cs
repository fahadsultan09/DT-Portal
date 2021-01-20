using Microsoft.EntityFrameworkCore;
using Models.ApplicationContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccessLayer.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public DistributorPortalDbContext _context = null;
        public DbSet<T> DbSet = null;
        public bool Disposed = false;
        public GenericRepository(DistributorPortalDbContext dbContext)
        {
            this._context = dbContext;
            DbSet = _context.Set<T>();
        }
        public void Delete(T obj)
        {
            DbSet.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        public IEnumerable<T> GetAllList()
        {
            return DbSet.ToList();
        }

        public T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public void Insert(T obj)
        {
            DbSet.Add(obj);
        }

        public void Update(T obj)
        {
            DbSet.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (!this.Disposed)
            {
                if (Disposing)
                {
                    _context.Dispose();
                }
            }
            this.Disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AddRange(List<T> obj)
        {
            _context.Set<T>().AddRange(obj);
        }
        public void UpdateRange(List<T> obj)
        {
            DbSet.AttachRange(obj);
            obj.ToList().ForEach(e =>
            {
                _context.Entry(e).State = EntityState.Modified;
            });
        }

        public List<T> Where(Expression<Func<T, bool>> predicate, params string[] navigationProperties)
        {
            List<T> list;
            var query = _context.Set<T>().AsQueryable();
            foreach (string navigationProperty in navigationProperties)
                query = query.Include(navigationProperty);
            list = query.Where(predicate).ToList<T>();
            return list;
        }

        public void DeleteRange(IEnumerable<T> List)
        {
            _context.Set<T>().RemoveRange(List);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            T item;
            var query = _context.Set<T>().AsQueryable();
            item = query.FirstOrDefault(predicate);
            return item;
        }
        public T First(Expression<Func<T, bool>> predicate)
        {
            T item;
            var query = _context.Set<T>().AsQueryable();
            item = query.First(predicate);
            return item;
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            var query = _context.Set<T>().AsQueryable();
            return query.Any(predicate);
        }
    }
}
