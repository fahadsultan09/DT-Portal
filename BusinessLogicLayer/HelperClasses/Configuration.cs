using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDU.BusinessLogicLayer.HelperClasses
{
    public class Configuration
    {
        public string BaseFilePath { get; set; }

        public Configuration(IConfiguration configuration)
        {
            BaseFilePath = configuration["Settings:FolderPath"];
        }
    }
}
