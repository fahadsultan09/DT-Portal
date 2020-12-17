using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Controllers;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System;
using System.Linq;
using Utility.HelperClasses;

namespace UserPortal.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserBLL _UserBLL;
        private readonly Configuration _Configuration;
        public UserController(IUnitOfWork unitOfWork, Configuration configuration)
        {
            _unitOfWork = unitOfWork;
            _UserBLL = new UserBLL(_unitOfWork);
            _Configuration = configuration;
        }
        // GET: User
        public ActionResult Index()
        {
            return View(_UserBLL.GetAllUser());
        }
        public IActionResult List()
        {
            return PartialView("List", _UserBLL.GetAllUser());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            return PartialView("Add", BindUser(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(User model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = NotificationMessage.RequiredFieldsValidation;
                }
                else
                {
                    if (_UserBLL.CheckUserName(model.Id, model.UserName))
                    {
                        if (model.Id > 0)
                        {
                            _UserBLL.UpdateUser(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "User");
                        }
                        else
                        {
                            model.Password = EncryptDecrypt.Encrypt(_Configuration.ResetPassword);
                            _UserBLL.AddUser(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "User");
                        }
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "User name already exist";
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
        public IActionResult Delete(int id)
        {
            try
            {
                _UserBLL.DeleteUser(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                ViewBag.Records = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private User BindUser(int Id)
        {
            User model = new User();
            if (Id > 0)
            {
                model = _UserBLL.GetUserById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            model.RoleList = new RoleBLL(_unitOfWork).DropDownRoleList(model.RoleId);
            model.DesignationList = new DesignationBLL(_unitOfWork).DropDownDesignationList(Convert.ToInt32(model.DesignationId));
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(Convert.ToInt32(model.DistributorId));
            model.CityList = new CityBLL(_unitOfWork).DropDownCityList(Convert.ToInt32(model.CityId));
            return model;
        }
        public JsonResult GetUserList()
        {
            return Json(_UserBLL.GetAllUser().ToList());
        }
        [HttpPost]
        public IActionResult ResetPassword(int id)
        {
            try
            {
                string password = EncryptDecrypt.Encrypt(_Configuration.ResetPassword);
                _UserBLL.ResetPassword(id, password);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                ViewBag.Records = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }

    }
}