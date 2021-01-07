using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models.ViewModel
{
    public class OrderStatusViewModel
    {
        public string SNO { get; set; } // Order Number from Distributor Portal
        public string ITEMNO { get; set; }
        public string PARTN_NUMB { get; set; } // Distributor Code
        public string DOC_TYPE { get; set; }  // Sale Order Type (mapping table)
        public string SALES_ORG { get; set; } //(mapping table)
        public string DISTR_CHAN { get; set; } // (mapping table)
        public string DIVISION { get; set; } // (mapping table)
        public string PURCH_NO { get; set; } // Reference No in Order Master
        public DateTime PURCH_DATE { get; set; } // Current Date
        public DateTime PRICE_DATE { get; set; } // Current Date
        public string ST_PARTN { get; set; } // Distributor Code
        public string MATERIAL { get; set; } // Product Code
        public string REQ_QTY { get; set; }  
        public string PLANT { get; set; }  // Dispatch Plant in (mapping table)
        public string STORE_LOC { get; set; } // S. Storage Location (mapping table)
        public string BATCH { get; set; } // Empty
        public string ITEM_CATEG { get; set; } // Sale Item Category (mapping table)
    }
}