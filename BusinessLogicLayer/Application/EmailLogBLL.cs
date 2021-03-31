﻿using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Models.Application;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility.HelperClasses;

namespace BusinessLogicLayer.Application
{
    public class EmailLogBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<EmailLog> repository;
        private readonly Configuration _Configuration;

        public EmailLogBLL(IUnitOfWork unitOfWork, Configuration _configuration)
        {
            _unitOfWork = unitOfWork;
            repository = _unitOfWork.GenericRepository<EmailLog>();
            _Configuration = _configuration;
        }
        public int AddEmailLog(EmailLog module)
        {
            module.CreatedBy = module.CreatedBy;
            module.CreatedDate = DateTime.Now;
            repository.Insert(module);
            return _unitOfWork.Save();
        }
        public int UpdateEmailLog(int id)
        {
            EmailLog module = GetEmailLogById(id);
            module.CreatedBy = module.CreatedBy;
            module.CreatedDate = DateTime.Now;
            module.IsSend = false;
            repository.Update(module);
            return _unitOfWork.Save();
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
        public void SendEmail(User User, EmailUserModel EmailUserModel)
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
            EmailHelper.SendMail(_unitOfWork, User.Email, EmailUserModel.CCEmail, EmailUserModel.Subject, ToAcceptTemplate, _Configuration, EmailUserModel.CreatedBy);
        }
    }
}
