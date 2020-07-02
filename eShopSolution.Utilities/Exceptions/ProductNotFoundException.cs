using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Utilities.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException()
        {

        }
        public ProductNotFoundException(string message): base(message)
        {

        }     
        public ProductNotFoundException(string message, Exception inner): base(message, inner)
        {

        }

    }
}
