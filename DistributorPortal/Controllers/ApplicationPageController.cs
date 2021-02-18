using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
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
    public class ApplicationPageController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationPageBLL _ApplicationPageBLL;
        private readonly ApplicationPageActionBLL _ApplicationPageActionBLL;
        public ApplicationPageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ApplicationPageBLL = new ApplicationPageBLL(_unitOfWork);
            _ApplicationPageActionBLL = new ApplicationPageActionBLL(_unitOfWork);
        }
        // GET: ApplicationPage
        public IActionResult Index()
        {
            return View(_ApplicationPageBLL.GetAllApplicationPage());
        }
        public IActionResult List()
        {
            return PartialView("List", _ApplicationPageBLL.GetAllApplicationPage());
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id=0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return PartialView("Add", BindApplicationPage(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(ApplicationPage model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = NotificationMessage.RequiredFieldsValidation;
                    return Json(new { data = jsonResponse });
                }
                else
                {
                    if (_ApplicationPageBLL.CheckApplicationPageName(model.Id, model.PageTitle))
                    {
                        var ApplicationPageActions = _ApplicationPageActionBLL.Where(e => e.ApplicationPageId == model.Id).ToList();
                        ApplicationPageActions.ForEach(x => x.ApplicationAction = null);
                        ApplicationPageActions.ForEach(x => x.ApplicationPage = null);
                        _ApplicationPageActionBLL.DeleteRange(ApplicationPageActions);
                        if (model.Id > 0)
                        {
                            _ApplicationPageBLL.UpdateApplicationPage(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.UpdateSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "ApplicationPage");
                        }
                        else
                        {
                            _ApplicationPageBLL.AddApplicationPage(model);
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                            jsonResponse.RedirectURL = Url.Action("Index", "ApplicationPage");
                        }

                        if (model.ApplicationActionsId != null)
                        {
                            foreach (var item in model.ApplicationActionsId)
                            {
                                ApplicationPageAction ApplicationPageAction = new ApplicationPageAction()
                                {
                                    ApplicationPageId = model.Id,
                                    ApplicationActionId = item,
                                    CreatedBy = SessionHelper.LoginUser.Id,
                                    CreatedDate = DateTime.Now,
                                };
                                _ApplicationPageActionBLL.AddApplicationPageAction(ApplicationPageAction);
                            }
                        }
                        return Json(new { data = jsonResponse });
                    }
                    else
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Application page name already exist";
                        return Json(new { data = jsonResponse });
                    }
                }
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
                int id=0;
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                _ApplicationPageBLL.DeleteApplicationPage(id);
                return Json(new { Result = true });
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                ViewBag.Records = NotificationMessage.ErrorOccurred;
                return RedirectToAction("List");
            }
        }
        private ApplicationPage BindApplicationPage(int Id)
        {
            ApplicationPage model = new ApplicationPage();
            if (Id > 0)
            {
                model = _ApplicationPageBLL.GetApplicationPageById(Id);
            }
            else
            {
                model.IsActive = true;
            }
            int[] ApplicationActionsId = new ApplicationPageActionBLL(_unitOfWork).GetAllApplicationPageActionByApplicationPageId(Id);
            model.ApplicationModuleList = new ApplicationModuleBLL(_unitOfWork).DropDownApplicationModuleList(model.ApplicationModuleId);
            model.ApplicationActionsList = new ApplicationActionBLL(_unitOfWork).DropDownActionNameList(ApplicationActionsId);
            model.ApplicationActionsId = ApplicationActionsId;
            return model;
        }
        public JsonResult GetApplicationPageList()
        {
            return Json(_ApplicationPageBLL.GetAllApplicationPage().ToList());
        }
    }
}