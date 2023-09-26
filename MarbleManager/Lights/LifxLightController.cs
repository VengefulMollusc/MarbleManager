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

        /**
         * Applies a colour palette
         */
        public Task ApplyPalette(PaletteObject _palette)
        {
            if (!config.applyPalette)
            {
                Console.WriteLine("Lifx palette support is disabled");
                return Task.CompletedTask;
            }

            // no palette support (yet?)
            Console.WriteLine("No Lifx palette support");
            return Task.CompletedTask;
        }

        /**
         * sets the current config
         */
        public void SetConfig(ConfigObject _config)
        {
            config = _config.lifxConfig;
        }

        /**
         * Turns lights on or off
         */
        public async Task SetOnOffState(bool _state)
        {
            List<Task> tasks = new List<Task>();
            foreach (string selector in config.SelectorList)
            {
                tasks.Add(SendPayload(
                    $"power={(_state ? "on" : "off")}",
                    $"v1/lights/{selector}/state"
                ));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("Lifx lights done");
        }

        /**
         * Sends a payload to Lifx lights at a given endpoint
         */
        private async Task SendPayload(string payload, string endpoint)
        {
            if (payload == null)
            {
                throw new Exception("null payload");
            }

            using (HttpClient client = new HttpClient())
            {
                // set timeout
                client.Timeout = TimeSpan.FromSeconds(5);

                // set URL
                client.BaseAddress = new Uri(baseUrl);

                // set headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.authKey}");

                // create payload content
                StringContent content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded");

                try
                {
                    // send the request
                    HttpResponseMessage response = await client.PutAsync(endpoint, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and process the response content (if any)
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Lifx Success: {endpoint}");
                    }
                    else
                    {
                        Console.WriteLine($"Lifx Error: {response.StatusCode}");
                    }
                } catch
                {
                    Console.WriteLine($"Lifx Timeout: {endpoint}");
                }
            }
        }
    }
}
