using MarbleManager.Colours;
using MarbleManager.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace MarbleManager.Lights
{
    internal class NanoleafLightController : ILightController
    {
        NanoleafConfig config;

        static string effectPayloadDir = "effect_payloads\\";
        static string baseUrl = "http://<nanoleafIp>:16021";
        string populatedUrl;

        public NanoleafLightController(ConfigObject _config)
        {
            SetConfig(_config);
        }

        public void ApplyPalette(PaletteObject _palette)
        {
            // get payload template based on config setting
            JObject payload = GetPayloadTemplate();

            // insert palette into payload
            //JArray palette = FormatPalette(_palette);
            //if (palette == null || palette.Count <= 0) {
            //    Console.WriteLine("nanoleaf palette should not be empty");
            //    return; 
            //}

            //payload["write"]["palette"] = palette;

            // send payload
            SendPayload(payload, "/effects");
        }

        public void SetConfig(ConfigObject _config)
        {
            config = _config.nanoleafConfig;

            // populate url with ip address
            populatedUrl = baseUrl.Replace("<nanoleafIp>", config.ipAddress);
        }

        public void SetOnOffState(bool _state)
        {
            if (populatedUrl == null)
            {
                Console.WriteLine("nanoleaf url not set up");
                throw new Exception("nanoleaf url not set up");
            }

            SendPayload(GetStatePayload(_state), "/state");
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
                HttpResponseMessage response = await client.PutAsync("api/v1/" + config.apiKey + endpoint, content);

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
        private JObject GetPayloadTemplate()
        {
            try
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, ConfigManager.TemplatesDirectory, effectPayloadDir, GetTemplateName());
                // load file here if exists
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    JObject payload = JsonConvert.DeserializeObject<JObject>(json);
                    Console.WriteLine("Loaded effect payload");
                    return payload;
                }
            }
            catch (Exception ex)
            {
                // file not found etc.
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Error loading payload");
            }
            return null;
        }

        private string GetTemplateName()
        {
            // returns the filename of the relevant payload template
            switch (config.effect)
            {
                case NanoleafEffect.Random:
                    return "random_template.json";
                case NanoleafEffect.Highlight:
                    return "random_template.json";
                default:
                    return "random_template.json";
            }
        }

        private JArray FormatPalette(PaletteObject _palette)
        {
            // formats palette into nanoleaf api format
            JArray palette = new JArray();
            
            foreach (SwatchObject swatch in _palette.Swatches)
            {
                if (swatch == null) { continue; }

                palette.Add(FormatColour(swatch));
            }

            return palette;
        }

        private JObject FormatColour(SwatchObject _swatch)
        {
            JObject colour = new JObject();
            colour["hue"] = _swatch.h;
            colour["saturation"] = _swatch.s * 100;
            colour["brightness"] = _swatch.l * 100;
            return colour;
        }

        private object GetStatePayload(bool _state)
        {
            return new
            {
                on = new
                {
                    value = _state
                }
            };
        }
    }
}
