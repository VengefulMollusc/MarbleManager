using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Scripts
{
    internal abstract class LightScriptBuilder
    {
        /**
         * Gets a string batch command turning the light on or off
         */
        internal abstract List<string> GetLightOnOffCommands(bool _lightOn, ConfigObject _configObject);

        internal List<string> FormatCommands(string _template, Dictionary<string, string> _baseValues, string _selectorVariable, List<string> _selectorValues)
        {
            // setup base command
            string baseCommand = Utilities.ReplaceValues(_template, _baseValues);

            // repeat command for each light listed in selector list
            List<string> commands = new List<string>();
            foreach (string selector in _selectorValues)
            {
                commands.Add(baseCommand.Replace(_selectorVariable, selector));
            }
            return commands;
        }
    }
}
