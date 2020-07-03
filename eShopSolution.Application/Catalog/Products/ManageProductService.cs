using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities.Exceptions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModel.Catatlog.Products.Manage;
using eShopSolution.ViewModel.Catatlog.Products;
using eShopSolution.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using eShopSolution.Application.Common;

namespace eShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbContext _context;
        private readonly IStorageService _storageService;

        public ManageProductService(EShopDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            product.ViewCount++;
            await _context.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,

                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Details,
                        SeoDescription = request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId
                    }
                },
            };

            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = request.SeoTitle,
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await SaveFileAsync(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }
            _context.Products.Add(product);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new ProductNotFoundException($"Cannot find product: {productId}");

            var images = _context.ProductImages.Where(x => x.ProductId == productId).ToList();

            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);
            }

            _context.Products.Remove(product);

            return await _context.SaveChangesAsync();
        }


        public async Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request)
        {
            // 1. Select
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };

            // 2. Filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));

            if (request.CategoryIds.Count > 0)
                query = query.Where(p => request.CategoryIds.Contains(p.pic.CategoryId));

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

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = _context.Products.FindAsync(request.Id);

            var productTranslation = await _context.ProductTranslations
                .FirstOrDefaultAsync(x => x.ProductId == request.Id && x.LanguageId == request.LanguageId);

            if (product == null || productTranslation == null)
                throw new ProductNotFoundException($"Cannot find product: {request.Id}");

            productTranslation.Name = request.Name;
            productTranslation.SeoTitle = request.SeoTitle;
            productTranslation.Description = request.Description;
            productTranslation.Details = request.Details;
            productTranslation.SeoDescription = request.SeoDescription;
            productTranslation.SeoAlias = request.SeoAlias;

            if (request.ThumbnailImage != null)
            {
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.ProductId == request.Id && x.IsDefault);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await SaveFileAsync(request.ThumbnailImage);

                    _context.ProductImages.Update(thumbnailImage);
                }

            }
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new ProductNotFoundException($"Cannot find product: {productId}");

            product.Price = newPrice;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new ProductNotFoundException($"Cannot find product: {productId}");

            product.Stock += addedQuantity;

            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var originalName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalName)}";

            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        public Task<int> AddImages(int productId, List<IFormFile> files)
        {
            throw new NotImplementedException();
        }
        public Task<int> RemoveImage(int imageId)
        {
            throw new NotImplementedException();
        }
        public Task<int> UpdateImage(int imageId, bool isDefault, string caption, int sortOrder)
        {
            throw new NotImplementedException();
        }
        public Task<List<ProductImageViewModel>> GetListImage(int productId)
        {
            throw new NotImplementedException();
        }
    }
}
