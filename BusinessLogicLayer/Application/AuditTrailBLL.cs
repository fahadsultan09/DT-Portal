using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Models.Application;
using Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Application
{
    public class AuditTrailBLL<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<AuditTrail> _repository;
        public AuditTrailBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<AuditTrail>();
        }
        public void AddAuditTrail(int PageId, int ActionId, object model, string Remarks, int CreatedBy)
        {
            try
            {
                AuditTrail _AuditTrail = new AuditTrail();
                _AuditTrail.JsonObject = JsonConvert.SerializeObject(model);
                AuditEntry AuditTrail = TrackChanges(ActionId);

                _AuditTrail.ActionId = ActionId;
                _AuditTrail.PageId = PageId;
                _AuditTrail.KeyValue = JsonConvert.SerializeObject(AuditTrail.KeyValue);
                _AuditTrail.OldValue = JsonConvert.SerializeObject(AuditTrail.OldValue);
                _AuditTrail.NewValue = JsonConvert.SerializeObject(AuditTrail.NewValue);
                _AuditTrail.Remarks = Remarks;
                _AuditTrail.CreatedBy = CreatedBy;
                _AuditTrail.CreatedDate = DateTime.Now;
                _repository.Insert(_AuditTrail);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private AuditEntry TrackChanges(int ActionId)
        {
            AuditEntry AuditEntry = new AuditEntry();
            _unitOfWork.ChangeTracker.DetectChanges();
            foreach (EntityEntry Entry in _unitOfWork.ChangeTracker.Entries<T>().Where(x => x.State != EntityState.Unchanged && x.State != EntityState.Detached))
            {
                if (Entry.State != EntityState.Detached || Entry.State != EntityState.Unchanged)
                {
                    AuditEntry.TableName = Entry.Metadata.GetTableName();
                    foreach (PropertyEntry property in Entry.Properties)
                    {
                        string propertyName = property.Metadata.Name;

                        if (propertyName != "UpdatedDate")
                        {
                            if (property.Metadata.IsPrimaryKey())
                            {
                                //AuditEntry.Key = (int)property.CurrentValue;
                                AuditEntry.KeyValue[propertyName] = property.CurrentValue;
                                continue;
                            }
                            switch (Entry.State)
                            {
                                case EntityState.Added:
                                    AuditEntry.NewValue[propertyName] = property.CurrentValue;
                                    AuditEntry.ActionId = ActionId;
                                    break;

                                case EntityState.Deleted:
                                    AuditEntry.OldValue[propertyName] = property.OriginalValue;
                                    AuditEntry.ActionId = ActionId;
                                    break;

                                case EntityState.Modified:
                                    //if (property.IsModified && property.OriginalValue != null && property.CurrentValue != null && !property.OriginalValue.Equals(property.CurrentValue))
                                    object OriginalValue = property.OriginalValue == null ? string.Empty : property.OriginalValue;
                                    object CurrentValue = property.CurrentValue == null ? string.Empty : property.CurrentValue;
                                    if (property.IsModified && !OriginalValue.Equals(CurrentValue))
                                    {
                                        AuditEntry.OldValue[propertyName] = property.OriginalValue;
                                        AuditEntry.NewValue[propertyName] = property.CurrentValue;
                                        AuditEntry.ActionId = ActionId;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return AuditEntry;
        }
        public List<AuditTrail> Where(Expression<Func<AuditTrail, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
    }
}
