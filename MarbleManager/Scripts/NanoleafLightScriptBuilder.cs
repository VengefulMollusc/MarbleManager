using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Scripts
{
    internal class NanoleafLightScriptBuilder : LightScriptBuilder
    {
        private string commandTemplate = "curl -X PUT -H \"Content-Type: application/json\" -d \"{\\\"on\\\":{\\\"value\\\":<lightState>}}\" http://<nanoleafIp>:16021/api/v1/<nanoleafApiKey>/state";

        internal override List<string> GetLightOnOffCommands(bool _lightOn, ConfigObject _configObject)
        {
            // setup base command
            Dictionary<string, string> baseValues = new Dictionary<string, string>()
            {
                { "<lightState>", _lightOn ? "true" : "false" },
                { "<nanoleafApiKey>", _configObject.nanoleafConfig.apiKey }
            };
            return FormatCommands(commandTemplate, baseValues, "<nanoleafIp>", _configObject.nanoleafConfig.LightIps);
        }
    }
}
