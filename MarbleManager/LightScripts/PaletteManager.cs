using PaletteSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.LightScripts
{
    internal class PaletteManager
    {
        public static Palette GetPaletteFromBitmap (Bitmap image)
        {
            if (image == null) { return null; }

            Palette palette = Palette.From(image).Generate();
            palette.Generate();
            return palette;
        }
    }
}
