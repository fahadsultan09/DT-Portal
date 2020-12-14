using DataAccessLayer.WorkProcess;
using Models.UserRights;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using BusinessLogicLayer.GeneralSetup;
using BusinessLogicLayer.HelperClasses;

namespace BusinessLogicLayer.FormLogic
{
    public class RolePermissionLogic
    {
        private ApplicationPageActionBLL ApplicationPageActionBLL;
        private readonly IUnitOfWork _unitOfWork;
        public RolePermissionLogic(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
             ApplicationPageActionBLL = new ApplicationPageActionBLL(_unitOfWork);
        }
        public Permission GetPermissionList(int RoleId)
        {
            var rolepermission = new RolePermissionBLL(_unitOfWork).GetAllRolePermission().Where(e => e.RoleId == RoleId).ToList();
            var pages = new ApplicationPageBLL(_unitOfWork).GetAllApplicationPage().Where(x => x.IsActive == true && x.ApplicationModule.IsActive == true).ToList();
            var applicationPageActions = ApplicationPageActionBLL.GetAllApplicationPageAction().ToList();

            Permission permission = new Permission();
            List<ModuleViewModel> list = new List<ModuleViewModel>();
            foreach (var mod in pages.DistinctBy(e => e.ApplicationModuleId).Select(e => e.ApplicationModule).ToList())
            {
                ModuleViewModel moduleViewModels = new ModuleViewModel();
                List<PageViewModel> pageViewModel = new List<PageViewModel>();
                foreach (var Page in pages.Where(e => e.ApplicationModuleId == mod.Id).ToList())
                {
                    List<ActionViewModel> actionViewModel = new List<ActionViewModel>();
                    foreach (var action in applicationPageActions.Where(e => e.ApplicationPageId == Page.Id).ToList())
                    {
                        actionViewModel.Add(new ActionViewModel()
                        {
                            IsActionAllow = rolepermission.Any(e => e.ApplicationActionId == action.ApplicationActionId && e.ApplicationPageId == Page.Id),
                            ActionId = action.ApplicationActionId,
                            ActionName = action.ApplicationAction.ActionName,
                        });
                    }
                    pageViewModel.Add(new PageViewModel()
                    {
                        IsPageAllow = rolepermission.Any(e => e.ApplicationPageId == Page.Id),
                        PageId = Page.Id,
                        PageName = Page.PageTitle,
                        ActionViewModel = actionViewModel
                    });
                }
                moduleViewModels.IsModuleAllow = rolepermission.Any(e => e.ApplicationPage.ApplicationModuleId == mod.Id);
                moduleViewModels.ModuleId = mod.Id;
                moduleViewModels.ModuleName = mod.ModuleName;
                moduleViewModels.PageViewModel = pageViewModel;
                list.Add(moduleViewModels);
            }
            permission.ModuleViewModels = list;
            permission.RoleId = RoleId;
            //permission.RoleName = new RoleBLL(_unitOfWork).GetRoleById(RoleId).RoleName;
            return permission;
        }

        public int UpdatePermission(Permission model)
        {
            RolePermissionBLL rolePermissionBLL = new RolePermissionBLL(_unitOfWork);
            ApplicationPageActionBLL applicationPageActionBLL = new ApplicationPageActionBLL(_unitOfWork);
            var applicationpageactionlist = applicationPageActionBLL.GetAllApplicationPageAction().ToList();
            List<RolePermission> rolePermissions = new List<RolePermission>();
            var list = rolePermissionBLL.Where(e => e.RoleId == model.RoleId).ToList();
            rolePermissionBLL.DeleteRange(list);
            foreach (var item in model.ModuleViewModels.ToList())
            {
                if (item.PageViewModel != null)
                {
                    foreach (var page in item.PageViewModel.ToList())
                    {
                        if (page.ActionViewModel != null)
                        {
                            foreach (var action in page.ActionViewModel.ToList())
                            {
                                if (action.IsActionAllow)
                                {
                                    rolePermissions.Add(new RolePermission()
                                    {
                                        ApplicationPageId = page.PageId,
                                        RoleId = model.RoleId,
                                        CreatedBy = SessionHelper.LoginUser.Id,
                                        CreatedDate = DateTime.Now,
                                        ApplicationActionId = action.ActionId,
                                        ApplicationPageActionId = 1
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return rolePermissionBLL.AddRange(rolePermissions);
        }
    }
}
