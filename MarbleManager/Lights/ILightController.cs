using MarbleManager.Colours;
using MarbleManager.Config;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    /**
     * Interface for individual light controller classes
     */
    internal interface ILightController
    {
        /**
         * Sets the config for the lights
         */
        void SetConfig(GlobalConfigObject _config);

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
