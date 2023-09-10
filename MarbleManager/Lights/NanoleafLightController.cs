using MarbleManager.Colours;
using MarbleManager.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    internal class NanoleafLightController : ILightController
    {
        NanoleafConfig config;

        static string baseUrl = "http://<nanoleafIp>:16021";
        string populatedUrl;

        public NanoleafLightController(ConfigObject _config)
        {
            SetConfig(_config);
        }

        public void ApplyPalette(PaletteObject _palette)
        {
            throw new NotImplementedException();
        }

        public void SetConfig(ConfigObject _config)
        {
            config = _config.nanoleafConfig;

            populatedUrl = baseUrl.Replace("<nanoleafIp>", config.ipAddress);
        }

        public void TurnOnOff(bool _state)
        {
            if (populatedUrl == null)
            {
                Console.WriteLine("nanoleaf url not set up");
                throw new Exception("nanoleaf url not set up");
            }

            SendPayload(OnOffPayload(_state), "api/v1/" + config.apiKey + "/state");
        }

        private async void SendPayload(object payload, string endpoint)
        {
            if (payload == null)
            {
                throw new Exception("null payload");
            }

            using (HttpClient client = new HttpClient())
            {
                // set URL
                client.BaseAddress = new Uri(populatedUrl);

                // create payload string
                string jsonPayload = JsonConvert.SerializeObject(payload);
                StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // send the request
                HttpResponseMessage response = await client.PutAsync(endpoint, content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read and process the response content (if any)
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Nanoleaf Success: " + responseContent);
                }
                else
                {
                    Console.WriteLine("Nanoleaf Error: " + response.StatusCode);
                }
            }
        }

        private object OnOffPayload(bool state)
        {
            return new
            {
                on = new
                {
                    value = state
                }
            };
        }
    }
}
