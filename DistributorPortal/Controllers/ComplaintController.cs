using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly NotificationBLL _NotificationBLL;
        private readonly IConfiguration _IConfiguration;
        private readonly Configuration _Configuration;
        private readonly EmailLogBLL _EmailLogBLL;
        private readonly IWebHostEnvironment _env;
        public ComplaintController(IUnitOfWork unitOfWork, IConfiguration IConfiguration, Configuration configuration, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _ComplaintBLL = new ComplaintBLL(_unitOfWork);
            _IConfiguration = IConfiguration;
            _Configuration = configuration;
            _ComplaintSubCategoryBLL = new ComplaintSubCategoryBLL(_unitOfWork);
            _ComplaintUserEmailBLL = new ComplaintUserEmailBLL(_unitOfWork);
            _NotificationBLL = new NotificationBLL(_unitOfWork);
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
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
            return View("ComplaintApproval", BindComplaint(id));
        }
        public IActionResult ComplaintView(string DPID, string RedirectURL)
        {
            TempData["RedirectURL"] = RedirectURL;
            int.TryParse(EncryptDecrypt.Decrypt(DPID), out int id);
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
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    jsonResponse.Message = message;
                    return Json(new { data = jsonResponse });
                }
                else
                {
                    string[] permittedExtensions = Common.permittedExtensions;
                    if (model.FormFile != null)
                    {
                        var ext = Path.GetExtension(model.FormFile.FileName).ToLowerInvariant();

                        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                        {
                            jsonResponse.Status = false;
                            jsonResponse.Message = NotificationMessage.FileTypeAllowed;
                            return Json(new { data = jsonResponse });
                        }
                        if (model.FormFile.Length >= Convert.ToInt64(_Configuration.FileSize))
                        {
                            jsonResponse.Status = false;
                            jsonResponse.Message = NotificationMessage.FileSizeAllowed;
                            return Json(new { data = jsonResponse });
                        }

                        Tuple<bool, string> tuple = FileUtility.UploadFile(model.FormFile, FolderName.Complaint, FolderPath);
                        if (tuple.Item1)
                        {
                            model.File = tuple.Item2;
                        }
                    }
                    model.Status = ComplaintStatus.Pending;
                    model.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
                    _ComplaintBLL.Add(model);
                    if (model.Id > 0)
                    {
                        _ComplaintBLL.UpdateSNo(model);
                    }
                }

                //Sending Email
                string EmailTemplate = _env.WebRootPath + "\\Attachments\\EmailTemplates\\NewComplaint.html";
                ComplaintEmailUserModel EmailUserModel = new ComplaintEmailUserModel();
                List<User> UserList = new List<User>();

                ComplaintSubCategory complaintSubCategory = _ComplaintSubCategoryBLL.Where(x => x.Id == model.ComplaintSubCategoryId).FirstOrDefault();
                if (complaintSubCategory != null)
                {
                    UserList.Add(complaintSubCategory.User);
                    EmailUserModel.Day = complaintSubCategory.KPIDay.ToString();
                    EmailUserModel.ComplaintCategory = complaintSubCategory.ComplaintCategory.ComplaintCategoryName + " - " + complaintSubCategory.ComplaintSubCategoryName;
                }
                EmailUserModel.ToAcceptTemplate = System.IO.File.ReadAllText(EmailTemplate);
                EmailUserModel.ComplaintNo = model.SNo.ToString();
                EmailUserModel.DistributorName = SessionHelper.LoginUser.Distributor.DistributorName;
                EmailUserModel.ComplaintDetail = model.Description.Trim();
                EmailUserModel.URL = _Configuration.URL;
                EmailUserModel.Attachment = string.IsNullOrEmpty(model.File) ? null : model.File.Split('_')[1];
                EmailUserModel.AttachmentPath = string.IsNullOrEmpty(model.File) ? null : _Configuration.URL + "Other/GetFile?filepath=" + model.File;
                EmailUserModel.ComplaintDate = DateTime.Now.ToString("dd/MMM/yyyy");
                EmailUserModel.CreatedBy = SessionHelper.LoginUser.Id;
                EmailUserModel.Subject = "New Customer Complaint (No. " + EmailUserModel.ComplaintNo.ToString() + ")";
                EmailUserModel.CCEmail = string.Join(',', _ComplaintUserEmailBLL.Where(x => x.ComplaintSubCategoryId == model.ComplaintSubCategoryId && x.EmailType == EmailType.CC).Select(x => x.UserEmailId).ToArray());

                _EmailLogBLL.ComplaintEmail(UserList, EmailUserModel);

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
        public JsonResult UpdateStatus(int Id, ComplaintStatus Status, string Remarks, IFormFile FormFile, string ResolvedRemarks)
        {
            JsonResponse jsonResponse = new JsonResponse();
            Notification notification = new Notification();
            string FolderPath = _IConfiguration.GetSection("Settings").GetSection("FolderPath").Value;
            try
            {
                Complaint model = _ComplaintBLL.GetById(Id);
                if (FormFile != null)
                {
                    var ext = Path.GetExtension(FormFile.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = NotificationMessage.FileTypeAllowed;
                        return Json(new { data = jsonResponse });
                    }
                    if (FormFile.Length >= Convert.ToInt64(_Configuration.FileSize))
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = NotificationMessage.FileSizeAllowed;
                        return Json(new { data = jsonResponse });
                    }

                    Tuple<bool, string> tuple = FileUtility.UploadFile(FormFile, FolderName.Order, FolderPath);
                    if (tuple.Item1)
                    {
                        model.ResolvedAttachment = tuple.Item2;
                    }
                }
                if (model != null)
                {
                    _ComplaintBLL.UpdateStatus(model, Status, Remarks, ResolvedRemarks);
                }
                jsonResponse.Message = Status == ComplaintStatus.Resolved ? NotificationMessage.Resolved : Status == ComplaintStatus.Approved ? NotificationMessage.ComplaintApproved : NotificationMessage.ComplaintRejected;
                if (Status == ComplaintStatus.Approved)
                {
                    jsonResponse.SignalRResponse = new SignalRResponse() { UserId = model.CreatedBy.ToString(), Number = "Request #: " + model.SNo, Message = jsonResponse.Message, Status = Enum.GetName(typeof(PaymentStatus), model.Status) };
                    notification.ApplicationPageId = (int)ApplicationPages.Complaint;
                    notification.DistributorId = model.DistributorId;
                    notification.RequestId = model.SNo;
                    notification.Status = model.Status.ToString();
                    notification.Message = jsonResponse.SignalRResponse.Message;
                    notification.URL = "/Complaint/ComplaintView?DPID=" + EncryptDecrypt.Encrypt(Id.ToString());
                    _NotificationBLL.Add(notification);
                }
                _unitOfWork.Save();
                jsonResponse.Status = true;
                jsonResponse.Message = Status == ComplaintStatus.Resolved ? NotificationMessage.Resolved : Status == ComplaintStatus.Approved ? NotificationMessage.ComplaintApproved : NotificationMessage.ComplaintRejected;
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
                if (SessionHelper.LoginUser.IsDistributor)
                {
                    model.ComplaintList = model.ComplaintList.Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                }
                else
                {
                    int[] ComplaintSubCategoryIds = _ComplaintSubCategoryBLL.Where(x => x.UserEmailTo == SessionHelper.LoginUser.Id).Select(x => x.Id).ToArray();
                    model.ComplaintList = model.ComplaintList.Where(x => ComplaintSubCategoryIds.Contains(x.ComplaintSubCategoryId)).ToList();
                }
            }
            return PartialView("List", model.ComplaintList);
        }
        public List<Complaint> GetComplaintList()
        {
            var list = _ComplaintBLL.Where(x => x.IsActive && !x.IsDeleted && SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            return list;
        }
    }
}
