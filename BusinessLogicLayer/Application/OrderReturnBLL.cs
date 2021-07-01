using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc;
using Models.Application;
using Models.ViewModel;
using OrderReturn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Xml;
using Utility;
using Utility.HelperClasses;
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
            item.Attachment = module.Attachment;
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
        public int UpdateSNo(OrderReturnMaster module)
        {
            var item = _repository.GetById(module.Id);
            if (_repository.GetAllList().Count() > 1)
            {
                item.SNo = _repository.GetAllList().Max(y => y.SNo) + 1;
            }
            else
            {
                item.SNo = 200000001;
            }
            _repository.Update(item);
            return _unitOfWork.Save();
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
                LamdaId = LamdaId.And(e => e.Status == model.Status);
            }
            if (model.OrderReturnNo != null)
            {
                LamdaId = LamdaId.And(e => e.SNo == model.OrderReturnNo);
            }
            if (model.TRNo != null)
            {
                LamdaId = LamdaId.And(e => e.TRNo == model.TRNo);
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
                         join u in _UserBLL.GetUsers().ToList()
                              on x.CreatedBy equals u.Id
                         select new OrderReturnMaster
                         {
                             Id = x.Id,
                             SNo = x.SNo,
                             TRNo = x.TRNo,
                             Distributor = x.Distributor,
                             Status = x.Status,
                             DistributorId = x.DistributorId,
                             OrderReturnReasonId = x.OrderReturnReasonId,
                             CreatedBy = x.CreatedBy,
                             CreatedName = (u.FirstName + " " + u.LastName + " (" + u.UserName + ")"),
                             CreatedDate = x.CreatedDate,
                         });

            return query.ToList();
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
                LamdaId = LamdaId.And(e => e.SNo == model.OrderReturnNo);
            }
            if (model.TRNo != null)
            {
                LamdaId = LamdaId.And(e => e.TRNo == model.TRNo);
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
                         join u in _UserBLL.GetUsers().ToList()
                              on x.CreatedBy equals u.Id
                         join ua in _UserBLL.GetUsers().ToList()
                              on x.ReceivedBy equals ua.Id into receivedGroup
                         from a1 in receivedGroup.DefaultIfEmpty()
                         select new OrderReturnMaster
                         {
                             Id = x.Id,
                             SNo = x.SNo,
                             TRNo = x.TRNo,
                             Distributor = x.Distributor,
                             Status = x.Status,
                             DistributorId = x.DistributorId,
                             OrderReturnReasonId = x.OrderReturnReasonId,
                             CreatedBy = x.CreatedBy,
                             CreatedName = (u.FirstName + " " + u.LastName + " (" + u.UserName + ")"),
                             CreatedDate = x.CreatedDate,
                             ReceivedBy = x.ReceivedBy,
                             ReceivedName = a1 == null ? string.Empty : (a1.FirstName + " " + a1.LastName + " (" + a1.UserName + ")"),
                             ReceivedDate = x.ReceivedDate,
                         });

            return query.ToList();
        }
        public JsonResponse UpdateOrderReturn(OrderReturnMaster model, OrderReturnStatus btnSubmit, IUrlHelper Url)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                _unitOfWork.Begin();
                if (SessionHelper.AddReturnProduct.Count > 0 && SessionHelper.AddReturnProduct != null)
                {
                    if (model.Id == 0)
                    {
                        model.DistributorId = (int)SessionHelper.LoginUser.DistributorId;
                        var detail = SessionHelper.AddReturnProduct;
                        model.TotalValue = SessionHelper.AddReturnProduct.Select(e => e.NetAmount).Sum();
                        Add(model);

                        if (model.Id > 0)
                            UpdateSNo(model);

                        detail.ForEach(e => e.OrderReturnId = model.Id);
                        detail.ForEach(e => e.CreatedBy = SessionHelper.LoginUser.Id);
                        detail.ForEach(e => e.CreatedDate = DateTime.Now);
                        detail.ForEach(e => e.ProductMaster = null);
                        detail.ForEach(e => e.PlantLocation = null);
                        _OrderReturnDetailBLL.AddRange(detail);
                    }
                    else
                    {
                        var item = _repository.GetById(model.Id);
                        model.DistributorId = item.DistributorId;
                        model.SNo = item.SNo;
                        model.Status = btnSubmit;
                        var detail = SessionHelper.AddReturnProduct;
                        model.TotalValue = SessionHelper.AddReturnProduct.Select(e => e.NetAmount).Sum();
                        Update(model);
                        var list = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == model.Id).ToList();
                        _OrderReturnDetailBLL.DeleteRange(list);
                        detail.ForEach(e => e.OrderReturnId = model.Id);
                        detail.ForEach(e => e.CreatedBy = SessionHelper.LoginUser.Id);
                        detail.ForEach(e => e.CreatedDate = DateTime.Now);
                        detail.ForEach(e => e.ProductMaster = null);
                        detail.ForEach(e => e.PlantLocation = null);
                        _OrderReturnDetailBLL.AddRange(detail);
                    }

                    if (btnSubmit == OrderReturnStatus.Draft)
                    {
                        model.Status = OrderReturnStatus.Draft;
                        jsonResponse.Status = true;
                        jsonResponse.Message = model.Status == OrderReturnStatus.Draft ? OrderContant.OrderDraft + Environment.NewLine + " Order Return Id: " + model.SNo : OrderContant.OrderDraft + " Order Return Id: " + model.SNo;
                        jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                    }
                    else
                    {
                        model.Status = OrderReturnStatus.Submitted;
                        jsonResponse.Status = true;
                        jsonResponse.Message = model.Status == OrderReturnStatus.Draft ? OrderContant.OrderReturnSubmit + Environment.NewLine + " Order Return Id: " + model.SNo : OrderContant.OrderReturnSubmit + " Order Return Id: " + model.SNo;
                        jsonResponse.RedirectURL = Url.Action("Index", "OrderReturn");
                    }
                }
                else
                {
                    jsonResponse.Status = false;
                    jsonResponse.Message = OrderContant.OrderItem;
                }
                _unitOfWork.Commit();
                new AuditLogBLL(_unitOfWork).AddAuditLog("OrderReturnMaster", "SaveEdit", "End Click on Save Button of ");
                return jsonResponse;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw ex;
            }
        }
        public List<SAPOrderStatus> PostDistributorOrderReturn(int OrderId, Configuration _Configuration)
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
            EndpointAddress address = new EndpointAddress("http://s049sappodev.samikhi.com:51000/XISOAPAdapter/MessageServlet?senderParty=&senderService=NSAP_DEV&receiverParty=&receiverService=&interface=Ord_return_Request_OUT&interfaceNamespace=http%3A%2F%2Fwww.sami.com%2FDP");
            Ord_return_Request_OUTClient client = new Ord_return_Request_OUTClient(binding, address);
            client.ClientCredentials.UserName.UserName = _Configuration.POUserName;
            client.ClientCredentials.UserName.Password = _Configuration.POPassword;
            if (client.InnerChannel.State == CommunicationState.Faulted)
            {
            }
            else
            {
                client.OpenAsync();
            }
            Ord_return_Request_IN disPortalRequestIn = new Ord_return_Request_IN();
            disPortalRequestIn.ORDERS = PlaceReturnOrderToSAP(OrderId).ToArray();
            Ord_return_Request_OUTRequest disPortalPORequest_OutRequest = new Ord_return_Request_OUTRequest(disPortalRequestIn);
            ZWAS_ORDER_RETURN_UPLOADResponse zSD_CREATE_SALE_ORDERResponse = client.Ord_return_Request_OUT(disPortalRequestIn);
            client.CloseAsync();
            List<SAPOrderStatus> list = new List<SAPOrderStatus>();
            if (zSD_CREATE_SALE_ORDERResponse != null)
            {
                for (int i = 0; i < zSD_CREATE_SALE_ORDERResponse.CREATED.Length; i++)
                {
                    list.Add(new SAPOrderStatus()
                    {
                        ProductCode = zSD_CREATE_SALE_ORDERResponse.CREATED[i].MATNR,
                        SAPOrderNo = zSD_CREATE_SALE_ORDERResponse.CREATED[i].VBELN
                    });
                }
            }
            return list;
        }
        public List<Ord_return_Request_main> PlaceReturnOrderToSAP(int OrderReturnId)
        {
            List<Ord_return_Request_main> model = new List<Ord_return_Request_main>();
            var orderproduct = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == OrderReturnId && SessionHelper.LoginUser.PlantLocationId == e.PlantLocationId && e.IsProductSelected == true && e.ReturnOrderNumber == null).ToList();
            var ProductDetail = productDetailBLL.Where(e => orderproduct.Select(c => c.ProductId).Contains(e.ProductMasterId)).ToList();
            foreach (var item in orderproduct)
            {
                model.Add(new Ord_return_Request_main()
                {
                    SNO = item.OrderReturnMaster.SNo.ToString(),
                    ITEMNO = item.ProductId.ToString(),
                    PARTN_NUMB = !string.IsNullOrEmpty(ProductDetail.First(e => e.ProductMasterId == item.ProductId).ParentDistributor) ? ProductDetail.First(e => e.ProductMasterId == item.ProductId).ParentDistributor : item.OrderReturnMaster.Distributor.DistributorSAPCode,
                    DOC_TYPE = ProductDetail.First(e => e.ProductMasterId == item.ProductId).R_OrderType,
                    SALES_ORG = ProductDetail.First(e => e.ProductMasterId == item.ProductId).SaleOrganization,
                    DISTR_CHAN = ProductDetail.First(e => e.ProductMasterId == item.ProductId).DistributionChannel,
                    DIVISION = ProductDetail.First(e => e.ProductMasterId == item.ProductId).Division,
                    PURCH_NO = item.OrderReturnMaster.TRNo,
                    PURCH_DATE = (DateTime.Now.Year.ToString() + string.Format("{0:00}", DateTime.Now.Month) + string.Format("{0:00}", DateTime.Now.Day)).ToString(),
                    PRICE_DATE = (DateTime.Now.Year.ToString() + string.Format("{0:00}", DateTime.Now.Month) + string.Format("{0:00}", DateTime.Now.Day)).ToString(),
                    ST_PARTN = item.OrderReturnMaster.Distributor.DistributorSAPCode,
                    MATERIAL = ProductDetail.First(e => e.ProductMasterId == item.ProductId).ProductMaster.SAPProductCode,
                    PLANT = ProductDetail.First(e => e.ProductMasterId == item.ProductId).DispatchPlant,
                    STORE_LOC = ProductDetail.First(e => e.ProductMasterId == item.ProductId).R_StorageLocation,
                    BATCH = item.BatchNo,
                    ITEM_CATEG = ProductDetail.First(e => e.ProductMasterId == item.ProductId).ReturnItemCategory,
                    REQ_QTY = item.ReceivedQty.ToString()
                });
            }
            return model;
        }
        public List<OrderStatusViewModel> PlaceReturnOrderPartiallyApprovedToSAP(int OrderReturnId)
        {
            List<OrderStatusViewModel> model = new List<OrderStatusViewModel>();
            var orderproduct = _OrderReturnDetailBLL.Where(e => e.OrderReturnId == OrderReturnId && e.ReturnOrderStatus == null).ToList();
            var ProductDetail = productDetailBLL.Where(e => orderproduct.Select(c => c.ProductId).Contains(e.ProductMasterId)).ToList();
            foreach (var item in orderproduct)
            {
                model.Add(new OrderStatusViewModel()
                {
                    SNO = item.OrderReturnMaster.SNo.ToString(),
                    ITEMNO = item.ProductId.ToString(),
                    PARTN_NUMB = !string.IsNullOrEmpty(ProductDetail.First(e => e.ProductMasterId == item.ProductId).ParentDistributor) ? ProductDetail.First(e => e.ProductMasterId == item.ProductId).ParentDistributor : item.OrderReturnMaster.Distributor.DistributorSAPCode,
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
                    BATCH = item.BatchNo,
                    ITEM_CATEG = ProductDetail.First(e => e.ProductMasterId == item.ProductId).ReturnItemCategory,
                    REQ_QTY = item.ReceivedQty.ToString()
                });
            }
            return model;
        }
    }
}
