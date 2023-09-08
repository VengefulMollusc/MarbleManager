using MarbleManager.Colours;
using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    internal class LifxLightController : ILightController
    {
        LifxConfig config;

        public void ApplyPalette(PaletteObject palette)
        {
            // no palette support (yet?)
            return;
        }

        public void SetConfig(ConfigObject config)
        {
            this.config = config.lifxConfig;
        }

        public void TurnOff()
        {
            throw new NotImplementedException();
        }

        public void TurnOn()
        {
            throw new NotImplementedException();
        }
    }
}
