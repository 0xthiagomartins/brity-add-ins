using RPAGO.AddIn;
using RPAGO.Common.Data;
using RPAGO.Common.Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using BrityWorks.AddIn.TwoCaptcha.Properties;

namespace BrityWorks.AddIn.TwoCaptcha.Activities.ReCaptchaV2
{
    internal class GetTaskResult : IActivityItem
    {
        // PropKeys
        public static readonly PropKey GRecaptchaResponsePropKey = new PropKey("TwoCaptcha", "GRecaptchaResponse");
        public static readonly PropKey TwoCaptchaApiKeyPropKey = new PropKey("TwoCaptcha", "TwoCaptchaApiKey");
        public static readonly PropKey TaskIdPropKey = new PropKey("TwoCaptcha", "TaskId");
        public string DisplayName => "Get reCAPTCHA V2 Task Result";
        public Bitmap Icon => Resources.ReCaptchaV2Icon;
        public LibraryHeadlessType Mode => LibraryHeadlessType.Both;
        public PropKey DisplayTextProperty => GRecaptchaResponsePropKey;
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
    internal class CreateTask : IActivityItem
    {
        public static readonly PropKey TwoCaptchaApiKeyPropKey = new PropKey("TwoCaptcha", "TwoCaptchaApiKey");
        public static readonly PropKey WebsiteURLPropKey = new PropKey("TwoCaptcha", "WebsiteURL");
        public static readonly PropKey WebsiteKeyPropKey = new PropKey("TwoCaptcha", "WebsiteKey");
        public static readonly PropKey IsInvisiblePropKey = new PropKey("TwoCaptcha", "IsInvisible");
        public static readonly PropKey CreatedTaskIdPropKey = new PropKey("TwoCaptcha", "TaskId");

        public string DisplayName => "Create reCAPTCHA V2 Task";
        public Bitmap Icon => Resources.ReCaptchaV2Icon;
        public LibraryHeadlessType Mode => LibraryHeadlessType.Both;
        public PropKey DisplayTextProperty => CreatedTaskIdPropKey;
        public PropKey OutputProperty => CreatedTaskIdPropKey;
        public List<Property> OnCreateProperties()
        {
            return new List<Property>
            {
                new Property(this, CreatedTaskIdPropKey, "RESULT").SetRequired(),
                new Property(this, TwoCaptchaApiKeyPropKey, "'TWOCAPTCHA_API_KEY'").SetRequired(),
                new Property(this, WebsiteURLPropKey, "'https://2captcha.com/demo/recaptcha-v2'").SetRequired(),
                new Property(this, WebsiteKeyPropKey, "'6LfD3PIbAAAAAJs_eEHvoOl75_83eXSqpPSRFJ_u'").SetRequired(),
                new Property(this, IsInvisiblePropKey, false)
            };  
        }

        public void OnLoad(PropertySet properties)
        {
            return;
        }

        public object OnRun(IDictionary<string, object> properties)
        {
            string twoCaptchaApiKey = properties[TwoCaptchaApiKeyPropKey]?.ToString() ?? "";
            string websiteURL = properties[WebsiteURLPropKey]?.ToString() ?? "";
            string websiteKey = properties[WebsiteKeyPropKey]?.ToString() ?? "";
            bool isInvisible = (properties[IsInvisiblePropKey] is bool b) && b;
            var body = new
            {
                clientKey = twoCaptchaApiKey,
                task = new
                {
                    type = "RecaptchaV2TaskProxyless", // Exemplo sem proxy
                    websiteURL = websiteURL,
                    websiteKey = websiteKey,
                    isInvisible = isInvisible
                }
            };

            string url = "https://api.2captcha.com/createTask";
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

                // Exemplo de response esperado (sucesso):
                // {
                //   "errorId":0,
                //   "taskId":123456789
                // }

                var createTaskResponse = JsonConvert.DeserializeObject<CreateTaskResponse>(responseBody);

                if (createTaskResponse == null)
                {
                    return "Failed to parse createTask response.";
                }

                if (createTaskResponse.errorId != 0)
                {
                    return $"2Captcha errorId: {createTaskResponse.errorId}";
                }

                properties[CreatedTaskIdPropKey] = createTaskResponse.taskId;
                return $"Task created successfully. Task ID = {createTaskResponse.taskId}";
            }
            catch (Exception ex)
            {
                return $"Error in CreateRecaptchaV2TaskActivity: {ex.Message}";
            }
        }

        private class CreateTaskResponse
        {
            public int errorId { get; set; }
            public long taskId { get; set; }
        }
    }
}
