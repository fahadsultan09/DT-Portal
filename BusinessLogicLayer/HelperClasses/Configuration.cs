using Microsoft.Extensions.Configuration;

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
