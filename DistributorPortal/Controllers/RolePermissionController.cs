using BusinessLogicLayer.Application;
using BusinessLogicLayer.FormLogic;
using DataAccessLayer.WorkProcess;
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
            new AuditLogBLL(_unitOfWork).AddAuditLog("RolePermission", "Index", " Form");
            return View(_rolePermissionLogic.GetPermissionList(Id));
        }
        public IActionResult List(int Id)
        {
            new AuditLogBLL(_unitOfWork).AddAuditLog("RolePermission", "Add", "Click on Add  Button of ");
            return PartialView(_rolePermissionLogic.GetPermissionList(Id));
        }

        [HttpPost]
        public IActionResult UpdatePermission(Permission models)
        {
            try
            {
                new AuditLogBLL(_unitOfWork).AddAuditLog("RolePermission", "UpdatePermission", "Start Click on Delete Button of ");
                var data = _rolePermissionLogic.UpdatePermission(models);
                TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                new AuditLogBLL(_unitOfWork).AddAuditLog("RolePermission", "UpdatePermission", "End Click on Delete Button of ");
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