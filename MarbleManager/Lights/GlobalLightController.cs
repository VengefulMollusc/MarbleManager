using MarbleManager.Colours;
using MarbleManager.Config;
using System;
using System.Drawing;
using System.IO;

namespace MarbleManager.Lights
{
    internal class GlobalLightController
    {
        ILightController[] lightControllers;
        WallpaperWatcher watcher;

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
        internal void UpdateConfig(ConfigObject _config)
        {
            // populate light controllers
            lightControllers = new ILightController[]
            {
                // in future populate this based on config values?
                // multiple lights of the same type should be handled by one controller?
                new LifxLightController(_config),
                new NanoleafLightController(_config),
            };

            // turn on watcher if syncing to wallpaper
            if (_config.generalConfig.syncOnWallpaperChange)
            {
                if (watcher == null)
                {
                    Console.WriteLine("Enabling wallpaper watcher");
                    watcher = new WallpaperWatcher();
                    watcher.OnChange += SyncOnWallpaperChange;
                }
            } else if (watcher != null)
            {
                Console.WriteLine("Disabling wallpaper watcher");
                watcher.OnChange -= SyncOnWallpaperChange;
                watcher.Dispose();
                watcher = null;
            }
        }

        private void SyncOnWallpaperChange(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("Changed!");
            //SyncToWallpaper();
        }

        /**
         * Applies a palette generated from the current wallpaper to the lights
         * 
         * NOTE: No calls to Console.WriteLine can be used in this due to cross-thread issues when auto syncing
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
