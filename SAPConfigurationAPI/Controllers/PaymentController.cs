using SAPConfigurationAPI.BusinessLogic;
using SAPConfigurationAPI.Models;
using SAPConfigurationAPI.ViewModel;
using System.Web.Http;

namespace SAPConfigurationAPI.Controllers
{
    public class PaymentController : ApiController
    {
        SAPConnectivity _SAPConnectivity;
        public PaymentController()
        {
            _SAPConnectivity = new SAPConnectivity();
        }
        [HttpPost]
        public SAPPaymentStatus Get(SAPPaymentViewModel Table) 
        {
            var data = _SAPConnectivity.AddPaymentToSAP("ZWAS_PAYMENT_BAPI_DP", Table);
            return data;
        }
    }
}
