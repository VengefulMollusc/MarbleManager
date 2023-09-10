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
        void SetConfig(ConfigObject _config);
        void TurnOnOff(bool _state);
        void ApplyPalette(PaletteObject _palette);
    }
}
