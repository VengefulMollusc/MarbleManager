using MarbleManager.Colours;
using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager
{
    internal static class Utilities
    {
        /**
         * Copies a file, renaming in the process.
         * Also replaces given values in the file
         * 
         * used for writing api key changes etc to static script bat files
         */
        internal static void CopyFileAndReplaceValues(string inputDir, string inputFile, string outputDir, string outputFile, Dictionary<string, string> toReplace)
        {
            try
            {
                // Read the content of the source file
                string fileContent = File.ReadAllText(inputDir + inputFile);

                // Replace <variables> with the new value
                foreach (var variable in toReplace)
                {
                    fileContent = fileContent.Replace(variable.Key, variable.Value);
                }

                // Create the destination directory if it doesn't exist
                Directory.CreateDirectory(outputDir);

                // Combine the destination directory and the file name
                string destinationFilePath = Path.Combine(outputDir, outputFile);

                // Write the modified content to the destination file
                File.WriteAllText(destinationFilePath, fileContent);

                Console.WriteLine("File " + inputFile + " copied and modified successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        /**
         * Converts RGB to HSL values
         * 
         * using own converion as PaletteSharp appears to be inaccurate
         */
        internal static void RgbToHsl(Color _rgb, out int _h, out int _s, out int _l)
        {
            RgbToHsl(_rgb.R, _rgb.G, _rgb.B, out _h, out _s, out _l);
        }

        internal static void RgbToHsl(int _r, int _g, int _b, out int _h, out int _s, out int _l)
        {
            float floatR = _r / 255f;
            float floatG = _g / 255f;
            float floatB = _b / 255f;

            float max = Math.Max(floatR, Math.Max(floatG, floatB));
            float min = Math.Min(floatR, Math.Min(floatG, floatB));

            float floatH, floatS, floatL;
            // Calculate lightness (L)
            floatL = (max + min) / 2;

            if (max == min)
            {
                // Achromatic (gray)
                floatH = 0;
                floatS = 0;
            }
            else
            {
                float d = max - min;
                floatS = floatL > 0.5 ? d / (2 - max - min) : d / (max + min);

                // Calculate hue (H)
                if (max == floatR)
                    floatH = (floatG - floatB) / d + (floatG < floatB ? 6 : 0);
                else if (max == floatG)
                    floatH = (floatB - floatR) / d + 2;
                else
                    floatH = (floatR - floatG) / d + 4;

                floatH /= 6;
            }

            _h = (int)Math.Round(floatH * 360f, 0, MidpointRounding.AwayFromZero);
            _s = (int)Math.Round(floatS * 100f, 0, MidpointRounding.AwayFromZero);
            _l = (int)Math.Round(floatL * 100f, 0, MidpointRounding.AwayFromZero);
        }
    }
}
