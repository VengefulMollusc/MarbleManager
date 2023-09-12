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
        static string paletteFileName = "palette.json";

        static string PaletteFilePath
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, ConfigManager.DataDirectory, paletteFileName);
            }
        }

        internal static PaletteObject LoadPalette ()
        {
            try
            {
                // load file here if exists
                using (StreamReader r = new StreamReader(PaletteFilePath))
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
                // Create the destination directory if it doesn't exist
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, ConfigManager.DataDirectory));

                string jsonString = JsonConvert.SerializeObject(_palette, Formatting.Indented);
                using (StreamWriter outputFile = new StreamWriter(PaletteFilePath))
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

            List<Swatch> relevantSwatches = new List<Swatch>()
            {
                _palette.GetDominantSwatch(),
                _palette.GetVibrantSwatch(),
                _palette.GetLightVibrantSwatch(),
                _palette.GetDarkVibrantSwatch(),
                _palette.GetMutedSwatch(),
                _palette.GetLightMutedSwatch(),
                _palette.GetDarkMutedSwatch(),
            };
            List<float> proportions = GetProportions(relevantSwatches);

            return new PaletteObject()
            {
                dominant = ConvertToSwatchObject(relevantSwatches[0], proportions[0]),
                vibrant = ConvertToSwatchObject(relevantSwatches[1], proportions[1]),
                lightVibrant = ConvertToSwatchObject(relevantSwatches[2], proportions[2]),
                darkVibrant = ConvertToSwatchObject(relevantSwatches[3], proportions[3]),
                muted = ConvertToSwatchObject(relevantSwatches[4], proportions[4]),
                lightMuted = ConvertToSwatchObject(relevantSwatches[5], proportions[5]),
                darkMuted = ConvertToSwatchObject(relevantSwatches[6], proportions[6]),
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

        private static List<float> GetProportions(List<Swatch> _swatches)
        {
            int totalPop = 0;

            foreach (Swatch swatch in _swatches)
            {
                if (swatch != null) totalPop += swatch.GetPopulation();
            }

            List<float> proportions = new List<float>();
            foreach (Swatch swatch in _swatches)
            {
                proportions.Add(swatch != null
                    ? (float)swatch.GetPopulation() / totalPop
                    : 0);
            }

            return proportions;
        }
    }
}
