using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DataAccessLayer.Repository;

namespace DataAccessLayer.WorkProcess
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> GenericRepository<T>() where T : class;
        int Save();
        IDbContextTransaction Begin();
        void Commit();
        void Rollback();
        ChangeTracker ChangeTracker { get; }
    }
}
