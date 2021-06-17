using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using DPPendingOrdersRequest;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using Newtonsoft.Json;
using RestSharp;
using SalesOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Utility;
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
        private readonly OrderValueBLL _OrderValueBLL;
        private readonly ProductDetailBLL ProductDetailBLL;
        private DistributorWiseProductDiscountAndPricesBLL discountAndPricesBll;
        private readonly UserBLL _UserBLL;
        private readonly PaymentBLL _PaymentBLL;
        public OrderBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<OrderMaster>();
            _OrderValuerepository = _unitOfWork.GenericRepository<OrderValue>();
            _orderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _OrderValueBLL = new OrderValueBLL(_unitOfWork);
            ProductDetailBLL = new ProductDetailBLL(_unitOfWork);
            _UserBLL = new UserBLL(_unitOfWork);
            _PaymentBLL = new PaymentBLL(_unitOfWork);
            discountAndPricesBll = new DistributorWiseProductDiscountAndPricesBLL(_unitOfWork);
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
        public int UpdateSNo(OrderMaster module)
        {
            var item = _repository.GetById(module.Id);
            if (_repository.GetAllList().Count() > 1)
            {
                item.SNo = _repository.GetAllList().Max(y => y.SNo) + 1;
            }
            else
            {
                item.SNo = 100000001;
            }

            _repository.Update(item);
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
            List<ProductDetail> productDetailList = new ProductDetailBLL(_unitOfWork).Where(x => orderDetails.Select(x => x.ProductId).Contains(x.ProductMaster.Id));
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
            if (SessionHelper.SAPOrderPendingValue != null && SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.SAMI.ToString()).SAPCompanyCode) != null)
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
                viewModel.SAMIUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == sami && x.DistributorId == orderDetails.First().OrderMaster.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.SAMINetPayable = viewModel.SAMITotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.SAMIPendingOrderValues + viewModel.SAMICurrentBalance - viewModel.SAMIUnConfirmedPayment <= 0 ? 0 : viewModel.SAMITotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.SAMIPendingOrderValues + viewModel.SAMICurrentBalance - viewModel.SAMIUnConfirmedPayment;

            var HealthTekproductDetails = productDetails.Where(e => e.CompanyId == HealthTek).ToList();
            viewModel.HealthTekSupplies0 = HealthTekproductDetails.Where(e => e.WTaxRate == "0").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.HealthTekSupplies1 = HealthTekproductDetails.Where(e => e.WTaxRate == "1").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.HealthTekSupplies4 = HealthTekproductDetails.Where(e => e.WTaxRate == "4").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.HealthTekTotalOrderValues = HealthTekproductDetails.Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            if (SessionHelper.SAPOrderPendingValue != null && SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Healthtek.ToString()).SAPCompanyCode) != null)
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
                viewModel.HealthTekUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == HealthTek && x.DistributorId == orderDetails.First().OrderMaster.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.HealthTekNetPayable = viewModel.HealthTekTotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.HealthTekPendingOrderValues + viewModel.HealthTekCurrentBalance - viewModel.HealthTekUnConfirmedPayment <= 0 ? 0 : viewModel.HealthTekTotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.HealthTekPendingOrderValues + viewModel.HealthTekCurrentBalance - viewModel.HealthTekUnConfirmedPayment;

            var PhytekproductDetails = productDetails.Where(e => e.CompanyId == Phytek).ToList();
            viewModel.PhytekSupplies0 = PhytekproductDetails.Where(e => e.WTaxRate == "0").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.PhytekSupplies1 = PhytekproductDetails.Where(e => e.WTaxRate == "1").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.PhytekSupplies4 = PhytekproductDetails.Where(e => e.WTaxRate == "4").Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.PhytekTotalOrderValues = PhytekproductDetails.Sum(e => e.TotalPrice - ((e.TotalPrice / 100) * Math.Abs(e.Discount)));
            if (SessionHelper.SAPOrderPendingValue != null && SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Phytek.ToString()).SAPCompanyCode) != null)
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
                viewModel.HealthTekUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == Phytek && x.DistributorId == orderDetails.First().OrderMaster.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.PhytekNetPayable = viewModel.PhytekTotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.PhytekPendingOrderValues + viewModel.PhytekCurrentBalance - viewModel.PhytekUnConfirmedPayment <= 0 ? 0 : viewModel.PhytekTotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.PhytekPendingOrderValues + viewModel.PhytekCurrentBalance - viewModel.PhytekUnConfirmedPayment;
            return viewModel;
        }
        public OrderValueViewModel GetOrderValueModel(List<OrderValue> OrderValue)
        {
            List<Company> companies = new CompanyBLL(_unitOfWork).GetAllCompany();
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
                if (SessionHelper.LoginUser.IsDistributor && SessionHelper.SAPOrderPendingValue != null && SessionHelper.SAPOrderPendingValue.Count() > 0)
                {
                    viewModel.SAMIPendingOrderValues = Convert.ToDouble(SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.SAMI.ToString()).SAPCompanyCode).PendingValue);
                }
                else
                {
                    viewModel.SAMIPendingOrderValues = SAMIproductDetails.PendingOrderValues;
                }
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
                if (SessionHelper.LoginUser.IsDistributor && SessionHelper.SAPOrderPendingValue != null && SessionHelper.SAPOrderPendingValue.Count() > 0)
                {
                    viewModel.HealthTekPendingOrderValues = Convert.ToDouble(SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Healthtek.ToString()).SAPCompanyCode).PendingValue);
                }
                else
                {
                    viewModel.HealthTekPendingOrderValues = HealthTekproductDetails.PendingOrderValues;
                }
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
                if (SessionHelper.LoginUser.IsDistributor && SessionHelper.SAPOrderPendingValue != null && SessionHelper.SAPOrderPendingValue.Count() > 0 && SessionHelper.SAPOrderPendingValue.Select(x => x.CompanyCode).Contains(companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Phytek.ToString()).SAPCompanyCode))
                {
                    viewModel.PhytekPendingOrderValues = Convert.ToDouble(SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Phytek.ToString()).SAPCompanyCode).PendingValue);
                }
                else
                {
                    viewModel.PhytekPendingOrderValues = PhytekproductDetails.PendingOrderValues;
                }
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
            viewModel.NetPayable = viewModel.SAMITotalOrderValues + viewModel.HealthTekTotalOrderValues + viewModel.PhytekTotalOrderValues;
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
        public JsonResponse Save(OrderMaster model, IUrlHelper Url)
        {
            JsonResponse jsonResponse = new JsonResponse();
            List<DistributorWiseProductDiscountAndPrices> DistributorWiseProductDiscountAndPrices = discountAndPricesBll.Where(e => e.DistributorId == SessionHelper.LoginUser.DistributorId && e.ProductDetail != null).ToList();
            double SalesTaxRate = SessionHelper.LoginUser.IsDistributor ? (SessionHelper.LoginUser.Distributor.IsFiler ? 17 : 20) : (new DistributorBLL(_unitOfWork).Where(x => x.Id == model.DistributorId).First().IsFiler ? 17 : 20);

            if (model.Id == 0)
            {
                model.TotalValue = model.DistributorWiseProduct.Select(e => e.ProductPrice * e.ProductDetail.ProductMaster.Quantity).Sum();
                model.DistributorId = SessionHelper.LoginUser.DistributorId ?? 1;
                Add(model);
                if (model.Id > 0)
                {
                    var item = _repository.GetById(model.Id);
                    model.SNo = item.SNo;
                    UpdateSNo(model);
                }
                List<OrderDetail> details = new List<OrderDetail>();
                foreach (var item in model.DistributorWiseProduct)
                {
                    details.Add(new OrderDetail()
                    {
                        OrderId = model.Id,
                        ProductId = item.ProductDetail.ProductMasterId,
                        Quantity = item.ProductDetail.ProductMaster.Quantity,
                        ApprovedQuantity = 0,
                        Amount = (item.ProductDetail.ProductMaster.Quantity * DistributorWiseProductDiscountAndPrices.FirstOrDefault(x => x.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).ProductPrice
                        * (1 - (-1 * DistributorWiseProductDiscountAndPrices.FirstOrDefault(x => x.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).Discount / 100)))
                        + (item.ProductDetail.ProductMaster.Quantity * (DistributorWiseProductDiscountAndPrices.FirstOrDefault(x => x.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).ProductPrice)
                        * (1 - (-1 * DistributorWiseProductDiscountAndPrices.FirstOrDefault(x => x.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).Discount / 100)) * (SalesTaxRate / 100 + 0)),
                        ProductPrice = DistributorWiseProductDiscountAndPrices.First(y => y.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).ProductPrice,
                        Discount = DistributorWiseProductDiscountAndPrices.First(y => y.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).Discount,
                        QuanityCarton = Math.Ceiling(item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize),
                        QuanitySF = Math.Ceiling((item.ProductDetail.ProductMaster.Quantity - ((item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize) / item.ProductDetail.ProductMaster.SFSize)).ToString() == "-∞" ? 0 : Math.Ceiling((item.ProductDetail.ProductMaster.Quantity - ((item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize) / item.ProductDetail.ProductMaster.SFSize)),
                        QuanityLoose = (item.ProductDetail.ProductMaster.Quantity - ((item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize) + ((item.ProductDetail.ProductMaster.Quantity - ((item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize) / item.ProductDetail.ProductMaster.SFSize) * item.ProductDetail.ProductMaster.SFSize)).ToString() == "NaN" ? 0 : (item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize + ((item.ProductDetail.ProductMaster.Quantity - ((item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize) / item.ProductDetail.ProductMaster.SFSize) * item.ProductDetail.ProductMaster.SFSize),
                        CreatedBy = SessionHelper.LoginUser.Id,
                        CreatedDate = DateTime.Now,
                    });
                }
                _orderDetailBLL.AddRange(details);
                var ProductsIds = _orderDetailBLL.Where(e => e.OrderId == model.Id).ToList().Select(e => e.ProductId);
                var ProductDetails = DistributorWiseProductDiscountAndPrices.Where(e => ProductsIds.Contains(e.ProductDetail.ProductMasterId) && e.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                var discountWiseProduct = DistributorWiseProductDiscountAndPrices.Where(e => ProductDetails.Select(c => c.ProductDetail.ProductMaster.SAPProductCode).Contains(e.SAPProductCode) && e.DistributorId == SessionHelper.LoginUser.DistributorId);
                ProductDetails.ForEach(x => x.ProductDetail.ProductMaster.Quantity = model.DistributorWiseProduct.First(y => y.ProductDetail.ProductMasterId == x.ProductDetail.ProductMasterId).ProductDetail.ProductMaster.Quantity);
                AddRange(GetValues(GetOrderValueModel(ProductDetails), model.Id));
                jsonResponse.Status = true;
                jsonResponse.Message = model.Status == OrderStatus.Draft ? OrderContant.OrderDraft + " Order Id: " + model.SNo : OrderContant.OrderSubmit + " Order Id: " + model.SNo;
                jsonResponse.RedirectURL = Url.Action("Index", "Order");
            }
            else
            {
                model.TotalValue = model.DistributorWiseProduct.Select(e => e.ProductPrice * e.ProductDetail.ProductMaster.Quantity).Sum();
                model.DistributorId = SessionHelper.LoginUser.DistributorId ?? 1;
                Update(model);
                _orderDetailBLL.DeleteRange(_orderDetailBLL.Where(e => e.OrderId == model.Id).ToList());
                List<OrderDetail> details = new List<OrderDetail>();
                foreach (var item in model.DistributorWiseProduct)
                {
                    details.Add(new OrderDetail()
                    {
                        OrderId = model.Id,
                        ProductId = item.ProductDetail.ProductMasterId,
                        Quantity = item.ProductDetail.ProductMaster.Quantity,
                        ApprovedQuantity = 0,
                        Amount = (item.ProductDetail.ProductMaster.Quantity * DistributorWiseProductDiscountAndPrices.FirstOrDefault(x => x.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).ProductPrice 
                        * (1 - (-1 * DistributorWiseProductDiscountAndPrices.FirstOrDefault(x => x.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).Discount / 100))) 
                        + (item.ProductDetail.ProductMaster.Quantity * (DistributorWiseProductDiscountAndPrices.FirstOrDefault(x => x.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).ProductPrice) 
                        * (1 - (-1 * DistributorWiseProductDiscountAndPrices.FirstOrDefault(x => x.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).Discount / 100)) * (SalesTaxRate / 100 + 0)),
                        ProductPrice = DistributorWiseProductDiscountAndPrices.First(y => y.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).ProductPrice,
                        Discount = DistributorWiseProductDiscountAndPrices.First(y => y.ProductDetail.ProductMasterId == item.ProductDetail.ProductMasterId).Discount,
                        QuanityCarton = Math.Ceiling(item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize),
                        QuanitySF = Math.Ceiling((item.ProductDetail.ProductMaster.Quantity - ((item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize) / item.ProductDetail.ProductMaster.SFSize)).ToString() == "-∞" ? 0 : Math.Ceiling((item.ProductDetail.ProductMaster.Quantity - ((item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize) / item.ProductDetail.ProductMaster.SFSize)),
                        QuanityLoose = (item.ProductDetail.ProductMaster.Quantity - ((item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize) + ((item.ProductDetail.ProductMaster.Quantity - ((item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize) / item.ProductDetail.ProductMaster.SFSize) * item.ProductDetail.ProductMaster.SFSize)).ToString() == "NaN" ? 0 : (item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize + ((item.ProductDetail.ProductMaster.Quantity - ((item.ProductDetail.ProductMaster.Quantity / item.ProductDetail.ProductMaster.CartonSize) * item.ProductDetail.ProductMaster.CartonSize) / item.ProductDetail.ProductMaster.SFSize) * item.ProductDetail.ProductMaster.SFSize),
                        CreatedBy = SessionHelper.LoginUser.Id,
                        CreatedDate = DateTime.Now,
                    });
                }
                _orderDetailBLL.AddRange(details);
                var ProductsIds = _orderDetailBLL.Where(e => e.OrderId == model.Id).ToList().Select(e => e.ProductId);
                var ProductDetails = DistributorWiseProductDiscountAndPrices.Where(e => ProductsIds.Contains(e.ProductDetail.ProductMasterId) && e.DistributorId == SessionHelper.LoginUser.DistributorId).ToList();
                var discountWiseProduct = DistributorWiseProductDiscountAndPrices.Where(e => ProductDetails.Select(c => c.ProductDetail.ProductMaster.SAPProductCode).Contains(e.SAPProductCode) && e.DistributorId == SessionHelper.LoginUser.DistributorId);
                ProductDetails.ForEach(x => x.ProductDetail.ProductMaster.Quantity = model.DistributorWiseProduct.First(y => y.ProductDetail.ProductMasterId == x.ProductDetail.ProductMasterId).ProductDetail.ProductMaster.Quantity);
                DeleteRange(_OrderValueBLL.GetOrderValueByOrderId(model.Id));
                AddRange(GetValues(GetOrderValueModel(ProductDetails), model.Id));
                jsonResponse.Status = true;
                model = _repository.GetById(model.Id);
                model.SNo = model.SNo;
                jsonResponse.Message = model.Status == OrderStatus.Draft ? OrderContant.OrderDraft + " Order Id: " + model.SNo : OrderContant.OrderSubmit + " Order Id: " + model.SNo;
                jsonResponse.RedirectURL = Url.Action("Index", "Order");
            }
            return jsonResponse;
        }
        public List<OrderStatusViewModel> PlaceOrderToSAP(int OrderId)
        {
            List<OrderStatusViewModel> model = new List<OrderStatusViewModel>();
            var orderproduct = _orderDetailBLL.Where(e => e.OrderId == OrderId).ToList();
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
                    REQ_QTY = item.Quantity.ToString()
                });
            }
            return model;
        }
        public List<OrderStatusViewModel> PlaceOrderPartiallyApprovedToSAP(int OrderId)
        {
            List<OrderStatusViewModel> model = new List<OrderStatusViewModel>();
            var orderproduct = _orderDetailBLL.Where(e => e.OrderId == OrderId && e.SAPOrderNumber == null).ToList();
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
                    REQ_QTY = item.Quantity.ToString()
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
                LamdaId = LamdaId.And(e => e.SNo == model.OrderNo);
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
                             SNo = x.SNo,
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
                             RejectedDate = x.RejectedDate,
                             RejectedComment = x.RejectedComment,
                             OnHoldComment = x.OnHoldComment,
                         }).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }
        public DistributorBalance GetBalance(string DistributorCode, Configuration configuration)
        {
            try
            {
                DistributorBalance distributorBalance = new DistributorBalance();
                Root root = new Root();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes("sami_po:wasay123")); //("Username:Password")  
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                    var result = client.GetAsync(new Uri("http://10.0.3.35:51000/RESTAdapter/DistBal?DISTRIBUTOR=" + DistributorCode)).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var JsonContent = result.Content.ReadAsStringAsync().Result;
                        root = JsonConvert.DeserializeObject<Root>(JsonContent);
                    }
                }
                if (distributorBalance == null)
                {
                    return new DistributorBalance();
                }
                else
                {
                    distributorBalance.SAMI = root.ZWASITDPDISTBALANCEBAPIResponse.BAL_SAMI;
                    distributorBalance.HealthTek = root.ZWASITDPDISTBALANCEBAPIResponse.BAL_HTL;
                    return distributorBalance;
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
                //var Client = new RestClient(_configuration.GetPendingOrderValue + "?DistributorId=" + DistributorCode);
                //var request = new RestRequest(Method.POST);
                //IRestResponse response = Client.Execute(request);
                //var SAPOrderPendingValue = JsonConvert.DeserializeObject<List<SAPOrderPendingValue>>(response.Content);
                List<SAPOrderPendingValue> sAPOrderPendingValue = new List<SAPOrderPendingValue>();
                Root root = new Root();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes("sami_po:wasay123"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                    var result = client.GetAsync(new Uri("http://10.0.3.35:51000/RESTAdapter/getSalesQuery?DISTRIBUTOR=" + DistributorCode)).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var JsonContent = result.Content.ReadAsStringAsync().Result;
                        root = JsonConvert.DeserializeObject<Root>(JsonContent.ToString());
                        if (root != null)
                        {
                            for (int i = 0; i < root.ZWAS_BI_SALES_QUERY_BAPI.PENDING.item.Count(); i++)
                            {
                                sAPOrderPendingValue.Add(new SAPOrderPendingValue()
                                {
                                    CompanyCode = root.ZWAS_BI_SALES_QUERY_BAPI.PENDING.item[i].VKORG,
                                    PendingValue = root.ZWAS_BI_SALES_QUERY_BAPI.PENDING.item[i].NETWR,
                                });
                            }
                        }
                    }
                }
                if (sAPOrderPendingValue == null)
                {
                    return new List<SAPOrderPendingValue>();
                }
                else
                {
                    return sAPOrderPendingValue;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return new List<SAPOrderPendingValue>();
            }
        }
        public OrderValueViewModel GetOrderValueModel(List<DistributorWiseProductDiscountAndPrices> productDetails)
        {
            List<Company> companies = new CompanyBLL(_unitOfWork).GetAllCompany();
            OrderValueViewModel viewModel = new OrderValueViewModel();
            List<OrderDetail> orderDetails = _orderDetailBLL.Where(x => x.OrderMaster.IsDeleted == false && x.OrderMaster.Status == OrderStatus.PendingApproval && (SessionHelper.LoginUser.IsDistributor == true ? x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId : true)).ToList();
            List<ProductDetail> productDetailList = ProductDetailBLL.Where(x => orderDetails.Select(x => x.ProductId).Contains(x.ProductMaster.Id));
            var sami = Convert.ToInt32(CompanyEnum.SAMI);
            var HealthTek = Convert.ToInt32(CompanyEnum.Healthtek);
            var Phytek = Convert.ToInt32(CompanyEnum.Phytek);
            productDetails.ForEach(e => e.ProductDetail.TotalPrice = e.ProductDetail.ProductMaster.Quantity * e.ProductPrice);
            var SAMIproductDetails = productDetails.Where(e => e.ProductDetail.CompanyId == sami).ToList();
            viewModel.SAMISupplies0 = SAMIproductDetails.Where(e => e.ProductDetail.WTaxRate == "0").Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.SAMISupplies1 = SAMIproductDetails.Where(e => e.ProductDetail.WTaxRate == "1").Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.SAMISupplies4 = SAMIproductDetails.Where(e => e.ProductDetail.WTaxRate == "4").Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.SAMITotalOrderValues = SAMIproductDetails.Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
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
            if (SessionHelper.SAPOrderPendingValue != null && SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.SAMI.ToString()).SAPCompanyCode) != null)
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
                viewModel.SAMIUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == sami && x.DistributorId == orderDetails.First().OrderMaster.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.SAMINetPayable = viewModel.SAMITotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.SAMIPendingOrderValues + viewModel.SAMICurrentBalance - viewModel.SAMIUnConfirmedPayment <= 0 ? 0 : viewModel.SAMITotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.SAMIPendingOrderValues + viewModel.SAMICurrentBalance - viewModel.SAMIUnConfirmedPayment;

            var HealthTekproductDetails = productDetails.Where(e => e.ProductDetail.CompanyId == HealthTek).ToList();
            viewModel.HealthTekSupplies0 = HealthTekproductDetails.Where(e => e.ProductDetail.WTaxRate == "0").Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.HealthTekSupplies1 = HealthTekproductDetails.Where(e => e.ProductDetail.WTaxRate == "1").Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.HealthTekSupplies4 = HealthTekproductDetails.Where(e => e.ProductDetail.WTaxRate == "4").Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.HealthTekTotalOrderValues = HealthTekproductDetails.Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            if (SessionHelper.SAPOrderPendingValue != null && SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Healthtek.ToString()).SAPCompanyCode) != null)
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
                viewModel.HealthTekUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == HealthTek && x.DistributorId == orderDetails.First().OrderMaster.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.HealthTekNetPayable = viewModel.HealthTekTotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.HealthTekPendingOrderValues + viewModel.HealthTekCurrentBalance - viewModel.HealthTekUnConfirmedPayment <= 0 ? 0 : viewModel.HealthTekTotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.HealthTekPendingOrderValues + viewModel.HealthTekCurrentBalance - viewModel.HealthTekUnConfirmedPayment;

            var PhytekproductDetails = productDetails.Where(e => e.ProductDetail.CompanyId == Phytek).ToList();
            viewModel.PhytekSupplies0 = PhytekproductDetails.Where(e => e.ProductDetail.WTaxRate == "0").Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.PhytekSupplies1 = PhytekproductDetails.Where(e => e.ProductDetail.WTaxRate == "1").Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.PhytekSupplies4 = PhytekproductDetails.Where(e => e.ProductDetail.WTaxRate == "4").Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            viewModel.PhytekTotalOrderValues = PhytekproductDetails.Sum(e => e.ProductDetail.TotalPrice - ((e.ProductDetail.TotalPrice / 100) * Math.Abs(e.Discount)));
            if (SessionHelper.SAPOrderPendingValue != null && SessionHelper.SAPOrderPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Phytek.ToString()).SAPCompanyCode) != null)
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
                viewModel.HealthTekUnConfirmedPayment = _PaymentBLL.Where(x => x.CompanyId == Phytek && x.DistributorId == orderDetails.First().OrderMaster.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.PhytekNetPayable = viewModel.PhytekTotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.PhytekPendingOrderValues + viewModel.PhytekCurrentBalance - viewModel.PhytekUnConfirmedPayment <= 0 ? 0 : viewModel.PhytekTotalOrderValues + viewModel.TotalUnapprovedOrderValues + viewModel.PhytekPendingOrderValues + viewModel.PhytekCurrentBalance - viewModel.PhytekUnConfirmedPayment;
            return viewModel;
        }
        public List<SAPOrderPendingQuantity> GetDistributorOrderPendingQuantity(string DistributorId, Configuration _configuration)
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                SendTimeout = TimeSpan.FromSeconds(100000),
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                AllowCookies = true,
                ReaderQuotas = XmlDictionaryReaderQuotas.Max,
            };
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            EndpointAddress address = new EndpointAddress("http://s049sappodev.samikhi.com:51000/XISOAPAdapter/MessageServlet?senderParty=&senderService=NSAP_DEV&receiverParty=&receiverService=&interface=DPPendingOrdersRequest_Out&interfaceNamespace=https://www.sami.com/DPPendingOrders");
            DPPendingOrdersRequest_OutClient client = new DPPendingOrdersRequest_OutClient(binding, address);
            client.ClientCredentials.UserName.UserName = "SAMI_PO";
            client.ClientCredentials.UserName.Password = "wasay123";
            if (client.InnerChannel.State == CommunicationState.Faulted)
            {
            }
            else
            {
                client.OpenAsync();
            }
            ZST_PENDING_ORDER_OUT[] ZST_PENDING_ORDER_OUTList = client.DPPendingOrdersRequest_Out(SessionHelper.LoginUser.Distributor.DistributorSAPCode);
            client.CloseAsync();
            List<SAPOrderPendingQuantity> OrderPendingQuantityList = new List<SAPOrderPendingQuantity>();
            if (ZST_PENDING_ORDER_OUTList != null)
            {
                for (int i = 0; i < ZST_PENDING_ORDER_OUTList.Length; i++)
                {
                    OrderPendingQuantityList.Add(new SAPOrderPendingQuantity()
                    {
                        ProductCode = ZST_PENDING_ORDER_OUTList[i].MATNR.TrimStart(new char[] { '0' }),
                        OrderQuantity = ZST_PENDING_ORDER_OUTList[i].KWMENG.ToString(),
                        DispatchQuantity = ZST_PENDING_ORDER_OUTList[i].LFIMG.ToString(),
                        PendingQuantity = ZST_PENDING_ORDER_OUTList[i].PENDING.ToString(),
                    });
                }
            }
            return OrderPendingQuantityList;
        }
        public List<SAPOrderStatus> PostDistributorOrder(int OrderId)
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                SendTimeout = TimeSpan.FromSeconds(100000),
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                AllowCookies = true,
                ReaderQuotas = XmlDictionaryReaderQuotas.Max,
            };
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            EndpointAddress address = new EndpointAddress("http://s049sappodev.samikhi.com:51000/XISOAPAdapter/MessageServlet?senderParty=&senderService=NSAP_DEV&receiverParty=&receiverService=&interface=DisPortalPORequest_Out&interfaceNamespace=http%3A%2F%2Fwww.sami.com%2FSalesOrderCreation");
            DisPortalPORequest_OutClient client = new DisPortalPORequest_OutClient(binding, address);
            client.ClientCredentials.UserName.UserName = "SAMI_PO";
            client.ClientCredentials.UserName.Password = "wasay123";
            if (client.InnerChannel.State == CommunicationState.Faulted)
            {
            }
            else
            {
                client.OpenAsync();
            }
            DisPortalRequestIn disPortalRequestIn = new DisPortalRequestIn();
            disPortalRequestIn.IM_SALEDATA = PlaceOrderToSAPPO(OrderId).ToArray();
            DisPortalPORequest_OutRequest disPortalPORequest_OutRequest = new DisPortalPORequest_OutRequest(disPortalRequestIn);
            ZSD_CREATE_SALE_ORDERResponse zSD_CREATE_SALE_ORDERResponse = client.DisPortalPORequest_Out(disPortalRequestIn);
            client.CloseAsync();
            List<SAPOrderStatus> list = new List<SAPOrderStatus>();
            if (zSD_CREATE_SALE_ORDERResponse != null)
            {
                for (int i = 0; i < zSD_CREATE_SALE_ORDERResponse.EX_OUTPUT.Length; i++)
                {
                    list.Add(new SAPOrderStatus()
                    {
                        ProductCode = zSD_CREATE_SALE_ORDERResponse.EX_OUTPUT[i].MATNR,
                        SAPOrderNo = zSD_CREATE_SALE_ORDERResponse.EX_OUTPUT[i].VBELN
                    });
                }
            }
            return list;
        }
        public List<DisPortalRequestInMain> PlaceOrderToSAPPO(int OrderId)
        {
            List<DisPortalRequestInMain> model = new List<DisPortalRequestInMain>();
            var orderproduct = _orderDetailBLL.Where(e => e.OrderId == OrderId && e.SAPOrderNumber == null).ToList();
            var ProductDetail = ProductDetailBLL.Where(e => orderproduct.Select(c => c.ProductId).Contains(e.ProductMasterId)).ToList();
            foreach (var item in orderproduct)
            {
                model.Add(new DisPortalRequestInMain()
                {
                    SNO = string.Format("{0:1000000000}", item.OrderId),
                    ITEMNO = item.OrderId.ToString(),
                    PARTN_NUMB = item.OrderMaster.Distributor.DistributorSAPCode,
                    DOC_TYPE = ProductDetail.First(e => e.ProductMasterId == item.ProductId).S_OrderType,
                    SALES_ORG = ProductDetail.First(e => e.ProductMasterId == item.ProductId).SaleOrganization,
                    DISTR_CHAN = ProductDetail.First(e => e.ProductMasterId == item.ProductId).DistributionChannel,
                    DIVISION = ProductDetail.First(e => e.ProductMasterId == item.ProductId).Division,
                    PURCH_NO = item.OrderMaster.ReferenceNo,
                    PURCH_DATE = (DateTime.Now.Year.ToString() + string.Format("{0:00}", DateTime.Now.Month) + string.Format("{0:00}", DateTime.Now.Day)).ToString(),
                    PRICE_DATE = (DateTime.Now.Year.ToString() + string.Format("{0:00}", DateTime.Now.Month) + string.Format("{0:00}", DateTime.Now.Day)).ToString(),
                    ST_PARTN = item.OrderMaster.Distributor.DistributorSAPCode,
                    MATERIAL = ProductDetail.First(e => e.ProductMasterId == item.ProductId).ProductMaster.SAPProductCode,
                    PLANT = ProductDetail.First(e => e.ProductMasterId == item.ProductId).DispatchPlant,
                    STORE_LOC = ProductDetail.First(e => e.ProductMasterId == item.ProductId).S_StorageLocation,
                    BATCH = "",
                    ITEM_CATEG = ProductDetail.First(e => e.ProductMasterId == item.ProductId).SalesItemCategory,
                    REQ_QTY = item.Quantity.ToString()
                });
            }
            return model;
        }
    }
}
