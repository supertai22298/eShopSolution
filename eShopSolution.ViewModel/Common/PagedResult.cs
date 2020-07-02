﻿using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModel.Common
{
    public class PagedResult<T>
    {
        public int TotalRecord { get; set; }

        public List<T> Items { get; set; }
    }
}
