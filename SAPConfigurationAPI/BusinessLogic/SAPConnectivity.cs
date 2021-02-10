using SAP.Middleware.Connector;
using SPEED.SAPConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Text;
using SAPConfigurationAPI.Models;
using SAPConfigurationAPI.ViewModel;

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
        public IRfcTable PostOrdertoSAP(string Function, string TableName, List<OrderStatusViewModel> Table)
        {
            SAPSystemConnect sapCfg = new SAPSystemConnect();
            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(sapCfg);
                RfcDestination rfcDest = null;
                rfcDest = RfcDestinationManager.GetDestination(SystemId);
                RfcRepository repo = rfcDest.Repository;
                IRfcFunction companyBapi = repo.CreateFunction(Function);

                var dt1 = Table.ToDataTable();
                IRfcTable tableimport = companyBapi.GetTable("ORDERS");
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    IRfcStructure structInputs = rfcDest.Repository.GetStructureMetadata(TableName).CreateStructure();

                    structInputs.SetValue("SNO", dt1.Rows[i]["SNO"].ToString());
                    structInputs.SetValue("ITEMNO", dt1.Rows[i]["ITEMNO"].ToString());
                    structInputs.SetValue("PARTN_NUMB", dt1.Rows[i]["PARTN_NUMB"].ToString());
                    structInputs.SetValue("DOC_TYPE", dt1.Rows[i]["DOC_TYPE"].ToString());
                    structInputs.SetValue("DISTR_CHAN", dt1.Rows[i]["DISTR_CHAN"].ToString());
                    structInputs.SetValue("DIVISION", dt1.Rows[i]["DIVISION"].ToString());
                    var PURCH_DATE = Convert.ToDateTime(dt1.Rows[i]["PURCH_DATE"].ToString());
                    var PRICE_DATE = Convert.ToDateTime(dt1.Rows[i]["PRICE_DATE"].ToString());
                    //      structInputs.SetValue("PURCH_DATE", Convert.ToDateTime(dt1.Rows[i]["PURCH_DATE"].ToString()).ToShortDateString());
                    structInputs.SetValue("PURCH_DATE", (PURCH_DATE.Year.ToString() + string.Format("{0:00}", PURCH_DATE.Month) + string.Format("{0:00}", PURCH_DATE.Day)).ToString());
                    structInputs.SetValue("PRICE_DATE", (PRICE_DATE.Year.ToString() + string.Format("{0:00}", PRICE_DATE.Month) + string.Format("{0:00}", PRICE_DATE.Day)).ToString());
                    structInputs.SetValue("ST_PARTN", dt1.Rows[i]["ST_PARTN"].ToString());
                    structInputs.SetValue("MATERIAL", dt1.Rows[i]["MATERIAL"].ToString());
                    structInputs.SetValue("REQ_QTY", dt1.Rows[i]["REQ_QTY"].ToString());
                    structInputs.SetValue("PLANT", dt1.Rows[i]["PLANT"].ToString());
                    structInputs.SetValue("STORE_LOC", dt1.Rows[i]["STORE_LOC"].ToString());
                    structInputs.SetValue("BATCH", dt1.Rows[i]["BATCH"].ToString());
                    structInputs.SetValue("ITEM_CATEG", dt1.Rows[i]["ITEM_CATEG"].ToString());
                    tableimport.Append(structInputs);

                    //IRfcStructure structInputs = destination.Repository.GetStructureMetadata("ZECOM_VA01").CreateStructure();

                    //tableimport.Insert(structInputs);

                }

                companyBapi.SetValue("ORDERS", tableimport);
                companyBapi.Invoke(rfcDest);
                return companyBapi.GetTable("CREATED");
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
        public SAPPaymentStatus AddPaymentToSAP(string Function, SAPPaymentViewModel Table)
        {
            SAPSystemConnect sapCfg = new SAPSystemConnect();
            try
            {
                RfcDestinationManager.RegisterDestinationConfiguration(sapCfg);
                RfcDestination rfcDest = null;
                rfcDest = RfcDestinationManager.GetDestination(SystemId);
                RfcRepository repo = rfcDest.Repository;
                IRfcFunction companyBapi = repo.CreateFunction(Function);
                companyBapi.SetValue("PAY_ID", Table.PAY_ID);
                companyBapi.SetValue("REF", Table.REF);
                companyBapi.SetValue("COMPANY", Table.COMPANY);
                companyBapi.SetValue("DISTRIBUTOR", Table.DISTRIBUTOR);
                companyBapi.SetValue("AMOUNT", Table.AMOUNT);
                companyBapi.SetValue("B_CODE", Table.B_CODE);
                companyBapi.Invoke(rfcDest);
                var DOCUMENT = companyBapi.GetValue("DOCUMENT");
                var COMPANY = companyBapi.GetValue("COMPANY");
                var FISCAL = companyBapi.GetValue("FISCAL");
                return new SAPPaymentStatus
                {
                    SAPDocumentNumber = DOCUMENT.ToString(),
                    SAPCompanyCode = COMPANY.ToString(),
                    SAPFiscalYear = FISCAL.ToString(),
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
        public IRfcTable GetOrderPendingQuantity(string Function, string DistributorId)
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
                return companyBapi.GetTable("PENDING");
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
        public IRfcTable GetPendingOrderStatus(string Function, string TableName, List<SAPOrderStatus> OrderNoList)
        {
            SAPSystemConnect sapCfg = new SAPSystemConnect();
            try
            {
                //Get destination
                RfcDestinationManager.RegisterDestinationConfiguration(sapCfg);
                RfcDestination rfcDest = null;
                rfcDest = RfcDestinationManager.GetDestination(SystemId);
                //Get SAP Repository
                RfcRepository repo = rfcDest.Repository;
                //Set function
                IRfcFunction companyBapi = repo.CreateFunction(Function);
                //Get reference to table object
                IRfcTable tableimport = companyBapi.GetTable("ORDERS");
                //Convert list to datatable
                var dt1 = OrderNoList.ToDataTable();

                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    IRfcStructure structInputs = rfcDest.Repository.GetStructureMetadata(TableName).CreateStructure();
                    ////Populate current MATNRSELECTION row with data from list
                    structInputs.SetValue("VBELN", dt1.Rows[i]["SAPOrderNo"].ToString());
                    ////Create new MATNRSELECTION row
                    tableimport.Append(structInputs);
                }
                companyBapi.SetValue("ORDERS", tableimport);
                companyBapi.Invoke(rfcDest);
                return companyBapi.GetTable("STATUSES");
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
        public IRfcTable GetPendingOrderValue(string Function, string DistributorId)
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
                return companyBapi.GetTable("PENDING");
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