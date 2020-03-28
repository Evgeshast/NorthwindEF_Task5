using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindEF
{
    public class ProductInfo
    {
        public Product Product { get; set; }
        public Supplier Supplier { get; set; }
    }
}
