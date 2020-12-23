using SAP.Middleware.Connector;
using SPEED.SAPConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

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
    }
}