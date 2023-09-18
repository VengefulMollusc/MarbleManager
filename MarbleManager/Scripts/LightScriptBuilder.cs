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
    }
}
