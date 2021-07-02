using BusinessLogicLayer.Application;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Utility.HelperClasses;

namespace BusinessLogicLayer.HelperClasses
{
    public class EmailHelper
    {
        private static IUnitOfWork _unitOfWork { get; set; }
        public static void SendMail(IUnitOfWork unitOfWork, string To, string CC, string Subject, string Body, Configuration configuration, int CreatedBy)
        {
            EmailLog emailLog = new EmailLog();
            _unitOfWork = unitOfWork;
            try
            {
                string From = configuration.FromEmail;
                string Password = configuration.Password;
                int Port = configuration.Port;
                string ServerAddress = configuration.ServerAddress;

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(From)
                };
                mail.To.Add(To);
                if (!string.IsNullOrEmpty(CC))
                {
                    string[] email = CC.Split(',');
                    for (int i = 0; i < email.Length; i++)
                    {
                        mail.CC.Add(email[i]);
                    }
                }
                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ServerAddress)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false
                };
                NetworkCredential NetworkCred = new NetworkCredential(From, Password);
                smtp.Credentials = NetworkCred;
                smtp.Port = Port;

                emailLog.ToEmail = To;
                emailLog.CCEmail = CC;
                emailLog.Subject = Subject;
                emailLog.Message = Body;
                emailLog.IsSend = true;
                emailLog.CreatedBy = CreatedBy;
                new EmailLogBLL(_unitOfWork, null).AddEmailLog(emailLog);
                smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                emailLog.IsSend = false;
                new EmailLogBLL(_unitOfWork, null).UpdateEmailLog(emailLog.Id);
                throw ex;
            }
        }
        public static void SendMail(IUnitOfWork unitOfWork, string To, string CC, string Subject, string Body, Configuration configuration)
        {
            EmailLog emailLog = new EmailLog();
            _unitOfWork = unitOfWork;
            try
            {
                string From = configuration.FromEmail;
                string Password = configuration.Password;
                int Port = configuration.Port;
                string ServerAddress = configuration.ServerAddress;

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(From)
                };
                mail.To.Add(To);
                if (!string.IsNullOrEmpty(CC))
                {
                    string[] email = CC.Split(',');
                    for (int i = 0; i < email.Length; i++)
                    {
                        mail.CC.Add(email[i]);
                    }
                }
                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ServerAddress)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false
                };
                NetworkCredential NetworkCred = new NetworkCredential(From, Password);
                smtp.Credentials = NetworkCred;
                smtp.Port = Port;

                emailLog.ToEmail = To;
                emailLog.CCEmail = CC;
                emailLog.Subject = Subject;
                emailLog.Message = Body;
                emailLog.IsSend = true;
                emailLog.CreatedBy = 0;
                new EmailLogBLL(_unitOfWork, null).AddEmailLog(emailLog);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                emailLog.IsSend = false;
                new EmailLogBLL(_unitOfWork, null).UpdateEmailLog(emailLog.Id);
                throw ex;
            }
        }
    }
}
