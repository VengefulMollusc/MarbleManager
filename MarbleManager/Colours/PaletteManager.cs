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

        internal static void SavePalette(PaletteObject _palette)
        {
            // save to file
            try
            {
                string jsonString = JsonConvert.SerializeObject(_palette, Formatting.Indented);
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

        internal static PaletteObject GetPaletteFromBitmap (Bitmap _image)
        {
            if (_image == null) { return null; }

            Palette palette = Palette.From(_image).Generate();
            palette.Generate();
            return ConvertToPaletteObject(palette);
        }

        private static PaletteObject ConvertToPaletteObject (Palette _palette)
        {
            if (_palette == null) { return null; }

            List<float> proportions = GetProportions(_palette);

            return new PaletteObject()
            {
                dominant = ConvertToSwatchObject(_palette.GetDominantSwatch(), proportions[0]),
                vibrant = ConvertToSwatchObject(_palette.GetVibrantSwatch(), proportions[1]),
                lightVibrant = ConvertToSwatchObject(_palette.GetLightVibrantSwatch(), proportions[2]),
                darkVibrant = ConvertToSwatchObject(_palette.GetDarkVibrantSwatch(), proportions[3]),
                muted = ConvertToSwatchObject(_palette.GetMutedSwatch(), proportions[4]),
                lightMuted = ConvertToSwatchObject(_palette.GetLightMutedSwatch(), proportions[5]),
                darkMuted = ConvertToSwatchObject(_palette.GetDarkMutedSwatch(), proportions[6]),
            };
        }

        private static SwatchObject ConvertToSwatchObject(Swatch _swatch, float _proportion)
        {
            if (_swatch == null) { return null; }

            Color rgb = _swatch.GetArgb();
            float[] hsl = _swatch.GetHsl();
            return new SwatchObject()
            {
                population = _swatch.GetPopulation(),
                proportion = _proportion,
                r = rgb.R,
                g = rgb.G,
                b = rgb.B,
                h = hsl[0],
                s = hsl[1],
                l = hsl[2]
            };
        }

        private static List<float> GetProportions(Palette _palette)
        {
            List<Swatch> swatches = _palette.GetSwatches();
            int totalPop = 0;

            foreach (Swatch swatch in swatches)
            {
                if (swatch != null) totalPop += swatch.GetPopulation();
            }

            List<float> proportions = new List<float>();
            foreach (Swatch swatch in swatches)
            {
                proportions.Add(swatch != null
                    ? swatch.GetPopulation() / totalPop
                    : 0);
            }

            return proportions;
        }
    }
}
