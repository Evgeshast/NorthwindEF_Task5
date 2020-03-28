using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindEF
{
    public class CategoryInfo
    {
        public Category Category { get; set; }

        public List<Product> Products { get; set; }
    }
}
