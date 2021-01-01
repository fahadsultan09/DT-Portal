using SAP.Middleware.Connector;
using SPEED.SAPConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Text;

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
        public object GETBalanceFromSAP(string Function, string TableName)
        {
            SAPSystemConnect sapCfg = new SAPSystemConnect();
            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(sapCfg);
                RfcDestination rfcDest = null;
                rfcDest = RfcDestinationManager.GetDestination(SystemId);
                RfcRepository repo = rfcDest.Repository;
                IRfcFunction companyBapi = repo.CreateFunction(Function);
                //IRfcTable rfcFields = companyBapi.GetTable(TableName);
                //companyBapi.SetValue("DISTRIBUTOR", rfcFields);
                companyBapi.SetValue("DISTRIBUTOR", "0010000004");
                //IRfcStructure param = companyBapi.GetStructure("DISTRIBUTOR");
                //param.SetValue("DISTRIBUTOR", 722);
                companyBapi.Invoke(rfcDest);
                var s = companyBapi.GetObject(0);
                return s;
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