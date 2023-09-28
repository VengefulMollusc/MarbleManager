using MarbleManager.Colours;
using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    internal class GlobalLightController
    {
        List<ILightController> lightControllers;
        WallpaperWatcher watcher;

        internal GlobalLightController() {
            ConfigObject config = ConfigManager.GetConfig();

            UpdateConfig(config);
        }

        /**
         * Triggers the lights to turn on or off
         */
        internal async Task TurnLightsOnOff(bool _state)
        {
            List<Task> tasks = new List<Task>();
            foreach (ILightController lightController in lightControllers)
            {
                tasks.Add(lightController.SetOnOffState(_state));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("All lights done");
        }

        /**
         * Updates the config for each light
         */
        internal async void UpdateConfig(ConfigObject _config)
        {
            // populate light controllers based on enabled lights
            lightControllers = new List<ILightController>();
            if (_config.lifxConfig.enabled)
                lightControllers.Add(new LifxLightController(_config));
            if (_config.nanoleafConfig.enabled)
                lightControllers.Add(new NanoleafLightController(_config));
            if (_config.wizConfig.enabled)
                lightControllers.Add(new WizLightController(_config));

            // turn on watcher if syncing to wallpaper
            if (_config.generalConfig.syncOnWallpaperChange)
            {
                if (watcher == null)
                {
                    Console.WriteLine("Enabling wallpaper watcher");
                    watcher = new WallpaperWatcher();
                    watcher.OnChange += SyncOnWallpaperChange;

                    // trigger initial sync
                    await SyncToWallpaper();
                }
            } else if (watcher != null)
            {
                Console.WriteLine("Disabling wallpaper watcher");
                watcher.OnChange -= SyncOnWallpaperChange;
                watcher.Dispose();
                watcher = null;
            }
        }

        private async void SyncOnWallpaperChange(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("Triggering auto-sync");
            await SyncToWallpaper();
        }

        /**
         * Applies a palette generated from the current wallpaper to the lights
         */
        internal async Task SyncToWallpaper()
        {
            await SyncToWallpaper(WallpaperManager.GetWallpaperBitmap());
        }

        internal async Task TurnOnAndSyncToWallpaper()
        {
            await SyncToWallpaper(WallpaperManager.GetWallpaperBitmap(), true);
        }

        internal async Task SyncToWallpaper(Bitmap _toSync, bool _turnOn = false)
        {
            // generate palette
            PaletteObject palette = PaletteManager.GetPaletteFromBitmap(_toSync);
            // save to file
            PaletteManager.SavePalette(palette);

            // apply to lights
            List<Task> tasks = new List<Task>();
            foreach (ILightController lightController in lightControllers)
            {
                tasks.Add(lightController.ApplyPalette(palette, _turnOn));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("All lights done");

            // dispose to free up memory
            _toSync.Dispose();
        }
    }
}
