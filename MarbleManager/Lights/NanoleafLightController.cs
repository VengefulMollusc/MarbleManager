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
        bool onlyUseMainSwatches;

        static string baseUrl = "http://<nanoleafIp>:16021";

        public NanoleafLightController(GlobalConfigObject _config)
        {
            SetConfig(_config);
        }

        /**
         * Applies a colour palette to the light
         */
        public async Task ApplyPalette(PaletteObject _palette, bool _turnOn = false)
        {
            if (!config.applyPalette)
            {
                Console.WriteLine("Nanoleaf palette support is disabled");
                // turn on if needed
                if (_turnOn)
                    await SetOnOffState(true);
                return;
            }

            // only send palettes to lights that are ON if _turnOn = false
            List<NanoleafConfig.Light> lightsToApply = _turnOn ? config.lights : await GetOnLightUrls();
            if (lightsToApply.Count <= 0)
            {
                LogManager.WriteLog("Nanoleaf: No valid lights to apply palette");
                return;
            }

            // get payload template based on config setting
            JObject payload = GetPayloadTemplate();

            // insert palette into payload
            JArray palette = FormatPalette(_palette);
            if (palette == null || palette.Count <= 0)
            {
                LogManager.WriteLog("nanoleaf palette should not be empty");
                return;
            }

            payload["write"]["palette"] = palette;

            // send payloads
            List<Task> tasks = new List<Task>();
            foreach (NanoleafConfig.Light light in lightsToApply)
            {
                tasks.Add(SendPayload(light, payload, "/effects"));
            }
            await Task.WhenAll(tasks);
            LogManager.WriteLog($"Nanoleaf lights synced", string.Join(",", lightsToApply.Select(l => l.ipAddress).ToList()));
        }

        /**
         * Sets the config object for the light
         */
        public void SetConfig(GlobalConfigObject _config)
        {
            config = _config.nanoleafConfig;
            onlyUseMainSwatches = _config.generalConfig.onlyUseMainSwatches;
        }

        /**
         * Turns the light on/off 
         */
        public async Task SetOnOffState(bool _state)
        {
            List<Task> tasks = new List<Task>();
            foreach (NanoleafConfig.Light light in config.lights)
            {
                tasks.Add(SendPayload(light, GetStatePayload(_state), "/state"));
            }
            await Task.WhenAll(tasks);
            LogManager.WriteLog($"Nanoleaf lights {(_state ? "on" : "off")}", string.Join(",", config.lights.Select(l => l.ipAddress).ToList()));
        }

        /**
         * Sends a payload to a given nanoleaf api endpoint
         */
        private async Task SendPayload(NanoleafConfig.Light _light, object _payload, string _endpoint)
        {
            if (_light == null || _light.ipAddress == null || _light.apiKey == null)
            {
                throw new Exception("Nanoleaf light data missing");
            }
            if (_payload == null)
            {
                throw new Exception("null payload");
            }

            using (HttpClient client = new HttpClient())
            {
                // set timeout
                client.Timeout = TimeSpan.FromSeconds(5);

                // set URL
                client.BaseAddress = new Uri(baseUrl.Replace("<nanoleafIp>", _light.ipAddress));

                // create payload string
                string jsonPayload = JsonConvert.SerializeObject(_payload);
                StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                bool success = false;

                for (int attempt = 1; attempt <= GlobalLightController.RetryCount; attempt++)
                {
                    try
                    {
                        // send the request
                        HttpResponseMessage response = await client.PutAsync($"api/v1/{_light.apiKey}{_endpoint}", content);

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read and process the response content (if any)
                            string responseContent = await response.Content.ReadAsStringAsync();
                            success = true;
                            break;
                        }
                        else
                        {
                            LogManager.WriteLog("Nanoleaf Error", response.StatusCode.ToString());
                        }
                    }
                    catch
                    {
                        LogManager.WriteLog("Nanoleaf timeout", _light.ipAddress);
                    }
                    // wait half second before retrying
                    await Task.Delay(GlobalLightController.RetryDelay);
                }

                if (!success)
                {
                    LogManager.WriteLog("Nanoleaf failed", "command failed after retrying.");
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
                    return payload;
                }
            }
            catch (Exception ex)
            {
                // file not found etc.
                LogManager.WriteLog("Error loading payload", ex.Message);
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

            bool overrideProb = config.overrideDominantColourProb;
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
            colour["probability"] = overrideProb ? config.dominantColourProb : (int)Math.Round(_swatch.proportion * 100f, 0, MidpointRounding.AwayFromZero);
            return colour;
        }

        /**
         * Gets a list of urls of lights that are ON
         * 
         * used for only triggering palette updates on lights that are already on
         */
        private async Task<List<NanoleafConfig.Light>> GetOnLightUrls()
        {
            // check all lights
            List<Task<NanoleafConfig.Light>> tasks = new List<Task<NanoleafConfig.Light>>();
            foreach (NanoleafConfig.Light light in config.lights)
            {
                tasks.Add(IsLightOn(light));
            }

            await Task.WhenAll(tasks);

            // return list of lights that are ON
            List<NanoleafConfig.Light> onLights = new List<NanoleafConfig.Light>();
            foreach (var task in tasks)
            {
                if (task.Result != null)
                {
                    onLights.Add(task.Result);
                }
            }
            return onLights;
        }

        /**
         * Checks if the light at the given URL is ON
         * 
         * returns the _baseUrl if the light is on
         * null if not
         */
        private async Task<NanoleafConfig.Light> IsLightOn(NanoleafConfig.Light _light)
        {
            using (HttpClient client = new HttpClient())
            {
                // set timeout
                client.Timeout = TimeSpan.FromSeconds(5);

                // set URL
                client.BaseAddress = new Uri(baseUrl.Replace("<nanoleafIp>", _light.ipAddress));

                // send the request
                HttpResponseMessage response = await client.GetAsync($"api/v1/{_light.apiKey}/state/on");

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read and process the response content (if any)
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if (responseContent == @"{""value"":true}")
                    {
                        return _light;
                    }
                }
                else
                {
                    LogManager.WriteLog($"Nanoleaf isOn Error", $"{_light.ipAddress} : {response.StatusCode}");
                }
            }
            return null;
        }

        /**
         * Returns a simple on/off state object for turning lights on/off
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
