using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                lightController.TurnOnOff(_state);
            }
        }

        internal void UpdateConfig(ConfigObject config)
        {
            // populate light controllers
            lightControllers = new ILightController[]
            {
                // in future populate this based on config values
                new LifxLightController(config),
                new NanoleafLightController(config),
            };
        }

        //public void SyncToWallpaper()
        //{

        //}
    }
}
