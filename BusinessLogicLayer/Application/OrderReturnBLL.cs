using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Utility;
using static Utility.Constant.Common;

namespace BusinessLogicLayer.Application
{
    public class OrderReturnBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<OrderReturnMaster> _repository;
        private readonly OrderReturnDetailBLL _OrderReturnDetailBLL;
        private readonly UserBLL _UserBLL;
        private readonly ProductDetailBLL productDetailBLL;
        public OrderReturnBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<OrderReturnMaster>();
            _OrderReturnDetailBLL = new OrderReturnDetailBLL(_unitOfWork);
            _UserBLL = new UserBLL(_unitOfWork);
            productDetailBLL = new ProductDetailBLL(_unitOfWork);
        }
        public int Add(OrderReturnMaster module)
        {
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.IsDeleted = false;
            module.IsActive = true;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            return _unitOfWork.Save();
        }
        public bool Update(OrderReturnMaster module)
        {
            var item = _repository.GetById(module.Id);
            item.Status = module.Status;
            item.DebitNoteDate = module.DebitNoteDate;
            item.DebitNoteNo = module.DebitNoteNo;
            item.DistributorId = module.DistributorId;
            item.OrderReturnReasonId = module.OrderReturnReasonId;
            item.TotalValue = module.TotalValue;
            item.Transporter = module.Transporter;
            item.TRDate = module.TRDate;
            item.TRNo = module.TRNo;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _unitOfWork.GenericRepository<OrderReturnMaster>().Update(item);
            return _unitOfWork.Save() > 0;
        }
        public int Delete(int id)
        {
            var item = _unitOfWork.GenericRepository<OrderReturnMaster>().GetById(id);
            item.IsDeleted = true;
            return _unitOfWork.Save();
        }
        public void UpdateStatus(OrderReturnMaster model, OrderReturnStatus OrderReturnStatus, string Remarks)
        {
            //model.Remarks = Remarks;
            //model.Status = OrderReturnStatus;
            model.UpdatedBy = SessionHelper.LoginUser.Id;
            model.UpdatedDate = DateTime.Now;
            _repository.Update(model);
        }
        public OrderReturnMaster GetById(int id)
        {
            return _unitOfWork.GenericRepository<OrderReturnMaster>().GetById(id);
        }
        public List<OrderReturnMaster> GetAllOrderReturn()
        {
            return _unitOfWork.GenericRepository<OrderReturnMaster>().GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<OrderReturnMaster> Where(Expression<Func<OrderReturnMaster, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public OrderReturnMaster FirstOrDefault(Expression<Func<OrderReturnMaster, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public List<OrderReturnMaster> Search(OrderReturnViewModel model)
        {
            var LamdaId = (Expression<Func<OrderReturnMaster, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.Status != null)
            {
                //LamdaId = LamdaId.And(e => e.Status == model.Status);
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
                         select new OrderReturnMaster
                         {
                             Id = x.Id,
                             Distributor = x.Distributor,
                             DistributorId = x.DistributorId,
                             CreatedDate = x.CreatedDate,
                         }).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }
        public List<OrderReturnMaster> SearchReport(OrderReturnSearch model)
        {
            var LamdaId = (Expression<Func<OrderReturnMaster, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.OrderReturnNo != null)
            {
                LamdaId = LamdaId.And(e => e.Id == model.OrderReturnNo);
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
            var Filter = _repository.Where(LamdaId).ToList();
            var query = (from x in Filter
                         join u in _UserBLL.GetAllUser().ToList()
                              on x.CreatedBy equals u.Id
                         select new OrderReturnMaster
                         {
                             Id = x.Id,
                             Distributor = x.Distributor,
                             Status = x.Status,
                             DistributorId = x.DistributorId,
                             CreatedBy = x.CreatedBy,
                             CreatedName = (u.FirstName + " " + u.LastName + " (" + u.UserName + ")"),
                             CreatedDate = x.CreatedDate,
                         }).ToList();

            return query.OrderByDescending(x => x.Id).ToList();
        }

        public JsonResponse UpdateOrderReturn(OrderReturnMaster model, SubmitStatus btnSubmit, IUrlHelper Url)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                _unitOfWork.Begin();                
                if (SessionHelper.AddReturnProduct.Count > 0)
                {
                    if (btnSubmit == SubmitStatus.Draft)
                    {
                        model.Status = OrderReturnStatus.Draft;
                        jsonResponse.Message = OrderContant.OrderDraft;                        
                    }
                    else
                    {
                        model.Status = OrderReturnStatus.Submitted;
                        jsonResponse.Message = OrderContant.OrderSubmit;
                        jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                    }
                    if (model.Id == 0)
                    {
                        model.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
                        var detail = SessionHelper.AddReturnProduct;
                        Add(model);
                        detail.ForEach(e => e.OrderReturnId = model.Id);
                        detail.ForEach(e => e.CreatedBy = SessionHelper.LoginUser.Id);
                        detail.ForEach(e => e.CreatedDate = DateTime.Now);
                        detail.ForEach(e => e.ProductMaster = null);
                        detail.ForEach(e => e.PlantLocation = null);
                        _OrderReturnDetailBLL.AddRange(detail);
                        jsonResponse.RedirectURL = model.Status == OrderReturnStatus.Draft ? Url.Action("Add", "OrderReturn", new { id = model.Id }) : Url.Action("Index", "OrderReturn");
                    }
                    else
                    {
                        model.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
                        var detail = SessionHelper.AddReturnProduct;
                        Update(model);
                        var list = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == model.Id).ToList();
                        _OrderReturnDetailBLL.DeleteRange(list);
                        detail.ForEach(e => e.OrderReturnId = model.Id);
                        detail.ForEach(e => e.CreatedBy = SessionHelper.LoginUser.Id);
                        detail.ForEach(e => e.CreatedDate = DateTime.Now);
                        detail.ForEach(e => e.ProductMaster = null);
                        detail.ForEach(e => e.PlantLocation = null);
                        _OrderReturnDetailBLL.AddRange(detail);
                        jsonResponse.RedirectURL = model.Status == OrderReturnStatus.Draft ? Url.Action("Add", "OrderReturn", new { id = model.Id }) : Url.Action("Index", "OrderReturn");
                    }
                    jsonResponse.Status = true;
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = OrderContant.OrderItem;
                }
                _unitOfWork.Commit();
                new AuditTrailBLL(_unitOfWork).AddAuditTrail("OrderReturnMaster", "SaveEdit", "End Click on Save Button of ");
                return jsonResponse;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw ex;
            }
            
        }

        public List<OrderStatusViewModel> PlaceReturnOrderToSAP(int OrderReturnId)
        {
            List<OrderStatusViewModel> model = new List<OrderStatusViewModel>();
            var orderproduct = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == OrderReturnId).ToList();
            var ProductDetail = productDetailBLL.Where(e => orderproduct.Select(c => c.ProductId).Contains(e.ProductMasterId)).ToList();
            foreach (var item in orderproduct)
            {
                model.Add(new OrderStatusViewModel()
                {
                    SNO = string.Format("{0:0000000000}", item.OrderReturnId),
                    ITEMNO = "",
                    PARTN_NUMB = item.OrderReturnMaster.Distributor.DistributorSAPCode,
                    DOC_TYPE = ProductDetail.First(e => e.ProductMasterId == item.ProductId).R_OrderType,
                    SALES_ORG = ProductDetail.First(e => e.ProductMasterId == item.ProductId).SaleOrganization,
                    DISTR_CHAN = ProductDetail.First(e => e.ProductMasterId == item.ProductId).DistributionChannel,
                    DIVISION = ProductDetail.First(e => e.ProductMasterId == item.ProductId).Division,
                    PURCH_NO = item.OrderReturnMaster.TRNo,
                    PURCH_DATE = DateTime.Now,
                    PRICE_DATE = DateTime.Now,
                    ST_PARTN = item.OrderReturnMaster.Distributor.DistributorSAPCode,
                    MATERIAL = ProductDetail.First(e => e.ProductMasterId == item.ProductId).ProductMaster.SAPProductCode,
                    PLANT = ProductDetail.First(e => e.ProductMasterId == item.ProductId).DispatchPlant,
                    STORE_LOC = ProductDetail.First(e => e.ProductMasterId == item.ProductId).R_StorageLocation,
                    BATCH = "",
                    ITEM_CATEG = ProductDetail.First(e => e.ProductMasterId == item.ProductId).ReturnItemCategory,
                    REQ_QTY = item.ReceivedQty.ToString()
                });
            }
            return model;
        }
    }
}
