using BusinessLogicLayer.Application;
using BusinessLogicLayer.ApplicationSetup;
using BusinessLogicLayer.ErrorLog;
using BusinessLogicLayer.GeneralSetup;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Utility.HelperClasses;
using Microsoft.AspNetCore.Hosting;

namespace Scheduler
{
    public class KPIEmailScheduler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ComplaintBLL _ComplaintBLL;
        private readonly ComplaintUserEmailBLL _ComplaintUserEmailBLL;
        private readonly EmailLogBLL _EmailLogBLL;
        private readonly Configuration _Configuration;
        private readonly IWebHostEnvironment _env;
        public KPIEmailScheduler(IUnitOfWork unitOfWork, Configuration configuration, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _ComplaintBLL = new ComplaintBLL(_unitOfWork);
            _ComplaintUserEmailBLL = new ComplaintUserEmailBLL(_unitOfWork);
            _Configuration = configuration;
            _EmailLogBLL = new EmailLogBLL(_unitOfWork, _Configuration);
            _env = env;
        }

        public void GetPendingComplaints()
        {
            try
            {
                List<Complaint> ComplaintList = _ComplaintBLL.GetPendingComplaint();
                List<ComplaintUserEmail> ComplaintUserEmailList = _ComplaintUserEmailBLL.GetAllComplaintUserEmailByComplaintSubCategoryId(ComplaintList.Select(x => x.ComplaintSubCategoryId).ToArray());

                foreach (var item in ComplaintUserEmailList)
                {
                    item.ComplaintSubCategory = new ComplaintSubCategoryBLL(_unitOfWork).FirstOrDefault(x => x.Id == item.ComplaintSubCategoryId);
                    if (item.CreatedDate.AddDays((int)item.ComplaintSubCategory.KPIDay) <= DateTime.Now)
                    {
                        ////Server
                        var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                        ////Local
                        //var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\")));
                        string EmailTemplate = path + "\\Attachments\\EmailTemplates\\KPIComplaint.html";
                        //EmailTemplate = _env.WebRootPath + "\\Attachments\\EmailTemplates\\KPIComplaint.html";
                        ComplaintEmailUserModel EmailUserModel = new ComplaintEmailUserModel();
                        User user = new User();
                        Complaint complaint = ComplaintList.First(x => x.ComplaintSubCategoryId == item.ComplaintSubCategoryId);
                        item.ComplaintSubCategory.ComplaintCategory = new ComplaintCategoryBLL(_unitOfWork).FirstOrDefault(x => x.Id == item.ComplaintSubCategory.ComplaintCategoryId);
                        complaint.Distributor = new DistributorBLL(_unitOfWork).FirstOrDefault(x => x.Id == complaint.DistributorId);
                        complaint.ComplaintSubCategory.User = new UserBLL(_unitOfWork).FirstOrDefault(x => x.Id == item.ComplaintSubCategory.UserEmailTo);

                        EmailUserModel.Day = item.ComplaintSubCategory.KPIDay.ToString();
                        EmailUserModel.ComplaintCategory = item.ComplaintSubCategory.ComplaintCategory.ComplaintCategoryName + " - " + item.ComplaintSubCategory.ComplaintSubCategoryName;
                        EmailUserModel.ToAcceptTemplate = File.ReadAllText(EmailTemplate);
                        EmailUserModel.ComplaintNo = complaint.SNo.ToString();
                        EmailUserModel.DistributorName = complaint.Distributor.DistributorName;
                        EmailUserModel.ComplaintDetail = complaint.Description;
                        EmailUserModel.ComplaintDate = DateTime.Now.ToString("dd/MMM/yyyy");
                        EmailUserModel.CreatedBy = complaint.ComplaintSubCategory.User.Id;
                        EmailUserModel.CCEmail = string.Join(',', item.UserEmailId);
                        EmailUserModel.Subject = "REMINDER: Customer Complaint (No. " + EmailUserModel.ComplaintNo.ToString() + ")";
                        EmailUserModel.URL = _Configuration.URL;

                        //Sending Email
                        _EmailLogBLL.ComplaintKPISendEmail(item.ComplaintSubCategory.User, EmailUserModel);
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogBLL(_unitOfWork).AddSchedulerExceptionLog(ex);
            }
        }
    }
}