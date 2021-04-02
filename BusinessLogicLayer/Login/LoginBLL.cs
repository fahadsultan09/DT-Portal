using BusinessLogicLayer.Application;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.WorkProcess;
using Models.Application;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
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
            List<UserSystemInfo> UserSystemInfoList = _UserSystemInfoBLL.Where(e => e.IsActive == true && e.IsDeleted == false && e.DistributorId == LoginUser.DistributorId).ToList();
            if (LoginUser != null)
            {
                foreach (var item1 in UserSystemInfoList.Select(x => x.OtherMACAddress).ToList())
                {
                    if (!string.IsNullOrEmpty(item1))
                    {
                        MACAddresses.Add(item1);
                    }
                }
                foreach (var item2 in UserSystemInfoList.Select(x => x.MACAddress).ToList())
                {
                    if (!string.IsNullOrEmpty(item2))
                    {
                        MACAddresses.Add(item2);
                    }
                }
                if (LoginUser.IsDistributor)
                {
                    if (MACAddresses != null && !MACAddresses.Contains(user.RegisteredAddress.Replace("-", "")) || MACAddresses.Count() == 0)
                    {
                        LoginUser = null;
                    }
                    if (LoginUser != null)
                    {
                    }
                    else
                    {
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
