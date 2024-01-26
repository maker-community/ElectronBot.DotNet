//using Microsoft.SemanticKernel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;

//namespace ElectronBot.Copilot
//{
//    public class VerdureChatKernel
//    {
//        private readonly ILocalSettingsService _localSettingsService;
//        public VerdureChatKernel()
//        {

//        }
//        private Microsoft.SemanticKernel.Kernel _kernel;

//        /// <summary>
//        /// 语义内核.
//        /// </summary>
//        public Kernel Kernel
//        {
//            get => _kernel;
//            set
//            {
//                _kernel = value;
//            }
//        }

//        private static async Task LoadOpenAIConfiguration(VerdureChatKernel kernel, string modelName = default)
//        {
//            var accessKey = GlobalSettings.TryGet<string>(SettingNames.OpenAIAccessKey);
//            var org = GlobalSettings.TryGet<string>(SettingNames.OpenAIOrganization);
//            var customEndpoint = GlobalSettings.TryGet<string>(SettingNames.OpenAICustomEndpoint);
//            var model = string.IsNullOrEmpty(modelName)
//                ? GlobalSettings.TryGet<string>(SettingNames.DefaultOpenAIChatModelName)
//                : modelName;

//            if (string.IsNullOrEmpty(accessKey)
//                || string.IsNullOrEmpty(model))
//            {
//                throw new Models.App.Args.KernelException(KernelExceptionType.InvalidConfiguration);
//            }

//            var result = await App.Get<>_localSettingsService
//           .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

//            if (result == null)
//            {
//                throw new Exception("配置为空");
//            }

//            _chatGptClient ??= new ChatGPTClient(result.ChatGPTSessionKey, "gpt-3.5-turbo");

//            _chatGptClient.Settings.OpenAIAPIBaseUri = result.OpenAIBaseUrl;



//            var hasCustomEndpoint = !string.IsNullOrEmpty(customEndpoint) && Uri.TryCreate(customEndpoint, UriKind.Absolute, out var _);
//            var customHttpClient = hasCustomEndpoint
//                ? GetProxyClient(customEndpoint)
//                : default;

//            kernel.Kernel = Kernel
//                .CreateBuilder()
//                .AddOpenAIChatCompletion(model, accessKey, org, httpClient: customHttpClient)
//                .Build();
//        }



//        private static HttpClient GetProxyClient(string baseUrl)
//        {
//            var httpClient = new HttpClient(new ProxyOpenAIHttpClientHandler(baseUrl));
//            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
//            return httpClient;
//        }
//    }
//}
