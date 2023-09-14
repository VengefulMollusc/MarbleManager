using MarbleManager.Colours;
using MarbleManager.Config;
using System.Drawing;

namespace MarbleManager.Lights
{
    internal class GlobalLightController
    {
        ILightController[] lightControllers;

        internal GlobalLightController() {
            ConfigObject config = ConfigManager.GetConfig();

            UpdateConfig(config);
        }

        /**
         * Triggers the lights to turn on or off
         */
        internal void TurnLightsOnOff(bool _state)
        {
            foreach (var lightController in lightControllers)
            {
                lightController.SetOnOffState(_state);
            }
        }

        /**
         * Updates the config for each light
         */
        internal void UpdateConfig(ConfigObject config)
        {
            // populate light controllers
            lightControllers = new ILightController[]
            {
                // in future populate this based on config values?
                // multiple lights of the same type should be handled by one controller?
                new LifxLightController(config),
                new NanoleafLightController(config),
            };
        }

        /**
         * Applies a palette generated from the current wallpaper to the lights
         */
        internal void SyncToWallpaper()
        {
            // fetch current wallpaper
            Bitmap wallpaper = WallpaperManager.GetWallpaperBitmap();

            // generate palette
            PaletteObject palette = PaletteManager.GetPaletteFromBitmap(wallpaper);
            // save to file
            PaletteManager.SavePalette(palette);

            // apply to lights
            foreach (var lightController in lightControllers)
            {
                lightController.ApplyPalette(palette);
            }

            // dispose to free up memory
            wallpaper.Dispose();
        }
    }
}
