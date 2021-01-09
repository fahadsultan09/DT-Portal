using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.UserRights;
using System;
using System.Linq;

namespace DistributorPortal.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleBLL _RoleBLL;
        public RoleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _RoleBLL = new RoleBLL(_unitOfWork);
        }
        // GET: Role
        public IActionResult Index()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Role", "Index", " Form");
            return View(_RoleBLL.GetAllRole());
        }
        public IActionResult List()
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Role", "Add", "Click on Add  Button of ");
            return PartialView("List", _RoleBLL.GetAllRole());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            new AuditTrailBLL(_unitOfWork).AddAuditTrail("Role", "Add", "Click on Add  Button of ");
            return PartialView("Add", BindRole(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Role model)
        {
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Role", "SaveEdit", "Start Click on SaveEdit Button of ");
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = NotificationMessage.RequiredFieldsValidation;
                    return PartialView("Add", model);
                }
                else
                {
                    if (_RoleBLL.CheckRoleName(model.Id, model.RoleName))
                    {
                        if (model.Id > 0)
                        {
                            _RoleBLL.UpdateRole(model);
                            TempData["Message"] = NotificationMessage.UpdateSuccessfully;
                        }
                        else
                        {
                            _RoleBLL.AddRole(model);
                            TempData["Message"] = NotificationMessage.SaveSuccessfully;
                        }
                    }
                    else
                    {
                        new AuditTrailBLL(_unitOfWork).AddAuditTrail("Role", "SaveEdit", "End Click on Save Button of ");
                        TempData["Message"] = "Role name already exist";
                        return PartialView("Add", model);
                    }
                }
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Role", "SaveEdit", "End Click on Save Button of ");
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Role", "Delete", "Start Click on Delete Button of ");
                _RoleBLL.DeleteRole(id);
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("Role", "Delete", "End Click on Delete Button of ");
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                TempData["Message"] = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private Role BindRole(int Id)
        {
            Role model = new Role();
            if (Id > 0)
            {
                model = _RoleBLL.GetRoleById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            return model;
        }
        public JsonResult GetRoleList()
        {
            return Json(_RoleBLL.GetAllRole().ToList());
        }
    }
}