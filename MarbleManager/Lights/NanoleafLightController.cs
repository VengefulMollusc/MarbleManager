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
using static System.Windows.Forms.AxHost;

namespace MarbleManager.Lights
{
    internal class NanoleafLightController : ILightController
    {
        NanoleafConfig config;
        bool onlyUseMainSwatches;

        static string baseUrl = "http://<nanoleafIp>:16021";
        List<string> populatedUrls;

        public NanoleafLightController(ConfigObject _config)
        {
            SetConfig(_config);
        }

        /**
         * Applies a colour palette to the light
         */
        public void ApplyPalette(PaletteObject _palette)
        {
            if (populatedUrls == null)
            {
                Console.WriteLine("nanoleaf urls not set up");
                throw new Exception("nanoleaf urls not set up");
            }

            // get payload template based on config setting
            JObject payload = GetPayloadTemplate();

            // insert palette into payload
            JArray palette = FormatPalette(_palette);
            if (palette == null || palette.Count <= 0)
            {
                Console.WriteLine("nanoleaf palette should not be empty");
                return;
            }

            payload["write"]["palette"] = palette;

            // send payload
            foreach (string url in populatedUrls)
            {
                SendPayload(url, payload, "/effects");
            }
        }

        /**
         * Sets the config object for the light
         */
        public void SetConfig(ConfigObject _config)
        {
            config = _config.nanoleafConfig;
            onlyUseMainSwatches = _config.generalConfig.onlyUseMainSwatches;

            // populate urls with ip addresses
            populatedUrls = config.LightIps.Select(ip => baseUrl.Replace("<nanoleafIp>", ip)).ToList(); ;
        }

        /**
         * Turns the light on/off 
         */
        public void SetOnOffState(bool _state)
        {
            if (populatedUrls == null)
            {
                Console.WriteLine("nanoleaf urls not set up");
                throw new Exception("nanoleaf urls not set up");
            }

            foreach(string url in populatedUrls)
            {
                SendPayload(url, GetStatePayload(_state), "/state");
            }
        }

        /**
         * Sends a payload to a given nanoleaf api endpoint
         */
        private async void SendPayload(string _baseUrl, object _payload, string _endpoint)
        {
            if (_payload == null)
            {
                throw new Exception("null payload");
            }

            using (HttpClient client = new HttpClient())
            {
                // set URL
                client.BaseAddress = new Uri(_baseUrl);

                // create payload string
                string jsonPayload = JsonConvert.SerializeObject(_payload);
                StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // send the request
                HttpResponseMessage response = await client.PutAsync($"api/v1/{config.apiKey}{_endpoint}", content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read and process the response content (if any)
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Nanoleaf Success: {responseContent}");
                }
                else
                {
                    Console.WriteLine($"Nanoleaf Error: {response.StatusCode}");
                }
            }
        }

        /**
         * Gets the API payload template object from file
         */
        private JObject GetPayloadTemplate()
        {
            try
            {
                string filePath = Path.Combine(PathManager.NanoleafEffectTemplateDir, GetTemplateName());
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

        /**
         * Returns the template file name according to the current effect setting
         */
        private string GetTemplateName()
        {
            // returns the filename of the relevant payload template
            switch (config.effect)
            {
                case NanoleafEffect.Random:
                    return "random_template.json";
                case NanoleafEffect.Highlight:
                    return "highlight_template.json";
                default:
                    return "random_template.json";
            }
        }

        /**
         * Creates a JSON array of colours in the Nanoleaf API format
         */
        private JArray FormatPalette(PaletteObject _palette)
        {
            // formats palette into nanoleaf api format
            JArray palette = new JArray();

            bool overrideProb = config.overrideMainColourProb;
            foreach (SwatchObject swatch in (onlyUseMainSwatches ? _palette.MainSwatches : _palette.AllSwatches))
            {
                if (swatch == null) { continue; }

                palette.Add(FormatColour(swatch, overrideProb));
                
                if (overrideProb) overrideProb = false;
            }

            return palette;
        }

        /**
         * Formats a swatch into the right JSON object for Nanoleaf API
         */
        private JObject FormatColour(SwatchObject _swatch, bool overrideProb)
        {
            JObject colour = new JObject();
            colour["hue"] = _swatch.h;
            colour["saturation"] = _swatch.s;
            colour["brightness"] = _swatch.l;
            colour["probability"] = overrideProb ? config.mainColourProb : (int)Math.Round(_swatch.proportion * 100f, 0, MidpointRounding.AwayFromZero);
            return colour;
        }

        /**
         * Returns a simple on/off state object
         */
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
