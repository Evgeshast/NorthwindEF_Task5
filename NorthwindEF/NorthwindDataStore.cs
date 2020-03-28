using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindEF
{
    public class NorthwindDataStore
    {
        private Northwind _context = new Northwind();

        public List<Order> GetOrdersList()
        {
            return _context.Orders.ToList();
        }
        public List<Employee> GetEmployeesList()
        {
            return _context.Employees.ToList();
        }
        
        public IEnumerable<Order_Detail> GetOrderDetailsByCategory(string category)
        {
            return _context.Orders
                .SelectMany(x => x.Order_Details.ToList().Where(d => d.Product.Category.CategoryName == category))
                .ToList();
        }

        public List<ProductInfo> GetProductsInfos()
        {
            using (Northwind _context = new Northwind())
            {
                List<Product> products = _context.Products.ToList();
                List<ProductInfo> productInfos = new List<ProductInfo>();

                foreach (var product in products)
                {
                    ProductInfo productInfo = new ProductInfo();
                    productInfo.Supplier = _context.Suppliers.FirstOrDefault(x => x.SupplierID == product.SupplierID);
                    productInfo.Product = product;
                    productInfos.Add(productInfo);
                }

                return productInfos;
            }
        }
        
        public List<EmployeeInfo> GetEmployeesInfos()
        {
            using (Northwind _context = new Northwind())
            {
                List<Employee> employees = _context.Employees.ToList();
                List<EmployeeInfo> employeesInfos = new List<EmployeeInfo>();

                foreach (var employee in employees)
                {
                    EmployeeInfo employeeInfo = new EmployeeInfo();
                    employeeInfo.Region = _context.Regions.FirstOrDefault(x => x.RegionDescription == employee.Region);
                    employeeInfo.Employee = employee;
                    employeesInfos.Add(employeeInfo);
                }

                return employeesInfos;
            }
        }

        public List<RegionInfo> GetRegionsStatistics()
        {
            using (Northwind _context = new Northwind())
            {
                List<Region> regions = _context.Regions.ToList();
                List<RegionInfo> regionsInfos = new List<RegionInfo>();
                List<Employee> employees = _context.Employees.ToList();
                foreach (var region in regions)
                {
                    RegionInfo info = new RegionInfo();
                    info.NumberOfEmployees = employees.Count(r => r.Region == region.RegionDescription);
                    info.Region = region;
                    regionsInfos.Add(info);
                }

                return regionsInfos;
            }
        }
        
        public List<EmployeeInfo> GetEmployeeShippers()
        {
            using (Northwind _context = new Northwind())
            {
                List<EmployeeInfo> employeeInfos = new List<EmployeeInfo>();
                List<Order> orders = _context.Orders.ToList();
                List<Employee> employees = _context.Employees.ToList();
                foreach (var order in orders)
                {
                    EmployeeInfo employeeInfo;

                    if (employeeInfos.Any(x => x.Employee.EmployeeID == order.EmployeeID))
                    {
                        employeeInfo = employeeInfos.Find(x => x.Employee.EmployeeID == order.EmployeeID);
                        employeeInfo.Shippers.Add(order.Shipper);
                    }
                    else
                    {
                        employeeInfo = new EmployeeInfo
                        {
                            Employee = employees.FirstOrDefault(x => x.EmployeeID == order.EmployeeID),
                            Shippers = new List<Shipper> {order.Shipper}
                        };
                        employeeInfos.Add(employeeInfo);
                    }
                }
                return employeeInfos;
            }
        }

        public List<CategoryInfo> GetCategoriesInfo()
        {
            using (Northwind _context = new Northwind())
            {
                List<Product> products = _context.Products.ToList();
                List<CategoryInfo> categoryInfos = new List<CategoryInfo>();

                foreach (var product in products)
                {
                    CategoryInfo categoryInfo;

                    if (categoryInfos.Any(x => x.Category.CategoryID == product.CategoryID))
                    {
                        categoryInfo = categoryInfos.Find(x => x.Category.CategoryID == product.CategoryID);
                        categoryInfo.Products.Add(product);
                    }
                    else
                    {
                        categoryInfo = new CategoryInfo()
                        {
                            Category = _context.Categories.FirstOrDefault(x => x.CategoryID == product.CategoryID),
                            Products = new List<Product> { product }
                        };
                        categoryInfos.Add(categoryInfo);
                    }
                }
                return categoryInfos;
            }
        }

        public CategoryInfo GetCategoryInfo(Category category)
        {
            return GetCategoriesInfo().FirstOrDefault(x => x.Category.CategoryID == category.CategoryID);
        }

        public void UpdateProductsCategory(List<Product> products, Category newCategory)
        {
            using (Northwind _context = new Northwind())
            {
                List<Product> productsList = new List<Product>();
                foreach (var product in products)
                {
                    productsList.Add(_context.Products.ToList().FirstOrDefault(x => x.ProductID == product.ProductID));
                }
                productsList.ForEach(a => a.CategoryID = newCategory.CategoryID);
                _context.SaveChanges();
            }
        }

        public void UpdateProductsInNotCompletedOrders()
        {
            using (Northwind _context = new Northwind())
            {
                List<Order> ordersToUpdate = _context.Orders.Where(x => x.ShippedDate == null).ToList();
                List<Order_Detail> ordersToUpdateProducts = new List<Order_Detail>();
                foreach (var order in ordersToUpdate)
                {
                    ordersToUpdateProducts.Add(_context.Order_Details.FirstOrDefault(x => x.OrderID == order.OrderID));
                }

                List<CategoryInfo> categories = GetCategoriesInfo();

                foreach (var product in ordersToUpdateProducts.Where(x => x != null))
                {
                    Product newProduct = categories.FirstOrDefault(x => x.Products.Any(p => p.ProductID == product.ProductID))
                        .Products.FirstOrDefault(x => x.ProductID != product.ProductID);
                    foreach (var orderDetail in _context.Order_Details.ToList().Where(d => d.OrderID == product.OrderID))
                    {
                        orderDetail.ProductID = newProduct.ProductID;
                        orderDetail.Product = newProduct;
                    }
                }
                _context.SaveChanges();
            }
        }

        public void AddProducts(List<CategoryInfo> categoryInfos, List<ProductInfo> productInfos)
        {
            using (Northwind _context = new Northwind())
            {
                foreach (var categoryInfo in categoryInfos)
                {
                    _context.Categories.Add(categoryInfo.Category);
                    //categoryInfo.Products.ForEach(x => x.CategoryID = categoryInfo.Category.CategoryID);
                    _context.Products.AddRange(categoryInfo.Products);
                }
                _context.SaveChanges();
            }
        }

        public void AddNewEmployee(Employee employeeEntity, List<Territory> territoriesEntity)
        {
            using (Northwind _context = new Northwind())
            {
                List<Territory> territories = _context.Territories.ToList();
                foreach (var territoryEntity in territoriesEntity)
                {
                    territoryEntity.TerritoryID = territories.FirstOrDefault(t => t.TerritoryDescription.Trim() == territoryEntity.TerritoryDescription).TerritoryID;
                }
                employeeEntity.Territories = territoriesEntity;
                _context.Employees.Add(employeeEntity);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
        }
    }
}
