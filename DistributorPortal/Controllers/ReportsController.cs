using BusinessLogicLayer.Application;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using Utility.HelperClasses;

namespace DistributorPortal.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OrderBLL _OrderBLL;
        private readonly OrderDetailBLL _OrderDetailBLL;
        private readonly OrderReturnBLL _OrderReturnBLL;
        private readonly OrderReturnDetailBLL _OrderReturnDetailBLL;
        private readonly PaymentBLL _PaymentBLL;
        private readonly ComplaintBLL _ComplaintBLL;
        private readonly OrderValueBLL _OrderValueBLL;
        private readonly DistributorWiseProductDiscountAndPricesBLL _DistributorWiseProductDiscountAndPricesBLL;
        private readonly ProductDetailBLL _ProductDetailBLL;
        public ReportsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _OrderBLL = new OrderBLL(_unitOfWork);
            _OrderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _OrderReturnBLL = new OrderReturnBLL(_unitOfWork);
            _OrderReturnDetailBLL = new OrderReturnDetailBLL(_unitOfWork);
            _PaymentBLL = new PaymentBLL(_unitOfWork);
            _ComplaintBLL = new ComplaintBLL(_unitOfWork);
            _OrderValueBLL = new OrderValueBLL(_unitOfWork);
            _DistributorWiseProductDiscountAndPricesBLL = new DistributorWiseProductDiscountAndPricesBLL(_unitOfWork);
            _ProductDetailBLL = new ProductDetailBLL(_unitOfWork);
        }

        #region Order
        public IActionResult Order()
        {
            return View();
        }
        public List<OrderMaster> GetOrderList(OrderSearch model)
        {
            List<OrderMaster> list = _OrderBLL.Search(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : x.Status != OrderStatus.Canceled && x.Status != OrderStatus.Draft).ToList();
            return list;
        }
        [HttpPost]
        public IActionResult OrderSearch(OrderSearch model, string Search)
        {
            if (Search == "Search")
            {
                return PartialView("OrderList", GetOrderList(model));
            }
            else
            {
                return PartialView("OrderList", GetOrderList(model));
            }
        }
        [HttpGet]
        public IActionResult GetOrderDetailList(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            var Detail = _OrderDetailBLL.GetOrderDetailByIdByMasterId(id);
            return PartialView("OrderDetailList", Detail);
        }
        #endregion

        #region OrderReturn
        public IActionResult OrderReturn()
        {
            return View();
        }
        public List<OrderReturnMaster> GetOrderReturnList(OrderReturnSearch model)
        {
            List<OrderReturnMaster> list = _OrderReturnBLL.SearchReport(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            return list;
        }
        [HttpPost]
        public IActionResult OrderReturnSearch(OrderReturnSearch model, string Search)
        {
            if (Search == "Search")
            {
                return PartialView("OrderReturnList", GetOrderReturnList(model));
            }
            else
            {
                return PartialView("OrderReturnList", GetOrderReturnList(model));
            }
        }
        [HttpGet]
        public IActionResult GetOrderReturnDetailList(string DPID)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(DPID))
            {
                int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
            }
            var Detail = _OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id);
            return PartialView("OrderReturnDetailList", Detail);
        }
        #endregion

        #region Payment
        public IActionResult Payment()
        {
            PaymentSearch paymentSearch = new PaymentSearch();
            paymentSearch.PaymentMasters = GetPaymentList(new PaymentSearch());
            return View(paymentSearch);
        }
        public List<PaymentMaster> GetPaymentList(PaymentSearch model)
        {
            List<PaymentMaster> list = _PaymentBLL.SearchReport(model).Where(x => SessionHelper.LoginUser.IsDistributor ? x.DistributorId == SessionHelper.LoginUser.DistributorId : SessionHelper.LoginUser.CompanyId != null ? x.CompanyId == SessionHelper.LoginUser.CompanyId : true).ToList();
            return list;
        }
        [HttpPost]
        public IActionResult PaymentSearch(PaymentSearch model, string Search)
        {
            if (Search == "Search")
            {
                return PartialView("PaymentList", GetPaymentList(model));
            }
            else
            {
                return PartialView("PaymentList", GetPaymentList(model));
            }
        }
        #endregion

        #region Complain
        public IActionResult Complaint()
        {
            return View();
        }
        public List<Complaint> GetComplaintList(ComplaintSearch model)
        {
            //List<Complaint> list = _ComplaintBLL.SearchReport(model).Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : true).ToList();
            //return list;
            List<Complaint> ComplaintList = new List<Complaint>();

            if (SessionHelper.LoginUser.IsDistributor)
            {
                ComplaintList = _ComplaintBLL.SearchReport(model).Where(x => x.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                return ComplaintList;
            }
            if (SessionHelper.NavigationMenu.Where(x => x.ApplicationPage.ControllerName == "ComplaintSubCategory").Select(x => x.ApplicationAction.Id).Contains((int)ApplicationActions.IsAdmin))
            {
                ComplaintList = _ComplaintBLL.SearchReport(model);
            }
            else
            {
                int[] ComplaintSubCategoryIds = new ComplaintSubCategoryBLL(_unitOfWork).Where(x => x.UserEmailTo == SessionHelper.LoginUser.Id).Select(x => x.Id).ToArray();
                ComplaintList = _ComplaintBLL.SearchReport(model).Where(x => ComplaintSubCategoryIds.Contains(x.ComplaintSubCategoryId)).ToList();
            }
            return ComplaintList;
        }
        [HttpPost]
        public IActionResult ComplaintSearch(ComplaintSearch model, string Search)
        {
            if (Search == "Search")
            {
                return PartialView("ComplaintList", GetComplaintList(model));
            }
            else
            {
                return PartialView("ComplaintList", GetComplaintList(model));
            }
        }
        #endregion

        #region Common
        public IActionResult Print(ApplicationPages ApplicationPage, string DPID, string SiteTRNo, string KorangiTRNo, string SITEPhytek, string KorangiPhytek)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                List<OrderReturnDetail> List = new List<OrderReturnDetail>();
                int id = 0;
                if (!string.IsNullOrEmpty(DPID))
                {
                    int.TryParse(EncryptDecrypt.Decrypt(DPID), out id);
                }
                switch (ApplicationPage)
                {
                    case ApplicationPages.Order:

                        return new ViewAsPdf("PrintOrder", BindOrderMaster(id))
                        {
                            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                            CustomSwitches = "--footer-left \" Use Controlled Copy Only \" --footer-center \" [page]/[toPage] \" --footer-right \" For Internal Use Only \"" +
                            " --footer-line --footer-font-size \"12\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
                        };

                    case ApplicationPages.OrderReturn:
                        if (!string.IsNullOrEmpty(SiteTRNo))
                        {
                            List<OrderReturnDetail> OrderReturnDetailList = _OrderReturnDetailBLL.Where(x => x.OrderReturnId == id && x.PlantLocationId == 1).ToList();
                            OrderReturnDetailList.ForEach(x => x.TRNo = SiteTRNo);
                            _OrderReturnDetailBLL.UpdateRange(OrderReturnDetailList);
                            List.AddRange(_OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => x.OrderReturnId == id && x.PlantLocationId == 1).ToList());
                        }
                        if (!string.IsNullOrEmpty(KorangiTRNo))
                        {
                            List<OrderReturnDetail> OrderReturnDetailList = _OrderReturnDetailBLL.Where(x => x.OrderReturnId == id && x.PlantLocationId == 2).ToList();
                            OrderReturnDetailList.ForEach(x => x.TRNo = KorangiTRNo);
                            _OrderReturnDetailBLL.UpdateRange(OrderReturnDetailList);
                            List.AddRange(_OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => x.OrderReturnId == id && x.PlantLocationId == 2).ToList());
                        }
                        if (!string.IsNullOrEmpty(SITEPhytek))
                        {
                            List<OrderReturnDetail> OrderReturnDetailList = _OrderReturnDetailBLL.Where(x => x.OrderReturnId == id && x.PlantLocationId == 3).ToList();
                            OrderReturnDetailList.ForEach(x => x.TRNo = SITEPhytek);
                            _OrderReturnDetailBLL.UpdateRange(OrderReturnDetailList);
                            List.AddRange(_OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => x.OrderReturnId == id && x.PlantLocationId == 3).ToList());
                        }
                        if (!string.IsNullOrEmpty(KorangiPhytek))
                        {
                            List<OrderReturnDetail> OrderReturnDetailList = _OrderReturnDetailBLL.Where(x => x.OrderReturnId == id && x.PlantLocationId == 4).ToList();
                            OrderReturnDetailList.ForEach(x => x.TRNo = KorangiPhytek);
                            _OrderReturnDetailBLL.UpdateRange(OrderReturnDetailList);
                            List.AddRange(_OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => x.OrderReturnId == id && x.PlantLocationId == 4).ToList());
                        }
                        if (string.IsNullOrEmpty(KorangiTRNo) && string.IsNullOrEmpty(SiteTRNo) && string.IsNullOrEmpty(SITEPhytek) && string.IsNullOrEmpty(KorangiPhytek))
                        {
                            List = _OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id).Where(x => x.OrderReturnId == id).ToList();
                        }
                        //return View("PrintOrderReturn", _OrderReturnDetailBLL.GetOrderDetailByIdByMasterId(id));
                        return new ViewAsPdf("PrintOrderReturn", List)
                        {
                            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                            CustomSwitches = "--footer-left \" Use Controlled Copy Only \" --footer-center \" [page]/[toPage] \" --footer-right \" For Internal Use Only \"" +
                            " --footer-line --footer-font-size \"12\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
                            //CustomSwitches = "--footer-center \" Created Date: " + DateTime.Now.Date.ToString("dd/MM/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"12\" --footer-spacing 1 --footer-font-name \"Segoe UI\""
                        };

                }
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                return Json(new { data = jsonResponse });

            }
            catch (Exception ex)
            {
                jsonResponse.Status = false;
                jsonResponse.Message = NotificationMessage.ErrorOccurred;
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return Json(new { data = jsonResponse });
            }
        }

        private OrderMaster BindOrderMaster(int Id)
        {
            OrderMaster model = _OrderBLL.GetOrderMasterById(Id);
            List<DistributorWiseProductDiscountAndPrices> DistributorWiseProductDiscountAndPricesList = _DistributorWiseProductDiscountAndPricesBLL.Where(x => x.DistributorId == model.DistributorId).ToList();
            model.OrderDetail = _OrderDetailBLL.GetOrderDetailByIdByMasterId(Id);
            model.OrderDetail.ForEach(x => x.ProductDetail = _ProductDetailBLL.FirstOrDefault(y => y.ProductMasterId == x.ProductMaster.Id) != null ? _ProductDetailBLL.FirstOrDefault(y => y.ProductMasterId == x.ProductMaster.Id) : null);
            model.OrderValueViewModel = _OrderBLL.GetOrderValueModel(_OrderValueBLL.GetOrderValueByOrderId(Id));
            return model;
        }
        #endregion
        #region Customer Ledger
        public IActionResult CustomerLedger()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CustomerLedgerViewPDF(CustomerLedgerSeach customerLedgerSeach)
        {
            ViewBag.CustomerLedgerViewPDF = "JVBERi0xLjMNCiXi48/TDQolUlNUWFBERjMgUGFyYW1ldGVyczogRFJTVFhoDQoyIDAgb2JqDQo8PA0KL1R5cGUgL0ZvbnREZXNjcmlwdG9yDQovQXNjZW50IDcyMA0KL0NhcEhlaWdodCA2NjANCi9EZXNjZW50IC0yNzANCi9GbGFncyAzNA0KL0ZvbnRCQm94IFstMTc3IC0yNjkgMTEyMyA4NjZdDQovRm9udE5hbWUgL1RpbWVzLVJvbWFuDQovSXRhbGljQW5nbGUgMA0KL1N0ZW1WIDEwNQ0KPj4NCmVuZG9iag0KMyAwIG9iag0KL1dpbkFuc2lFbmNvZGluZw0KZW5kb2JqDQo0IDAgb2JqDQo8PA0KJURldnR5cGUgU0FQV0lOICAgRm9udCBUSU1FUyAgICBub3JtYWwgTGFuZyBFTg0KL1R5cGUgL0ZvbnQNCi9TdWJ0eXBlIC9UeXBlMQ0KL0Jhc2VGb250IC9UaW1lcy1Sb21hbg0KL05hbWUgL0YwMDENCi9FbmNvZGluZyAzIDAgUg0KL0ZpcnN0Q2hhciAzMg0KL0xhc3RDaGFyIDI1NQ0KJUNoYXJ3aWR0aCB2YWx1ZXMgZnJvbSBTQVBXSU4gVElNRVMgMDgwIG5vcm1hbA0KL1dpZHRocw0KWyAyNTAgMzMxIDQwMCA1MDAgNTAwIDgzMSA3NjkgMTY5IDMzMSAzMzEgNTAwIDU2MyAyNTAgMzMxIDI1MCAyODEgNTAwIDUwMCA1MDAgNTAwIDUwMCA1MDAgNTAwIDUwMCA1MDAgNTAwIDI4MSAyODEgNTYzIDU2MyA1NjMgNDYzIDkxOSA3MTkgNjYzIDY2OSA3MTkgNjEzIDU1MCA3MTkgNzE5IDMxMyA0MDAgNzE5IDU4OCA4ODggNzE5IDcxOSA1NjMgNzE5IDY2OSA1NjMgNjEzIDcxOSA3MTkgOTUwIDcxOSA3MTkgNjAwIDMzOCAyNjMgMzM4IDQ2OQ0KIDUwMCAzMTkgNDM4IDUwMCA0MzggNTAwIDQzOCAzMzEgNDg4IDUwMCAyODEgMjgxIDUwMCAyODEgNzgxIDUwMCA1MDAgNTAwIDUwMCAzMzEgMzg4IDI4MSA1MDAgNTAwIDcxOSA1MTMgNDY5IDQ1MCA0ODEgMTgxIDQ4MSA1MzggMCA1MDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDMzMSA1MDAgNTAwIDUwMCA1MDAgMTgxIDUwMCAzNTAgNzYzIDI4MSA0NjMgNTYzIDUwMCA3NjMNCiA1MDAgNDAwIDU1MCAzMDAgMzAwIDMxOSA1ODEgNDUwIDI1MCAyODggMzAwIDMxMyA0NjMgNzUwIDc1MCA3NTAgNDUwIDcxOSA3MTkgNzE5IDcxOSA3MTkgNzE5IDg2MyA2NjkgNjEzIDYxMyA2MTMgNjEzIDMxMyAzMTMgMzEzIDMxMyA3MTkgNzE5IDcxOSA3MTkgNzE5IDcxOSA3MTkgNTYzIDcxOSA3MTkgNzE5IDcxOSA3MTkgNzE5IDU2MyA1MDAgNDM4IDQzOCA0MzggNDM4IDQzOCA0MzggNjY5IDQzOCA0MzggNDM4IDQzOCA0MzggMjgxIDI4MQ0KIDI4MSAyODEgNTAwIDUwMCA1MDAgNTAwIDUwMCA1MDAgNTAwIDU1MCA1MDAgNTAwIDUwMCA1MDAgNTAwIDQ2OSA0ODggNDY5XQ0KL0ZvbnREZXNjcmlwdG9yIDIgMCBSDQo+Pg0KZW5kb2JqDQo1IDAgb2JqDQo8PA0KL1R5cGUgL0ZvbnREZXNjcmlwdG9yDQovQXNjZW50IDcyMA0KL0NhcEhlaWdodCA2NjANCi9EZXNjZW50IC0yNzANCi9GbGFncyAzNA0KL0ZvbnRCQm94IFstMTc3IC0yNjkgMTEyMyA4NjZdDQovRm9udE5hbWUgL1RpbWVzLUJvbGQNCi9JdGFsaWNBbmdsZSAwDQovU3RlbVYgMTA1DQo+Pg0KZW5kb2JqDQo2IDAgb2JqDQo8PA0KJURldnR5cGUgU0FQV0lOICAgRm9udCBUSU1FUyAgICBib2xkIExhbmcgRU4NCi9UeXBlIC9Gb250DQovU3VidHlwZSAvVHlwZTENCi9CYXNlRm9udCAvVGltZXMtQm9sZA0KL05hbWUgL0YwMDINCi9FbmNvZGluZyAzIDAgUg0KL0ZpcnN0Q2hhciAzMg0KL0xhc3RDaGFyIDI1NQ0KJUNoYXJ3aWR0aCB2YWx1ZXMgZnJvbSBTQVBXSU4gVElNRVMgMTQwIGJvbGQNCi9XaWR0aHMNClsgMjUwIDMyOSA1NjEgNTAwIDUwMCAxMDExIDgyOSAyNzkgMzI5IDMyOSA1MDAgNTcxIDI1MCAzMjkgMjUwIDI3OSA1MDAgNTAwIDUwMCA1MDAgNTAwIDUwMCA1MDAgNTAwIDUwMCA1MDAgMzI5IDMyOSA1NzEgNTcxIDU3MSA1MDAgOTI5IDcyMSA2NzEgNzIxIDcyMSA2NzEgNjAwIDc3MSA3NzkgMzg5IDUwMCA3NzEgNjcxIDkzOSA3MjEgNzc5IDYwMCA3NzkgNzIxIDU2MSA2NzEgNzIxIDcyMSAxMDAwIDcyMSA3MjEgNjYxIDMyOSAyNzkgMzI5DQogNTc5IDUwMCAzMjkgNTAwIDU2MSA0MzkgNTYxIDQzOSAzMzkgNTAwIDU2MSAyNzkgMzI5IDU2MSAyNzkgODIxIDU2MSA1MDAgNTYxIDU2MSA0MzkgMzg5IDMyOSA1NjEgNTAwIDcyOSA1MDAgNTAwIDQzOSAzODkgMjExIDM4OSA1MjEgMCA1MDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDAgMCAwIDMzOSA1MDAgNTAwIDUwMCA1MDAgMjExIDUwMCAzNjEgNzUwIDMwMCA1MDAgNTcxIDUwMA0KIDc1MCA1MDAgNDAwIDU1MCAzMDAgMzAwIDMyOSA1NzkgNTM5IDIzOSAzMjkgMzAwIDMyOSA1MDAgNzUwIDc1MCA3NTAgNTAwIDcyMSA3MjEgNzIxIDcyMSA3MjEgNzIxIDEwMDAgNzIxIDY3MSA2NzEgNjcxIDY3MSAzODkgMzg5IDM4OSAzODkgNzIxIDcyMSA3NzkgNzc5IDc3OSA3NzkgNzc5IDU3MSA3NzkgNzIxIDcyMSA3MjEgNzIxIDcyMSA2MTEgNTYxIDUwMCA1MDAgNTAwIDUwMCA1MDAgNTAwIDcyMSA0MzkgNDM5IDQzOSA0MzkgNDM5IDI3OQ0KIDI3OSAyNzkgMjc5IDUwMCA1NjEgNTAwIDUwMCA1MDAgNTAwIDUwMCA1NTAgNTAwIDU2MSA1NjEgNTYxIDU2MSA1MDAgNTYxIDUwMF0NCi9Gb250RGVzY3JpcHRvciA1IDAgUg0KPj4NCmVuZG9iag0KNyAwIG9iag0KPDwNCi9GaWx0ZXIgOCAwIFINCi9MZW5n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MjA0QzY1NjQ2NzY1NzI+VGogRVQgcSAwIDAgMCByZyA0OC45NSAwIDAgNTYuMTUgNTAxIDc2OSBjbSAvMDAwMTAgRG8gUSBxIDAuODMxIDAuODMxIDAuODMxIHJnIDE0LjE1IDY2Ni41MCA1NS4zMCAxNC4wMCByZSBmIFEgcSAwLjgzMSAwLjgzMSAwLjgzMSByZyA2OS40NSA2NjYuNTAgNTUuMzAgMTQuMDAgcmUgZiBRIHENCiAwLjgzMSAwLjgzMSAwLjgzMSByZyAxMjQuNzUgNjY2LjUwIDgzLjA1IDE0LjAwIHJlIGYgUSBxIDAuODMxIDAuODMxIDAuODMxIHJnIDIwNy44MCA2NjYuNTAgNzcuNjUgMTQuMDAgcmUgZiBRIHEgMC44MzEgMC44MzEgMC44MzEgcmcgMjg1LjQ1IDY2Ni41MCAyOC4zNSAxNC4wMCByZSBmIFEgcSAwLjgzMSAwLjgzMSAwLjgzMSByZyAzMTMuODAgNjY2LjUwIDg1LjA1IDE0LjAwIHJlIGYgUSBxIDAuODMxIDAuODMxIDAuODMxIHJnDQogMzk4Ljg1IDY2Ni41MCA4NS4wNSAxNC4wMCByZSBmIFEgcSAwLjgzMSAwLjgzMSAwLjgzMSByZyA0ODMuOTAgNjY2LjUwIDg1LjA1IDE0LjAwIHJlIGYgUSAvRjAwMiAxMC4wMCBUZiAwLjAwIFR3IDAgZyBCVCAxNC4xNSA3MzcuMTAgVGQgPDU0NzI2MTZFNzM2MTYzNzQ2OTZGNkUyMDQ0NjE3NDY1PlRqIEVUIC9GMDAxIDEwLjAwIFRmIDAuMDAgVHcgMCBnIEJUIDE1Ny4zMCA3MzcuMTAgVGQNCiA8M0EyMDMwMzEyRTMxMzIyRTMyMzAzMTM3MjA3NDZGMjAzMDM2MkUzMDM3MkUzMjMwMzEzOD5UaiBFVCAvRjAwMiAxMC4wMCBUZiAwLjAwIFR3IDAgZyBCVCAxNC4xNSA3MjIuOTUgVGQgPDQzNzU3Mzc0NkY2RDY1NzIyMDRFNjE2RDY1PlRqIEVUIC9GMDAxIDEwLjAwIFRmIDAuMDAgVHcgMCBnIEJUIDE1Ny4zMCA3MjIuOTUgVGQgPDNBMjA0MTJFNEQyRTQ0MjA0NDY5NzM3NDcyNjk2Mjc1NzQ2RjcyPlRqIEVUIC9GMDAyIDEwLjAwIFRmDQogMC4wMCBUdyAwIGcgQlQgMTQuMTUgNzA4LjgwIFRkIDw0Mzc1NzM3NDZGNkQ2NTcyMjA0MzZGNjQ2NT5UaiBFVCAvRjAwMSAxMC4wMCBUZiAwLjAwIFR3IDAgZyBCVCAxNTcuMzAgNzA4LjgwIFRkIDwzQTIwMzEzMDMwMzAzMDMwMzAzMD5UaiBFVCAvRjAwMiAxMC4wMCBUZiAwLjAwIFR3IDAgZyBCVCAxNC4xNSA2OTQuNjUgVGQgPDQzNjk3NDc5PlRqIEVUIC9GMDAxIDEwLjAwIFRmIDAuMDAgVHcgMCBnIEJUIDE1Ny4zMCA2OTQuNjUgVGQNCiA8M0EyMDRCNjE3MjYxNjM2ODY5PlRqIEVUIC9GMDAyIDguMDAgVGYgMC4wMCBUdyAwIGcgQlQgMjUuMjUgNjY4LjUwIFRkIDw0NDZGNjMyRTIwNDQ2MTc0NjU+VGogRVQgMCBnIEJUIDgyLjYwIDY2OC41MCBUZCA8NDQ2RjYzMkUyMDRFNkYyRT5UaiBFVCAwIGcgQlQgMTQwLjUwIDY2OC41MCBUZCA8NDQ2RjYzNzU2RDY1NkU3NDIwNzQ3OTcwNjU+VGogRVQgMCBnIEJUIDIyMi44MCA2NjguNTAgVGQNCiA8NTI2NTY2NjU3MjY1NkU2MzY1MjA0RTZGMkU+VGogRVQgMCBnIEJUIDI5MS4wMCA2NjguNTAgVGQgPDQzNzU3MjcyPlRqIEVUIDAgZyBCVCAzNDcuMDAgNjY4LjUwIFRkIDw0NDY1NjI2OTc0PlRqIEVUIDAgZyBCVCA0MzAuMzAgNjY4LjUwIFRkIDw0MzcyNjU2NDY5NzQ+VGogRVQgMCBnIEJUIDUxMi45MCA2NjguNTAgVGQgPDQyNjE2QzYxNkU2MzY1PlRqIEVUIC9GMDAyIDEwLjAwIFRmIDAuMDAgVHcgMCBnIEJUIDEyNC43NSA2NTQuNTAgVGQNCiA8NEY3MDY1NkU2OTZFNjcyMDQyNjE2QzYxNkU2MzY1M0E+VGogRVQgL0YwMDEgOC4wMCBUZiAwLjAwIFR3IDAgZyBCVCA1MzAuMzAgNjU0LjUwIFRkIDwzMTMxMkMzMjMwMzMyRTM2MzYzMDJEPlRqIEVUIDAgZyBCVCAyMy44MCA2NDAuMDAgVGQgPDMyMzkyRTMwMzMyRTMyMzAzMTM4PlRqIEVUIDAgZyBCVCA3Ny4xMCA2NDAuMDAgVGQgPDMxMzQzMDMwMzAzMDMxMzYzNjM2PlRqIEVUIDAgZyBCVCAxMjcuMDAgNjQwLjAwIFRkDQogPDUyNjU2MzY1Njk3MDc0PlRqIEVUIDAgZyBCVCAyOTEuODUgNjQwLjAwIFRkIDw1MDRCNTI+VGogRVQgMCBnIEJUIDM3OC44NSA2NDAuMDAgVGQgPDMwMkUzMDMwMzAyMD5UaiBFVCAwIGcgQlQgNDQxLjkwIDY0MC4wMCBUZCA8MzEzNTM1MkMzMDMwMzAyRTMwMzAzMDIwPlRqIEVUIDAgZyBCVCA1MjYuMzAgNjQwLjAwIFRkIDwzMTM2MzYyQzMyMzAzMzJFMzYzNjMwMkQ+VGogRVQgMCBnIEJUIDIzLjgwIDYyNi4wMCBUZA0KIDwzMzMwMkUzMDM0MkUzMjMwMzEzOD5UaiBFVCAwIGcgQlQgODcuMTAgNjI2LjAwIFRkIDwzMzMzMzIzMzMyPlRqIEVUIDAgZyBCVCAxMjcuMDAgNjI2LjAwIFRkIDw1MzQxNEQ0OTIwNDk2RTc2NkY2OTYzNjU+VGogRVQgMCBnIEJUIDIyNi42NSA2MjYuMDAgVGQgPDM4MzEzMTMwMzAzMzMyMzMzMTM0PlRqIEVUIDAgZyBCVCAyOTEuODUgNjI2LjAwIFRkIDw1MDRCNTI+VGogRVQgMCBnIEJUIDM1Ni44NSA2MjYuMDAgVGQNCiA8MzEzNTM1MkMzMDMwMzAyRTMwMzAzMDIwPlRqIEVUIDAgZyBCVCA0NjMuOTAgNjI2LjAwIFRkIDwzMDJFMzAzMDMwMjA+VGogRVQgMCBnIEJUIDUzMC4zMCA2MjYuMDAgVGQgPDMxMzEyQzMyMzAzMzJFMzYzNjMwMkQ+VGogRVQgMCBnIEJUIDIzLjgwIDYxMi4wMCBUZCA8MzMzMDJFMzAzNDJFMzIzMDMxMzg+VGogRVQgMCBnIEJUIDc3LjEwIDYxMi4wMCBUZCA8MzEzNDMwMzAzMDMwMzIzMjM1MzE+VGogRVQgMCBnIEJUDQogMTI3LjAwIDYxMi4wMCBUZCA8NTI2NTYzNjU2OTcwNzQ+VGogRVQgMCBnIEJUIDI5MS44NSA2MTIuMDAgVGQgPDUwNEI1Mj5UaiBFVCAwIGcgQlQgMzc4Ljg1IDYxMi4wMCBUZCA8MzAyRTMwMzAzMDIwPlRqIEVUIDAgZyBCVCA0NDUuOTAgNjEyLjAwIFRkIDwzNjM3MkMzNTMwMzAyRTMwMzAzMDIwPlRqIEVUIDAgZyBCVCA1MzAuMzAgNjEyLjAwIFRkIDwzNzM4MkMzNzMwMzMyRTM2MzYzMDJEPlRqIEVUIDAgZyBCVCAyMy44MCA1OTguMDAgVGQNCiA8MzMzMTJFMzAzNTJFMzIzMDMxMzg+VGogRVQgMCBnIEJUIDg3LjEwIDU5OC4wMCBUZCA8MzQzNTMyMzMzMD5UaiBFVCAwIGcgQlQgMTI3LjAwIDU5OC4wMCBUZCA8NTM0MTRENDkyMDQ5NkU3NjZGNjk2MzY1PlRqIEVUIDAgZyBCVCAyMjYuNjUgNTk4LjAwIFRkIDwzODMxMzEzMDMwMzQzNDM0MzIzOD5UaiBFVCAwIGcgQlQgMjkxLjg1IDU5OC4wMCBUZCA8NTA0QjUyPlRqIEVUIDAgZyBCVCAzNTYuODUgNTk4LjAwIFRkDQogPDMxMzUzNTJDMzAzMDMwMkUzMDMwMzAyMD5UaiBFVCAwIGcgQlQgNDYzLjkwIDU5OC4wMCBUZCA8MzAyRTMwMzAzMDIwPlRqIEVUIDAgZyBCVCA1MzAuOTUgNTk4LjAwIFRkIDwzNzM2MkMzMjM5MzYyRTMzMzQzMDIwPlRqIEVUIDAgZyBCVCAyMy44MCA1ODQuMDAgVGQgPDMwMzEyRTMwMzYyRTMyMzAzMTM4PlRqIEVUIDAgZyBCVCA4Ny4xMCA1ODQuMDAgVGQgPDM3MzAzNTM4Mzg+VGogRVQgMCBnIEJUIDEyNy4wMCA1ODQuMDAgVGQNCiA8NDQ2NTYyNjk3NDIwNEQ2NTZENkY+VGogRVQgMCBnIEJUIDIyNi42NSA1ODQuMDAgVGQgPDMwMzAzOTMwMzAzMDMwMzczNTMwPlRqIEVUIDAgZyBCVCAyOTEuODUgNTg0LjAwIFRkIDw1MDRCNTI+VGogRVQgMCBnIEJUIDM2NC44NSA1ODQuMDAgVGQgPDMzMkMzOTM1MzAyRTMwMzAzMDIwPlRqIEVUIDAgZyBCVCA0NjMuOTAgNTg0LjAwIFRkIDwzMDJFMzAzMDMwMjA+VGogRVQgMCBnIEJUIDUzMC45NSA1ODQuMDAgVGQNCiA8MzgzMDJDMzIzNDM2MkUzMzM0MzAyMD5UaiBFVCAwIGcgQlQgMjMuODAgNTcwLjAwIFRkIDwzMDMxMkUzMDM2MkUzMjMwMzEzOD5UaiBFVCAwIGcgQlQgODcuMTAgNTcwLjAwIFRkIDwzNjM5MzYzMDMwPlRqIEVUIDAgZyBCVCAxMjcuMDAgNTcwLjAwIFRkIDw1MzQxNEQ0OTIwNTI0RDQyMjA0MzcyNjU2NDY5NzQyMDRFNkY3NDY1PlRqIEVUIDAgZyBCVCAyMjYuNjUgNTcwLjAwIFRkIDwzODM1MzIzMDMwMzAzMDM4MzUzND5UaiBFVCAwIGcgQlQNCiAyOTEuODUgNTcwLjAwIFRkIDw1MDRCNTI+VGogRVQgMCBnIEJUIDM3OC44NSA1NzAuMDAgVGQgPDMwMkUzMDMwMzAyMD5UaiBFVCAwIGcgQlQgNDQ1LjkwIDU3MC4wMCBUZCA8MzgzNzJDMzUzMDMwMkUzMDMwMzAyMD5UaiBFVCAwIGcgQlQgNTM0LjMwIDU3MC4wMCBUZCA8MzcyQzMyMzUzMzJFMzYzNjMwMkQ+VGogRVQgMCBnIEJUIDIzLjgwIDU1Ni4wMCBUZCA8MzAzMTJFMzAzNjJFMzIzMDMxMzg+VGogRVQgMCBnIEJUDQogODcuMTAgNTU2LjAwIFRkIDwzNjM5MzYzMDMxPlRqIEVUIDAgZyBCVCAxMjcuMDAgNTU2LjAwIFRkIDw1MzQxNEQ0OTIwNTI0RDQyMjA0MzcyNjU2NDY5NzQyMDRFNkY3NDY1PlRqIEVUIDAgZyBCVCAyMjYuNjUgNTU2LjAwIFRkIDwzODM1MzIzMDMwMzAzMDM4MzUzNT5UaiBFVCAwIGcgQlQgMjkxLjg1IDU1Ni4wMCBUZCA8NTA0QjUyPlRqIEVUIDAgZyBCVCAzNzguODUgNTU2LjAwIFRkIDwzMDJFMzAzMDMwMjA+VGogRVQgMCBnIEJUDQogNDQ1LjkwIDU1Ni4wMCBUZCA8MzgzNzJDMzUzMDMwMkUzMDMwMzAyMD5UaiBFVCAwIGcgQlQgNTMwLjMwIDU1Ni4wMCBUZCA8MzkzNDJDMzczNTMzMkUzNjM2MzAyRD5UaiBFVCAwIGcgQlQgMjMuODAgNTQyLjAwIFRkIDwzMjM2MkUzMDM2MkUzMjMwMzEzOD5UaiBFVCAwIGcgQlQgNzcuMTAgNTQyLjAwIFRkIDwzMTM0MzAzMDMwMzAzMzM0MzAzNz5UaiBFVCAwIGcgQlQgMTI3LjAwIDU0Mi4wMCBUZCA8NTI2NTYzNjU2OTcwNzQ+VGogRVQgMCBnDQogQlQgMjkxLjg1IDU0Mi4wMCBUZCA8NTA0QjUyPlRqIEVUIDAgZyBCVCAzNzguODUgNTQyLjAwIFRkIDwzMDJFMzAzMDMwMjA+VGogRVQgMCBnIEJUIDQ0NS45MCA1NDIuMDAgVGQgPDM2MzcyQzM1MzAzMDJFMzAzMDMwMjA+VGogRVQgMCBnIEJUIDUyNi4zMCA1NDIuMDAgVGQgPDMxMzYzMjJDMzIzNTMzMkUzNjM2MzAyRD5UaiBFVCAwIGcgQlQgMjMuODAgNTI4LjAwIFRkIDwzMzMwMkUzMDM2MkUzMjMwMzEzOD5UaiBFVCAwIGcgQlQNCiA4Ny4xMCA1MjguMDAgVGQgPDM2MzYzOTM5Mzc+VGogRVQgMCBnIEJUIDEyNy4wMCA1MjguMDAgVGQgPDUzNDE0RDQ5MjA0OTZFNzY2RjY5NjM2NT5UaiBFVCAwIGcgQlQgMjI2LjY1IDUyOC4wMCBUZCA8MzgzMTMxMzAzMDM2MzYzMTMzMzY+VGogRVQgMCBnIEJUIDI5MS44NSA1MjguMDAgVGQgPDUwNEI1Mj5UaiBFVCAwIGcgQlQgMzU2Ljg1IDUyOC4wMCBUZCA8MzEzMTMyMkMzNTMwMzAyRTMwMzAzMDIwPlRqIEVUIDAgZyBCVA0KIDQ2My45MCA1MjguMDAgVGQgPDMwMkUzMDMwMzAyMD5UaiBFVCAwIGcgQlQgNTMwLjMwIDUyOC4wMCBUZCA8MzQzOTJDMzczNTMzMkUzNjM2MzAyRD5UaiBFVCAvRjAwMiAxMC4wMCBUZiAwLjAwIFR3IDAgZyBCVCAxMjQuNzUgNTE0LjAwIFRkIDw0MzZDNkY3MzY5NkU2NzIwNDI2MTZDNjE2RTYzNjUzQT5UaiBFVCAvRjAwMSA4LjAwIFRmIDAuMDAgVHcgMCBnIEJUIDUzMC4zMCA1MTQuMDAgVGQgPDM0MzkyQzM3MzUzMzJFMzYzNjMwMkQ+VGogRVQNCiBxIDAgMCAwIFJHIDAuNzUgdyAxNC4xNSA2ODAuNTAgbSAxNC4xNSA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuNDUgNjgwLjUwIG0gNjkuNDUgNjY2LjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEzLjgwIDY4MC41MCBtIDY5Ljg1IDY4MC41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMy44MCA2NjYuNTAgbSA2OS44NSA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuNDUgNjgwLjUwIG0NCiA2OS40NSA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0Ljc1IDY4MC41MCBtIDEyNC43NSA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuMTAgNjgwLjUwIG0gMTI1LjE1IDY4MC41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS4xMCA2NjYuNTAgbSAxMjUuMTUgNjY2LjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC43NSA2ODAuNTAgbSAxMjQuNzUgNjY2LjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMjA3LjgwIDY4MC41MCBtIDIwNy44MCA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0LjQwIDY4MC41MCBtIDIwOC4yMCA2ODAuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0LjQwIDY2Ni41MCBtIDIwOC4yMCA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjA3LjgwIDY4MC41MCBtIDIwNy44MCA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjQ1IDY4MC41MCBtIDI4NS40NSA2NjYuNTAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAyMDcuNDUgNjgwLjUwIG0gMjg1Ljg1IDY4MC41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuNDUgNjY2LjUwIG0gMjg1Ljg1IDY2Ni41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuNDUgNjgwLjUwIG0gMjg1LjQ1IDY2Ni41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuODAgNjgwLjUwIG0gMzEzLjgwIDY2Ni41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuMTAgNjgwLjUwIG0NCiAzMTQuMjAgNjgwLjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS4xMCA2NjYuNTAgbSAzMTQuMjAgNjY2LjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy44MCA2ODAuNTAgbSAzMTMuODAgNjY2LjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC44NSA2ODAuNTAgbSAzOTguODUgNjY2LjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy40NSA2ODAuNTAgbSAzOTkuMjUgNjgwLjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMzEzLjQ1IDY2Ni41MCBtIDM5OS4yNSA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4Ljg1IDY4MC41MCBtIDM5OC44NSA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjkwIDY4MC41MCBtIDQ4My45MCA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4LjUwIDY4MC41MCBtIDQ4NC4zMCA2ODAuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4LjUwIDY2Ni41MCBtIDQ4NC4zMCA2NjYuNTAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyA0ODMuOTAgNjgwLjUwIG0gNDgzLjkwIDY2Ni41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA1NjguOTUgNjgwLjUwIG0gNTY4Ljk1IDY2Ni41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuNTUgNjgwLjUwIG0gNTY5LjM1IDY4MC41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuNTUgNjY2LjUwIG0gNTY5LjM1IDY2Ni41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxNC4xNSA2NjYuNTAgbQ0KIDE0LjE1IDY1Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMy44MCA2NjYuNTAgbSA2OS41MCA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTMuODAgNjUyLjAwIG0gNjkuNTAgNjUyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjQ1IDY2Ni41MCBtIDEyNC43NSA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuNDUgNjUyLjAwIG0gMTI0Ljc1IDY1Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdw0KIDEyNC43NSA2NjYuNTAgbSAyMDcuODAgNjY2LjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC43NSA2NTIuMDAgbSAyMDcuODAgNjUyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy44MCA2NjYuNTAgbSAyODUuNDUgNjY2LjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy44MCA2NTIuMDAgbSAyODUuNDUgNjUyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS40NSA2NjYuNTAgbSAzMTMuODAgNjY2LjUwIGwgUyBRIHENCiAwIDAgMCBSRyAwLjc1IHcgMjg1LjQ1IDY1Mi4wMCBtIDMxMy44MCA2NTIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjgwIDY2Ni41MCBtIDM5OC44NSA2NjYuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjgwIDY1Mi4wMCBtIDM5OC44NSA2NTIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjkwIDY2Ni41MCBtIDQ4My45MCA2NTIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4Ljg1IDY2Ni41MCBtDQogNDg0LjMwIDY2Ni41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguODUgNjUyLjAwIG0gNDg0LjMwIDY1Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuOTAgNjY2LjUwIG0gNDgzLjkwIDY1Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA1NjguOTUgNjY2LjUwIG0gNTY4Ljk1IDY1Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuNTUgNjY2LjUwIG0gNTY5LjM1IDY2Ni41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdw0KIDQ4My41NSA2NTIuMDAgbSA1NjkuMzUgNjUyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDE0LjE1IDY1Mi4wMCBtIDE0LjE1IDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS40NSA2NTIuMDAgbSA2OS40NSA2MzguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTMuODAgNjUyLjAwIG0gNjkuODUgNjUyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEzLjgwIDYzOC4wMCBtIDY5Ljg1IDYzOC4wMCBsIFMgUSBxDQogMCAwIDAgUkcgMC43NSB3IDY5LjQ1IDY1Mi4wMCBtIDY5LjQ1IDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNzUgNjUyLjAwIG0gMTI0Ljc1IDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS4xMCA2NTIuMDAgbSAxMjUuMTUgNjUyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjEwIDYzOC4wMCBtIDEyNS4xNSA2MzguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0Ljc1IDY1Mi4wMCBtDQogMTI0Ljc1IDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuODAgNjUyLjAwIG0gMjA3LjgwIDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNDAgNjUyLjAwIG0gMjA4LjIwIDY1Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNDAgNjM4LjAwIG0gMjA4LjIwIDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuODAgNjUyLjAwIG0gMjA3LjgwIDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdw0KIDI4NS40NSA2NTIuMDAgbSAyODUuNDUgNjM4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy40NSA2NTIuMDAgbSAyODUuODUgNjUyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy40NSA2MzguMDAgbSAyODUuODUgNjM4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS40NSA2NTIuMDAgbSAyODUuNDUgNjM4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy44MCA2NTIuMDAgbSAzMTMuODAgNjM4LjAwIGwgUyBRIHENCiAwIDAgMCBSRyAwLjc1IHcgMjg1LjEwIDY1Mi4wMCBtIDMxNC4yMCA2NTIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjEwIDYzOC4wMCBtIDMxNC4yMCA2MzguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjgwIDY1Mi4wMCBtIDMxMy44MCA2MzguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4Ljg1IDY1Mi4wMCBtIDM5OC44NSA2MzguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjQ1IDY1Mi4wMCBtDQogMzk5LjI1IDY1Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuNDUgNjM4LjAwIG0gMzk5LjI1IDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguODUgNjUyLjAwIG0gMzk4Ljg1IDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuOTAgNjUyLjAwIG0gNDgzLjkwIDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguNTAgNjUyLjAwIG0gNDg0LjMwIDY1Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdw0KIDM5OC41MCA2MzguMDAgbSA0ODQuMzAgNjM4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My45MCA2NTIuMDAgbSA0ODMuOTAgNjM4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDU2OC45NSA2NTIuMDAgbSA1NjguOTUgNjM4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My41NSA2NTIuMDAgbSA1NjkuMzUgNjUyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My41NSA2MzguMDAgbSA1NjkuMzUgNjM4LjAwIGwgUyBRIHENCiAwIDAgMCBSRyAwLjc1IHcgMTQuMTUgNjM4LjAwIG0gMTQuMTUgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjQ1IDYzOC4wMCBtIDY5LjQ1IDYyNC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMy44MCA2MzguMDAgbSA2OS44NSA2MzguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTMuODAgNjI0LjAwIG0gNjkuODUgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjQ1IDYzOC4wMCBtIDY5LjQ1IDYyNC4wMCBsDQogUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC43NSA2MzguMDAgbSAxMjQuNzUgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjEwIDYzOC4wMCBtIDEyNS4xNSA2MzguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuMTAgNjI0LjAwIG0gMTI1LjE1IDYyNC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNzUgNjM4LjAwIG0gMTI0Ljc1IDYyNC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuODAgNjM4LjAwIG0NCiAyMDcuODAgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC40MCA2MzguMDAgbSAyMDguMjAgNjM4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC40MCA2MjQuMDAgbSAyMDguMjAgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy44MCA2MzguMDAgbSAyMDcuODAgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS40NSA2MzguMDAgbSAyODUuNDUgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMjA3LjQ1IDYzOC4wMCBtIDI4NS44NSA2MzguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjA3LjQ1IDYyNC4wMCBtIDI4NS44NSA2MjQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjQ1IDYzOC4wMCBtIDI4NS40NSA2MjQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjgwIDYzOC4wMCBtIDMxMy44MCA2MjQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjEwIDYzOC4wMCBtIDMxNC4yMCA2MzguMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAyODUuMTAgNjI0LjAwIG0gMzE0LjIwIDYyNC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuODAgNjM4LjAwIG0gMzEzLjgwIDYyNC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguODUgNjM4LjAwIG0gMzk4Ljg1IDYyNC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuNDUgNjM4LjAwIG0gMzk5LjI1IDYzOC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuNDUgNjI0LjAwIG0NCiAzOTkuMjUgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC44NSA2MzguMDAgbSAzOTguODUgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My45MCA2MzguMDAgbSA0ODMuOTAgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC41MCA2MzguMDAgbSA0ODQuMzAgNjM4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC41MCA2MjQuMDAgbSA0ODQuMzAgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogNDgzLjkwIDYzOC4wMCBtIDQ4My45MCA2MjQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNTY4Ljk1IDYzOC4wMCBtIDU2OC45NSA2MjQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjU1IDYzOC4wMCBtIDU2OS4zNSA2MzguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjU1IDYyNC4wMCBtIDU2OS4zNSA2MjQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTQuMTUgNjI0LjAwIG0gMTQuMTUgNjEwLjAwIGwgUyBRIHENCiAwIDAgMCBSRyAwLjc1IHcgNjkuNDUgNjI0LjAwIG0gNjkuNDUgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEzLjgwIDYyNC4wMCBtIDY5Ljg1IDYyNC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMy44MCA2MTAuMDAgbSA2OS44NSA2MTAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuNDUgNjI0LjAwIG0gNjkuNDUgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC43NSA2MjQuMDAgbQ0KIDEyNC43NSA2MTAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuMTAgNjI0LjAwIG0gMTI1LjE1IDYyNC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS4xMCA2MTAuMDAgbSAxMjUuMTUgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC43NSA2MjQuMDAgbSAxMjQuNzUgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy44MCA2MjQuMDAgbSAyMDcuODAgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMTI0LjQwIDYyNC4wMCBtIDIwOC4yMCA2MjQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0LjQwIDYxMC4wMCBtIDIwOC4yMCA2MTAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjA3LjgwIDYyNC4wMCBtIDIwNy44MCA2MTAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjQ1IDYyNC4wMCBtIDI4NS40NSA2MTAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjA3LjQ1IDYyNC4wMCBtIDI4NS44NSA2MjQuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAyMDcuNDUgNjEwLjAwIG0gMjg1Ljg1IDYxMC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuNDUgNjI0LjAwIG0gMjg1LjQ1IDYxMC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuODAgNjI0LjAwIG0gMzEzLjgwIDYxMC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuMTAgNjI0LjAwIG0gMzE0LjIwIDYyNC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuMTAgNjEwLjAwIG0NCiAzMTQuMjAgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy44MCA2MjQuMDAgbSAzMTMuODAgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC44NSA2MjQuMDAgbSAzOTguODUgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy40NSA2MjQuMDAgbSAzOTkuMjUgNjI0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy40NSA2MTAuMDAgbSAzOTkuMjUgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMzk4Ljg1IDYyNC4wMCBtIDM5OC44NSA2MTAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjkwIDYyNC4wMCBtIDQ4My45MCA2MTAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4LjUwIDYyNC4wMCBtIDQ4NC4zMCA2MjQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4LjUwIDYxMC4wMCBtIDQ4NC4zMCA2MTAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjkwIDYyNC4wMCBtIDQ4My45MCA2MTAuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyA1NjguOTUgNjI0LjAwIG0gNTY4Ljk1IDYxMC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuNTUgNjI0LjAwIG0gNTY5LjM1IDYyNC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuNTUgNjEwLjAwIG0gNTY5LjM1IDYxMC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxNC4xNSA2MTAuMDAgbSAxNC4xNSA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuNDUgNjEwLjAwIG0NCiA2OS40NSA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTMuODAgNjEwLjAwIG0gNjkuODUgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEzLjgwIDU5Ni4wMCBtIDY5Ljg1IDU5Ni4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS40NSA2MTAuMDAgbSA2OS40NSA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0Ljc1IDYxMC4wMCBtIDEyNC43NSA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcNCiA2OS4xMCA2MTAuMDAgbSAxMjUuMTUgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjEwIDU5Ni4wMCBtIDEyNS4xNSA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0Ljc1IDYxMC4wMCBtIDEyNC43NSA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjA3LjgwIDYxMC4wMCBtIDIwNy44MCA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0LjQwIDYxMC4wMCBtIDIwOC4yMCA2MTAuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAxMjQuNDAgNTk2LjAwIG0gMjA4LjIwIDU5Ni4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuODAgNjEwLjAwIG0gMjA3LjgwIDU5Ni4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuNDUgNjEwLjAwIG0gMjg1LjQ1IDU5Ni4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuNDUgNjEwLjAwIG0gMjg1Ljg1IDYxMC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuNDUgNTk2LjAwIG0NCiAyODUuODUgNTk2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS40NSA2MTAuMDAgbSAyODUuNDUgNTk2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy44MCA2MTAuMDAgbSAzMTMuODAgNTk2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS4xMCA2MTAuMDAgbSAzMTQuMjAgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS4xMCA1OTYuMDAgbSAzMTQuMjAgNTk2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMzEzLjgwIDYxMC4wMCBtIDMxMy44MCA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4Ljg1IDYxMC4wMCBtIDM5OC44NSA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjQ1IDYxMC4wMCBtIDM5OS4yNSA2MTAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjQ1IDU5Ni4wMCBtIDM5OS4yNSA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4Ljg1IDYxMC4wMCBtIDM5OC44NSA1OTYuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyA0ODMuOTAgNjEwLjAwIG0gNDgzLjkwIDU5Ni4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguNTAgNjEwLjAwIG0gNDg0LjMwIDYxMC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguNTAgNTk2LjAwIG0gNDg0LjMwIDU5Ni4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuOTAgNjEwLjAwIG0gNDgzLjkwIDU5Ni4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA1NjguOTUgNjEwLjAwIG0NCiA1NjguOTUgNTk2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My41NSA2MTAuMDAgbSA1NjkuMzUgNjEwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My41NSA1OTYuMDAgbSA1NjkuMzUgNTk2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDE0LjE1IDU5Ni4wMCBtIDE0LjE1IDU4Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS40NSA1OTYuMDAgbSA2OS40NSA1ODIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcNCiAxMy44MCA1OTYuMDAgbSA2OS44NSA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTMuODAgNTgyLjAwIG0gNjkuODUgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjQ1IDU5Ni4wMCBtIDY5LjQ1IDU4Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNzUgNTk2LjAwIG0gMTI0Ljc1IDU4Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS4xMCA1OTYuMDAgbSAxMjUuMTUgNTk2LjAwIGwgUyBRIHENCiAwIDAgMCBSRyAwLjc1IHcgNjkuMTAgNTgyLjAwIG0gMTI1LjE1IDU4Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNzUgNTk2LjAwIG0gMTI0Ljc1IDU4Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuODAgNTk2LjAwIG0gMjA3LjgwIDU4Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNDAgNTk2LjAwIG0gMjA4LjIwIDU5Ni4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNDAgNTgyLjAwIG0NCiAyMDguMjAgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy44MCA1OTYuMDAgbSAyMDcuODAgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS40NSA1OTYuMDAgbSAyODUuNDUgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy40NSA1OTYuMDAgbSAyODUuODUgNTk2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy40NSA1ODIuMDAgbSAyODUuODUgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMjg1LjQ1IDU5Ni4wMCBtIDI4NS40NSA1ODIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjgwIDU5Ni4wMCBtIDMxMy44MCA1ODIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjEwIDU5Ni4wMCBtIDMxNC4yMCA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjEwIDU4Mi4wMCBtIDMxNC4yMCA1ODIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjgwIDU5Ni4wMCBtIDMxMy44MCA1ODIuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAzOTguODUgNTk2LjAwIG0gMzk4Ljg1IDU4Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuNDUgNTk2LjAwIG0gMzk5LjI1IDU5Ni4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuNDUgNTgyLjAwIG0gMzk5LjI1IDU4Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguODUgNTk2LjAwIG0gMzk4Ljg1IDU4Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuOTAgNTk2LjAwIG0NCiA0ODMuOTAgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC41MCA1OTYuMDAgbSA0ODQuMzAgNTk2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC41MCA1ODIuMDAgbSA0ODQuMzAgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My45MCA1OTYuMDAgbSA0ODMuOTAgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDU2OC45NSA1OTYuMDAgbSA1NjguOTUgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogNDgzLjU1IDU5Ni4wMCBtIDU2OS4zNSA1OTYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjU1IDU4Mi4wMCBtIDU2OS4zNSA1ODIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTQuMTUgNTgyLjAwIG0gMTQuMTUgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjQ1IDU4Mi4wMCBtIDY5LjQ1IDU2OC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMy44MCA1ODIuMDAgbSA2OS44NSA1ODIuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAxMy44MCA1NjguMDAgbSA2OS44NSA1NjguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuNDUgNTgyLjAwIG0gNjkuNDUgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC43NSA1ODIuMDAgbSAxMjQuNzUgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjEwIDU4Mi4wMCBtIDEyNS4xNSA1ODIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuMTAgNTY4LjAwIG0NCiAxMjUuMTUgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC43NSA1ODIuMDAgbSAxMjQuNzUgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy44MCA1ODIuMDAgbSAyMDcuODAgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC40MCA1ODIuMDAgbSAyMDguMjAgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC40MCA1NjguMDAgbSAyMDguMjAgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMjA3LjgwIDU4Mi4wMCBtIDIwNy44MCA1NjguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjQ1IDU4Mi4wMCBtIDI4NS40NSA1NjguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjA3LjQ1IDU4Mi4wMCBtIDI4NS44NSA1ODIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjA3LjQ1IDU2OC4wMCBtIDI4NS44NSA1NjguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjQ1IDU4Mi4wMCBtIDI4NS40NSA1NjguMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAzMTMuODAgNTgyLjAwIG0gMzEzLjgwIDU2OC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuMTAgNTgyLjAwIG0gMzE0LjIwIDU4Mi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuMTAgNTY4LjAwIG0gMzE0LjIwIDU2OC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuODAgNTgyLjAwIG0gMzEzLjgwIDU2OC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguODUgNTgyLjAwIG0NCiAzOTguODUgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy40NSA1ODIuMDAgbSAzOTkuMjUgNTgyLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy40NSA1NjguMDAgbSAzOTkuMjUgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC44NSA1ODIuMDAgbSAzOTguODUgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My45MCA1ODIuMDAgbSA0ODMuOTAgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMzk4LjUwIDU4Mi4wMCBtIDQ4NC4zMCA1ODIuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4LjUwIDU2OC4wMCBtIDQ4NC4zMCA1NjguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjkwIDU4Mi4wMCBtIDQ4My45MCA1NjguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNTY4Ljk1IDU4Mi4wMCBtIDU2OC45NSA1NjguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjU1IDU4Mi4wMCBtIDU2OS4zNSA1ODIuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyA0ODMuNTUgNTY4LjAwIG0gNTY5LjM1IDU2OC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxNC4xNSA1NjguMDAgbSAxNC4xNSA1NTQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuNDUgNTY4LjAwIG0gNjkuNDUgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEzLjgwIDU2OC4wMCBtIDY5Ljg1IDU2OC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMy44MCA1NTQuMDAgbQ0KIDY5Ljg1IDU1NC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS40NSA1NjguMDAgbSA2OS40NSA1NTQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0Ljc1IDU2OC4wMCBtIDEyNC43NSA1NTQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuMTAgNTY4LjAwIG0gMTI1LjE1IDU2OC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS4xMCA1NTQuMDAgbSAxMjUuMTUgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMTI0Ljc1IDU2OC4wMCBtIDEyNC43NSA1NTQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjA3LjgwIDU2OC4wMCBtIDIwNy44MCA1NTQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0LjQwIDU2OC4wMCBtIDIwOC4yMCA1NjguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0LjQwIDU1NC4wMCBtIDIwOC4yMCA1NTQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjA3LjgwIDU2OC4wMCBtIDIwNy44MCA1NTQuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAyODUuNDUgNTY4LjAwIG0gMjg1LjQ1IDU1NC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuNDUgNTY4LjAwIG0gMjg1Ljg1IDU2OC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuNDUgNTU0LjAwIG0gMjg1Ljg1IDU1NC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuNDUgNTY4LjAwIG0gMjg1LjQ1IDU1NC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuODAgNTY4LjAwIG0NCiAzMTMuODAgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS4xMCA1NjguMDAgbSAzMTQuMjAgNTY4LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS4xMCA1NTQuMDAgbSAzMTQuMjAgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy44MCA1NjguMDAgbSAzMTMuODAgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC44NSA1NjguMDAgbSAzOTguODUgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMzEzLjQ1IDU2OC4wMCBtIDM5OS4yNSA1NjguMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjQ1IDU1NC4wMCBtIDM5OS4yNSA1NTQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4Ljg1IDU2OC4wMCBtIDM5OC44NSA1NTQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjkwIDU2OC4wMCBtIDQ4My45MCA1NTQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4LjUwIDU2OC4wMCBtIDQ4NC4zMCA1NjguMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAzOTguNTAgNTU0LjAwIG0gNDg0LjMwIDU1NC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuOTAgNTY4LjAwIG0gNDgzLjkwIDU1NC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA1NjguOTUgNTY4LjAwIG0gNTY4Ljk1IDU1NC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuNTUgNTY4LjAwIG0gNTY5LjM1IDU2OC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuNTUgNTU0LjAwIG0NCiA1NjkuMzUgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDE0LjE1IDU1NC4wMCBtIDE0LjE1IDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS40NSA1NTQuMDAgbSA2OS40NSA1NDAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTMuODAgNTU0LjAwIG0gNjkuODUgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEzLjgwIDU0MC4wMCBtIDY5Ljg1IDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdw0KIDY5LjQ1IDU1NC4wMCBtIDY5LjQ1IDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNzUgNTU0LjAwIG0gMTI0Ljc1IDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS4xMCA1NTQuMDAgbSAxMjUuMTUgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjEwIDU0MC4wMCBtIDEyNS4xNSA1NDAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0Ljc1IDU1NC4wMCBtIDEyNC43NSA1NDAuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAyMDcuODAgNTU0LjAwIG0gMjA3LjgwIDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNDAgNTU0LjAwIG0gMjA4LjIwIDU1NC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNDAgNTQwLjAwIG0gMjA4LjIwIDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuODAgNTU0LjAwIG0gMjA3LjgwIDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuNDUgNTU0LjAwIG0NCiAyODUuNDUgNTQwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy40NSA1NTQuMDAgbSAyODUuODUgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy40NSA1NDAuMDAgbSAyODUuODUgNTQwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS40NSA1NTQuMDAgbSAyODUuNDUgNTQwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy44MCA1NTQuMDAgbSAzMTMuODAgNTQwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMjg1LjEwIDU1NC4wMCBtIDMxNC4yMCA1NTQuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjEwIDU0MC4wMCBtIDMxNC4yMCA1NDAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjgwIDU1NC4wMCBtIDMxMy44MCA1NDAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzk4Ljg1IDU1NC4wMCBtIDM5OC44NSA1NDAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjQ1IDU1NC4wMCBtIDM5OS4yNSA1NTQuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAzMTMuNDUgNTQwLjAwIG0gMzk5LjI1IDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguODUgNTU0LjAwIG0gMzk4Ljg1IDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA0ODMuOTAgNTU0LjAwIG0gNDgzLjkwIDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguNTAgNTU0LjAwIG0gNDg0LjMwIDU1NC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguNTAgNTQwLjAwIG0NCiA0ODQuMzAgNTQwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My45MCA1NTQuMDAgbSA0ODMuOTAgNTQwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDU2OC45NSA1NTQuMDAgbSA1NjguOTUgNTQwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My41NSA1NTQuMDAgbSA1NjkuMzUgNTU0LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My41NSA1NDAuMDAgbSA1NjkuMzUgNTQwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMTQuMTUgNTQwLjAwIG0gMTQuMTUgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjQ1IDU0MC4wMCBtIDY5LjQ1IDUyNi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMy44MCA1NDAuMDAgbSA2OS44NSA1NDAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTMuODAgNTI2LjAwIG0gNjkuODUgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjQ1IDU0MC4wMCBtIDY5LjQ1IDUyNi4wMCBsIFMgUSBxIDAgMCAwIFJHDQogMC43NSB3IDEyNC43NSA1NDAuMDAgbSAxMjQuNzUgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjEwIDU0MC4wMCBtIDEyNS4xNSA1NDAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNjkuMTAgNTI2LjAwIG0gMTI1LjE1IDUyNi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAxMjQuNzUgNTQwLjAwIG0gMTI0Ljc1IDUyNi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuODAgNTQwLjAwIG0gMjA3LjgwIDUyNi4wMCBsDQogUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC40MCA1NDAuMDAgbSAyMDguMjAgNTQwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEyNC40MCA1MjYuMDAgbSAyMDguMjAgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy44MCA1NDAuMDAgbSAyMDcuODAgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDI4NS40NSA1NDAuMDAgbSAyODUuNDUgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDIwNy40NSA1NDAuMDAgbQ0KIDI4NS44NSA1NDAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjA3LjQ1IDUyNi4wMCBtIDI4NS44NSA1MjYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjQ1IDU0MC4wMCBtIDI4NS40NSA1MjYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMzEzLjgwIDU0MC4wMCBtIDMxMy44MCA1MjYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMjg1LjEwIDU0MC4wMCBtIDMxNC4yMCA1NDAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcNCiAyODUuMTAgNTI2LjAwIG0gMzE0LjIwIDUyNi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuODAgNTQwLjAwIG0gMzEzLjgwIDUyNi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzOTguODUgNTQwLjAwIG0gMzk4Ljg1IDUyNi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuNDUgNTQwLjAwIG0gMzk5LjI1IDU0MC4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAzMTMuNDUgNTI2LjAwIG0gMzk5LjI1IDUyNi4wMCBsIFMgUSBxDQogMCAwIDAgUkcgMC43NSB3IDM5OC44NSA1NDAuMDAgbSAzOTguODUgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My45MCA1NDAuMDAgbSA0ODMuOTAgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC41MCA1NDAuMDAgbSA0ODQuMzAgNTQwLjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC41MCA1MjYuMDAgbSA0ODQuMzAgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My45MCA1NDAuMDAgbQ0KIDQ4My45MCA1MjYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNTY4Ljk1IDU0MC4wMCBtIDU2OC45NSA1MjYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjU1IDU0MC4wMCBtIDU2OS4zNSA1NDAuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjU1IDUyNi4wMCBtIDU2OS4zNSA1MjYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTQuMTUgNTI2LjAwIG0gMTQuMTUgNTExLjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMTMuODAgNTI2LjAwIG0gNjkuNTAgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDEzLjgwIDUxMS41MCBtIDY5LjUwIDUxMS41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyA2OS40NSA1MjYuMDAgbSAxMjQuNzUgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDY5LjQ1IDUxMS41MCBtIDEyNC43NSA1MTEuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgMTI0Ljc1IDUyNi4wMCBtIDIwNy44MCA1MjYuMDAgbCBTIFEgcQ0KIDAgMCAwIFJHIDAuNzUgdyAxMjQuNzUgNTExLjUwIG0gMjA3LjgwIDUxMS41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuODAgNTI2LjAwIG0gMjg1LjQ1IDUyNi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyMDcuODAgNTExLjUwIG0gMjg1LjQ1IDUxMS41MCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuNDUgNTI2LjAwIG0gMzEzLjgwIDUyNi4wMCBsIFMgUSBxIDAgMCAwIFJHIDAuNzUgdyAyODUuNDUgNTExLjUwIG0NCiAzMTMuODAgNTExLjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy44MCA1MjYuMDAgbSAzOTguODUgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDMxMy44MCA1MTEuNTAgbSAzOTguODUgNTExLjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDQ4My45MCA1MjYuMDAgbSA0ODMuOTAgNTExLjUwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3IDM5OC44NSA1MjYuMDAgbSA0ODQuMzAgNTI2LjAwIGwgUyBRIHEgMCAwIDAgUkcgMC43NSB3DQogMzk4Ljg1IDUxMS41MCBtIDQ4NC4zMCA1MTEuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjkwIDUyNi4wMCBtIDQ4My45MCA1MTEuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNTY4Ljk1IDUyNi4wMCBtIDU2OC45NSA1MTEuNTAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjU1IDUyNi4wMCBtIDU2OS4zNSA1MjYuMDAgbCBTIFEgcSAwIDAgMCBSRyAwLjc1IHcgNDgzLjU1IDUxMS41MCBtIDU2OS4zNSA1MTEuNTAgbCBTIFENCmVuZHN0cmVhbQ0KZW5kb2JqDQoxNCAwIG9iag0KMjUxMDMNCmVuZG9iag0KMTUgMCBvYmoNCjw8DQovVHlwZSAvUGFnZQ0KL01lZGlhQm94DQpbMCAwIDU5NSA4NDJdDQovUGFyZW50IDEgMCBSDQovUmVzb3VyY2VzDQo8PA0KL1Byb2NTZXQNClsvUERGIC9UZXh0IC9JbWFnZUldDQovRm9udA0KPDwNCi9GMDAxIDQgMCBSDQovRjAwMiA2IDAgUg0KPj4NCi9YT2JqZWN0DQo8PA0KLzAwMDEwIDEwIDAgUg0KPj4NCj4+DQovQ29udGVudHMgMTMgMCBSDQo+Pg0KZW5kb2JqDQoxNiAwIG9iag0KPDwNCi9BdXRob3IgKFNBTE1BTl9BQkFQICkNCi9DcmVhdGlvbkRhdGUgKEQ6MjAyMTA3MjYwODIzMjIpDQovQ3JlYXRvciAoRm9ybSBaRklfSFpfQ1VTVE9NRVJfTEVER0VSIEVOKQ0KL1Byb2R1Y2VyIChTQVAgTmV0V2VhdmVyIDc1MCApDQolU0FQaW5mb1N0YXJ0IFRPQV9EQVJBDQolRlVOQ1RJT049KCAgICApDQolTUFOREFOVD0oICAgKQ0KJURFTF9EQVRFPSggICAgICAgICkNCiVTQVBfT0JKRUNUPSggICAgICAgICAgKQ0KJUFSX09CSkVDVD0oICAgICAgICAgICkNCiVPQkpFQ1RfSUQ9KCAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgKQ0KJUZPUk1fSUQ9KCAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICApDQolRk9STUFSQ0hJVj0oICApDQolUkVTRVJWRT0oICAgICAgICAgICAgICAgICAgICAgICAgICAgKQ0KJU5PVElaPSggICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgKQ0KJS0oICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICkNCiUtKCAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICApDQolLSggICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgKQ0KJVNBUGluZm9FbmQgVE9BX0RBUkENCj4+DQplbmRvYmoNCjEgMCBvYmoNCjw8DQovVHlwZSAvUGFnZXMNCi9LaWRzDQpbIDE1IDAgUg0KXQ0KL0NvdW50IDENCj4+DQplbmRvYmoNCjE3IDAgb2JqDQo8PA0KL1R5cGUgL0NhdGFsb2cNCi9QYWdlcyAxIDAgUg0KL1BhZ2VNb2RlIC9Vc2VOb25lDQo+Pg0KZW5kb2JqDQp4cmVmDQowIDE4DQowMDAwMDAwMDAwIDY1NTM1IGYNCjAwMDAwMzAzMzcgMDAwMDAgbg0KMDAwMDAwMDA0NyAwMDAwMCBuDQowMDAwMDAwMjM0IDAwMDAwIG4NCjAwMDAwMDAyNjkgMDAwMDAgbg0KMDAwMDAwMTM3NiAwMDAwMCBuDQowMDAwMDAxNTYyIDAwMDAwIG4NCjAwMDAwMDI2NjcgMDAwMDAgbg0KMDAwMDAwMzE0MSAwMDAwMCBuDQowMDAwMDAzMTE5IDAwMDAwIG4NCjAwMDAwMDMxNzYgMDAwMDAgbg0KMDAwMDAwNDExNSAwMDAwMCBuDQowMDAwMDA0MDkyIDAwMDAwIG4NCjAwMDAwMDQxNTEgMDAwMDAgbg0KMDAwMDAyOTMxNSAwMDAwMCBuDQowMDAwMDI5MzQwIDAwMDAwIG4NCjAwMDAwMjk1NjQgMDAwMDAgbg0KMDAwMDAzMDQwNiAwMDAwMCBuDQp0cmFpbGVyDQo8PA0KL1NpemUgMTgNCi9Sb290IDE3IDAgUg0KL0luZm8gMTYgMCBSDQo+Pg0Kc3RhcnR4cmVmDQozMDQ4Mg0KJSVFT0YNCg==";
            return PartialView();
        }
        #endregion
        #region Invoice Ledger
        public IActionResult Invoice()
        {
            return View();
        }
        #endregion
        #region Sale Return Credit Note
        public IActionResult SaleReturnCreditNote()
        {
            return View();
        }
        #endregion
        #region Customer Balance report
        public IActionResult CustomerBalance()
        {
            return View();
        }
        #endregion
    }
}
