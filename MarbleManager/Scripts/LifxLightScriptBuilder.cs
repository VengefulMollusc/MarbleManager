﻿using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Scripts
{
    internal class LifxLightScriptBuilder : LightScriptBuilder
    {
        private string commandTemplate = "curl -X PUT \"https://api.lifx.com/v1/lights/<lifxSelector>/state\" -H \"Authorization: Bearer <lifxAuthKey>\" -d \"power=<lightState>\" --ssl-no-revoke";

        internal override List<string> GetLightOnOffCommands(bool _lightOn, ConfigObject _configObject)
        {
            // setup base command
            Dictionary<string, string> baseValues = new Dictionary<string, string>()
            {
                { "<lightState>", _lightOn ? "on" : "off" },
                { "<lifxAuthKey>", _configObject.lifxConfig.authKey }
            };
            return FormatCommands(commandTemplate, baseValues, "<lifxSelector>", _configObject.lifxConfig.LightSelectors);
        }
    }
}