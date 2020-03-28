using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindEF;

namespace NorthwindTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetProductsByCategory()
        { 
            NorthwindDataStore northwindDataStore = new NorthwindDataStore(); 
            IEnumerable<Order_Detail> orders =  northwindDataStore.GetOrderDetailsByCategory("Beverages");
            Assert.AreNotEqual(0, orders.Count());
        }
        
        [TestMethod]
        public void GetProductsAndSuppliers()
        {
            NorthwindDataStore northwindDataStore = new NorthwindDataStore();
            IEnumerable<ProductInfo> products = northwindDataStore.GetProductsInfos();
            Assert.AreNotEqual(0, products.Count());
        }

        [TestMethod]
        public void GetEmployeesAndRegions()
        {
            NorthwindDataStore northwindDataStore = new NorthwindDataStore();
            IEnumerable<EmployeeInfo> employees = northwindDataStore.GetEmployeesInfos();
            Assert.AreNotEqual(0, employees.Count());
        }
        
        [TestMethod]
        public void GetRegionsStatistics()
        {
            NorthwindDataStore northwindDataStore = new NorthwindDataStore();
            IEnumerable<RegionInfo> regions = northwindDataStore.GetRegionsStatistics();
            Assert.AreNotEqual(0, regions.Count());
        }

        [TestMethod]
        public void GetEmployeesShippersInfo()
        {
            NorthwindDataStore northwindDataStore = new NorthwindDataStore();
            IEnumerable<EmployeeInfo> employees = northwindDataStore.GetEmployeeShippers();
            Assert.AreNotEqual(0, employees.Count());
        }

        [TestMethod]
        public void AddEmployeeWithTerritory()
        {
            NorthwindDataStore northwindDataStore = new NorthwindDataStore();
            IEnumerable<Employee> employees = northwindDataStore.GetEmployeesList();
            Employee employee = new Employee();
            employee.LastName = "Strelkova";
            employee.FirstName = "Zhenya";
            employee.Orders = null;
            employee.BirthDate = DateTime.Today.AddYears(-20);
            employee.Title = "Sales Manager";
            List<Territory> territories = new List<Territory>();
            territories.Add(new Territory()
            {
                TerritoryDescription = "Morristown"
            });
            territories.Add(new Territory()
            {
                TerritoryDescription = "Chicago"
            });
            northwindDataStore.AddNewEmployee(employee, territories);
            Assert.AreNotEqual(employees.Count(), northwindDataStore.GetEmployeesList());
        }

        [TestMethod]
        public void UpdateProductCategory()
        {
            NorthwindDataStore northwindDataStore = new NorthwindDataStore();
            List<CategoryInfo> categoryInfos = northwindDataStore.GetCategoriesInfo();
            List<Product> productsToChange = categoryInfos.FirstOrDefault().Products.Skip(2).ToList();
            Category newCategory = categoryInfos.Last().Category;
            CategoryInfo categoryInfo = northwindDataStore.GetCategoryInfo(newCategory);
            northwindDataStore.UpdateProductsCategory(productsToChange, newCategory); 
            Assert.AreNotEqual(categoryInfo.Products.Count, northwindDataStore.GetCategoryInfo(newCategory).Products.Count);
        }        
        
        [TestMethod]
        public void AddProduct()
        {
            Random rnd = new Random();
            CategoryInfo categoryInfo = new CategoryInfo();
            ProductInfo productInfo = new ProductInfo();
            productInfo.Product = new Product()
            {
                ProductName = "New Product",
                Discontinued = false
            };
            productInfo.Supplier = new Supplier()
            {
                CompanyName = "New supplier"
            };
            ProductInfo productInfo2 = new ProductInfo();
            productInfo2.Product = new Product()
            {
                ProductName = "New Product2",
                Discontinued = true
            };
            productInfo2.Supplier = new Supplier()
            {
                CompanyName = "New supplier2"
            };
            categoryInfo.Products = new List<Product>()
            {
                productInfo.Product,
                productInfo2.Product
            };
            categoryInfo.Category = new Category()
            {
                CategoryName = $"New Category{rnd.Next(150)}"
            };
            List<CategoryInfo> categoryInfos = new List<CategoryInfo>() { categoryInfo };
            List<ProductInfo> productsInfos = new List<ProductInfo>() { productInfo, productInfo2 };
            categoryInfos.ForEach(c => c.Products.ForEach(p => p.CategoryID = c.Category.CategoryID));
            NorthwindDataStore northwindDataStore = new NorthwindDataStore();
            northwindDataStore.AddProducts(categoryInfos, productsInfos);
            Assert.IsTrue(northwindDataStore.GetCategoriesInfo().Any(x => x.Category.CategoryName == categoryInfo.Category.CategoryName));
        }

        [TestMethod]
        public void UpdateProductInOrder()
        {
            NorthwindDataStore northwindDataStore = new NorthwindDataStore();
            northwindDataStore.UpdateProductsInNotCompletedOrders();
        }
    }
}
