using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BusinessLogicLayer.SAPBLL
{

    public class DATA2
    {
        public string AUFNR { get; set; }
        public string DWERK { get; set; }
        public string MTBEZ { get; set; }
        public string MTART { get; set; }
        public string MATKL { get; set; }
        public string PLNBEZ { get; set; }
        public string MAKTX { get; set; }
        public string GSTRP { get; set; }
        public string CHARG { get; set; }
        public string GAMNG { get; set; }
        public string LABST { get; set; }
        public string UOM { get; set; }
        public string UMREN { get; set; }
        public string VORNR { get; set; }
        public string LTXA1 { get; set; }
        public string ARBPL { get; set; }
        public string KTEXT { get; set; }
        public string NO_DAYS { get; set; }
        public string HSDAT { get; set; }
        public string VFDAT { get; set; }
        public string LMNGA { get; set; }
        public string YAUOM { get; set; }
        public string VAR_QTY { get; set; }
        public string PERCENT { get; set; }
        public string STATUS { get; set; }


    }
    public class SAP
    {
        //public DataTable GetSAPData()
        //{
        //    try
        //    {
        //        SapNwRfc.Pooling.SapConnectionPool sapConnectionPool = new SapNwRfc.Pooling.SapConnectionPool()
        //        //SAPSystemConnect sapCfg = new SAPSystemConnect();
        //        //RfcDestinationManager.RegisterDestinationConfiguration(sapCfg);
        //        //RfcDestination rfcDest = null;
        //        //rfcDest = RfcDestinationManager.GetDestination("DEV");
        //        //RfcRepository repo = rfcDest.Repository;
        //        //IRfcFunction companyBapi = repo.CreateFunction("ZWAS_WIP_DASHBOARD_API");
        //        //companyBapi.Invoke(rfcDest);
        //        //IRfcTable tab = companyBapi.GetTable(0);
        //        List<DATA2> data = new List<DATA2>();
        //        //for (int i = 0; i < tab.RowCount; i++)
        //        //{
        //        //    DATA2 d = new DATA2();
        //        //    d.AUFNR = tab[i].GetString("AUFNR");
        //        //    d.DWERK = tab[i].GetString("DWERK");
        //        //    d.MTBEZ = tab[i].GetString("MTBEZ");
        //        //    d.MTART = tab[i].GetString("MTART");
        //        //    d.MATKL = tab[i].GetString("MATKL");
        //        //    d.PLNBEZ = tab[i].GetString("PLNBEZ");
        //        //    d.MAKTX = tab[i].GetString("MAKTX");
        //        //    d.GSTRP = tab[i].GetString("GSTRP");
        //        //    d.CHARG = tab[i].GetString("CHARG");
        //        //    d.GAMNG = tab[i].GetString("GAMNG");
        //        //    d.LABST = tab[i].GetString("LABST");
        //        //    d.UOM = tab[i].GetString("UOM");
        //        //    d.UMREN = tab[i].GetString("UMREN");
        //        //    d.VORNR = tab[i].GetString("VORNR");
        //        //    d.LTXA1 = tab[i].GetString("LTXA1");
        //        //    d.ARBPL = tab[i].GetString("ARBPL");
        //        //    d.KTEXT = tab[i].GetString("KTEXT");
        //        //    d.NO_DAYS = tab[i].GetString("NO_DAYS");
        //        //    d.HSDAT = tab[i].GetString("HSDAT");
        //        //    d.VFDAT = tab[i].GetString("VFDAT");
        //        //    d.LMNGA = tab[i].GetString("LMNGA");
        //        //    d.YAUOM = tab[i].GetString("YAUOM");
        //        //    d.VAR_QTY = tab[i].GetString("VAR_QTY");
        //        //    d.PERCENT = tab[i].GetString("PERCENT");
        //        //    d.STATUS = tab[i].GetString("STATUS");
        //        //    data.Add(d);
        //        //}
        //        //data.Where(e => e.GSTRP == "0000-00-00").ToList().ForEach(e => e.GSTRP = null);
        //        //data.Where(e => e.HSDAT == "0000-00-00").ToList().ForEach(e => e.HSDAT = null);
        //        //data.Where(e => e.VFDAT == "0000-00-00").ToList().ForEach(e => e.VFDAT = null);

        //        //RfcDestinationManager.UnregisterDestinationConfiguration(sapCfg);
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

    }
}
