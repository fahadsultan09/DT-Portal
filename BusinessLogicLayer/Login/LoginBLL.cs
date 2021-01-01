using System.Linq;
using DataAccessLayer.WorkProcess;
using Utility;
using Utility.HelperClasses;
using Models.Application;
using BusinessLogicLayer.HelperClasses;
using BusinessLogicLayer.GeneralSetup;

namespace BusinessLogicLayer.Login
{
    public class LoginBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        public LoginBLL(IUnitOfWork Repository)
        {
            _unitOfWork = Repository;
        }
        
        public LoginStatus CheckLogin(User user)
        {           
            var Password = EncryptDecrypt.Encrypt(user.Password);
            var LoginUser = _unitOfWork.GenericRepository<User>().Where(e => e.IsActive == true && e.IsDeleted == false && e.UserName == user.UserName && e.Password == Password).FirstOrDefault();
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
            var Password = EncryptDecrypt.Encrypt(password);
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
