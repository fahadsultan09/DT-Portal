using BusinessLogicLayer.Application;
using BusinessLogicLayer.FormLogic;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModel;
using System;
using Utility.HelperClasses;

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
        public IActionResult Index(string DPID)
        {
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            return View(_rolePermissionLogic.GetPermissionList(id));
        }
        public IActionResult List(string DPID)
        {
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            return PartialView(_rolePermissionLogic.GetPermissionList(id));
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