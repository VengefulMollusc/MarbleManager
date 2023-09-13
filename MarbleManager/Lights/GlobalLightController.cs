using MarbleManager.Colours;
using MarbleManager.Config;
using Microsoft.Win32;
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

            // if setting for auto lights is on, trigger to turn on
            // this should happen on app boot
            // NEED ANOTHER APPROACH FOR THIS, App boot is too late
            //if (config.generalConfig.autoTurnOnOff) TurnLightsOnOff(true);
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

            //// hook up lights to logoff event
            //if (config.generalConfig.autoTurnLightsOnOff)
            //{
            //    SystemEvents.SessionEnding += OnSessionEnding;
            //} else
            //{
            //    SystemEvents.SessionEnding -= OnSessionEnding;
            //}
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

        ///**
        // * Triggers lights off on session ending
        // */
        //private void OnSessionEnding(object sender, SessionEndingEventArgs e)
        //{
        //    Console.WriteLine("User is logging off: triggering lights off");
        //    // turn off lights
        //    TurnLightsOnOff(false);

        //    // unsubscribe from the event
        //    SystemEvents.SessionEnding -= OnSessionEnding;
        //}
    }
}
