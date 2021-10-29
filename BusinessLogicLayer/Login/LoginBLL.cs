using BusinessLogicLayer.Application;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System.Collections.Generic;
using System.Linq;
using Utility;
using Utility.HelperClasses;

namespace BusinessLogicLayer.Login
{
    public class LoginBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserBLL _UserBLL;
        private readonly UserSystemInfoBLL _UserSystemInfoBLL;
        private readonly SubDistributorBLL _SubDistributorBLL;
        public LoginBLL(IUnitOfWork Repository)
        {
            _unitOfWork = Repository;
            _UserBLL = new UserBLL(_unitOfWork);
            _UserSystemInfoBLL = new UserSystemInfoBLL(_unitOfWork);
            _SubDistributorBLL = new SubDistributorBLL(_unitOfWork);
        }
        public LoginStatus CheckLogin(User user)
        {
            List<string> MACAddresses = new List<string>();
            var Password = EncryptDecrypt.Encrypt(user.Password);
            User LoginUser = _UserBLL.Where(e => e.IsActive == true && e.IsDeleted == false && e.UserName == user.UserName && e.Password == Password).FirstOrDefault();
            if (LoginUser != null && !LoginUser.IsDistributor && !string.IsNullOrEmpty(user.MacAddresses))
            {
                LoginUser = null;
                return LoginStatus.NotRegistered;
            }
            if (LoginUser != null && LoginUser.IsDistributor)
            {
                if (string.IsNullOrEmpty(LoginUser.AccessToken) && !string.IsNullOrEmpty(user.AccessToken))
                {
                    LoginUser.AccessToken = user.AccessToken;
                    _UserBLL.UpdateAccessToken(LoginUser);
                }
                else
                {
                    if ((LoginUser.AccessToken == user.AccessToken && LoginUser.AccessToken != null) || string.IsNullOrEmpty(user.MacAddresses))
                    {
                        LoginUser = null;
                        return LoginStatus.Failed;
                    }
                    else
                    {
                        LoginUser.AccessToken = user.AccessToken;
                        if (!string.IsNullOrEmpty(LoginUser.AccessToken))
                        {
                            _UserBLL.UpdateAccessToken(LoginUser);
                        }
                    }
                }
                string[] MacAddresses = user.MacAddresses.Split(',').ToArray();
                List<UserSystemInfo> UserSystemInfoList = _UserSystemInfoBLL.Where(x => x.IsActive && !x.IsDeleted && x.DistributorId == LoginUser.DistributorId && MacAddresses.Contains(x.MACAddress)).ToList();
                if (UserSystemInfoList == null || UserSystemInfoList.Count() == 0)
                {
                    LoginUser = null;
                    return LoginStatus.NotRegistered;
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
        public bool CheckUserPassword(User user, string password)
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
