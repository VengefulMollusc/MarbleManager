using MarbleManager.Colours;
using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    internal class NanoleafLightController : ILightController
    {
        NanoleafConfig config;

        public void ApplyPalette(PaletteObject palette)
        {
            throw new NotImplementedException();
        }

        public void SetConfig(ConfigObject config)
        {
            this.config = config.nanoleafConfig;
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
