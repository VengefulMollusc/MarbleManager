using MarbleManager.Colours;
using MarbleManager.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    internal class WizLightController : ILightController
    {
        WizConfig config;

        private static int port = 38899;

        public WizLightController(GlobalConfigObject _config)
        {
            SetConfig(_config);
        }

        public async Task ApplyPalette(PaletteObject _palette, bool _turnOn = false)
        {
            if (!config.applyPalette)
            {
                Console.WriteLine("Wiz palette support is disabled");
                // turn on if needed
                if (_turnOn)
                    await SetOnOffState(true);
                return;
            }

            // select swatch
            SwatchObject toSend = _palette.Highlight;
            await SendPayloadToLights(CreateSetColourPayload(toSend), _turnOn);
            LogManager.WriteLog("Wiz lights synced");
        }

        public void SetConfig(GlobalConfigObject _config)
        {
            config = _config.wizConfig;
        }

        public async Task SetOnOffState(bool _state)
        {
            await SendPayloadToLights(CreateSetStatePayload(_state));
            LogManager.WriteLog($"Wiz lights {(_state ? "on" : "off")}");
        }

        /**
         * Sends a payload to all the lights
         */
        private async Task SendPayloadToLights(JObject _payload, bool _sendToAll = true)
        {
            List<string> ips = _sendToAll ? config.IpAddressList : await GetOnLightIps();
            List<Task> tasks = new List<Task>();
            foreach (string ip in ips)
            {
                tasks.Add(SendUdpCommand(ip, _payload));
            }
            await Task.WhenAll(tasks);
        }

        /**
         * Sends a payload to a given light ip via Udp
         */
        private async Task<string> SendUdpCommand (string _ipAddress, JObject _payload)
        {
            string jsonCommand = JsonConvert.SerializeObject(_payload);
            byte[] data = Encoding.UTF8.GetBytes(jsonCommand);

            using (UdpClient udpClient = new UdpClient())
            {
                bool success = false;
                string response = null;

                for (int attempt = 1;  attempt <= GlobalLightController.RetryCount;  attempt++)
                {
                    try
                    {
                        udpClient.Send(data, data.Length, _ipAddress, port);

                        // receive the response
                        UdpReceiveResult responseData = await udpClient.ReceiveAsync();

                        // extract the response string
                        response = Encoding.UTF8.GetString(responseData.Buffer);

                        success = true;
                        break;
                    }
                    catch (Exception e)
                    {
                        LogManager.WriteLog("Wiz error", $"Error sending UDP command: {e.Message}");
                    }
                    // wait half second before retrying
                    await Task.Delay(GlobalLightController.RetryDelay);
                }

                if (!success)
                {
                    LogManager.WriteLog("Wiz failed", "UDP command failed after retrying.");
                }

                return response;
            }
        }

        /**
         * returns a list of light ips that are on
         */
        private async Task<List<string>> GetOnLightIps()
        {
            // check all lights
            List<Task<string>> tasks = new List<Task<string>>();
            JObject payload = CreateGetStatePayload();
            foreach (string ip in config.IpAddressList)
            {
                tasks.Add(IsLightOn(ip, payload));
            }

            await Task.WhenAll(tasks);

            // return list of lights that are ON
            List<string> onLights = new List<string>();
            foreach (var task in tasks)
            {
                if (task.Result != null)
                {
                    onLights.Add(task.Result);
                }
            }
            return onLights;
        }

        private async Task<string> IsLightOn(string _ip, JObject _payload)
        {
            string response = await SendUdpCommand(_ip, _payload);

            if (response == null)
                return null;

            ResponseObject responseObj = JsonConvert.DeserializeObject<ResponseObject>(response);

            if (responseObj != null && responseObj.result.state)
            {
                // light is on
                return _ip;
            }

            return null;
        }

        /**
         * Returns a payload object for setting the light colour
         */
        private JObject CreateSetColourPayload(SwatchObject _swatch)
        {
            JObject parameters = new JObject();
            parameters["r"] = _swatch.r;
            parameters["g"] = _swatch.g;
            parameters["b"] = _swatch.b;
            parameters["dimming"] = 100; // test this?? maybe just a constant

            JObject payload = new JObject();
            payload["Id"] = 1;
            payload["method"] = "setPilot";
            payload["params"] = parameters;

            return payload;
        }

        /**
         * Returns a simple on/off state object for turning lights on/off
         */
        private JObject CreateSetStatePayload(bool _state)
        {
            JObject parameters = new JObject();
            parameters["state"] = _state;

            JObject payload = new JObject();
            payload["Id"] = 1;
            payload["method"] = "setState";
            payload["params"] = parameters;

            return payload;
        }

        /**
         * Returns a payload for getting the light state
         */
        private JObject CreateGetStatePayload()
        {
            JObject parameters = new JObject();
            JObject payload = new JObject();
            payload["method"] = "getPilot";
            payload["params"] = parameters;

            return payload;
        }

        private class ResponseObject
        {
            public string method { get; set; }
            public string env { get; set; }
            public ResponseResultObject result { get; set; }

        }

        private class ResponseResultObject
        {
            public string mac { get; set; }
            public int rssi { get; set; }
            public string src { get; set; }
            public bool state { get; set; }
            public int sceneId { get; set; }
            public int r { get; set; }
            public int g { get; set; }
            public int b { get; set; }
            public int c { get; set; }
            public int w { get; set; }
            public int dimming { get; set; }
        }
    }
}
