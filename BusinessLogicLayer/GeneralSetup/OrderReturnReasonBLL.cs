using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.GeneralSetup
{
    public class OrderReturnReasonBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<OrderReturnReason> repository;
        public OrderReturnReasonBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<OrderReturnReason>();
        }
        public bool AddOrderReturnReason(OrderReturnReason module)
        {
            module.ReasonName.Trim();
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateOrderReturnReason(OrderReturnReason module)
        {
            var item = repository.GetById(module.Id);
            item.ReasonName = module.ReasonName.Trim();
            item.IsActive = module.IsActive;
            repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public bool DeleteOrderReturnReason(int id)
        {
            var item = repository.GetById(id);
            item.IsDeleted = false;
            repository.Delete(item);
            return _unitOfWork.Save() > 0;
        }
        public OrderReturnReason GetOrderReturnReasonById(int id)
        {
            return repository.GetById(id);
        }
        public List<OrderReturnReason> GetAllOrderReturnReason()
        {
            return repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public bool CheckReasonName(int Id, string ModuleName)
        {
            int? DosageFormId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<OrderReturnReason>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.ReasonName == ModuleName && x.Id != DosageFormId || (DosageFormId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SelectList DropDownOrderReturnReasonList(int SelectedValue)
        {
            var selectList = GetAllOrderReturnReason().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ReasonName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }
        public SelectList DropDownOrderReturnReasonList()
        {
            var selectList = GetAllOrderReturnReason().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.ReasonName.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
