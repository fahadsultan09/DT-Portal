using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Hosting;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility.HelperClasses;

namespace BusinessLogicLayer.Application
{
    public class EmailLogBLL
    {
        private IUnitOfWork _unitOfWork;
        private IGenericRepository<EmailLog> repository;
        private Configuration _Configuration;

        public EmailLogBLL(IUnitOfWork unitOfWork, Configuration _configuration)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<EmailLog>();
            _Configuration = _configuration;
        }
        public async Task<int> AddEmailLog(EmailLog module)
        {
            module.CreatedBy = module.CreatedBy;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return await _unitOfWork.SaveAsync();
        }
        public async Task UpdateEmailLog(int id)
        {
            EmailLog module = GetEmailLogById(id);
            module.CreatedBy = module.CreatedBy;
            module.CreatedDate = DateTime.Now;
            module.IsSend = false;
            repository.Update(module);
            await _unitOfWork.SaveAsync();
        }
        public EmailLog GetEmailLogById(int id)
        {
            return _unitOfWork.GenericRepository<EmailLog>().GetById(id);
        }
        public void EmailSend(List<User> UserList, EmailUserModel EmailUserModel)
        {

            foreach (var user in UserList)
            {
                SendEmail(user, EmailUserModel);
            }
        }
        public async Task SendEmail(User User, EmailUserModel EmailUserModel)
        {
            string ToAcceptTemplate = EmailUserModel.ToAcceptTemplate;
            ToAcceptTemplate = ToAcceptTemplate.Replace("{{Name}}", User.FirstName + " " + User.LastName);
            ToAcceptTemplate = ToAcceptTemplate.Replace("{{Day}}", EmailUserModel.Day.ToString());
            ToAcceptTemplate = ToAcceptTemplate.Replace("{{ComplaintNo}}", EmailUserModel.ComplaintNo.ToString());
            ToAcceptTemplate = ToAcceptTemplate.Replace("{{ComplaintDate}}", EmailUserModel.ComplaintDate.ToString());
            ToAcceptTemplate = ToAcceptTemplate.Replace("{{DistributorName}}", EmailUserModel.DistributorName);
            ToAcceptTemplate = ToAcceptTemplate.Replace("{{ComplaintCategory}}", EmailUserModel.ComplaintCategory);
            ToAcceptTemplate = ToAcceptTemplate.Replace("{{Attachment}}", EmailUserModel.Attachment);
            ToAcceptTemplate = ToAcceptTemplate.Replace("{{ComplaintDetail}}", EmailUserModel.ComplaintDetail);
            ToAcceptTemplate = ToAcceptTemplate.Replace("{{URL}}", EmailUserModel.URL);
            await EmailHelper.SendMail(_unitOfWork, User.Email, EmailUserModel.CCEmail, "New Customer Complaint (No. " + EmailUserModel.ComplaintNo.ToString() + ")", ToAcceptTemplate, _Configuration, EmailUserModel.CreatedBy);

        }
    }
}
