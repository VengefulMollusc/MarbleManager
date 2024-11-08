﻿using MarbleManager.Colours;
using MarbleManager.Config;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    /**
     * Controls lights, including on/off commands and syncing palettes
     * Also handles updating configs for light controllers
     */
    internal sealed class GlobalLightController
    {
        // locked instance to use as a singleton
        private static GlobalLightController _instance = null;
        private static readonly object _lock = new object();

        // individual light controllers for enabled lights
        List<ILightController> lightControllers;
        WallpaperWatcher watcher;
        bool isSyncing;
        bool syncOnWallpaperChange;

        // retry variables for failed calls
        internal static int RetryCount = 2;
        internal static int RetryDelay = 500;

        private GlobalLightController(bool _fullBoot, bool _turnOnSync) {
            GlobalConfigObject config = ConfigManager.GetConfig();
            UpdateConfig(config, _fullBoot, _turnOnSync);
        }

        internal static GlobalLightController GetInstance(bool _fullBoot, bool _turnOnSync)
        {
            // double check lock for thread safety
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GlobalLightController(_fullBoot, _turnOnSync);
                    }
                }
            }
            return _instance;
        }

        /**
         * Triggers the lights to turn on or off
         */
        internal async Task TurnLightsOnOff(bool _state)
        {
            bool syncOn = _state && syncOnWallpaperChange;
            LogManager.WriteLog($"Lights: Turning {(_state ? "ON" : "OFF")}... {(syncOn ? "and syncing" : "")}");
            if (syncOn)
            {
                await SyncToWallpaper(null, true);
            } else
            {
                List<Task> tasks = new List<Task>();
                foreach (ILightController lightController in lightControllers)
                {
                    tasks.Add(lightController.SetOnOffState(_state));
                }
                await Task.WhenAll(tasks);
            }
            LogManager.WriteLog($"Lights: Turning {(_state ? "ON" : "OFF")} done.");
        }

        /**
         * Updates the config for each light
         */
        internal async void UpdateConfig(GlobalConfigObject _config, bool _canEnableWatcher = true, bool _turnOnSync = true)
        {
            // set local variables
            syncOnWallpaperChange = _config.generalConfig.syncOnWallpaperChange;

            LogManager.WriteLog("GlobalLightController updating config. Can start watcher: " + _canEnableWatcher);
            // populate light controllers based on enabled lights
            lightControllers = new List<ILightController>();
            if (_config.lifxConfig.enabled)
                lightControllers.Add(new LifxLightController(_config));
            if (_config.nanoleafConfig.enabled)
                lightControllers.Add(new NanoleafLightController(_config));
            if (_config.wizConfig.enabled)
                lightControllers.Add(new WizLightController(_config));
            if (_config.picoConfig.enabled)
                lightControllers.Add(new PicoLightController(_config));

            // turn on watcher if syncing to wallpaper
            if (syncOnWallpaperChange && _canEnableWatcher)
            {
                if (watcher == null)
                {
                    LogManager.WriteLog("Wallpaper watcher: Starting");
                    watcher = new WallpaperWatcher();
                    watcher.OnChange += SyncOnWallpaperChange;

                    if (_turnOnSync)
                    {
                        // trigger initial sync
                        await SyncToWallpaper();
                    }
                }
            } else if (watcher != null)
            {
                LogManager.WriteLog("Wallpaper watcher: Stopping");
                watcher.OnChange -= SyncOnWallpaperChange;
                watcher.Dispose();
                watcher = null;
            }
        }

        /**
         * Method triggered by WallpaperWatcher to begin a sync to a new wallpaper
         */
        private async void SyncOnWallpaperChange(object source, FileSystemEventArgs e)
        {
            LogManager.WriteLog("Wallpaper watcher: change triggered auto-sync");
            await SyncToWallpaper();
        }

        /**
         * Triggers a wallpaper sync if one is not already occuring
         */
        internal async Task SyncToWallpaper(Bitmap _toSync = null, bool _turnOn = false)
        {
            if (isSyncing)
            {
                LogManager.WriteLog("Already syncing - returning");
                return;
            }

            isSyncing = true;
            try
            {
                await PerformSyncToWallpaper(_toSync != null ? _toSync : WallpaperManager.GetWallpaperBitmap(), _turnOn);
                await Task.Delay(1500); // delay to avoid double-sync
            }
            finally
            {
                isSyncing = false;
            }
        }

        /**
         * Applies a palette generated from the current wallpaper to the lights
         */
        private async Task PerformSyncToWallpaper(Bitmap _toSync, bool _turnOn)
        {
            LogManager.WriteLog($"Lights: syncing {(_turnOn ? "and turning on" : "")}");
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

            LogManager.WriteLog($"Lights: synced {(_turnOn ? "and turned on" : "")}");

            // dispose to free up memory
            _toSync.Dispose();
        }
    }
}
