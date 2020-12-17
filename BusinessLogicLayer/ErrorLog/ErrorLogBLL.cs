using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Models.ErrorLog;
using System;
using System.Diagnostics;

namespace BusinessLogicLayer.ErrorLog
{
    public class ErrorLogBLL
    {
        private IUnitOfWork _repository;
        public ErrorLogBLL(IUnitOfWork repository)
        {
            _repository = repository;
        }

        public bool AddExceptionLog(Exception ex)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            ExceptionLog module = new ExceptionLog()
            {
                ClassName = trace.GetFrame(0).GetMethod().ReflectedType.FullName,
                ErrorLineNumber = "Line : " + trace.GetFrame(0).GetFileLineNumber(),
                ExceptionMessage = ex.Message,
                MethodBase = new StackTrace(ex).GetFrame(0).GetMethod().Name,
                CreatedBy = SessionHelper.LoginUser == null ? 0 : SessionHelper.LoginUser.Id,
                CreatedDate = DateTime.Now,
                Trace = ex.StackTrace,
                Source = ex.Source,
                HelpLink = ex.HelpLink,
                MemberType = ex.GetType().Name
            };
            _repository.GenericRepository<ExceptionLog>().Insert(module);
            return _repository.Save() > 0;
        }
    }
}
