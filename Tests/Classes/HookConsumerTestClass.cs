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
    public class HookParserTestClass : GithubHookParser
    {
        public HookParserTestClass(Func<string, bool> fileNameValidator = null) : base(fileNameValidator)
        {
        }

        protected override IEnumerable<GithubFileEntry> GetFileUrls(GithubHook hook)
        {
            return base.GetFileUrls(hook);
        }

        public IEnumerable<GithubFileEntry> GetFileUrlsFromHook(GithubHook hook)
        {
            return GetFileUrls(hook);
        }

        public GithubHook GetHookFromJsonPublic(string hookJson)
        {
            return GetHookFromJson(hookJson);
        }
    }

    public class HookConsumerTestClass 
    {            
        class HookConsumer : GithubHookConsumer
        {
            public HookConsumer(ISalesHandlerFactory saleFileHandlerFactory, string token, GithubHookParser hookParser) : base(saleFileHandlerFactory, token, hookParser)
            {
            }
            
            public override Task ConsumeHookAsync(string hookJson)
            {
                return base.ConsumeHookAsync(hookJson);
            }
        }

        //=============================================================================================

        private HookConsumer hookConsumer;

        public HookConsumerTestClass(string token)
        {
            ISalesHandlerFactory handlerFactory = new FileHandlerFactoryTestClass();
            hookConsumer = new HookConsumer(handlerFactory, token, new HookParserTestClass(fileName => FileNameValidator.Validate(fileName)));
        }

        public void ConsumeHook(string hookJson)
        {
            hookConsumer.ConsumeHookAsync(hookJson).GetAwaiter().GetResult();
        }
    }
}
