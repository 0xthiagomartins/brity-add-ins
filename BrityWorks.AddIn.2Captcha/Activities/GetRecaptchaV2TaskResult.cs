using RPAGO.AddIn;
using RPAGO.Common.Data;
using RPAGO.Common.Library;
using RPAGO.Common.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using BrityWorks.AddIn.TwoCaptcha.Properties;

namespace BrityWorks.AddIn.TwoCaptcha.Activities
{
    internal class GetRecaptchaV2TaskResult : IActivityItem
    {
        // PropKeys
        public static readonly PropKey TwoCaptchaApiKeyPropKey = new PropKey("TwoCaptcha", "TwoCaptchaApiKey");
        public static readonly PropKey TaskIdPropKey = new PropKey("TwoCaptcha", "TaskId");

        public static readonly PropKey GRecaptchaResponsePropKey = new PropKey("TwoCaptcha", "GRecaptchaResponse");

        public string DisplayName => "Get reCAPTCHA V2 Task Result";

        public Bitmap Icon => Resources.ReCaptchaV2Icon;
        public LibraryHeadlessType Mode => LibraryHeadlessType.Both;

        public PropKey DisplayTextProperty => GRecaptchaResponsePropKey;

        // Como saída, retornamos o gRecaptchaResponse
        public PropKey OutputProperty => GRecaptchaResponsePropKey;

        public List<Property> OnCreateProperties()
        {
            return new List<Property>
            {
                new Property(this, GRecaptchaResponsePropKey, "RESULT").SetRequired(),
                new Property(this, TwoCaptchaApiKeyPropKey, "'TWOCAPTCHA_API_KEY'").SetRequired(),
                new Property(this, TaskIdPropKey, 0).SetRequired()
            };
        }

        public void OnLoad(PropertySet properties)
        {
            return;
        }

        public object OnRun(IDictionary<string, object> properties)
        {
            string twoCaptchaApiKey = properties[TwoCaptchaApiKeyPropKey]?.ToString() ?? "";
            long taskId = 0;
            if (properties[TaskIdPropKey] is long l) taskId = l;
            else if (properties[TaskIdPropKey] is int i) taskId = i;

            // Montar JSON
            var body = new
            {
                clientKey = twoCaptchaApiKey,
                taskId = taskId
            };

            string url = "https://api.2captcha.com/getTaskResult";
            var httpClient = new HttpClient();

            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var responseTask = httpClient.PostAsync(url, content);
                responseTask.Wait();

                var response = responseTask.Result;
                if (!response.IsSuccessStatusCode)
                {
                    return $"HTTP error: {response.StatusCode}";
                }

                var readTask = response.Content.ReadAsStringAsync();
                readTask.Wait();
                string responseBody = readTask.Result;

                // Exemplo de resposta esperada:
                // {
                //   "errorId": 0,
                //   "status": "ready",
                //   "solution": {
                //       "gRecaptchaResponse": "03ADUVZw...UWxTAe6ncIa",
                //       "token": "03ADUVZw...UWxTAe6ncIa"
                //   }
                //   ...
                // }

                var taskResult = JsonConvert.DeserializeObject<GetTaskResultResponse>(responseBody);
                if (taskResult == null)
                {
                    return "Failed to parse getTaskResult response.";
                }

                if (taskResult.errorId != 0)
                {
                    return $"2Captcha errorId: {taskResult.errorId}";
                }

                if (taskResult.status != "ready")
                {
                    return "Captcha is not ready yet. Please try again later.";
                }

                // Se chegou até aqui, captcha resolvido
                string recaptchaResponse = taskResult.solution?.gRecaptchaResponse ?? "";

                // Armazena na propriedade de saída
                properties[GRecaptchaResponsePropKey] = recaptchaResponse;

                return $"Captcha solved. gRecaptchaResponse: {recaptchaResponse}";
            }
            catch (Exception ex)
            {
                return $"Error in GetRecaptchaV2TaskResultActivity: {ex.Message}";
            }
        }

        // Classe auxiliar para desserializar a resposta
        private class GetTaskResultResponse
        {
            public int errorId { get; set; }
            public string status { get; set; }
            public Solution solution { get; set; }

            public class Solution
            {
                public string gRecaptchaResponse { get; set; }
                public string token { get; set; }
            }
        }
    }
}
