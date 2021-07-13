using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace DistributorPortal.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationBLL _NotificationBLL;
        public NotificationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _NotificationBLL = new NotificationBLL(_unitOfWork);
        }
        public JsonResult UpdateCount(ApplicationPages ApplicationPageId)
        {
            JsonResponse jsonResponse = new JsonResponse();
            List<Notification> notifications = new List<Notification>();
            jsonResponse.Status = true;
            if (SessionHelper.LoginUser.IsDistributor)
            {
                notifications = _NotificationBLL.Where(x => !x.IsView).ToList();
                notifications.ForEach(x => x.IsView = true);
                _NotificationBLL.UpdateRange(notifications);
            }
            else
            {
                switch (ApplicationPageId)
                {
                    case ApplicationPages.Order:
                        notifications = SessionHelper.Notification;
                        notifications.ForEach(x => x.IsOrderView = true);
                        _NotificationBLL.UpdateRange(notifications);
                        break;
                    case ApplicationPages.OrderReturn:
                        notifications = SessionHelper.Notification;
                        notifications.ForEach(x => x.IsOrderReturnView = true);
                        _NotificationBLL.UpdateRange(notifications);
                        break;
                    case ApplicationPages.Payment:
                        notifications = SessionHelper.Notification;
                        notifications.ForEach(x => x.IsPaymentView = true);
                        _NotificationBLL.UpdateRange(notifications);
                        break;
                }
            }
            return Json(new { data = jsonResponse });
        }
    }
}
