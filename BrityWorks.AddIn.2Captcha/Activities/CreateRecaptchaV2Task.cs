using RPAGO.AddIn;
using RPAGO.Common.Data;
using RPAGO.Common.Library;
using RPAGO.Common.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BrityWorks.AddIn.TwoCaptcha.Properties;

namespace BrityWorks.AddIn.TwoCaptcha.Activities
{
    internal class CreateRecaptchaV2Task : IActivityItem
    {
        // PropKeys
        public static readonly PropKey ClientKeyPropKey = new PropKey("TwoCaptcha", "ClientKey");
        public static readonly PropKey WebsiteURLPropKey = new PropKey("TwoCaptcha", "WebsiteURL");
        public static readonly PropKey WebsiteKeyPropKey = new PropKey("TwoCaptcha", "WebsiteKey");
        public static readonly PropKey IsInvisiblePropKey = new PropKey("TwoCaptcha", "IsInvisible");
        public static readonly PropKey CreatedTaskIdPropKey = new PropKey("TwoCaptcha", "TaskId");

        // Nome da Activity
        public string DisplayName => "Create reCAPTCHA V2 Task";

        // Ícone (opcional)
        public Bitmap Icon => Resources.Icon;

        // Modo headless ou UI
        public LibraryHeadlessType Mode => LibraryHeadlessType.Both;

        // Se quiser exibir algum texto "ao vivo", poderia usar um DisplayTextProperty aqui
        public PropKey DisplayTextProperty => CreatedTaskIdPropKey;

        // Definimos o TaskId como nossa "saída" principal
        public PropKey OutputProperty => CreatedTaskIdPropKey;

        // Cria as propriedades que o usuário vai preencher antes de rodar a Activity
        public List<Property> OnCreateProperties()
        {
            return new List<Property>
            {
                new Property(this, CreatedTaskIdPropKey, "RESULT").SetRequired(),
                new Property(this, ClientKeyPropKey, "YOUR_API_KEY").SetRequired(),
                new Property(this, WebsiteURLPropKey, "https://2captcha.com/demo/recaptcha-v2").SetRequired(),
                new Property(this, WebsiteKeyPropKey, "6LfD3PIbAAAAAJs_eEHvoOl75_83eXSqpPSRFJ_u").SetRequired(),
                new Property(this, IsInvisiblePropKey, false) // Default: false
            };
        }

        public void OnLoad(PropertySet properties)
        {
            // Não precisamos fazer nada aqui por enquanto
            return;
        }

        public object OnRun(IDictionary<string, object> properties)
        {
            // 1. Ler as propriedades
            string clientKey = properties[ClientKeyPropKey]?.ToString() ?? "";
            string websiteURL = properties[WebsiteURLPropKey]?.ToString() ?? "";
            string websiteKey = properties[WebsiteKeyPropKey]?.ToString() ?? "";
            bool isInvisible = (properties[IsInvisiblePropKey] is bool b) && b;

            // 2. Montar o JSON (utilizando a doc do 2captcha)
            var body = new
            {
                clientKey = clientKey,
                task = new
                {
                    type = "RecaptchaV2TaskProxyless", // Exemplo sem proxy
                    websiteURL = websiteURL,
                    websiteKey = websiteKey,
                    isInvisible = isInvisible
                }
            };

            // 3. Fazer a requisição HTTP
            string url = "https://api.2captcha.com/createTask";
            var httpClient = new HttpClient();

            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var responseTask = httpClient.PostAsync(url, content);
                responseTask.Wait(); // Sincronamente para simplificar. Em produção use async/await

                var response = responseTask.Result;
                if (!response.IsSuccessStatusCode)
                {
                    return $"HTTP error: {response.StatusCode}";
                }

                // 4. Ler o response JSON
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

                // 5. Obter taskId e salvar na propriedade
                properties[CreatedTaskIdPropKey] = createTaskResponse.taskId;

                return $"Task created successfully. Task ID = {createTaskResponse.taskId}";
            }
            catch (Exception ex)
            {
                return $"Error in CreateRecaptchaV2TaskActivity: {ex.Message}";
            }
        }

        // Classe auxiliar para desserializar a resposta do 2captcha
        private class CreateTaskResponse
        {
            public int errorId { get; set; }
            public long taskId { get; set; }
        }
    }
}
