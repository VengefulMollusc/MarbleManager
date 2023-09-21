using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Scripts
{
    internal class NanoleafLightScriptBuilder : ILightScriptBuilder
    {
        private string commandTemplate = "curl -X PUT -H \"Content-Type: application/json\" -d \"{\\\"on\\\":{\\\"value\\\":<lightState>}}\" http://<nanoleafIp>:16021/api/v1/<nanoleafApiKey>/state";

        public List<string> GetLightOnOffCommands(bool _lightOn, ConfigObject _configObject)
        {
            List<string> commands = new List<string>();
            foreach (Dictionary<string, string> variables in GetCommandVariables(_configObject.nanoleafConfig.lights, _lightOn))
            {
                commands.Add(Utilities.ReplaceValues(commandTemplate, variables));
            }
            return commands;
        }

        private List<Dictionary<string, string>> GetCommandVariables(List<NanoleafConfig.Light> _lights, bool _lightOn)
        {
            List<Dictionary<string, string>> variables = new List<Dictionary<string, string>>();
            foreach (NanoleafConfig.Light light in _lights)
            {
                variables.Add(new Dictionary<string, string>()
                {
                    { "<lightState>", _lightOn ? "true" : "false" },
                    { "<nanoleafIp>", light.ipAddress },
                    { "<nanoleafApiKey>", light.apiKey }
                });
            }
            return variables;
        }
    }
}
