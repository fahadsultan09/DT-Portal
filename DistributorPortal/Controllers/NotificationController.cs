using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult UpdateCount()
        {
            List<Notification> notifications = _NotificationBLL.Where(x => !x.IsView).ToList();
            notifications.ForEach(x => x.IsView = true);
            _NotificationBLL.UpdateRange(notifications);
            return View();
        }
    }
}
