using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.HelperClasses;
using Dapper;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Models.ViewModel;
using Newtonsoft.Json;
using ProductWisePriceDiscount;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Utility;
using Utility.HelperClasses;

namespace BusinessLogicLayer.Application
{
    public class DistributorWiseProductDiscountAndPricesBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<DistributorWiseProductDiscountAndPrices> _repository;
        private readonly DistributorPendingQuanityBLL _DistributorPendingQuanityBLL;
        public DistributorWiseProductDiscountAndPricesBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<DistributorWiseProductDiscountAndPrices>();
            _DistributorPendingQuanityBLL = new DistributorPendingQuanityBLL(_unitOfWork);
        }
        public bool AddRange(List<DistributorWiseProductViewModel> module, int DistributorId, string[] ProductIds)
        {
            try
            {
                _unitOfWork.Begin();
                List<DistributorWiseProductDiscountAndPrices> distributorWiseProductDiscountAndPricesList = new List<DistributorWiseProductDiscountAndPrices>();
                var distributorData = GetAllDistributorWiseProductDiscountAndPrices();
                if (distributorData.Count > 0 && ProductIds != null && ProductIds.Count() > 0)
                {
                    distributorData = distributorData.Where(x => ProductIds.Contains(x.SAPProductCode)).ToList();
                    DeleteDistributorWiseProductDiscountAndPrices(distributorData);
                }
                if (distributorData.Count > 0 && ProductIds == null && DistributorId > 0)
                {
                    distributorData = distributorData.Where(x => x.DistributorId == DistributorId).ToList();
                    DeleteDistributorWiseProductDiscountAndPrices(distributorData);
                }
                if (distributorData.Count > 0 && ProductIds == null && DistributorId == 0)
                {
                    DeleteDistributorWiseProductDiscountAndPrices(distributorData);
                }
                foreach (var item in module)
                {
                    distributorWiseProductDiscountAndPricesList.Add(new DistributorWiseProductDiscountAndPrices()
                    {
                        CartonSize = item.CartonSize,
                        Rate = item.Rate,
                        Discount = item.Discount,
                        PackSize = item.PackSize,
                        ProductDescription = item.ProductDescription,
                        ProductPrice = item.ProductPrice,
                        SAPProductCode = item.SAPProductCode,
                        CreatedBy = SessionHelper.LoginUser.Id,
                        CreatedDate = DateTime.Now,
                        DistributorId = item.DistributorId,
                        ProductDetailId = item.ProductDetailId,
                        ReturnMRPDicount = item.ReturnMRPDicount
                    });
                }
                _repository.AddRange(distributorWiseProductDiscountAndPricesList.Distinct().ToList());
                _unitOfWork.Save();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                new ErrorLogBLL(_unitOfWork).AddExceptionLog(ex);
                return false;
            }

        }
        public List<DistributorWiseProductDiscountAndPrices> GetAllDistributorWiseProductDiscountAndPrices()
        {
            return _repository.GetAllList().ToList();
        }
        public List<DistributorWiseProductDiscountAndPrices> Where(Expression<Func<DistributorWiseProductDiscountAndPrices, bool>> model)
        {
            return _repository.Where(model).ToList();
        }
        public void DeleteDistributorWiseProductDiscountAndPrices(List<DistributorWiseProductDiscountAndPrices> distributorData)
        {
            _repository.DeleteRange(distributorData);
        }
        public List<DistributorWiseProductViewModel> GetProductWiseDiscountAndPrices(string[] ProductId, Configuration configuration)
        {
            List<DistributorWiseProductViewModel> distributorsProduct = new List<DistributorWiseProductViewModel>();
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
            EndpointAddress address = new EndpointAddress(configuration.ProductWisePriceDiscount);
            DIS_REQ_OUTClient client = new DIS_REQ_OUTClient(binding, address);
            client.ClientCredentials.UserName.UserName = configuration.POUserName;
            client.ClientCredentials.UserName.Password = configuration.POPassword;
            if (client.InnerChannel.State == CommunicationState.Faulted)
            {
            }
            else
            {
                client.OpenAsync();
            }
            MAT_DISC_OUT disPortalRequestIn = new MAT_DISC_OUT();
            disPortalRequestIn.MAT_DTL = PlaceProductIdsToSAPPO(ProductId).ToArray();
            ZSS_PRICE_DISCOUNT_DISTWISEResponse ZWAS_IT_DP_PRICE_DISCOUNT = client.DIS_REQ_OUT(disPortalRequestIn);
            client.CloseAsync();
            if (ZWAS_IT_DP_PRICE_DISCOUNT != null)
            {
                for (int i = 0; i < ZWAS_IT_DP_PRICE_DISCOUNT.DIST_WISE.Count(); i++)
                {
                    distributorsProduct.Add(new DistributorWiseProductViewModel()
                    {
                        SAPDistributorCode = ZWAS_IT_DP_PRICE_DISCOUNT.DIST_WISE[i].KUNNR,
                        SAPProductCode = ZWAS_IT_DP_PRICE_DISCOUNT.DIST_WISE[i].MATNR.TrimStart(new char[] { '0' }),
                        PackSize = ZWAS_IT_DP_PRICE_DISCOUNT.DIST_WISE[i].MVGR2T,
                        ProductDescription = ZWAS_IT_DP_PRICE_DISCOUNT.DIST_WISE[i].MAKTX,
                        ProductPrice = Convert.ToDouble(ZWAS_IT_DP_PRICE_DISCOUNT.DIST_WISE[i].KBETR),
                        CartonSize = Convert.ToDouble(ZWAS_IT_DP_PRICE_DISCOUNT.DIST_WISE[i].CARTON),
                        Rate = Convert.ToDouble(ZWAS_IT_DP_PRICE_DISCOUNT.DIST_WISE[i].KBETR),
                        Discount = Convert.ToDouble(ZWAS_IT_DP_PRICE_DISCOUNT.DIST_WISE[i].DISCOUNT),
                        LicenseType = ZWAS_IT_DP_PRICE_DISCOUNT.DIST_WISE[i].MTPOS,
                    });
                }
            }
            return distributorsProduct;
        }
        public List<MAT_DISC_OUTItem> PlaceProductIdsToSAPPO(string[] ProductId)
        {
            List<MAT_DISC_OUTItem> List = new List<MAT_DISC_OUTItem>();
            foreach (string item in ProductId)
            {
                MAT_DISC_OUTItem model = new MAT_DISC_OUTItem
                {
                    MATNR = item.ToString()
                };
                List.Add(model);
            }
            return List;
        }
        public List<DistributorWiseProductViewModel> GetDistributorWiseDiscountAndPrices(string DistributorSAPCode, Configuration _configuration)
        {
            Root root = new Root();
            List<DistributorWiseProductViewModel> distributorsProduct = new List<DistributorWiseProductViewModel>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(_configuration.POUserName + ":" + _configuration.POPassword));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                var result = client.GetAsync(new Uri(_configuration.DistributorWiseProduct + DistributorSAPCode)).Result;
                if (result.IsSuccessStatusCode)
                {
                    var JsonContent = result.Content.ReadAsStringAsync().Result;
                    root = JsonConvert.DeserializeObject<Root>(JsonContent.ToString());
                }
                if (root != null && root.ZWAS_IT_DP_PRICE_DISCOUNT != null && root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES != null && root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES.item != null)
                {
                    for (int i = 0; i < root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES.item.Count(); i++)
                    {
                        distributorsProduct.Add(new DistributorWiseProductViewModel()
                        {
                            SAPProductCode = root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES.item[i].MATNR.TrimStart(new char[] { '0' }),
                            PackSize = root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES.item[i].MVGR2T,
                            ProductDescription = root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES.item[i].MAKTX,
                            ProductPrice = Convert.ToDouble(root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES.item[i].KBETR),
                            CartonSize = Convert.ToDouble(root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES.item[i].CARTON),
                            Rate = Convert.ToDouble(root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES.item[i].KBETR),
                            Discount = Convert.ToDouble(root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES.item[i].DISCOUNT),
                            LicenseType = root.ZWAS_IT_DP_PRICE_DISCOUNT.PRICES.item[i].MTPOS,
                        });
                    }
                }
            }
            return distributorsProduct;
        }
        public DistributorWiseProductDiscountAndPrices FirstOrDefault(Expression<Func<DistributorWiseProductDiscountAndPrices, bool>> predicate)
        {
            return _repository.FirstOrDefault(predicate);
        }
        public List<ProductPending> GetProductPendings(ProductPendingSearch model,IDapper _dapper)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@pCompanyId", model.CompanyId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@pProductId", model.ProductId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@pDistributorId", model.DistributorId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@pStatus", model.Status, DbType.Int32, ParameterDirection.Input);
            List<ProductPending> _ProductPending = _dapper.GetAll<ProductPending>("sp_PendingProduct", parameter, commandType: CommandType.StoredProcedure);
            return _ProductPending;
        }
    }
}
