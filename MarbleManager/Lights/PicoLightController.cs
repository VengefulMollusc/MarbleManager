using MarbleManager.Colours;
using MarbleManager.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using static System.Windows.Forms.AxHost;
using System.Drawing;

namespace MarbleManager.Lights
{
    internal class PicoLightController : ILightController
    {
        PicoConfig config;

        public PicoLightController(GlobalConfigObject _config)
        {
            SetConfig(_config);
        }

        public async Task ApplyPalette(PaletteObject _palette, bool _turnOn = false)
        {
            if (!config.applyPalette)
            {
                Console.WriteLine("Pico palette support is disabled");
                // turn on if needed
                if (_turnOn)
                    await SetOnOffState(true);
                return;
            }

            // select swatch
            Dictionary<string, string> paletteQuery = GetPaletteQueryDict(_palette);
            //if (_turnOn)
            //{
                paletteQuery.Add("brightness", $"{config.brightness}");
            //}
            await SendCommandToLights(BuildQueryString(paletteQuery));
            LogManager.WriteLog("Pico lights synced");
        }

        public void SetConfig(GlobalConfigObject _config)
        {
            config = _config.picoConfig;
        }

        public async Task SetOnOffState(bool _state)
        {
            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            if (_state)
            {
                // if turning on, set brightness rather than state
                paramDict.Add("brightness", $"{config.brightness}");
            } else
            {
                paramDict.Add("state", "off");
            }
            string commandParams = BuildQueryString(paramDict);
            await SendCommandToLights(commandParams);
            LogManager.WriteLog($"Pico lights {(_state ? "on" : "off")}");
        }

        /**
         * Sends a payload to all the lights
         */
        private async Task SendCommandToLights(string _params, bool _sendToAll = true)
        {
            //List<string> ips = _sendToAll ? config.IpAddressList : await GetOnLightIps();
            List<string> ips = config.IpAddressList;
            List<Task> tasks = new List<Task>();
            foreach (string ip in ips)
            {
                tasks.Add(SendHTTPCommand(ip, _params));
            }
            await Task.WhenAll(tasks);
        }

        /**
         * Sends an http request with the given parameters to the pico
         */
        private static async Task<string> SendHTTPCommand(string _ip, string _params = "")
        {
            HttpClient client = new HttpClient();

            string requestUrl = $"http://{_ip}?{_params}";

            bool success = false;
            string responseString = null;

            for (int attempt = 1; attempt <= GlobalLightController.RetryCount; attempt++)
            {
                try
                {
                    // send the GET request
                    HttpResponseMessage response = await client.GetAsync(requestUrl);
                    response.EnsureSuccessStatusCode();

                    // read response
                    responseString = await response.Content.ReadAsStringAsync();
                    success = true;
                    break;
                }
                catch (Exception e)
                {
                    LogManager.WriteLog("Pico failed", $"{_ip} HTTP Request error: {e.Message}");
                }
                // wait half second before retrying
                await Task.Delay(GlobalLightController.RetryDelay);
            }
            if (!success)
            {
                LogManager.WriteLog("Pico failed", $"{_ip} HTTP command failed after retrying.");
            }
            return responseString;
        }

        ///**
        // * returns a list of light ips that are on
        // */
        //private async Task<List<string>> GetOnLightIps()
        //{
        //    // check all lights
        //    List<Task<string>> tasks = new List<Task<string>>();
        //    foreach (string ip in config.IpAddressList)
        //    {
        //        tasks.Add(IsLightOn(ip));
        //    }

        //    await Task.WhenAll(tasks);

        //    // return list of lights that are ON
        //    List<string> onLights = new List<string>();
        //    foreach (var task in tasks)
        //    {
        //        if (task.Result != null)
        //        {
        //            onLights.Add(task.Result);
        //        }
        //    }
        //    return onLights;
        //}

        //private async Task<string> IsLightOn(string _ip)
        //{
        //    string response = await SendHTTPCommand(_ip);

        //    if (response == null)
        //        return null;

        //    ResponseObject responseObj = JsonConvert.DeserializeObject<ResponseObject>(response);

        //    if (responseObj != null && responseObj.state)
        //    {
        //        // light is on
        //        return _ip;
        //    }

        //    return null;
        //}

        /**
         * constructs a string of query parameters for an http request
         */
        private static string BuildQueryString(Dictionary<string, string> parameters)
        {
            var paramArray = new List<string>();
            foreach (var param in parameters)
            {
                paramArray.Add($"{param.Key}={param.Value}");
            }
            return string.Join("&", paramArray);
        }

        /**
         * Converts a palette into a dict of col1... to be converted to query params
         */
        private Dictionary<string, string> GetPaletteQueryDict(PaletteObject _palette)
        {
            Dictionary<string, string> colours = new Dictionary<string, string>();
            List<SwatchObject> swatches = _palette.MainSwatches;
            for (int i = 0; i < swatches.Count; i++)
            {
                string hexCode;
                if (config.juiceColours)
                {
                    float h = swatches[i].h / 360f;
                    float s = swatches[i].s / 100f;
                    float l = swatches[i].l / 100f;

                    //float juiced_s = Utilities.Clamp01(s + 0.5f); // boost saturation
                    //float juiced_l = Utilities.Clamp01((float)Math.Pow(l * 2f, 2f) / 2f); // increase contrast

                    //Console.WriteLine($"{Math.Round(s, 2)}:{Math.Round(l, 2)} - OLD");
                    //Console.WriteLine($"{Math.Round(juiced_s, 2)}:{Math.Round(juiced_l, 2)} - NEW");

                    float juiced_s = Utilities.Map(s, 0f, 1f, 0.5f, 1f); // 3rd float here adjusts juice 'floor'

                    int r, g, b;
                    Utilities.HslToRgb(h, juiced_s, l, out r, out g, out b);
                    hexCode = Utilities.RgbToHex(r, g, b, false);
                } else
                {
                    hexCode = Utilities.RgbToHex(swatches[i].r, swatches[i].g, swatches[i].b, false);
                }
                // colours start at col1
                colours.Add($"col{i+1}", hexCode);
            }
            return colours;
        }

        //private class ResponseObject
        //{
        //    public bool state { get; set; }
        //}
    }
}
