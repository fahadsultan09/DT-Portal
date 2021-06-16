using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModel;
using System.Collections.Generic;
using System.Linq;

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
        public JsonResult UpdateCount()
        {
            JsonResponse jsonResponse = new JsonResponse();
            jsonResponse.Status = true;
            List<Notification> notifications = _NotificationBLL.Where(x => !x.IsView).ToList();
            notifications.ForEach(x => x.IsView = true);
            _NotificationBLL.UpdateRange(notifications);
            return Json(new { data = jsonResponse });
        }
    }
}
