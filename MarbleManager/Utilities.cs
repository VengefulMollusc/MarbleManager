﻿using IWshRuntimeLibrary;
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
         * Performs bulk copy/replace functionality on a list of files and outputs
         */
        internal static void CopyFilesAndReplaceValues(List<CopyReplaceFilesData> _copyReplaceFilesData)
        {
            foreach (var data in _copyReplaceFilesData)
            {
                if (data == null) continue;
                foreach (var fileInOutNames in data.fileInOutNames)
                {
                    CopyFileAndReplaceValues(data.inputDir, fileInOutNames.Key, data.outputDir, fileInOutNames.Value, data.toReplace);
                }
            }
        }

        /**
         * Copies a file, renaming in the process.
         * Also replaces given values in the file
         * 
         * used for writing api key changes etc to static script bat files
         */
        internal static void CopyFileAndReplaceValues(string _inputDir, string _inputFile, string _outputDir, string _outputFile, Dictionary<string, string> _toReplace)
        {
            try
            {
                // Read the content of the source file
                string fileContent = System.IO.File.ReadAllText(_inputDir + _inputFile);

                // Replace <variables> with the new value
                foreach (var variable in _toReplace)
                {
                    fileContent = fileContent.Replace(variable.Key, variable.Value);
                }

                // Create the destination directory if it doesn't exist
                Directory.CreateDirectory(_outputDir);

                // Combine the destination directory and the file name
                string destinationFilePath = Path.Combine(_outputDir, _outputFile);

                // Write the modified content to the destination file
                System.IO.File.WriteAllText(destinationFilePath, fileContent);

                Console.WriteLine("Processed: " + _inputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error copying: " + ex.Message);
            }
        }

        /**
         * Object class for copy/replace function
         */
        internal class CopyReplaceFilesData
        {
            public string inputDir {  get; set; }
            public string outputDir { get; set; }
            public Dictionary<string, string> fileInOutNames {  get; set; }
            public Dictionary<string, string> toReplace { get; set; }

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

        /**
         * Creates a shortcut to a given file in a given directory
         * 
         * mainly used for adding this program to startup directory
         */
        internal static void CreateShortcut(string _shortcutFilePath, string _targetFilePath)
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(_shortcutFilePath);

            //shortcut.Description = "My shortcut description";   // The description of the shortcut
            //shortcut.IconLocation = @"c:\myicon.ico";           // The icon of the shortcut
            shortcut.WorkingDirectory = Environment.CurrentDirectory;
            shortcut.TargetPath = _targetFilePath;                 // The path of the file that will launch when the shortcut is run
            shortcut.Save();                                    // Save the shortcut
        }

        /**
         * Deletes a file if it exists
         */
        internal static void DeleteFile(string _filePath)
        {
            try
            {
                // Check if the file exists before attempting to delete it
                if (System.IO.File.Exists(_filePath))
                {
                    // Delete the file
                    System.IO.File.Delete(_filePath);
                    Console.WriteLine("File " + _filePath + " deleted.");
                }
                else
                {
                    Console.WriteLine("File " + _filePath + " does not exist to delete.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
