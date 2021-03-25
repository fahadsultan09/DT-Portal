using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;
using Utility.Constant;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace DistributorPortal.Controllers
{
    public class ComplaintController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ComplaintBLL _ComplaintBLL;
        private readonly ComplaintSubCategoryBLL _ComplaintSubCategoryBLL;
        private readonly ComplaintUserEmailBLL _ComplaintUserEmailBLL;
        private readonly IConfiguration _IConfiguration;
        private readonly Configuration _Configuration;
        private readonly EmailLogBLL _EmailLogBLL;
        private readonly IHostingEnvironment _env;
        public ComplaintController(IUnitOfWork unitOfWork, IConfiguration IConfiguration, Configuration configuration, IHostingEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _ComplaintBLL = new ComplaintBLL(_unitOfWork);
            _IConfiguration = IConfiguration;
            _Configuration = configuration;
            _ComplaintSubCategoryBLL = new ComplaintSubCategoryBLL(_unitOfWork);
            _ComplaintUserEmailBLL = new ComplaintUserEmailBLL(_unitOfWork);
            _env = env;
            _EmailLogBLL = new EmailLogBLL(_unitOfWork, _Configuration);
        }
        // GET: Complaint
        public IActionResult Index()
        {
            ComplaintViewModel model = new ComplaintViewModel();
            model.DistributorList = new DistributorBLL(_unitOfWork).DropDownDistributorList(null);

            if (SessionHelper.LoginUser.IsDistributor)
            {
                model.ComplaintList = GetComplaintList().Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                return View(model);
            }
            if (SessionHelper.NavigationMenu.Where(x => x.ApplicationPage.ControllerName == "ComplaintSubCategory").Select(x => x.ApplicationAction.Id).Contains((int)ApplicationActions.IsAdmin))
            {
                model.ComplaintList = GetComplaintList();
            }
            else
            {
                int[] ComplaintSubCategoryIds = _ComplaintSubCategoryBLL.Where(x => x.UserEmailTo == SessionHelper.LoginUser.Id).Select(x => x.Id).ToArray();
                model.ComplaintList = GetComplaintList().Where(x => ComplaintSubCategoryIds.Contains(x.ComplaintSubCategoryId)).ToList();
            }
            return View(model);
        }
        public ComplaintViewModel List(ComplaintViewModel model)
        {
            if (model.DistributorId is null && model.Status is null && model.FromDate is null && model.ToDate is null && model.ComplaintNo is null)
            {
                model.ComplaintList = GetComplaintList();
            }
            else
            {
                model.ComplaintList = _ComplaintBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            }
            return model;
        }
        [HttpGet]
        public IActionResult Add(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            return View("Add", BindComplaint(id));
        }
        [HttpGet]
        public IActionResult ComplaintApproval(string DPID)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            return View("ComplaintApproval", BindComplaint(id));
        }

        public IActionResult ComplaintView(string DPID)
        {
            int id = 0;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            return View("ComplaintView", BindComplaint(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(Complaint model)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string FolderPath = _IConfiguration.GetSection("Settings").GetSection("FolderPath").Value;
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
                    string[] permittedExtensions = Common.permittedExtensions;
                    if (model.FormFile != null)
                    {
                        var ext = Path.GetExtension(model.FormFile.FileName).ToLowerInvariant();
                        if (permittedExtensions.Contains(ext) && model.FormFile.Length < Convert.ToInt64(5242880))
                        {
                            Tuple<bool, string> tuple = FileUtility.UploadFile(model.FormFile, FolderName.Order, FolderPath);
                            if (tuple.Item1)
                            {
                                model.File = tuple.Item2;
                            }
                        }
                    }
                    model.Status = ComplaintStatus.Pending;
                    model.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
                    _ComplaintBLL.Add(model);
                }

                //Sending Email
                string EmailTemplate = _env.WebRootPath + "\\Attachments\\EmailTemplates\\NewComplaint.html";
                EmailUserModel EmailUserModel = new EmailUserModel();
                List<User> UserList = new List<User>();

                ComplaintSubCategory complaintSubCategory = _ComplaintSubCategoryBLL.Where(x => x.Id == model.ComplaintSubCategoryId).FirstOrDefault();
                if (complaintSubCategory != null)
                {
                    UserList.Add(complaintSubCategory.User);
                    EmailUserModel.Day = complaintSubCategory.KPIDay.ToString();
                    EmailUserModel.ComplaintCategory = complaintSubCategory.ComplaintCategory.ComplaintCategoryName + " - " + complaintSubCategory.ComplaintSubCategoryName;
                }
                EmailUserModel.ToAcceptTemplate = System.IO.File.ReadAllText(EmailTemplate);
                EmailUserModel.ComplaintNo = string.Format("{0:1000000000}", model.Id);
                EmailUserModel.DistributorName = SessionHelper.LoginUser.Distributor.DistributorName;
                EmailUserModel.ComplaintDetail = model.Description;
                EmailUserModel.ComplaintDate = DateTime.Now.ToString("dd/MM/yyyy");
                EmailUserModel.CreatedBy = SessionHelper.LoginUser.Id;
                EmailUserModel.CCEmail = string.Join(',', _ComplaintUserEmailBLL.Where(x => x.ComplaintSubCategoryId == model.ComplaintSubCategoryId && x.EmailType == EmailType.CC).Select(x => x.UserEmailId).ToArray());

                _EmailLogBLL.EmailSend(UserList, EmailUserModel);

                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.SaveSuccessfully;
                jsonResponse.RedirectURL = Url.Action("Index", "Complaint");
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
        private Complaint BindComplaint(int Id)
        {
            Complaint model = new Complaint();
            if (Id > 0)
            {
                model = _ComplaintBLL.GetById(Id);
            }
            else
            {

            }
            model.Distributor = SessionHelper.LoginUser.Distributor;
            model.ComplaintCategoryList = new ComplaintCategoryBLL(_unitOfWork).DropDownComplaintCategoryList(model.ComplaintCategoryId);
            model.ComplaintSubCategoryList = new ComplaintSubCategoryBLL(_unitOfWork).DropDownComplaintSubCategoryList(model.ComplaintCategoryId, model.ComplaintSubCategoryId);
            return model;
        }
        [HttpPost]
        public JsonResult UpdateStatus(int Id, ComplaintStatus Status, string Remarks)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                Complaint model = _ComplaintBLL.GetById(Id);
                if (model != null)
                {
                    _ComplaintBLL.UpdateStatus(model, Status, Remarks);
                }
                _unitOfWork.Save();
                jsonResponse.Status = true;
                jsonResponse.Message = NotificationMessage.Resolved;
                jsonResponse.RedirectURL = Url.Action("Index", "Complaint");
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
        public IActionResult Search(ComplaintViewModel model, string Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                model = List(model);
            }
            else
            {
                model.ComplaintList = GetComplaintList();
            }
            if (SessionHelper.NavigationMenu.Where(x => x.ApplicationPage.ControllerName == "ComplaintSubCategory").Select(x => x.ApplicationAction.Id).Contains((int)ApplicationActions.IsAdmin))
            {
                model.ComplaintList = model.ComplaintList;
            }
            else
            {
                if (!SessionHelper.LoginUser.IsDistributor)
                {
                    int[] ComplaintSubCategoryIds = _ComplaintSubCategoryBLL.Where(x => x.UserEmailTo == SessionHelper.LoginUser.Id).Select(x => x.Id).ToArray();
                    model.ComplaintList = GetComplaintList().Where(x => ComplaintSubCategoryIds.Contains(x.ComplaintSubCategoryId)).ToList();
                }
            }
            return PartialView("List", model.ComplaintList);
        }
        public List<Complaint> GetComplaintList()
        {
            var list = _ComplaintBLL.GetAllComplaint().Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).OrderByDescending(x => x.Id).ToList();
            return list;
        }
    }
}
