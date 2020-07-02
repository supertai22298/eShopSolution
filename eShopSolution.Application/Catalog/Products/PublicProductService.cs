
using eShopSolution.Data.EF;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModel.Catatlog.Products;
using eShopSolution.ViewModel.Catatlog.Products.Public;
using eShopSolution.ViewModel.Common;

namespace eShopSolution.Application.Catalog.Products
{
    class PublicProductService : IPublicProductService
    {
        private readonly EShopDbContext _context;

        public PublicProductService(EShopDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<ProductViewModel>> GetAllByCategoryId(GetProductPagingRequest request)
        {
            // 1. Select
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic, c };

            // 2. Filter
            if (request.CategoryId.HasValue && request.CategoryId > 0)
                query = query.Where(result => result.c.Id ==request.CategoryId);
            // 3. Paging

            int totalRow = await query.CountAsync();

            var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(result => new ProductViewModel()
                {
                    Id = result.p.Id,
                    Price = result.p.Price,
                    OriginalPrice = result.p.OriginalPrice,
                    Stock = result.p.Stock,
                    ViewCount = result.p.ViewCount,
                    DateCreated = result.p.DateCreated,
                    SeoAlias = result.pt.SeoAlias,
                    Name = result.pt.Name,
                    Description = result.pt.Description,
                    Details = result.pt.Details,
                    SeoDescription = result.pt.SeoDescription,
                    SeoTitle = result.pt.SeoTitle,
                    LanguageId = result.pt.LanguageId

                }).ToList();

            // 4. Select and Projection

            var pagedResult = new PagedResult<ProductViewModel>()
            {
                TotalRecord = totalRow,
                Items = data
            };
            return pagedResult;
        }
    }
}
