using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer.Application
{
    public class OrderBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddOrderMaster(OrderMaster module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<OrderMaster>().Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateOrderMaster(OrderMaster module)
        {
            var item = _unitOfWork.GenericRepository<OrderMaster>().GetById(module.Id);
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<OrderMaster>().Update(item);
            return _unitOfWork.Save();
        }

        public int DeleteOrderMaster(int id)
        {
            var item = _unitOfWork.GenericRepository<OrderMaster>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<OrderMaster>().Delete(item);
            return _unitOfWork.Save();
        }

        public OrderMaster GetOrderMasterById(int id)
        {
            return _unitOfWork.GenericRepository<OrderMaster>().GetById(id);
        }

        public List<OrderMaster> GetAllOrderMaster()
        {
            return _unitOfWork.GenericRepository<OrderMaster>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
    }
}
