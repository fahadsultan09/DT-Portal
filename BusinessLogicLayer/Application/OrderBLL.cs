using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Application;
using Models.ViewModel;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Utility;
using Utility.Constant;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace BusinessLogicLayer.Application
{
    public class OrderBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<OrderMaster> _repository;
        private readonly IGenericRepository<OrderValue> _OrderValuerepository;
        private readonly OrderDetailBLL _orderDetailBLL;
        private readonly OrderValueBLL _orderValueBLL;
        private readonly ProductDetailBLL ProductDetailBLL;
        private readonly UserBLL _UserBLL;
        private readonly PaymentBLL _PaymentBLL;
        public OrderBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<OrderMaster>();
            _OrderValuerepository = _unitOfWork.GenericRepository<OrderValue>();
            _orderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _orderValueBLL = new OrderValueBLL(_unitOfWork);
            ProductDetailBLL = new ProductDetailBLL(_unitOfWork);
            _UserBLL = new UserBLL(_unitOfWork);
            _PaymentBLL = new PaymentBLL(_unitOfWork);
        }
        public int Add(OrderMaster module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsActive = true;
            module.IsDeleted = false;
            module.CreatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<OrderMaster>().Insert(module);
            return _unitOfWork.Save();
        }


        public int Update(OrderMaster module)
        {
            var item = _unitOfWork.GenericRepository<OrderMaster>().GetById(module.Id);
            item.ReferenceNo = module.ReferenceNo;
            item.Remarks = module.Remarks;
            item.Status = module.Status;
            item.Attachment = module.Attachment;
            item.OnHoldBy = module.OnHoldBy;
            item.OnHoldDate = module.OnHoldDate;
            item.RejectedBy = module.RejectedBy; ;
            item.RejectedDate = module.RejectedDate;
            item.RejectedComment = module.RejectedComment;
            item.ApprovedBy = module.ApprovedBy;
            item.ApprovedDate = module.ApprovedDate;
            item.OnHoldComment = module.OnHoldComment;
            item.IsActive = module.IsActive;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<OrderMaster>().Update(item);
            return _unitOfWork.Save();
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<OrderMaster>().GetById(id);
            item.IsDeleted = true;
            item.IsActive = false;
            item.DeletedBy = SessionHelper.LoginUser.Id;
            item.DeletedDate = DateTime.Now;
            _unitOfWork.GenericRepository<OrderMaster>().Delete(item);
            return _unitOfWork.Save();
        }
        public OrderMaster GetOrderMasterById(int id)
        {
            return _unitOfWork.GenericRepository<OrderMaster>().GetById(id);
        }
        public List<OrderMaster> GetAllOrderMaster()
        {
            return _unitOfWork.GenericRepository<OrderMaster>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }

        public OrderValueViewModel GetOrderValueModel(List<ProductDetail> productDetails)
        {
            List<Company> companies = new CompanyBLL(_unitOfWork).GetAllCompany();

            OrderValueViewModel viewModel = new OrderValueViewModel();
            List<OrderDetail> orderDetails = _orderDetailBLL.GetAllOrderDetail().Where(x => x.OrderMaster.IsDeleted == false && x.OrderMaster.Status == OrderStatus.PendingApproval && (SessionHelper.LoginUser.IsDistributor == true ? x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId : true)).ToList();
            List<ProductDetail> productDetailList = new ProductDetailBLL(_unitOfWork).GetAllProductDetail();
            var sami = Convert.ToInt32(CompanyEnum.SAMI);
            var HealthTek = Convert.ToInt32(CompanyEnum.Healthtek);
            var Phytek = Convert.ToInt32(CompanyEnum.Phytek);
            productDetails.ForEach(e => e.TotalPrice = e.ProductMaster.Quantity * e.ProductMaster.Rate);
            var SAMIproductDetails = productDetails.Where(e => e.CompanyId == sami).ToList();
            viewModel.SAMISupplies0 = SAMIproductDetails.Where(e => e.WTaxRate == "0").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.SAMISupplies1 = SAMIproductDetails.Where(e => e.WTaxRate == "1").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.SAMISupplies4 = SAMIproductDetails.Where(e => e.WTaxRate == "4").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.SAMITotalOrderValues = SAMIproductDetails.Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.SAMITotalUnapprovedOrderValues = (from od in orderDetails
                                                        join p in productDetailList on od.ProductId equals p.ProductMasterId
                                                        where p.CompanyId == 1
                                                        group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                                        let Amount = odp.Sum(m => m.od.Amount)
                                                        select Amount).Sum(x => x);
            viewModel.HealthTekTotalUnapprovedOrderValues = (from od in orderDetails
                                                             join p in productDetailList on od.ProductId equals p.ProductMasterId
                                                             where p.CompanyId == 3
                                                             group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                                             let Amount = odp.Sum(m => m.od.Amount)
                                                             select Amount).Sum(x => x);
            viewModel.PhytekTotalUnapprovedOrderValues = (from od in orderDetails
                                                          join p in productDetailList on od.ProductId equals p.ProductMasterId
                                                          where p.CompanyId == 2
                                                          group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                                          let Amount = odp.Sum(m => m.od.Amount)
                                                          select Amount).Sum(x => x);
            if (SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.SAMI.ToString()).SAPCompanyCode) != null)
            {
                viewModel.SAMIPendingOrderValues = Convert.ToDouble(SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.SAMI.ToString()).SAPCompanyCode).PendingValue);
            }
            viewModel.SAMICurrentBalance = SessionHelper.DistributorBalance.SAMI;
            if (SessionHelper.LoginUser.IsDistributor)
            {
                viewModel.SAMIUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == sami && x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            else
            {
                //viewModel.SAMIUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == sami && x.DistributorId == SAMIproductDetails.OrderMaster.Distributor.Id && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.SAMINetPayable = (viewModel.SAMITotalOrderValues - (-1 * SessionHelper.DistributorBalance.SAMI)) > 0 ? viewModel.SAMITotalOrderValues - SessionHelper.DistributorBalance.SAMI : 0;

            var HealthTekproductDetails = productDetails.Where(e => e.CompanyId == HealthTek).ToList();
            viewModel.HealthTekSupplies0 = HealthTekproductDetails.Where(e => e.WTaxRate == "0").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.HealthTekSupplies1 = HealthTekproductDetails.Where(e => e.WTaxRate == "1").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.HealthTekSupplies4 = HealthTekproductDetails.Where(e => e.WTaxRate == "4").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.HealthTekTotalOrderValues = HealthTekproductDetails.Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            if (SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Healthtek.ToString()).SAPCompanyCode) != null)
            {
                viewModel.HealthTekPendingOrderValues = Convert.ToDouble(SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Healthtek.ToString()).SAPCompanyCode).PendingValue);
            }
            viewModel.HealthTekCurrentBalance = SessionHelper.DistributorBalance.HealthTek;
            if (SessionHelper.LoginUser.IsDistributor)
            {
                viewModel.HealthTekUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == HealthTek && x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            else
            {
                //viewModel.HealthTekUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == HealthTek && x.DistributorId == HealthTekproductDetails.OrderMaster.Distributor.Id && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.HealthTekNetPayable = (viewModel.HealthTekTotalOrderValues - (-1 * SessionHelper.DistributorBalance.HealthTek)) > 0 ? viewModel.HealthTekTotalOrderValues - SessionHelper.DistributorBalance.HealthTek : 0;

            var PhytekproductDetails = productDetails.Where(e => e.CompanyId == Phytek).ToList();
            viewModel.PhytekSupplies0 = PhytekproductDetails.Where(e => e.WTaxRate == "0").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.PhytekSupplies1 = PhytekproductDetails.Where(e => e.WTaxRate == "1").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.PhytekSupplies4 = PhytekproductDetails.Where(e => e.WTaxRate == "4").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.PhytekTotalOrderValues = PhytekproductDetails.Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            if (SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Phytek.ToString()).SAPCompanyCode) != null)
            {
                viewModel.PhytekPendingOrderValues = Convert.ToDouble(SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Phytek.ToString()).SAPCompanyCode).PendingValue);
            }
            viewModel.PhytekCurrentBalance = SessionHelper.DistributorBalance.PhyTek;
            if (SessionHelper.LoginUser.IsDistributor)
            {
                viewModel.PhytekUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == Phytek && x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            else
            {
                //viewModel.HealthTekUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == Phytek && x.DistributorId == HealthTekproductDetails.OrderMaster.Distributor.Id && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);tor.Id && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.PhytekNetPayable = (viewModel.PhytekTotalOrderValues - (-1 * SessionHelper.DistributorBalance.PhyTek)) > 0 ? viewModel.PhytekTotalOrderValues - SessionHelper.DistributorBalance.PhyTek : 0;
            return viewModel;
        }

        public OrderValueViewModel GetOrderValueModel(List<OrderValue> OrderValue)
        {
            List<PaymentMaster> PaymentMasterList = _PaymentBLL.Where(x => SessionHelper.LoginUser.IsDistributor == true ? x.DistributorId == SessionHelper.LoginUser.DistributorId : x.DistributorId == OrderValue.FirstOrDefault().OrderMaster.DistributorId).ToList();
            List<OrderDetail> orderDetails = _orderDetailBLL.GetAllOrderDetail().Where(x => x.OrderMaster.IsDeleted == false && x.OrderMaster.Status == OrderStatus.PendingApproval && (SessionHelper.LoginUser.IsDistributor == true ? x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId : x.OrderMaster.DistributorId == OrderValue.FirstOrDefault().OrderMaster.DistributorId)).ToList();
            List<ProductDetail> productDetailList = new ProductDetailBLL(_unitOfWork).GetAllProductDetail();
            OrderValueViewModel viewModel = new OrderValueViewModel();
            var sami = Convert.ToInt32(CompanyEnum.SAMI);
            var HealthTek = Convert.ToInt32(CompanyEnum.Healthtek);
            var Phytek = Convert.ToInt32(CompanyEnum.Phytek);

            var SAMIproductDetails = OrderValue.FirstOrDefault(e => e.CompanyId == sami);
            if (SAMIproductDetails != null)
            {
                viewModel.SAMISupplies0 = SAMIproductDetails.SuppliesZero;
                viewModel.SAMISupplies1 = SAMIproductDetails.SuppliesOne;
                viewModel.SAMISupplies4 = SAMIproductDetails.SuppliesFour;
                viewModel.SAMITotalOrderValues = SAMIproductDetails.TotalOrderValues;
                viewModel.SAMIPendingOrderValues = SAMIproductDetails.PendingOrderValues;
                viewModel.SAMICurrentBalance = SAMIproductDetails.CurrentBalance;
                viewModel.SAMIUnConfirmedPayment = PaymentMasterList.Where(x => x.CompanyId == SAMIproductDetails.CompanyId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                viewModel.SAMINetPayable = SAMIproductDetails.NetPayable;
                viewModel.SAMITotalUnapprovedOrderValues = (from od in orderDetails
                                                            join p in productDetailList on od.ProductId equals p.ProductMasterId
                                                            where p.CompanyId == 1
                                                            group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                                            let Amount = odp.Sum(m => m.od.Amount)
                                                            select Amount).Sum(x => x);
            }
            var HealthTekproductDetails = OrderValue.FirstOrDefault(e => e.CompanyId == HealthTek);
            if (HealthTekproductDetails != null)
            {
                viewModel.HealthTekSupplies0 = HealthTekproductDetails.SuppliesZero;
                viewModel.HealthTekSupplies1 = HealthTekproductDetails.SuppliesOne;
                viewModel.HealthTekSupplies4 = HealthTekproductDetails.SuppliesFour;
                viewModel.HealthTekTotalOrderValues = HealthTekproductDetails.TotalOrderValues;
                viewModel.HealthTekPendingOrderValues = HealthTekproductDetails.PendingOrderValues;
                viewModel.HealthTekCurrentBalance = HealthTekproductDetails.CurrentBalance;
                viewModel.HealthTekUnConfirmedPayment = PaymentMasterList.Where(x => x.CompanyId == HealthTekproductDetails.CompanyId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                viewModel.HealthTekNetPayable = HealthTekproductDetails.NetPayable;
                viewModel.HealthTekTotalUnapprovedOrderValues = (from od in orderDetails
                                                                 join p in productDetailList on od.ProductId equals p.ProductMasterId
                                                                 where p.CompanyId == 3
                                                                 group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                                                 let Amount = odp.Sum(m => m.od.Amount)
                                                                 select Amount).Sum(x => x);
            }
            var PhytekproductDetails = OrderValue.FirstOrDefault(e => e.CompanyId == Phytek);
            if (PhytekproductDetails != null)
            {
                viewModel.PhytekSupplies0 = PhytekproductDetails.SuppliesZero;
                viewModel.PhytekSupplies1 = PhytekproductDetails.SuppliesOne;
                viewModel.PhytekSupplies4 = PhytekproductDetails.SuppliesFour;
                viewModel.PhytekTotalOrderValues = PhytekproductDetails.TotalOrderValues;
                viewModel.PhytekPendingOrderValues = PhytekproductDetails.PendingOrderValues;
                viewModel.PhytekCurrentBalance = PhytekproductDetails.CurrentBalance;
                viewModel.PhytekUnConfirmedPayment = PaymentMasterList.Where(x => x.CompanyId == PhytekproductDetails.CompanyId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
                viewModel.PhytekNetPayable = PhytekproductDetails.NetPayable;
                viewModel.PhytekTotalUnapprovedOrderValues = (from od in orderDetails
                                                              join p in productDetailList on od.ProductId equals p.ProductMasterId
                                                              where p.CompanyId == 2
                                                              group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                                              let Amount = odp.Sum(m => m.od.Amount)
                                                              select Amount).Sum(x => x);
            }
            return viewModel;
        }
        public List<OrderValue> GetValues(OrderValueViewModel orderValueViewModel, int OrderId)
        {
            var sami = Convert.ToInt32(CompanyEnum.SAMI);
            var HealthTek = Convert.ToInt32(CompanyEnum.Healthtek);
            var Phytek = Convert.ToInt32(CompanyEnum.Phytek);
            List<OrderValue> model = new List<OrderValue>();
            model.Add(new OrderValue()
            {
                CompanyId = sami,
                CreatedBy = SessionHelper.LoginUser.Id,
                CreatedDate = DateTime.Now,
                CurrentBalance = orderValueViewModel.SAMICurrentBalance,
                PendingOrderValues = orderValueViewModel.SAMIPendingOrderValues,
                SuppliesZero = orderValueViewModel.SAMISupplies0,
                SuppliesOne = orderValueViewModel.SAMISupplies1,
                SuppliesFour = orderValueViewModel.SAMISupplies4,
                UnConfirmedPayment = orderValueViewModel.SAMIUnConfirmedPayment,
                TotalOrderValues = orderValueViewModel.SAMITotalOrderValues,
                NetPayable = orderValueViewModel.SAMINetPayable,
                OrderId = OrderId
            });
            model.Add(new OrderValue()
            {
                CompanyId = HealthTek,
                CreatedBy = SessionHelper.LoginUser.Id,
                CreatedDate = DateTime.Now,
                CurrentBalance = orderValueViewModel.HealthTekCurrentBalance,
                PendingOrderValues = orderValueViewModel.HealthTekPendingOrderValues,
                SuppliesZero = orderValueViewModel.HealthTekSupplies0,
                SuppliesOne = orderValueViewModel.HealthTekSupplies1,
                SuppliesFour = orderValueViewModel.HealthTekSupplies4,
                UnConfirmedPayment = orderValueViewModel.HealthTekUnConfirmedPayment,
                TotalOrderValues = orderValueViewModel.HealthTekTotalOrderValues,
                NetPayable = orderValueViewModel.HealthTekNetPayable,
                OrderId = OrderId
            });
            model.Add(new OrderValue()
            {
                CompanyId = Phytek,
                CreatedBy = SessionHelper.LoginUser.Id,
                CreatedDate = DateTime.Now,
                CurrentBalance = orderValueViewModel.PhytekCurrentBalance,
                PendingOrderValues = orderValueViewModel.PhytekPendingOrderValues,
                SuppliesZero = orderValueViewModel.PhytekSupplies0,
                SuppliesOne = orderValueViewModel.PhytekSupplies1,
                SuppliesFour = orderValueViewModel.PhytekSupplies4,
                UnConfirmedPayment = orderValueViewModel.PhytekUnConfirmedPayment,
                TotalOrderValues = orderValueViewModel.PhytekTotalOrderValues,
                NetPayable = orderValueViewModel.PhytekNetPayable,
                OrderId = OrderId
            });
            return model;
        }

        public List<OrderMaster> Where(Expression<Func<OrderMaster, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public OrderMaster FirstOrDefault(Expression<Func<OrderMaster, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }

        public int AddRange(List<OrderValue> module)
        {
            _OrderValuerepository.AddRange(module);
            return _unitOfWork.Save();
        }

        public int DeleteRange(List<OrderValue> module)
        {
            _OrderValuerepository.DeleteRange(module);
            return _unitOfWork.Save();
        }

        public JsonResponse Save(OrderMaster model, IConfiguration configuration, IUrlHelper Url)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string FolderPath = configuration.GetSection("Settings").GetSection("FolderPath").Value;
            string[] permittedExtensions = Common.permittedExtensions;
            if (model.AttachmentFormFile != null)
            {
                var ext = Path.GetExtension(model.AttachmentFormFile.FileName).ToLowerInvariant();
                if (permittedExtensions.Contains(ext) && model.AttachmentFormFile.Length < Convert.ToInt64(5242880))
                {
                    Tuple<bool, string> tuple = FileUtility.UploadFile(model.AttachmentFormFile, FolderName.Order, FolderPath);
                    if (tuple.Item1)
                    {
                        model.Attachment = tuple.Item2;
                    }
                }
            }
            if (model.Id == 0)
            {
                model.TotalValue = SessionHelper.AddProduct.Select(e => e.TotalPrice).Sum();
                model.DistributorId = SessionHelper.LoginUser.DistributorId ?? 1;
                Add(model);
                List<OrderDetail> details = new List<OrderDetail>();
                foreach (var item in SessionHelper.AddProduct)
                {
                    details.Add(new OrderDetail()
                    {
                        Amount = item.TotalPrice,
                        OrderId = model.Id,
                        ProductId = item.ProductMasterId,
                        Quantity = item.ProductMaster.Quantity,
                        CreatedBy = SessionHelper.LoginUser.Id,
                        CreatedDate = DateTime.Now,
                        ApprovedQuantity = 0
                    });
                }
                _orderDetailBLL.AddRange(details);
                AddRange(GetValues(GetOrderValueModel(SessionHelper.AddProduct), model.Id));
                jsonResponse.Status = true;
                jsonResponse.Message = model.Status == OrderStatus.Draft ? OrderContant.OrderDraft : OrderContant.OrderSubmit;
                jsonResponse.RedirectURL = model.Status == OrderStatus.Draft ? Url.Action("Add", "Order", new { DPID = EncryptDecrypt.Encrypt(model.Id.ToString()) }) : Url.Action("Index", "Order");
            }
            else
            {
                model.TotalValue = SessionHelper.AddProduct.Select(e => e.TotalPrice).Sum();
                model.DistributorId = SessionHelper.LoginUser.DistributorId ?? 1;
                Update(model);
                _orderDetailBLL.DeleteRange(_orderDetailBLL.Where(e => e.OrderId == model.Id).ToList());
                List<OrderDetail> details = new List<OrderDetail>();
                foreach (var item in SessionHelper.AddProduct)
                {
                    details.Add(new OrderDetail()
                    {
                        Amount = item.TotalPrice,
                        OrderId = model.Id,
                        ProductId = item.ProductMasterId,
                        Quantity = item.ProductMaster.Quantity,
                        CreatedBy = SessionHelper.LoginUser.Id,
                        CreatedDate = DateTime.Now
                    });
                }
                _orderDetailBLL.AddRange(details);
                DeleteRange(_orderValueBLL.GetOrderValueByOrderId(model.Id));
                AddRange(GetValues(GetOrderValueModel(SessionHelper.AddProduct), model.Id));
                jsonResponse.Status = true;
                jsonResponse.Message = model.Status == OrderStatus.Draft ? OrderContant.OrderDraft : OrderContant.OrderSubmit;
                jsonResponse.RedirectURL = model.Status == OrderStatus.Draft ? Url.Action("Add", "Order", new { DPID = EncryptDecrypt.Encrypt(model.Id.ToString()) }) : Url.Action("Index", "Order");
            }
            return jsonResponse;
        }

        public List<OrderStatusViewModel> PlaceOrderToSAP(int OrderId)
        {
            List<OrderStatusViewModel> model = new List<OrderStatusViewModel>();
            var orderproduct = _orderDetailBLL.Where(e => e.OrderId == OrderId && e.IsProductSelected == true && e.ApprovedQuantity > 0 && e.SAPOrderNumber == null).ToList();
            var ProductDetail = ProductDetailBLL.Where(e => orderproduct.Select(c => c.ProductId).Contains(e.ProductMasterId)).ToList();
            foreach (var item in orderproduct)
            {
                model.Add(new OrderStatusViewModel()
                {
                    SNO = string.Format("{0:1000000000}", item.OrderId),
                    ITEMNO = "",
                    PARTN_NUMB = item.OrderMaster.Distributor.DistributorSAPCode,
                    DOC_TYPE = ProductDetail.First(e => e.ProductMasterId == item.ProductId).S_OrderType,
                    SALES_ORG = ProductDetail.First(e => e.ProductMasterId == item.ProductId).SaleOrganization,
                    DISTR_CHAN = ProductDetail.First(e => e.ProductMasterId == item.ProductId).DistributionChannel,
                    DIVISION = ProductDetail.First(e => e.ProductMasterId == item.ProductId).Division,
                    PURCH_NO = item.OrderMaster.ReferenceNo,
                    PURCH_DATE = DateTime.Now,
                    PRICE_DATE = DateTime.Now,
                    ST_PARTN = item.OrderMaster.Distributor.DistributorSAPCode,
                    MATERIAL = ProductDetail.First(e => e.ProductMasterId == item.ProductId).ProductMaster.SAPProductCode,
                    PLANT = ProductDetail.First(e => e.ProductMasterId == item.ProductId).DispatchPlant,
                    STORE_LOC = ProductDetail.First(e => e.ProductMasterId == item.ProductId).S_StorageLocation,
                    BATCH = "",
                    ITEM_CATEG = ProductDetail.First(e => e.ProductMasterId == item.ProductId).SalesItemCategory,
                    REQ_QTY = item.ApprovedQuantity.ToString()
                });
            }
            return model;
        }
        public List<OrderMaster> Search(OrderSearch model)
        {
            var LamdaId = (Expression<Func<OrderMaster, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.OrderNo != null)
            {
                LamdaId = LamdaId.And(e => e.Id == model.OrderNo);
            }
            if (model.Status != null)
            {
                LamdaId = LamdaId.And(e => e.Status == model.Status);
            }
            if (model.FromDate != null)
            {
                LamdaId = LamdaId.And(e => e.CreatedDate.Date >= Convert.ToDateTime(model.FromDate).Date);
            }
            if (model.ToDate != null)
            {
                LamdaId = LamdaId.And(e => e.CreatedDate.Date <= Convert.ToDateTime(model.ToDate).Date);
            }
            if (model.FromDate != null && model.ToDate != null)
            {
                LamdaId = LamdaId.And(e => e.CreatedDate.Date >= Convert.ToDateTime(model.FromDate).Date || e.CreatedDate.Date <= Convert.ToDateTime(model.ToDate).Date);
            }
            var Filter = _repository.Where(LamdaId).ToList();
            var query = (from x in Filter
                         join u in _UserBLL.GetAllUser().ToList()
                              on x.CreatedBy equals u.Id
                         join ua in _UserBLL.GetAllUser().ToList()
                              on x.ApprovedBy equals ua.Id into approvedGroup
                         from a1 in approvedGroup.DefaultIfEmpty()
                         join ur in _UserBLL.GetAllUser().ToList()
                              on x.RejectedBy equals ur.Id into rejectedGroup
                         from a2 in rejectedGroup.DefaultIfEmpty()
                         select new OrderMaster
                         {
                             Id = x.Id,
                             Distributor = x.Distributor,
                             Status = x.Status,
                             DistributorId = x.DistributorId,
                             CreatedBy = x.CreatedBy,
                             CreatedName = (u.FirstName + " " + u.LastName + " (" + u.UserName + ")"),
                             CreatedDate = x.CreatedDate,
                             ApprovedBy = x.ApprovedBy,
                             ApprovedName = a1 == null ? string.Empty : (a1.FirstName + " " + a1.LastName + " (" + a1.UserName + ")"),
                             ApprovedDate = x.ApprovedDate,
                             RejectedBy = x.RejectedBy,
                             RejectedName = a2 == null ? string.Empty : (a2.FirstName + " " + a2.LastName + " (" + a2.UserName + ")"),
                             RejectedDate = x.RejectedDate
                         }).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }

        public DistributorBalance GetBalance(string DistributorCode, Configuration configuration)
        {
            try
            {

                var Client = new RestClient(configuration.SyncDistributorBalanceURL + "/Get?DistributorId=" + DistributorCode);
                var request = new RestRequest(Method.GET);
                IRestResponse response = Client.Execute(request);
                var resp = JsonConvert.DeserializeObject<DistributorBalance>(response.Content);
                if (resp == null)
                {
                    return new DistributorBalance();
                }
                else
                {
                    return resp;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return new DistributorBalance();
            }
        }
        public List<SAPOrderPendingQuantity> GetDistributorPendingQuantity(string DistributorCode, Configuration _configuration)
        {
            try
            {
                var Client = new RestClient(_configuration.GetPendingQuantity + "?DistributorId=" + DistributorCode);
                var request = new RestRequest(Method.GET);
                IRestResponse response = Client.Execute(request);
                var SAPDistributor = JsonConvert.DeserializeObject<List<SAPOrderPendingQuantity>>(response.Content);
                if (SAPDistributor == null)
                {
                    return new List<SAPOrderPendingQuantity>();
                }
                else
                {
                    return SAPDistributor;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return new List<SAPOrderPendingQuantity>();
            }
        }
        public List<SAPOrderPendingValue> GetPendingOrderValue(string DistributorCode, Configuration _configuration)
        {
            try
            {
                var Client = new RestClient(_configuration.GetPendingOrderValue + "?DistributorId=" + DistributorCode);
                var request = new RestRequest(Method.POST);
                IRestResponse response = Client.Execute(request);
                var SAPOrderPendingValue = JsonConvert.DeserializeObject<List<SAPOrderPendingValue>>(response.Content);
                if (SAPOrderPendingValue == null)
                {
                    return new List<SAPOrderPendingValue>();
                }
                else
                {
                    return SAPOrderPendingValue;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return new List<SAPOrderPendingValue>();
            }
        }
    }
}
