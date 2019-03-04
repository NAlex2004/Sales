using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sales.SaleSource;
using Sales.SaleSource.Factory;
using Sales.SaleSource.Github;
using Sales.SaleSource.Validation;

namespace Tests.Classes
{
    public class HookConsumerTestClass 
    {
        class HookConsumer : GithubHookConsumer
        {
            public HookConsumer(ISalesHandlerFactory saleFileHandlerFactory, string token, Func<string, bool> fileNameValidator = null) : base(saleFileHandlerFactory, token, fileNameValidator)
            {
            }

            protected override IEnumerable<string> GetFileUrls(GithubHook hook)
            {
                return base.GetFileUrls(hook);
            }

            public IEnumerable<string> GetFileUrlsFromHook(GithubHook hook)
            {
                return GetFileUrls(hook);
            }

            public override Task ConsumeHookAsync(string hookJson)
            {
                return base.ConsumeHookAsync(hookJson);
            }

            public GithubHook GetHookFromJsonPublic(string hookJson)
            {
                return GetHookFromJson(hookJson);
            }
        }

        //=============================================================================================

        private HookConsumer hookConsumer;

        public HookConsumerTestClass(string token)
        {
            ISalesHandlerFactory handlerFactory = new FileHandlerFactoryTestClass();
            hookConsumer = new HookConsumer(handlerFactory, token, fileName => FileNameValidator.Validate(fileName));
        }

        public IEnumerable<string> GetFileUrlsFromHook(GithubHook hook)
        {
            return hookConsumer.GetFileUrlsFromHook(hook);
        }

        public GithubHook GetHookFromJson(string hookJson)
        {
            return hookConsumer.GetHookFromJsonPublic(hookJson);
        }

        public void ConsumeHook(string hookJson)
        {
            hookConsumer.ConsumeHookAsync(hookJson).GetAwaiter().GetResult();
        }
    }
}
