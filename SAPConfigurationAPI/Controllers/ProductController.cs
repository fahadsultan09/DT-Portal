using SAPConfigurationAPI.BusinessLogic;
using SAPConfigurationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAPConfigurationAPI.Controllers
{
    public class ProductController : ApiController
    {
        private SAPConnectivity connectivity;
        public ProductController()
        {
            connectivity = new SAPConnectivity();
        }
        public List<Product> Get()
        {
            var table = connectivity.GETTableFromSAP("ZWAS_IT_HRMS_BAPI", "PRODUCT");
            List<Product> products = new List<Product>();
            for (int i = 0; i < table.RowCount; i++)
            {
                products.Add(new Product()
                {
                    SAPProductCode = table[i].GetString("MATNR"),
                    PackSize = table[i].GetString("MVGR2T"),
                    ProductName = table[i].GetString("MVGR4T"),
                    ProductDesc = table[i].GetString("MAKTX"),
                    ProductPrice = table[i].GetDouble("KBETR"),
                    CartonSize = table[i].GetString("Carton"),
                    Rate = table[i].GetDouble("KBETR"),
                    Discount = table[i].GetDouble("DISCOUNT")
                });
            }
            return products;
        }
    }
}
