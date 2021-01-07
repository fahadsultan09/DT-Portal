using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using Models.UserRights;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.GeneralSetup
{
    public class DesignationBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public DesignationBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int AddDesignation(Designation module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<Designation>().Insert(module);
            return _unitOfWork.Save();
        }

        public int UpdateDesignation(Designation module)
        {
            var item = _unitOfWork.GenericRepository<Designation>().GetById(module.Id);
            item.DesignationName = module.DesignationName;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<Designation>().Update(item);
            return _unitOfWork.Save();
        }

        public int DeleteDesignation(int id)
        {
            var item = _unitOfWork.GenericRepository<Designation>().GetById(id);
            item.IsDeleted = true;
            _unitOfWork.GenericRepository<Designation>().Delete(item);
            return _unitOfWork.Save();
        }

        public Designation GetDesignationById(int id)
        {
            return _unitOfWork.GenericRepository<Designation>().GetById(id);
        }

        public List<Designation> GetAllDesignation()
        {
            return _unitOfWork.GenericRepository<Designation>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public bool CheckDesignationName(int Id, string DesignationName)
        {
            int? DesignationId = Id == 0 ? null : (int?)Id;
            var model = _unitOfWork.GenericRepository<Designation>().GetAllList().ToList().Where(x => x.IsDeleted == false && x.DesignationName == DesignationName && x.Id != DesignationId || (DesignationId == null && x.Id == null)).FirstOrDefault();
            if (model != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public SelectList DropDownDesignationByUserId(int[] Designation)
        {
            var selectList = GetAllDesignation().Where(x => x.IsActive == true && Designation.Contains(x.Id)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.DesignationName.ToString()
            });

            return new SelectList(selectList, "Value", "Text");
        }


        public SelectList DropDownDesignationList(int SelectedValue)
        {
            var selectList = GetAllDesignation().Where(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.DesignationName.ToString()
            });

            return new SelectList(selectList, "Value", "Text", SelectedValue);
        }

        public MultiSelectList DropDownDesignationMultiList(int[] SelectedValue)
        {
            var selectList = GetAllDesignation().Where(x => x.IsActive == true).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.DesignationName.ToString()
            });

            return new MultiSelectList(selectList, "Value", "Text", selectedValues: SelectedValue);
        }
    }
}
