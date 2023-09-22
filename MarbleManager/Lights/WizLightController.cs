using MarbleManager.Colours;
using MarbleManager.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    internal class WizLightController : ILightController
    {
        public WizLightController(ConfigObject _config)
        {
            SetConfig(_config);
        }

        public Task ApplyPalette(PaletteObject _palette)
        {
            // do nothing for now
            return null;
        }

        public void SetConfig(ConfigObject _config)
        {
            // do nothing for now
        }

        public Task SetOnOffState(bool _state)
        {
            string jsonCommand = JsonConvert.SerializeObject(GetStatePayload(_state));
            byte[] data = Encoding.UTF8.GetBytes(jsonCommand);
            string ipAddress = "192.168.68.83";
            int port = 38899;

            using (UdpClient udpClient = new UdpClient())
            {
                try
                {
                    udpClient.Send(data, data.Length, ipAddress, port);
                    Console.WriteLine("UDP command sent successfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error sending UDP command: {e.Message}");
                }
            }
            return null;
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
