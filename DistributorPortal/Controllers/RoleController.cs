using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.UserRights;
using Models.ViewModel;
using System;
using System.Linq;
using Utility.HelperClasses;

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
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindRole(id));
        }
        [HttpPost]
        public IActionResult SaveEdit(Role model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                TempData["Message"] = string.Empty;
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
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Role");
                        }
                        else
                        {
                            _RoleBLL.AddRole(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "Role");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Role name already exist";
                        return PartialView("Add", model);
                    }
                }
                return Json(new { data = jsonResponse });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                return Json(new { data = jsonResponse });
            }
        }
        [HttpPost]
        public IActionResult Delete(string DPID)
        {
            try
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
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