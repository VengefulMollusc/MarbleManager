using MarbleManager.Config;
using Newtonsoft.Json;
using PaletteSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MarbleManager.Colours
{
    internal static class PaletteManager
    {
        /**
         * Loads a PaletteObject from json file
         */
        internal static PaletteObject LoadPalette ()
        {
            try
            {
                // load file here if exists
                using (StreamReader r = new StreamReader(PathManager.PaletteFile))
                {
                    string json = r.ReadToEnd();
                    PaletteObject palette = JsonConvert.DeserializeObject<PaletteObject>(json);
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

        /**
         * Saves a PaletteObject to a json file
         */
        internal static void SavePalette(PaletteObject _palette)
        {
            // save to file
            try
            {
                // Create the destination directory if it doesn't exist
                Directory.CreateDirectory(PathManager.DataOutputDir);

                string jsonString = JsonConvert.SerializeObject(_palette, Formatting.Indented);
                using (StreamWriter outputFile = new StreamWriter(PathManager.PaletteFile))
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

        /**
         * Creates a PaletteObject of colours generated from an image
         */
        internal static PaletteObject GetPaletteFromBitmap (Bitmap _image)
        {
            if (_image == null) { return null; }

            Palette palette = Palette.From(_image).Generate();
            palette.Generate();
            return ConvertToPaletteObject(palette);
        }

        /**
         * Converts a PaletteSharp Palette to a local PaletteObject for easier json storage and handling etc.
         */
        private static PaletteObject ConvertToPaletteObject(Palette _palette)
        {
            if (_palette == null) { return null; }

            List<SwatchObject> mainSwatches = new List<SwatchObject>()
            {
                ConvertToSwatchObject(_palette.GetDominantSwatch()),
                ConvertToSwatchObject(_palette.GetVibrantSwatch()),
                ConvertToSwatchObject(_palette.GetLightVibrantSwatch()),
                ConvertToSwatchObject(_palette.GetDarkVibrantSwatch()),
                ConvertToSwatchObject(_palette.GetMutedSwatch()),
                ConvertToSwatchObject(_palette.GetLightMutedSwatch()),
                ConvertToSwatchObject(_palette.GetDarkMutedSwatch()),
            };
            mainSwatches = GenerateProportions(mainSwatches);

            List<SwatchObject> allSwatches = _palette.GetSwatches().Select(ConvertToSwatchObject).ToList(); ;
            allSwatches = GenerateProportions(allSwatches);

            return new PaletteObject()
            {
                dominant = mainSwatches[0],
                vibrant = mainSwatches[1],
                lightVibrant = mainSwatches[2],
                darkVibrant = mainSwatches[3],
                muted = mainSwatches[4],
                lightMuted = mainSwatches[5],
                darkMuted = mainSwatches[6],
                AllSwatches = allSwatches.OrderByDescending(x => x.population).ToList()
            };
        }

        /**
         * Converts a PaletteSharp Swatch to a local SwatchObject for easier json storage and handling etc.
         */
        private static SwatchObject ConvertToSwatchObject(Swatch _swatch)
        {
            if (_swatch == null) { return null; }

            Color rgb = _swatch.GetArgb();

            // PaletteSharp's hsl conversion appears to be WRONG
            // therefore doing own conversion from rgb here
            int h, s, l;
            Utilities.RgbToHsl(rgb, out h, out s, out l);

            return new SwatchObject()
            {
                population = _swatch.GetPopulation(),
                r = rgb.R,
                g = rgb.G,
                b = rgb.B,
                h = h,
                s = s,
                l = l
            };
        }

        /**
         * Creates a list of proportional values corresponding to the relative populations of the swatches
         */
        private static List<SwatchObject> GenerateProportions(List<SwatchObject> _swatches)
        {
            int totalPop = 0;

            foreach (SwatchObject swatch in _swatches)
            {
                if (swatch != null) totalPop += swatch.population;
            }

            if (totalPop == 0)
            {
                // there are apparently no non-null swatches
                return _swatches;
            }

            foreach (SwatchObject swatch in _swatches)
            {
                if (swatch != null) swatch.proportion = (float)swatch.population / totalPop;
            }

            return _swatches;
        }
    }
}
