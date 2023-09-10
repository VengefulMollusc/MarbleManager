﻿using MarbleManager.Colours;
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
    internal class LifxLightController : ILightController
    {
        LifxConfig config;

        static string baseUrl = "https://api.lifx.com";

        public LifxLightController(ConfigObject _config)
        {
            SetConfig(_config);
        }

        public void ApplyPalette(PaletteObject _palette)
        {
            // no palette support (yet?)
            return;
        }

        public void SetConfig(ConfigObject _config)
        {
            config = _config.lifxConfig;
        }

        public void TurnOnOff(bool _state)
        {
            SendPayload(
                "power=" + (_state ? "on" : "off"), 
                "v1/lights/" + config.selector + "/state"
            );
        }

        private async void SendPayload(string payload, string endpoint)
        {
            if (payload == null)
            {
                throw new Exception("null payload");
            }

            using (HttpClient client = new HttpClient())
            {
                // set URL
                client.BaseAddress = new Uri(baseUrl);

                // set headers
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + config.authKey);

                // create payload content
                StringContent content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

                // send the request
                HttpResponseMessage response = await client.PutAsync(endpoint, content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read and process the response content (if any)
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Lifx Success: " + responseContent);
                }
                else
                {
                    Console.WriteLine("Lifx Error: " + response.StatusCode);
                }
            }
        }
    }
}