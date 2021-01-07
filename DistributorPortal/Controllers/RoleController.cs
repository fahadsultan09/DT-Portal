using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
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
            return View(_RoleBLL.GetAllRole());
        }

        public IActionResult List()
        {
            return PartialView("List", _RoleBLL.GetAllRole());
        }

        [HttpGet]
        public IActionResult Add(int id)
        {
            return PartialView("Add", BindRole(id));
        }

        [HttpPost]
        public IActionResult SaveEdit(Role model)
        {
            try
            {
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
                        TempData["Message"] = "Role name already exist";
                        return PartialView("Add", model);
                    }
                }
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
                _RoleBLL.DeleteRole(id);
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