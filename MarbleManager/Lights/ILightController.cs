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
         * Sets the config for the lights
         */
        void SetConfig(ConfigObject _config);

        /**
         * Turns the lights on/off
         */
        Task SetOnOffState(bool _state);

        /**
         * Applies a colour palette to the lights
         */
        Task ApplyPalette(PaletteObject _palette, bool _turnOn = false);
    }
}
