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
        public LoginBLL(IUnitOfWork Repository)
        {
            _unitOfWork = Repository;
            _UserBLL = new UserBLL(_unitOfWork);
            _UserSystemInfoBLL = new UserSystemInfoBLL(_unitOfWork);
        }
        public LoginStatus CheckLogin(User user)
        {
            List<string> MACAddresses = new List<string>();
            var Password = EncryptDecrypt.Encrypt(user.Password);
            User LoginUser = _UserBLL.Where(e => e.IsActive == true && e.IsDeleted == false && e.UserName == user.UserName && e.Password == Password).FirstOrDefault();
            if (LoginUser != null && LoginUser.DistributorId != null)
            {
                List<UserSystemInfo> UserSystemInfoList = _UserSystemInfoBLL.Where(e => e.DistributorId == LoginUser.DistributorId).ToList();
                
                if (UserSystemInfoList != null)
                {

                    foreach (var item in UserSystemInfoList.Select(x => x.MACAddress).ToList())
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            MACAddresses.Add(item);
                        }
                    }
                }
                if (LoginUser.IsDistributor)
                {
                    bool check = false;
                    if (MACAddresses.Count > 0)
                    {
                        if(MACAddresses.Contains(user.RegisteredAddress.Replace("-","")))
                        {
                            check = true;
                        }
                    }
                    if(check == false)
                    {
                        LoginUser = null;
                        return LoginStatus.NotRegistered;
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
