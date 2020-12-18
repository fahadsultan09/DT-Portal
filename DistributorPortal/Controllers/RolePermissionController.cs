using BusinessLogicLayer.FormLogic;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Controllers;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModel;
using System;

namespace DistributorPortal.Controllers
{
    public class RolePermissionController : BaseController
    {
        private readonly RolePermissionLogic _rolePermissionLogic;
        private readonly IUnitOfWork _unitOfWork;
        public RolePermissionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _rolePermissionLogic = new RolePermissionLogic(_unitOfWork);
        }

        // GET: Role Permission
        public IActionResult Index(int Id)
        {
            return View(_rolePermissionLogic.GetPermissionList(Id));
        }
        public IActionResult List(int Id)
        {
            return PartialView(_rolePermissionLogic.GetPermissionList(Id));
        }

        [HttpPost]
        public IActionResult UpdatePermission(Permission models)
        {
            try
            {
                var data = _rolePermissionLogic.UpdatePermission(models);
                TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                return PartialView("List", _rolePermissionLogic.GetPermissionList(models.RoleId));
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Index", "Role");
            }

        }
    }
}