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
        public async static Task SendMail(IUnitOfWork unitOfWork, string To, string CC, string Subject, string Body, Configuration configuration, int CreatedBy)
        {
            EmailLog emailLog = new EmailLog();
            _unitOfWork = unitOfWork;
            try
            {
                string From = configuration.FromEmail;
                string Password = configuration.Password;
                int Port = configuration.Port;
                string ServerAddress = configuration.ServerAddress;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(From);
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
                SmtpClient smtp = new SmtpClient(ServerAddress);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                NetworkCredential NetworkCred = new NetworkCredential(From, Password);
                smtp.Credentials = NetworkCred;
                smtp.Port = Port;

                emailLog.ToEmail = To;
                emailLog.Subject = Subject;
                emailLog.Message = Body;
                emailLog.IsSend = true;
                emailLog.CreatedBy = CreatedBy;
                await new EmailLogBLL(_unitOfWork, null).AddEmailLog(emailLog);

                await Task.Delay(10000).ContinueWith(t => smtp.SendMailAsync(mail));
            }
            catch (Exception ex)
            {
                emailLog.ToEmail = To;
                emailLog.Subject = Subject;
                emailLog.Message = Body;
                emailLog.IsSend = false;
                emailLog.CreatedBy = CreatedBy;
                await new EmailLogBLL(_unitOfWork, null).UpdateEmailLog(emailLog.Id);
            }
        }
    }
}
