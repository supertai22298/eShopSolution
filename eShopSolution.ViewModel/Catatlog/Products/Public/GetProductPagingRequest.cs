using eShopSolution.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModel.Catatlog.Products.Public
{
    public class GetProductPagingRequest : PagingRequestBase
    {
        public int? CategoryId { get; set; }
    }
}
