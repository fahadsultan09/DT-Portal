using Microsoft.Extensions.Configuration;

namespace Utility.HelperClasses
{
    public class Configuration
    {
        public string ResetPassword { get; set; }

        public Configuration(IConfiguration configuration)
        {
            ResetPassword = configuration["AppSettings:ResetPassword"];
        }
    }
}
