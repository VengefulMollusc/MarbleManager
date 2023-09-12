using MarbleManager.Colours;
using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    internal interface ILightController
    {
        /**
         * Sets the config for the light
         */
        void SetConfig(ConfigObject _config);

        /**
         * Turns the light on/off
         */
        void SetOnOffState(bool _state);

        /**
         * Applies a colour palette to the light
         */
        void ApplyPalette(PaletteObject _palette);
    }
}
