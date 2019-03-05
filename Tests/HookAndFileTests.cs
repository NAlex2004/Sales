using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Classes;
using System.IO;
using Sales.SaleSource.Github;
using System.Linq;
using Sales.SaleSource;
using Sales.Storage.DTO;
using Sales.Storage.Management;
using Sales.DAL.Interfaces;
using Sales.DAL.Database;
using Sales.SaleSource.Factory;
using Sales.SaleSource.Validation;

namespace Tests
{
    [TestClass]
    public class HookAndFileTests
    {
        HookConsumerTestClass hookConsumer;
        SalesHandlerBase fileHandlerTestClass;
        string token = File.ReadAllText("../../Data/token.txt");

        public HookAndFileTests()
        {
            hookConsumer = new HookConsumerTestClass(token);
        }

        FileHandlerTestClass FileHandler
        {
            get
            {
                if (fileHandlerTestClass == null)
                {
                    FileHandlerFactoryTestClass factory = new FileHandlerFactoryTestClass(false);
                    fileHandlerTestClass = factory.GetSalesHandler();
                }

                return fileHandlerTestClass as FileHandlerTestClass;
            }
        }

        private GithubHook GetHook()
        {            
            string hookJson = File.ReadAllText("../../Data/hook1.json");
            GithubHook hook = hookConsumer.GetHookFromJson(hookJson);

            return hook;
        }

        [TestMethod]
        public void GetHookFromJson_Correct()
        {
            GithubHook hook = GetHook();

            Assert.AreEqual("NAlex2004/SalesData", hook.Repository.Full_Name);
            Assert.AreEqual(4, hook.Commits.Count);
            Assert.AreEqual(5, hook.Commits[0].Added.Count);
            Assert.AreEqual(0, hook.Commits[0].Modified.Count);
            Assert.AreEqual(1, hook.Commits[1].Removed.Count);
            Assert.AreEqual(1, hook.Commits[1].Added.Count);
            Assert.AreEqual(0, hook.Commits[1].Modified.Count);
            Assert.AreEqual(1, hook.Commits[2].Added.Count);
            Assert.AreEqual(1, hook.Commits[3].Removed.Count);
            Assert.AreEqual(9, hook.Commits.Sum(c => c.Added.Count + c.Modified.Count + c.Removed.Count));
        }

        [TestMethod]
        public void GetFileUrlsFromHook_CorrectUrls()
        {
            GithubHook hook = GetHook();
            var fileUrls = hookConsumer.GetFileUrlsFromHook(hook).OrderBy(f => f.Url).ToArray();
            string baseUrl = "https://api.github.com/repos/NAlex2004/SalesData/contents/";

            Assert.AreEqual(4, fileUrls.Length);
            Assert.AreEqual(baseUrl + "Manager_2/AlNaz_04032019.json", fileUrls[0].Url);
            Assert.AreEqual(baseUrl + "Manager_2/ErErr_04032019.json", fileUrls[1].Url);
            Assert.AreEqual(baseUrl + "Manager_2/ErNot_04032019.json", fileUrls[2].Url);
            Assert.AreEqual(baseUrl + "Manager_2/VaAli_03032019.json", fileUrls[3].Url);
        }

        [TestMethod]
        public void GetHookFromBadJson_returnsNull()
        {
            GithubHook nullHook = hookConsumer.GetHookFromJson(null);
            GithubHook badHook1 = hookConsumer.GetHookFromJson("aaa");
            string badHookJson1 = File.ReadAllText("../../Data/bad_hook1.json");
            string badHookJson2 = File.ReadAllText("../../Data/bad_hook2.json");
            string noCommitsHook = File.ReadAllText("../../Data/no_commits.json");
            GithubHook badHookJson1_hook = hookConsumer.GetHookFromJson(badHookJson1);
            GithubHook badHookJson2_hook = hookConsumer.GetHookFromJson(badHookJson2);
            GithubHook noCommitsHook_hook = hookConsumer.GetHookFromJson(noCommitsHook);


            Assert.IsNull(nullHook);
            Assert.IsNull(badHook1);
            Assert.IsNull(badHookJson1_hook);
            Assert.IsNull(badHookJson2_hook);
            Assert.IsNull(noCommitsHook_hook);
        }

        [TestMethod]
        public void HookConsuming_DoesNotFail_WithFakeFileHandler()
        {
            hookConsumer.ConsumeHook(null);
            hookConsumer.ConsumeHook(File.ReadAllText("../../Data/hook1.json"));
        }

        [TestMethod]
        public void GetSalesFromGithub_FailsOnBadToken()
        {
            string url = "https://api.github.com/repos/NAlex2004/SalesData/contents/Manager_2/AlNaz_04032019.json";
            FileHandlerTestClass fileHandler = new FileHandlerTestClass("123");
            SaleDataDto saleData = fileHandler.GetSalesFromGithubSync(url);

            Assert.AreEqual(0, saleData.Sales.Count);
        }

        [TestMethod]
        public void GetSalesFromGithub_SucceededWithCorrectToken()
        {
            string url = "https://api.github.com/repos/NAlex2004/SalesData/contents/Manager_2/AlNaz_04032019.json";            
            SaleDataDto saleData = FileHandler.GetSalesFromGithubSync(url);

            Assert.AreEqual(5, saleData.Sales.Count);
            Assert.AreEqual(1348.7m, saleData.Sales.Sum(s => s.TotalSum));
        }

        [TestMethod]
        public void Real_SaleFileHandling_ErrorRemoved_SecondTimeSameDataAddsCorrect()
        {
            string url = "https://api.github.com/repos/NAlex2004/SalesData/contents/Manager_2/AlNaz_04032019.json";
            //string token = File.ReadAllText("../../Data/token.txt");

            var manager = new SaleDbDataManager();
            using (GithubSalesHandler fileHandler = new GithubSalesHandler(manager))
            {                                
                using (ISalesUnitOfWork unitOfWork = new SalesDbUnitOfWork(new Sales.SalesEntity.Entity.SalesDbContext()))
                {
                    var errorAdded = manager.AddErrorAsync(new SaleManagementResult() { FileName = "AlNaz_04032019.json", ErrorMessage = "Test error" }).GetAwaiter().GetResult();

                    Assert.IsTrue(errorAdded.Succeeded);

                    int errors = unitOfWork.ErrorFiles.Get().Count();                    
                    fileHandler.HandleSaleSourceAsync(new GithubSaleDataSource(new GithubFileEntry() { Url = url, CommitDate = DateTime.Now }, token)).GetAwaiter().GetResult();
                    int errorsAfter = unitOfWork.ErrorFiles.Get().Count();

                    Assert.AreEqual(errors - 1, errorsAfter);
                }

                fileHandler.HandleSaleSourceAsync(new GithubSaleDataSource(new GithubFileEntry() { Url = url, CommitDate = DateTime.Now }, token)).GetAwaiter().GetResult();
            }            
        }

        [TestMethod]
        public void GithubHookConsumer_NothingInDb_WhenBadHook()
        {
            ISalesHandlerFactory fileHandlerFactory = new GithubSalesHandlerFactory();
            IHookConsumer hookConsumer = new GithubHookConsumer(fileHandlerFactory, token, name => FileNameValidator.Validate(name));

            using (ISalesUnitOfWork unitOfWork = new SalesDbUnitOfWork(new Sales.SalesEntity.Entity.SalesDbContext()))
            {
                int customersCount = unitOfWork.Customers.Get().Count();
                int productsCount = unitOfWork.Products.Get().Count();
                int sourceFilesCount = unitOfWork.SourceFiles.Get().Count();
                int salesCount = unitOfWork.Sales.Get().Count();
                int errorsCount = unitOfWork.ErrorFiles.Get().Count();
                hookConsumer.ConsumeHookAsync("../../Data/bad_hook1.json").GetAwaiter().GetResult();
                int customersCountAfter = unitOfWork.Customers.Get().Count();
                int productsCountAfter = unitOfWork.Products.Get().Count();
                int sourceFilesCountAfter = unitOfWork.SourceFiles.Get().Count();
                int salesCountAfter = unitOfWork.Sales.Get().Count();
                int errorsCountAfter = unitOfWork.ErrorFiles.Get().Count();

                Assert.AreEqual(customersCount, customersCountAfter);
                Assert.AreEqual(productsCount, productsCountAfter);
                Assert.AreEqual(sourceFilesCount, sourceFilesCountAfter);
                Assert.AreEqual(salesCount, salesCountAfter);
                Assert.AreEqual(errorsCount, errorsCountAfter);
            }            
        }

        [TestMethod]
        public void GithubHookConsumer_Ok_WhenGoodHook()
        {
            ISalesHandlerFactory fileHandlerFactory = new GithubSalesHandlerFactory();
            IHookConsumer hookConsumer = new GithubHookConsumer(fileHandlerFactory, token, name => FileNameValidator.Validate(name));

            using (ISalesUnitOfWork unitOfWork = new SalesDbUnitOfWork(new Sales.SalesEntity.Entity.SalesDbContext()))
            {
                unitOfWork.Customers.Delete(x => true);
                unitOfWork.Products.Delete(x => true);
                unitOfWork.SourceFiles.Delete(x => true);
                unitOfWork.Sales.Delete(x => true);
                unitOfWork.ErrorFiles.Delete(x => true);
                string hookJson = File.ReadAllText("../../Data/hook1.json");
                hookConsumer.ConsumeHookAsync(hookJson).GetAwaiter().GetResult();
                int customersCountAfter = unitOfWork.Customers.Get().Count();
                int productsCountAfter = unitOfWork.Products.Get().Count();
                int sourceFilesCountAfter = unitOfWork.SourceFiles.Get().Count();
                int salesCountAfter = unitOfWork.Sales.Get().Count();
                int errorsCountAfter = unitOfWork.ErrorFiles.Get().Count();

                //Assert.AreEqual(customersCount, customersCountAfter);
                //Assert.AreEqual(productsCount, productsCountAfter);
                //Assert.AreEqual(sourceFilesCount, sourceFilesCountAfter);
                //Assert.AreEqual(salesCount, salesCountAfter);
                //Assert.AreEqual(errorsCount, errorsCountAfter);
            }
        }
    }
}
