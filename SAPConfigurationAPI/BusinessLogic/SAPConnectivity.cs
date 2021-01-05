using SAP.Middleware.Connector;
using SPEED.SAPConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Text;
using SAPConfigurationAPI.Models;

namespace SAPConfigurationAPI.BusinessLogic
{
    public class SAPConnectivity
    {
        private static readonly string SystemId = ConfigurationManager.AppSettings["SystemId"].ToString();
        public IRfcTable GETTableFromSAP(string Function, string TableName)
        {
            SAPSystemConnect sapCfg = new SAPSystemConnect();
            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(sapCfg);
                RfcDestination rfcDest = null;
                rfcDest = RfcDestinationManager.GetDestination(SystemId);
                RfcRepository repo = rfcDest.Repository;
                IRfcFunction companyBapi = repo.CreateFunction(Function);
                companyBapi.Invoke(rfcDest);
                return companyBapi.GetTable(TableName);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                RfcDestinationManager.UnregisterDestinationConfiguration(sapCfg);
            }


        }
        public DistributorBalance GETBalanceFromSAP(string Function, string TableName, string DistributorId)
        {
            SAPSystemConnect sapCfg = new SAPSystemConnect();
            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(sapCfg);
                RfcDestination rfcDest = null;
                rfcDest = RfcDestinationManager.GetDestination(SystemId);
                RfcRepository repo = rfcDest.Repository;
                IRfcFunction companyBapi = repo.CreateFunction(Function);
                companyBapi.SetValue("DISTRIBUTOR", DistributorId);
                companyBapi.Invoke(rfcDest);
                var sami = companyBapi.GetValue("BAL_SAMI");
                var HTL = companyBapi.GetValue("BAL_HTL");
                return new DistributorBalance
                {
                    SAMI = Convert.ToDouble(sami),
                    HealthTek = Convert.ToDouble(HTL)
                };
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                RfcDestinationManager.UnregisterDestinationConfiguration(sapCfg);
            }
        }
        public DistributorBalance UpdateDistributorBalance(string Function, string DistributorSAPCode, double Amount)
        {
            SAPSystemConnect sapCfg = new SAPSystemConnect();
            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(sapCfg);
                RfcDestination rfcDest = null;
                rfcDest = RfcDestinationManager.GetDestination(SystemId);
                RfcRepository repo = rfcDest.Repository;
                IRfcFunction companyBapi = repo.CreateFunction(Function);
                companyBapi.SetValue("", DistributorSAPCode);
                companyBapi.SetValue("", Amount);
                companyBapi.Invoke(rfcDest);
                var sami = companyBapi.GetValue("");
                var HTL = companyBapi.GetValue("");
                return new DistributorBalance
                {
                    SAMI = Convert.ToDouble(sami),
                    HealthTek = Convert.ToDouble(HTL)
                };
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                RfcDestinationManager.UnregisterDestinationConfiguration(sapCfg);
            }
        }
    }
}