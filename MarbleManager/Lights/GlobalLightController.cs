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
        ILightController[] lightControllers;
        WallpaperWatcher watcher;

        internal GlobalLightController() {
            ConfigObject config = ConfigManager.GetConfig();

            UpdateConfig(config);
        }

        /**
         * Triggers the lights to turn on or off
         */
        internal async void TurnLightsOnOff(bool _state)
        {
            List<Task> tasks = new List<Task>();
            foreach (var lightController in lightControllers)
            {
                Task task = lightController.SetOnOffState(_state);
                if (task != null)
                {
                    tasks.Add(task);
                }
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("All lights done");
        }

        /**
         * Updates the config for each light
         */
        internal void UpdateConfig(ConfigObject _config)
        {
            // populate light controllers
            lightControllers = new ILightController[]
            {
                // in future populate this based on config 'enabled' values?
                // multiple lights of the same type should be handled by one controller?
                new LifxLightController(_config),
                new NanoleafLightController(_config),
                new WizLightController(_config),
            };

            // turn on watcher if syncing to wallpaper
            if (_config.generalConfig.syncOnWallpaperChange)
            {
                if (watcher == null)
                {
                    Console.WriteLine("Enabling wallpaper watcher");
                    watcher = new WallpaperWatcher();
                    watcher.OnChange += SyncOnWallpaperChange;

                    // trigger initial sync
                    SyncToWallpaper();
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
            Console.WriteLine("Triggering auto-sync");
            SyncToWallpaper();
        }

        /**
         * Applies a palette generated from the current wallpaper to the lights
         */
        internal async void SyncToWallpaper(Bitmap _toSync = null)
        {
            // Select image to sync
            Bitmap image = _toSync != null ? _toSync : WallpaperManager.GetWallpaperBitmap();

            // generate palette
            PaletteObject palette = PaletteManager.GetPaletteFromBitmap(image);
            // save to file
            PaletteManager.SavePalette(palette);

            // apply to lights
            List<Task> tasks = new List<Task>();
            foreach (var lightController in lightControllers)
            {
                Task task = lightController.ApplyPalette(palette);
                if (task != null)
                {
                    tasks.Add(task);
                }
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("All lights done");

            // dispose to free up memory
            image.Dispose();
        }
    }
}
