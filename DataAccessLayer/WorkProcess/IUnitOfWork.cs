using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DataAccessLayer.Repository;
using System.Threading.Tasks;

namespace DataAccessLayer.WorkProcess
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> GenericRepository<T>() where T : class;
        int Save();
        Task<int> SaveAsync();
        IDbContextTransaction Begin();
        void Commit();
        void Rollback();
        ChangeTracker ChangeTracker { get; }
    }
}
