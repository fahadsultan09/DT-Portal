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
            var table = connectivity.GETTableFromSAP("ZWAS_IT_HRMS_BAPI", "PRODUCTS");
            List<Product> products = new List<Product>();
            for (int i = 0; i < table.RowCount; i++)
            {
                products.Add(new Product()
                {
                    SAPProductCode = table[i].GetString("MATNR").TrimStart(new char[] { '0' }),
                    PackSize = table[i].GetString("MVGR2T"),
                    ProductName = table[i].GetString("MVGR4T"),
                    ProductDescription = table[i].GetString("MAKTX"),
                    ProductPrice = table[i].GetDouble("KBETR"),
                    CartonSize = table[i].GetString("Carton"),
                    Rate = table[i].GetDouble("KBETR"),
                    Discount = table[i].GetDouble("DISCOUNT"),
                    LicenseType = table[i].GetString("MTPOS"),
                    SFSize = table[i].GetString("SF")
                });
            }
            return products;
        }

        [HttpGet]
        [Route("api/Product/GetProductWiseDiscount")]
        public List<Product> GetProductWiseDiscount(string DistributorId)
        {
            var table = connectivity.GetDistributorWiseProductDiscount("ZWAS_IT_DP_PRICE_DISCOUNT", DistributorId);
            List<Product> products = new List<Product>();
            for (int i = 0; i < table.RowCount; i++)
            {
                products.Add(new Product()
                {
                    SAPProductCode = table[i].GetString("MATNR").TrimStart(new char[] { '0' }),
                    PackSize = table[i].GetString("MVGR2T"),
                    ProductName = table[i].GetString("MVGR4T"),
                    ProductDescription = table[i].GetString("MAKTX"),
                    ProductPrice = table[i].GetDouble("KBETR"),
                    CartonSize = table[i].GetString("Carton"),
                    Rate = table[i].GetDouble("KBETR"),
                    Discount = table[i].GetDouble("DISCOUNT"),
                    LicenseType = table[i].GetString("MTPOS")
                });
            }
            return products;
        }
    }
}
