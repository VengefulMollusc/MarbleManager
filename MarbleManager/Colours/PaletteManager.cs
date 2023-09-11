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

namespace MarbleManager.Colours
{
    internal static class PaletteManager
    {
        static string paletteFilePath = "output\\palette.json";

        internal static PaletteObject LoadPalette ()
        {
            try
            {
                // load file here if exists
                using (StreamReader r = new StreamReader(paletteFilePath))
                {
                    string json = r.ReadToEnd();
                    PaletteObject palette = JsonConvert.DeserializeObject<PaletteObject>(json);
                    Console.WriteLine("Palette file loaded");
                    return palette;
                }
            }
            catch (Exception ex)
            {
                // file not found etc.
                Console.WriteLine(ex.ToString());
                Console.WriteLine("No palette file found to load");
                return null;
            }
        }

        internal static void SavePalette(PaletteObject palette)
        {
            // save to file
            try
            {
                string jsonString = JsonConvert.SerializeObject(palette, Formatting.Indented);
                using (StreamWriter outputFile = new StreamWriter(paletteFilePath))
                {
                    outputFile.WriteLine(jsonString);
                }
                Console.WriteLine("Palette saved");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Error saving palette");
            }
        }

        internal static PaletteObject GetPaletteFromBitmap (Bitmap image)
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
            float[] hsl = swatch.GetHsl();
            return new SwatchObject()
            {
                population = swatch.GetPopulation(),
                r = rgb.R,
                g = rgb.G,
                b = rgb.B,
                h = hsl[0],
                s = hsl[1],
                l = hsl[2]
            };
        }
    }
}
