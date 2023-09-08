using MarbleManager.Colours;
using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Lights
{
    internal interface ILightController
    {
        void SetConfig(ConfigObject config);
        void TurnOn();
        void TurnOff();
        void ApplyPalette(PaletteObject palette);
    }
}
