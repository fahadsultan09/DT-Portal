using System.Linq;
using DataAccessLayer.WorkProcess;
using Utility;
using Utility.HelperClasses;
using Models.Application;
using BusinessLogicLayer.HelperClasses;
using BusinessLogicLayer.GeneralSetup;
using System.Management;
using BusinessLogicLayer.Application;

namespace BusinessLogicLayer.Login
{
    public class LoginBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserBLL _UserBLL;
        public LoginBLL(IUnitOfWork Repository)
        {
            _unitOfWork = Repository;
            _UserBLL = new UserBLL(_unitOfWork);
        }
        
        public LoginStatus CheckLogin(User user)
        {
            string CPUID = string.Empty;
            var Password = EncryptDecrypt.Encrypt(user.Password);
            User LoginUser = _unitOfWork.GenericRepository<User>().Where(e => e.IsActive == true && e.IsDeleted == false && e.UserName == user.UserName && e.Password == Password).FirstOrDefault();
            if (LoginUser.IsDistributor)
            {
                ManagementClass mc = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    CPUID = user.CPUID = mo.Properties["processorID"].Value.ToString();
                    break;
                }
                if (string.IsNullOrEmpty(LoginUser.CPUID))
                {
                    _UserBLL.UpdateCPUID(user);
                }
                else
                {
                    if (LoginUser.CPUID != CPUID)
                    {
                        LoginUser = new User();
                    }
                }
            }
            if (LoginUser != null)
            {
                SessionHelper.LoginUser = LoginUser;
                SessionHelper.NavigationMenu = new RolePermissionBLL(_unitOfWork).Where(e => e.RoleId == SessionHelper.LoginUser.RoleId && e.ApplicationPage.IsDeleted == false).ToList();
                return LoginStatus.Success;
            }
            else
            {
                return LoginStatus.Failed;
            }
        }
        public bool CheckUserPassword(User user,string password)
        {
            var Password = EncryptDecrypt.Encrypt(user.Password);
            if (_unitOfWork.GenericRepository<User>().Any(e => e.UserName == user.UserName && e.Password == Password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
