using BusinessLogicLayer.HelperClasses;
using CustomerBalance;
using CustomerLedger;
using Dapper;
using DataAccessLayer.Repository;
using Invoice;
using Models.ViewModel;
using SaleReturnCreditNote;
using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.Xml;
using Utility;
using Utility.HelperClasses;

namespace BusinessLogicLayer.Application
{
    public class ReportsBLL
    {
        public string CustomerLedger(CustomerLedgerSeach customerLedgerSeach, Configuration configuration)
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
            EndpointAddress address = new EndpointAddress(configuration.CustomerLedger);
            ServerPORespOutClient client = new ServerPORespOutClient(binding, address);
            client.ClientCredentials.UserName.UserName = configuration.POUserName;
            client.ClientCredentials.UserName.Password = configuration.POPassword;
            if (client.InnerChannel.State == CommunicationState.Faulted)
            {
            }
            else
            {
                client.OpenAsync();
            }
            string base64 = client.ServerPORespOut(Convert.ToDateTime(customerLedgerSeach.ToDate).ToString("yyyyMMdd"), Convert.ToDateTime(customerLedgerSeach.FromDate).ToString("yyyyMMdd"), customerLedgerSeach.SAPCompanyCode, SessionHelper.LoginUser.IsDistributor ? SessionHelper.LoginUser.Distributor.DistributorSAPCode : customerLedgerSeach.DistributorSAPCode);
            client.CloseAsync();
            return base64;
        }
        public List<CustomerBalanceViewModel> CustomerBalance(CustomerBalanceSearch customerBalanceSearch, Configuration configuration)
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
            EndpointAddress address = new EndpointAddress(configuration.CustomerBalance);
            CustPORespOutClient client = new CustPORespOutClient(binding, address);
            client.ClientCredentials.UserName.UserName = configuration.POUserName;
            client.ClientCredentials.UserName.Password = configuration.POPassword;
            if (client.InnerChannel.State == CommunicationState.Faulted)
            {
            }
            else
            {
                client.OpenAsync();
            }
            Cust_resp_out cust_Resp_Out = new Cust_resp_out();
            cust_Resp_Out.S_KUNNR_LOW = customerBalanceSearch.DistributorSAPCode ?? string.Empty;
            cust_Resp_Out.S_KUNNR_HIGH = string.Empty;
            cust_Resp_Out.P_BUKRS = customerBalanceSearch.SAPCompanyCode;
            ZSS_CUST_BALResponse zSS_CUST_BALResponse = client.CustPORespOut(cust_Resp_Out);
            client.CloseAsync();
            List<CustomerBalanceViewModel> customerBalanceViewModels = new List<CustomerBalanceViewModel>();
            if (zSS_CUST_BALResponse != null)
            {
                for (int i = 0; i < zSS_CUST_BALResponse.FINAL_TAB.Length; i++)
                {
                    customerBalanceViewModels.Add(new CustomerBalanceViewModel()
                    {
                        DistributorName = zSS_CUST_BALResponse.FINAL_TAB[i].NAME1,
                        DistributorCode = zSS_CUST_BALResponse.FINAL_TAB[i].KUNNR,
                        City = zSS_CUST_BALResponse.FINAL_TAB[i].ORT01,
                        Balance = zSS_CUST_BALResponse.FINAL_TAB[i].BALANCE,
                        DebitCredit = zSS_CUST_BALResponse.FINAL_TAB[i].INDI == "C" ? "Credit" : "Debit",
                    });
                }
            }
            return customerBalanceViewModels;
        }
        public string Invoice(InvoiceSearch invoiceSearch, Configuration configuration)
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
            EndpointAddress address = new EndpointAddress(configuration.Invoice);
            POservRespOutClient client = new POservRespOutClient(binding, address);
            client.ClientCredentials.UserName.UserName = configuration.POUserName;
            client.ClientCredentials.UserName.Password = configuration.POPassword;
            if (client.InnerChannel.State == CommunicationState.Faulted)
            {
            }
            else
            {
                client.OpenAsync();
            }
            string base64 = client.POservRespOut(SessionHelper.LoginUser.IsDistributor ? SessionHelper.LoginUser.Distributor.DistributorSAPCode : string.Empty, invoiceSearch.InvoiceNo.ToString());
            client.CloseAsync();
            return base64;
        }
        public string SaleReturnCreditNote(SaleReturnCreditNoteSearch saleReturnCreditNoteSearch, Configuration configuration)
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
            EndpointAddress address = new EndpointAddress(configuration.SaleReturnCreditNote);
            CN_SerRespOutClient client = new CN_SerRespOutClient(binding, address);
            client.ClientCredentials.UserName.UserName = configuration.POUserName;
            client.ClientCredentials.UserName.Password = configuration.POPassword;
            if (client.InnerChannel.State == CommunicationState.Faulted)
            {
            }
            else
            {
                client.OpenAsync();
            }
            string base64 = client.CN_SerRespOut(SessionHelper.LoginUser.IsDistributor ? SessionHelper.LoginUser.Distributor.DistributorSAPCode : string.Empty, saleReturnCreditNoteSearch.SaleReturnNo.ToString());
            client.CloseAsync();
            return base64;
        }
        public List<CustomerReceivable> CustomerReceivable(CustomerReceivableSearch model, IDapper _dapper, Configuration configuration)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@pCompanyId", model.CompanyId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@pDistributorId", SessionHelper.LoginUser.IsDistributor ? SessionHelper.LoginUser.DistributorId : model.DistributorId, DbType.Int32, ParameterDirection.Input);
            List<CustomerReceivable> _CustomerReceivable = _dapper.GetAll<CustomerReceivable>("sp_CustomerReceivable", parameter, commandType: CommandType.StoredProcedure);
            CustomerBalanceSearch customerBalanceSearch;
            foreach(var i in _CustomerReceivable)
            {
                customerBalanceSearch = new CustomerBalanceSearch();
                customerBalanceSearch.SAPCompanyCode = i.SAPCompanyCode;
                customerBalanceSearch.DistributorSAPCode = i.DistributorSAPCode;
                i.CustomerBalance = CustomerBalance(customerBalanceSearch, configuration);
                if(i.CustomerBalance.Count>0)
                {
                    i.CurrentBalance = i.CustomerBalance[0].Balance;
                    i.DebitCreditIndicator = i.CustomerBalance[0].DebitCredit == "Credit" ? DebitCreditIndicator.Credit : DebitCreditIndicator.Debit;
                }
                else
                {
                    i.CurrentBalance = 0;
                    i.DebitCreditIndicator =  DebitCreditIndicator.Debit;
                }

                i.NetValue = i.ApprovedOrders + i.UnapprovedOrders + i.CurrentBalance - i.UnapprovedPayments;
                i.ReceivableAdvanceIndicator = i.NetValue >= 0 ? ReceivableAdvanceIndicator.Receivable : ReceivableAdvanceIndicator.Advance;
            }
            return _CustomerReceivable;
        }
    }
}
