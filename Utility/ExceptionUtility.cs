using Microsoft.AspNetCore.Hosting;
using System;

namespace Utility
{
    public sealed class ExceptionUtility
    {
        private Exception m_Exception;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ExceptionUtility(Exception exc, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            m_Exception = exc;
        }

      
        private string m_ErrorMessage;
        public String ErrorMessage
        {
            get { return m_ErrorMessage; }
        }
    }
}
