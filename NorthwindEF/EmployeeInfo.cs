using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindEF
{
    public class EmployeeInfo
    {
        public Employee Employee { get; set; }
        public Region Region { get; set; }
        public List<Shipper> Shippers { get; set; }
    }
}
