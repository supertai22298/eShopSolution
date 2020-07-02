using eShopSolution.Application.Catalog.Products.Dtos.Manage;
using eShopSolution.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    class PublicProductService : IPublicProductService
    {
        public Task<PagedResult<ProductViewModel>> GetAllByCategoryId(int categoryId, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
