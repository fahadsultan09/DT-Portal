using DataAccessLayer.Repository;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Models.ApplicationContext;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.WorkProcess
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool disposed;
        private Dictionary<string, object> repositories;
        private readonly DistributorPortalDbContext _context;

        private IDbContextTransaction _transaction;

        public UnitOfWork(DistributorPortalDbContext context)
        {
            _context = context;
        }
        public void Dispose()
        {
            Dispose(true);
            if (_transaction != null)
                _transaction.Dispose();
            _transaction = null;
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (!this.disposed)
            {
                if (Disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public IGenericRepository<T> GenericRepository<T>() where T : class
        {
            if (repositories == null)
            {
                repositories = new Dictionary<string, object>();
            }
            var type = typeof(T).Name;
            if (!repositories.ContainsKey(type))
            {
                var repositorytype = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositorytype.MakeGenericType(typeof(T)), _context);
                repositories.Add(type, repositoryInstance);
            }
            return (GenericRepository<T>)repositories[type];
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public IDbContextTransaction Begin()
        {
            _transaction = _context.Database.BeginTransaction();
            return _transaction;
        }

        public void Commit()
        {
            _transaction.Commit();
            Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
            Dispose();
        }

        public ChangeTracker ChangeTracker { get { return _context.ChangeTracker; } }
    }
}
