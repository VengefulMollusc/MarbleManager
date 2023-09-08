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
            // populate light controllers
            lightControllers = new ILightController[]
            {
                new NanoleafLightController(),
                new LifxLightController(),
            };
        }
    }
}
