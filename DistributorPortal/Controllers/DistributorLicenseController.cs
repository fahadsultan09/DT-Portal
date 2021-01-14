using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using DistributorPortal.BusinessLogicLayer.ApplicationSetup;
using DistributorPortal.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility.Constant;
using Utility.HelperClasses;
using static Utility.Constant.Common;

namespace DistributorPortal.Controllers
{
    public class DistributorLicenseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DistributorLicenseBLL _DistributorLicenseBLL;
        private readonly DistributorBLL _distributorBll;
        private readonly LicenseControlBLL _licenseControlBLL;
        private readonly IConfiguration _IConfiguration;
        public DistributorLicenseController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _DistributorLicenseBLL = new DistributorLicenseBLL(_unitOfWork);
            _distributorBll = new DistributorBLL(_unitOfWork);
            _licenseControlBLL = new LicenseControlBLL(_unitOfWork);
            _IConfiguration = configuration;
        }
        // GET: DistributorLicense
        public ActionResult Index()
        {
            ViewBag.Distributor = _distributorBll.DropDownDistributorList(null);
            ViewBag.License = _licenseControlBLL.DropDownLicenseControlList(0);
            return View();
        }
    }
}
