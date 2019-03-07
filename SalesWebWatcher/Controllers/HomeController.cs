using Sales.DAL.Interfaces;
using Sales.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sales.SalesWebWatcher.BL;

namespace Sales.SalesWebWatcher.Controllers
{
    public class HomeController : Controller
    {
        private DataManager dataManager;

        public HomeController(DataManager dataManager)
        {
            this.dataManager = dataManager ?? throw new ArgumentNullException();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Products()
        {
            var products = dataManager.GetProducts();
            return View(products);
        }

        public ActionResult Customers()
        {
            var customers = dataManager.GetCustomers();
            return View(customers);
        }

        public ActionResult SourceFiles()
        {
            var sourceFiles = dataManager.GetSourceFiles();
            return View(sourceFiles);
        }

        public ActionResult Errors()
        {
            var errors = dataManager.GetErrors();
            return View(errors);
        }

        public ActionResult Sales()
        {
            var sales = dataManager.GetSales();
            return View(sales);
        }
    }
}