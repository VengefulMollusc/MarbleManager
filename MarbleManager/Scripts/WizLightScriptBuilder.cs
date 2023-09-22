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
        public List<string> GetLightOnOffCommands(bool _lightOn, ConfigObject _configObject)
        {
            Console.WriteLine("WizLightScriptBuilder not set up");
            return new List<string>();
        }
    }
}
