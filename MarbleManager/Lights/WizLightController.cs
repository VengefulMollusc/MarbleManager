using MarbleManager.Colours;
using MarbleManager.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    internal class WizLightController : ILightController
    {
        WizConfig config;

        private static int port = 38899;

        public WizLightController(ConfigObject _config)
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

            // do nothing for now
            Console.WriteLine("No Wiz palette support");
            // turn on if needed
            if (_turnOn)
                await SetOnOffState(true);
            return;
        }

        public void SetConfig(ConfigObject _config)
        {
            config = _config.wizConfig;
        }

        public Task SetOnOffState(bool _state)
        {
            JObject payload = GetStatePayload(_state);
            foreach (string ip in config.IpAddressList)
            {
                SendPayload(ip, payload);
            }
            Console.WriteLine("Wiz lights done");
            return Task.CompletedTask;
        }

        /**
         * Sends a payload to a given light ip
         */
        private void SendPayload (string _ipAddress, JObject _payload)
        {
            string jsonCommand = JsonConvert.SerializeObject(_payload);
            byte[] data = Encoding.UTF8.GetBytes(jsonCommand);

            using (UdpClient udpClient = new UdpClient())
            {
                try
                {
                    udpClient.Send(data, data.Length, _ipAddress, port);
                    Console.WriteLine("UDP command sent successfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error sending UDP command: {e.Message}");
                }
            }
        }

        /**
         * Returns a simple on/off state object for turning lights on/off
         */
        private JObject GetStatePayload(bool _state)
        {
            JObject parameters = new JObject();
            parameters["state"] = _state;

            JObject payload = new JObject();
            payload["Id"] = 1;
            payload["method"] = "setState";
            payload["params"] = parameters;

            return payload;
        }
    }
}
