using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utility;
using Utility.HelperClasses;

namespace BusinessLogicLayer.Application
{
    public class PaymentBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<PaymentMaster> _repository;
        private readonly CompanyBLL _CompanyBLL;
        private readonly UserBLL _UserBLL;
        private readonly OrderDetailBLL _OrderDetailBLL;
        private readonly AuditTrailBLL<PaymentMaster> _AuditTrailPaymentMaster;
        public PaymentBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<PaymentMaster>();
            _CompanyBLL = new CompanyBLL(_unitOfWork);
            _UserBLL = new UserBLL(_unitOfWork);
            _OrderDetailBLL = new OrderDetailBLL(_unitOfWork);
            _AuditTrailPaymentMaster = new AuditTrailBLL<PaymentMaster>(_unitOfWork);
        }
        public bool Add(PaymentMaster module)
        {
            module.IsDeleted = false;
            module.IsActive = true;
            module.CreatedBy = SessionHelper.LoginUser.Id;
            module.CreatedDate = DateTime.Now;
            _repository.Insert(module);
            _AuditTrailPaymentMaster.AddAuditTrail((int)ApplicationPages.AddPayment, (int)ApplicationActions.Insert, module, "Save Payment", module.CreatedBy);
            return _unitOfWork.Save() > 0;
        }
        public bool Update(PaymentMaster module)
        {
            var item = _repository.GetById(module.Id);
            item.Status = module.Status;
            item.DepositDate = module.DepositDate;
            item.ValueClearingDate = module.ValueClearingDate;
            item.CompanyId = module.CompanyId;
            item.PaymentModeId = module.PaymentModeId;
            item.PaymentModeNo = module.PaymentModeNo;
            item.Amount = module.Amount;
            item.DepositorBankName = module.DepositorBankName;
            item.DepositorBankCode = module.DepositorBankCode;
            item.CompanyBankName = module.CompanyBankName;
            item.CompanyBankCode = module.CompanyBankCode;
            item.IsDeleted = false;
            item.IsActive = true;
            item.UpdatedBy = SessionHelper.LoginUser.Id;
            item.UpdatedDate = DateTime.Now;
            _repository.Update(item);
            _AuditTrailPaymentMaster.AddAuditTrail((int)ApplicationPages.AddPayment, (int)ApplicationActions.Update, module, "Update Payment", (int)item.UpdatedBy);
            return _unitOfWork.Save() > 0;
        }
        public bool UpdateApproval(PaymentMaster module)
        {
            var item = _repository.GetById(module.Id);
            item.Status = PaymentStatus.Verified;
            item.ValueClearingDate = module.ValueClearingDate;
            item.PaymentModeId = module.PaymentModeId;
            item.PaymentModeNo = module.PaymentModeNo;
            item.CompanyBankName = module.CompanyBankName;
            item.SAPCompanyCode = module.SAPCompanyCode;
            item.SAPDocumentNumber = module.SAPDocumentNumber;
            item.SAPFiscalYear = module.SAPFiscalYear;
            item.ApprovedBy = SessionHelper.LoginUser.Id;
            item.ApprovedDate = DateTime.Now;
            _repository.Update(item);
            return _unitOfWork.Save() > 0;
        }
        public int UpdateSNo(PaymentMaster module)
        {
            var item = _repository.GetById(module.Id);
            if (_repository.GetAllList().Count() > 1)
            {
                item.SNo = _repository.GetAllList().Max(y => y.SNo) + 1;
            }
            else
            {
                item.SNo = 300000001;
            }
            _repository.Update(item);
            return _unitOfWork.Save();
        }
        public bool UpdateStatus(PaymentMaster model, PaymentStatus paymentStatus, string Remarks)
        {
            if (PaymentStatus.Rejected == paymentStatus)
            {
                model.RejectedBy = SessionHelper.LoginUser.Id;
                model.RejectedDate = DateTime.Now;
                model.Remarks = Remarks;
            }
            else
            {
                model.ResubmitRemarks = Remarks;
                model.UpdatedBy = SessionHelper.LoginUser.Id;
                model.UpdatedDate = DateTime.Now;
            }
            model.Status = paymentStatus;
            _repository.Update(model);
            return _unitOfWork.Save() > 0;
        }
        public PaymentMaster GetById(int id)
        {
            return _repository.GetById(id);
        }
        public List<PaymentMaster> GetAllPaymentMaster()
        {
            return _repository.GetAllList().Where(x => x.IsDeleted == false).ToList();
        }
        public List<PaymentMaster> Where(Expression<Func<PaymentMaster, bool>> predicate)
        {
            return _repository.Where(predicate);
        }
        public List<PaymentMaster> Search(PaymentViewModel model)
        {
            var LamdaId = (Expression<Func<PaymentMaster, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.PaymentNo != null)
            {
                LamdaId = LamdaId.And(e => e.SNo == model.PaymentNo);
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
                         join u in _UserBLL.GetUsers().ToList()
                              on x.CreatedBy equals u.Id
                         join ua in _UserBLL.GetUsers().ToList()
                              on x.ApprovedBy equals ua.Id into approvedGroup
                         from a1 in approvedGroup.DefaultIfEmpty()
                         join ur in _UserBLL.GetUsers().ToList()
                              on x.RejectedBy equals ur.Id into rejectedGroup
                         from a2 in rejectedGroup.DefaultIfEmpty()
                         select new PaymentMaster
                         {
                             Id = x.Id,
                             SNo = x.SNo,
                             CompanyId = x.CompanyId,
                             Company = x.Company,
                             Distributor = x.Distributor,
                             PaymentMode = x.PaymentMode,
                             Amount = x.Amount,
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
                         });

            return query.ToList();
        }
        public List<PaymentMaster> SearchReport(PaymentSearch model)
        {
            var LamdaId = (Expression<Func<PaymentMaster, bool>>)(x => x.IsDeleted == false);
            if (model.DistributorId != null)
            {
                LamdaId = LamdaId.And(e => e.DistributorId == model.DistributorId);
            }
            if (model.PaymentNo != null)
            {
                LamdaId = LamdaId.And(e => e.SNo == model.PaymentNo);
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
                              on x.ApprovedBy equals ua.Id into approvedGroup
                         from a1 in approvedGroup.DefaultIfEmpty()
                         join ur in _UserBLL.GetUsers().ToList()
                              on x.RejectedBy equals ur.Id into rejectedGroup
                         from a2 in rejectedGroup.DefaultIfEmpty()
                         select new PaymentMaster
                         {
                             Id = x.Id,
                             SNo = x.SNo,
                             Company = x.Company,
                             CompanyId = x.CompanyId,
                             PaymentMode = x.PaymentMode,
                             Distributor = x.Distributor,
                             Status = x.Status,
                             Amount = x.Amount,
                             SAPDocumentNumber = x.SAPDocumentNumber,
                             SAPFiscalYear = x.SAPFiscalYear,
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
                             Remarks = x.Remarks,
                         });

            return query.ToList();
        }
        public SAPPaymentViewModel AddPaymentToSAP(int PaymentId, DateTime? ValueClearingDate)
        {
            var payment = GetAllPaymentMaster().Where(e => e.Id == PaymentId).FirstOrDefault();
            SAPPaymentViewModel model = new SAPPaymentViewModel()
            {
                IsPaymentAllowedInSAP = payment.Company.IsPaymentAllowedInSAP,
                PAY_ID = payment.PaymentModeId.ToString(),
                REF = payment.PaymentModeNo.ToString(),
                COMPANY = _CompanyBLL.GetAllCompany().FirstOrDefault(x => x.Id == payment.CompanyId).SAPCompanyCode,
                AMOUNT = payment.Amount.ToString(),
                DISTRIBUTOR = payment.Distributor.DistributorSAPCode,
                B_CODE = new BankBLL(_unitOfWork).GetAllBank().FirstOrDefault(x => x.BranchCode == payment.CompanyBankCode && x.CompanyId == GetById(PaymentId).CompanyId).GLAccount,
                P_DATE = payment.ValueClearingDate == null ? (Convert.ToDateTime(ValueClearingDate).Year.ToString() + string.Format("{0:00}", Convert.ToDateTime(ValueClearingDate).Month.ToString()) + string.Format("{0:00}", Convert.ToDateTime(ValueClearingDate).Day.ToString())).ToString()
                : (Convert.ToDateTime(payment.ValueClearingDate).Year.ToString() + string.Format("{0:00}", Convert.ToDateTime(payment.ValueClearingDate).Month.ToString()) + string.Format("{0:00}", Convert.ToDateTime(payment.ValueClearingDate).Day.ToString())).ToString(),
            };
            return model;
        }
        public PaymentValueViewModel GetOrderValueModel(int DistributorId)
        {
            PaymentValueViewModel viewModel = new PaymentValueViewModel();
            var sami = Convert.ToInt32(CompanyEnum.SAMI);
            var HealthTek = Convert.ToInt32(CompanyEnum.Healthtek);
            var Phytek = Convert.ToInt32(CompanyEnum.Phytek);
            List<Company> companies = new CompanyBLL(_unitOfWork).GetAllCompany();
            List<OrderDetail> orderDetails = _OrderDetailBLL.Where(x => x.OrderMaster.IsActive && !x.OrderMaster.IsDeleted && x.OrderMaster.Status == OrderStatus.PendingApproval
            && (SessionHelper.LoginUser.IsDistributor == true ? x.OrderMaster.DistributorId == SessionHelper.LoginUser.DistributorId : x.OrderMaster.DistributorId == DistributorId)).ToList();
            List<ProductDetail> productDetails = new ProductDetailBLL(_unitOfWork).Where(x => orderDetails.Select(x => x.ProductId).Contains(x.ProductMaster.Id));

            viewModel.SAMITotalUnapprovedOrderValues = (from od in orderDetails
                                                        join p in productDetails on od.ProductId equals p.ProductMasterId
                                                        where od.ProductId == p.ProductMasterId && p.CompanyId == sami
                                                        group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                                        let Amount = odp.Sum(m => m.od.Amount)
                                                        select Amount).Sum(x => x);

            viewModel.HealthTekTotalUnapprovedOrderValues = (from od in orderDetails
                                                             join p in productDetails on od.ProductId equals p.ProductMasterId
                                                             where od.ProductId == p.ProductMasterId && p.CompanyId == HealthTek
                                                             group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                                             let Amount = odp.Sum(m => m.od.Amount)
                                                             select Amount).Sum(x => x);

            viewModel.PhytekTotalUnapprovedOrderValues = (from od in orderDetails
                                                          join p in productDetails on od.ProductId equals p.ProductMasterId
                                                          where od.ProductId == p.ProductMasterId && p.CompanyId == Phytek
                                                          group new { od, p } by new { od.OrderId, p.CompanyId } into odp
                                                          let Amount = odp.Sum(m => m.od.Amount)
                                                          select Amount).Sum(x => x);

            if (SessionHelper.DistributorPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.SAMI.ToString()).SAPCompanyCode) != null)
            {
                viewModel.SAMIPendingOrderValues = Convert.ToDouble(SessionHelper.DistributorPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.SAMI.ToString()).SAPCompanyCode).PendingValue);
            }
            viewModel.SAMICurrentBalance = SessionHelper.DistributorBalance.SAMI;
            if (SessionHelper.LoginUser.IsDistributor)
            {
                viewModel.SAMIUnConfirmedPayment = Where(x => x.CompanyId == sami && x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            else
            {
                viewModel.SAMIUnConfirmedPayment = Where(x => x.CompanyId == sami && x.DistributorId == DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.SAMINetPayable = viewModel.SAMITotalUnapprovedOrderValues + viewModel.SAMIPendingOrderValues + viewModel.SAMICurrentBalance - viewModel.SAMIUnConfirmedPayment <= 0 ? 0 : viewModel.SAMITotalUnapprovedOrderValues + viewModel.SAMIPendingOrderValues + viewModel.SAMICurrentBalance - viewModel.SAMIUnConfirmedPayment;

            var HealthTekproductDetails = productDetails.Where(e => e.CompanyId == HealthTek).ToList();
            if (SessionHelper.DistributorPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Healthtek.ToString()).SAPCompanyCode) != null)
            {
                viewModel.HealthTekPendingOrderValues = Convert.ToDouble(SessionHelper.DistributorPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Healthtek.ToString()).SAPCompanyCode).PendingValue);
            }
            viewModel.HealthTekCurrentBalance = SessionHelper.DistributorBalance.HealthTek;
            if (SessionHelper.LoginUser.IsDistributor)
            {
                viewModel.HealthTekUnConfirmedPayment = Where(x => x.CompanyId == HealthTek && x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            else
            {
                viewModel.HealthTekUnConfirmedPayment = Where(x => x.CompanyId == HealthTek && x.DistributorId == DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.HealthTekNetPayable = viewModel.HealthTekTotalUnapprovedOrderValues + viewModel.HealthTekPendingOrderValues + viewModel.HealthTekCurrentBalance - viewModel.HealthTekUnConfirmedPayment <= 0 ? 0 : viewModel.HealthTekTotalUnapprovedOrderValues + viewModel.HealthTekPendingOrderValues + viewModel.HealthTekCurrentBalance - viewModel.HealthTekUnConfirmedPayment;

            var PhytekproductDetails = productDetails.Where(e => e.CompanyId == Phytek).ToList();
            if (SessionHelper.DistributorPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Phytek.ToString()).SAPCompanyCode) != null)
            {
                viewModel.PhytekPendingOrderValues = Convert.ToDouble(SessionHelper.DistributorPendingValue.FirstOrDefault(x => x.CompanyCode == companies.FirstOrDefault(x => x.CompanyName == CompanyEnum.Phytek.ToString()).SAPCompanyCode).PendingValue);
            }
            viewModel.PhytekCurrentBalance = SessionHelper.DistributorBalance.PhyTek;
            if (SessionHelper.LoginUser.IsDistributor)
            {
                viewModel.PhytekUnConfirmedPayment = Where(x => x.CompanyId == Phytek && x.DistributorId == SessionHelper.LoginUser.DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            else
            {
                viewModel.PhytekUnConfirmedPayment = Where(x => x.CompanyId == Phytek && x.DistributorId == DistributorId && x.Status == PaymentStatus.Unverified).Sum(x => x.Amount);
            }
            viewModel.PhytekNetPayable = viewModel.PhytekTotalUnapprovedOrderValues + viewModel.PhytekPendingOrderValues + viewModel.PhytekCurrentBalance - viewModel.PhytekUnConfirmedPayment <= 0 ? 0 : viewModel.PhytekTotalUnapprovedOrderValues + viewModel.PhytekPendingOrderValues + viewModel.PhytekCurrentBalance - viewModel.PhytekUnConfirmedPayment;
            return viewModel;
        }
        public DistributorBalance GetDistributorBalance(string DistributorCode, Configuration configuration)
        {
            DistributorBalance distributorBalance = new DistributorBalance();
            Root root = new Root();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(configuration.POUserName + ":" + configuration.POPassword));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                var result = client.GetAsync(new Uri(configuration.SyncDistributorBalanceURL + DistributorCode)).Result;
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
    }
}
