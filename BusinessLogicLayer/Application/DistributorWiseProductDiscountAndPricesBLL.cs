using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using BusinessLogicLayer.HelperClasses;
using DataAccessLayer.Repository;
using DataAccessLayer.WorkProcess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Application;
using Models.UserRights;
using Models.ViewModel;

namespace BusinessLogicLayer.Application
{
    public class DistributorWiseProductDiscountAndPricesBLL
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<DistributorWiseProductDiscountAndPrices> _repository;
        public DistributorWiseProductDiscountAndPricesBLL(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.GenericRepository<DistributorWiseProductDiscountAndPrices>();
        }
        public bool AddRange(List<DistributorWiseProductViewModel> module)
        {
            try
            {
                _unitOfWork.Begin();
                List<DistributorWiseProductDiscountAndPrices> distributorWiseProductDiscountAndPricesList =
                    new List<DistributorWiseProductDiscountAndPrices>();
                var distributorData = GetAllDistributorWiseProductDiscountAndPrices();
                if (distributorData.Count > 0)
                {
                    DeleteDistributorWiseProductDiscountAndPrices(distributorData);
                }
                foreach (var item in module)
                {
                    distributorWiseProductDiscountAndPricesList.Add(new DistributorWiseProductDiscountAndPrices()
                    {
                        CartonSize = item.CartonSize,
                        Discount = item.Discount,
                        PackSize = item.PackSize,
                        ProductDescription = item.ProductDescription,
                        ProductName = item.ProductName,
                        ProductPrice = item.ProductPrice,
                        SAPProductCode = item.SAPProductCode,
                        CreatedBy = SessionHelper.LoginUser.Id,
                        CreatedDate = DateTime.Now,
                        DistributorId = item.DistributorId,
                        ProductDetailId = item.ProductDetailId
                    });
                }
                _repository.AddRange(distributorWiseProductDiscountAndPricesList); 
                _unitOfWork.Save();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                return false;
            }
            
        }
        public List<DistributorWiseProductDiscountAndPrices> GetAllDistributorWiseProductDiscountAndPrices()
        {
            return _repository.GetAllList().ToList();
        }

        public List<DistributorWiseProductDiscountAndPrices> Where(Expression<Func<DistributorWiseProductDiscountAndPrices, bool>> model)
        {
            return _repository.Where(model).ToList();
        }

        public void DeleteDistributorWiseProductDiscountAndPrices(List<DistributorWiseProductDiscountAndPrices> distributorData)
        {
            _repository.DeleteRange(distributorData);
        }
    }
}
