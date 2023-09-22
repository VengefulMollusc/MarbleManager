using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Scripts
{
    internal class LifxLightScriptBuilder : ILightScriptBuilder
    {
        private string commandTemplate = "curl -X PUT \"https://api.lifx.com/v1/lights/<lifxSelector>/state\" -H \"Authorization: Bearer <lifxAuthKey>\" -d \"power=<lightState>\" --ssl-no-revoke";

        public List<string> GetLightOnOffCommands(bool _lightOn, ConfigObject _configObject)
        {
            // setup base command
            Dictionary<string, string> baseValues = new Dictionary<string, string>()
            {
                { "<lightState>", _lightOn ? "on" : "off" },
                { "<lifxAuthKey>", _configObject.lifxConfig.authKey }
            };

            // setup base command
            string baseCommand = Utilities.ReplaceValues(commandTemplate, baseValues);

            // repeat command for each light listed in selector list
            List<string> commands = new List<string>();
            foreach (string selector in _configObject.lifxConfig.SelectorList)
            {
                commands.Add(baseCommand.Replace("<lifxSelector>", selector));
            }
            return commands;
        }
    }
}
