using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Classes;
using System.IO;
using Sales.SaleSource.Github;
using System.Linq;
using Sales.SaleSource;
using Sales.Storage.DTO;

namespace Tests
{
    [TestClass]
    public class HookAndFileTests
    {
        HookConsumerTestClass hookConsumer = new HookConsumerTestClass();
        SaleFileHandlerBase fileHandlerTestClass;

        FileHandlerTestClass FileHandler
        {
            get
            {
                if (fileHandlerTestClass == null)
                {
                    FileHandlerFactoryTestClass factory = new FileHandlerFactoryTestClass(false);
                    fileHandlerTestClass = factory.GetSaleFileHandler();
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
            var fileUrls = hookConsumer.GetFileUrlsFromHook(hook).OrderBy(f => f).ToArray();
            string baseUrl = "https://api.github.com/repos/NAlex2004/SalesData/contents/";

            Assert.AreEqual(4, fileUrls.Length);
            Assert.AreEqual(baseUrl + "Manager_2/AlNaz_04032019.json", fileUrls[0]);
            Assert.AreEqual(baseUrl + "Manager_2/ErErr_04032019.json", fileUrls[1]);
            Assert.AreEqual(baseUrl + "Manager_2/ErNot_04032019.json", fileUrls[2]);
            Assert.AreEqual(baseUrl + "Manager_2/VaAli_03032019.json", fileUrls[3]);
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
            SaleDataDto saleData = FileHandler.GetSalesFromGithubSync(url);

            Assert.IsNull(saleData);
        }
    }
}
