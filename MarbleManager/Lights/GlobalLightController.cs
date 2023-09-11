using MarbleManager.Colours;
using MarbleManager.Config;
using PaletteSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace MarbleManager.Lights
{
    internal class GlobalLightController
    {
        ILightController[] lightControllers;

        public GlobalLightController() {
            ConfigObject config = ConfigManager.GetConfig();

            UpdateConfig(config);
        }

        internal void TurnLightsOnOff(bool _state)
        {
            foreach (var lightController in lightControllers)
            {
                lightController.SetOnOffState(_state);
            }
        }

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
