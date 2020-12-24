using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utility.Constant;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace DistributorPortal.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly IConfiguration _IConfiguration;
        public OrderController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _IConfiguration = configuration;
        }
        // GET: Order
        public ActionResult Index()
        {
            return View(_OrderBLL.GetAllOrderMaster());
        }
        public IActionResult List()
        {
            return PartialView("List", _OrderBLL.GetAllOrderMaster());
        }
        [HttpGet]
        public IActionResult Add(int id)
        {
            return PartialView("Add", BindOrderMaster(id));
        }
        [HttpPost]
        public JsonResult SaveEdit(OrderMaster model)
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
                    if (model.OrderDetail is null || model.OrderDetail.Where(x => x.IsRowDeleted == false).Count() == 0)
                    {
                        jsonResponse.Status = false;
                        jsonResponse.Message = "Must add products before creating order.";
                        return Json(new { data = jsonResponse });

                    }
                    string[] permittedExtensions = Common.permittedExtensions;
                    if (model.AttachmentFormFile != null)
                    {
                        var ext = Path.GetExtension(model.AttachmentFormFile.FileName).ToLowerInvariant();

                        if (string.IsNullOrEmpty(ext) || (!permittedExtensions.Contains(ext)))
                        {
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.FileSizeAllowed;
                            return Json(new { data = jsonResponse });
                        }
                        if (model.AttachmentFormFile.Length > Convert.ToInt64(5242880))
                        {
                            jsonResponse.Status = true;
                            jsonResponse.Message = NotificationMessage.FileTypeAllowed;
                            return Json(new { data = jsonResponse });
                        }
                    }
                    if (model.AttachmentFormFile != null)
                    {
                        Tuple<bool, string> tuple = FileUtility.UploadFile(model.AttachmentFormFile, FolderName.Order, FolderPath);

                        if (tuple.Item1)
                        {
                            model.Attachment = tuple.Item2;
                        }
                    }
                    _OrderBLL.Add(model);
                    _unitOfWork.Save();
                    return Json(new { Result = true, model.Id });
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
        private OrderMaster BindOrderMaster(int Id)
        {
            OrderMaster model = new OrderMaster();
            if (Id > 0)
            {
                model = _OrderBLL.GetOrderMasterById(Id);
            }
            else
            {
            }
            model.Distributor = SessionHelper.LoginUser.Distributor;
            model.ProductList = new ProductMasterBLL(_unitOfWork).DropDownProductList();
            return model;
        }

    }
}
