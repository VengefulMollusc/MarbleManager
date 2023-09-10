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
            // populate light controllers
            lightControllers = new ILightController[]
            {
                new LifxLightController(config),
                new NanoleafLightController(config),
            };
        }

        public void TurnLightsOnOff(bool _state)
        {
            foreach (var lightController in lightControllers)
            {
                lightController.TurnOnOff(_state);
            }
        }

        //public void SyncToWallpaper()
        //{

        //}
    }
}
