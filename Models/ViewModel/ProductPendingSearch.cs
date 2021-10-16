using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Utility;

namespace Models.ViewModel
{
    public class ProductPendingSearch : Search
    {
        public OrderStatus? Status { get; set; }
        public int? CompanyId { get; set; }
        public SelectList CompanyList { get; set; }
        public int ProductId { get; set; }
        public SelectList ProductList { get; set; }

        public List<ProductPending> ProductPending { get; set; }
        


    }
}
