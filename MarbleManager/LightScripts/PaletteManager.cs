using MarbleManager.Config;
using Newtonsoft.Json;
using PaletteSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MarbleManager.LightScripts
{
    internal static class PaletteManager
    {
        static string paletteFilePath = "output\\palette.json";

        public static PaletteObject LoadPalette ()
        {
            try
            {
                // load file here if exists
                using (StreamReader r = new StreamReader(paletteFilePath))
                {
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<PaletteObject>(json);
                }
            }
            catch (Exception ex)
            {
                // file not found etc.
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static void SavePalette(Palette palette)
        {
            SavePalette(ConvertToPaletteObject(palette));
        }

        public static void SavePalette(PaletteObject palette)
        {
            // save to file
            try
            {
                string jsonString = JsonConvert.SerializeObject(palette, Formatting.Indented);
                using (StreamWriter outputFile = new StreamWriter(paletteFilePath))
                {
                    outputFile.WriteLine(jsonString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static PaletteObject GetPaletteFromBitmap (Bitmap image)
        {
            if (image == null) { return null; }

            Palette palette = Palette.From(image).Generate();
            palette.Generate();
            return ConvertToPaletteObject(palette);
        }

        private static PaletteObject ConvertToPaletteObject (Palette palette)
        {
            if (palette == null) { return null; }

            return new PaletteObject()
            {
                dominant = ConvertToSwatchObject(palette.GetDominantSwatch()),
                vibrant = ConvertToSwatchObject(palette.GetVibrantSwatch()),
                lightVibrant = ConvertToSwatchObject(palette.GetLightVibrantSwatch()),
                darkVibrant = ConvertToSwatchObject(palette.GetDarkVibrantSwatch()),
                muted = ConvertToSwatchObject(palette.GetMutedSwatch()),
                lightMuted = ConvertToSwatchObject(palette.GetLightMutedSwatch()),
                darkMuted = ConvertToSwatchObject(palette.GetDarkMutedSwatch()),
            };
        }

        private static SwatchObject ConvertToSwatchObject(Swatch swatch)
        {
            if (swatch == null) { return null; }

            Color rgb = swatch.GetArgb();
            return new SwatchObject()
            {
                population = swatch.GetPopulation(),
                r = rgb.R,
                g = rgb.G,
                b = rgb.B,
                hsl = swatch.GetHsl(),
            };
        }
    }
}
