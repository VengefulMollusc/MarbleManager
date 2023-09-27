using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Scripts
{
    internal class WizLightScriptBuilder : ILightScriptBuilder
    {
        private string commandTemplate = "echo {\"Id\":1,\"method\":\"setState\",\"params\":{\"state\":<lightState>}} | <ncatPath> --udp --send-only <ipAddress> 38899";

        public List<string> GetLightOnOffCommands(bool _lightOn, ConfigObject _configObject)
        {
            Dictionary<string, string> baseValues = new Dictionary<string, string>()
            {
                { "<lightState>", _lightOn ? "true" : "false" },
                { "<ncatPath>", PathManager.NcatFile }
            };

            // setup base command
            string baseCommand = Utilities.ReplaceValues(commandTemplate, baseValues);

            // repeat command for each light listed in selector list
            List<string> commands = new List<string>();
            foreach (string ip in _configObject.wizConfig.IpAddressList)
            {
                commands.Add(baseCommand.Replace("<ipAddress>", ip));
            }
            return commands;
        }
    }
}
