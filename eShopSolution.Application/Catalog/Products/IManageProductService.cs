using eShopSolution.ViewModel.Catatlog.Products;
using eShopSolution.ViewModel.Catatlog.Products.Manage;
using eShopSolution.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request);
        Task<int> Update(ProductUpdateRequest request);
        Task<int> Delete(int productId);
        Task<bool> UpdatePrice(int productId, decimal newPrice);
        Task<bool> UpdateStock(int productId, int addedQuantity);
        Task AddViewCount(int productId);
        Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request);

        Task<int> AddImages(int productId, List<IFormFile> files);
        Task<int> RemoveImage(int imageId);
        Task<int> UpdateImage(int imageId, bool isDefault, string caption, int sortOrder);

        Task<List<ProductImageViewModel>> GetListImage(int productId);
    }
}
