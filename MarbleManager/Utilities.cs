using IWshRuntimeLibrary;
using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace MarbleManager
{
    /**
     * Static methods for value conversion, replacing values in files and general helper methods
     */
    internal static class Utilities
    {
        /**
         * Performs bulk copy/replace functionality on compiled resources and output files
         */
        internal static void CopyResourcesAndReplaceValues(CopyReplaceResourcesData _data)
        {
            if (_data == null) return;
            foreach (var fileInOutNames in _data.resourceInOutNames)
            {
                CopyResourceToFileAndReplaceValues(fileInOutNames.Key, _data.outputDir, fileInOutNames.Value, _data.toReplace);
            }
        }

        /**
         * Loads a resource file as a list of strings and replaces values
         */
        internal static List<string> LoadResourceAsListAndReplaceValues(string _resourceName, Dictionary<string, string> _toReplace)
        {
            try
            {
                // Open the embedded resource stream
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_resourceName))
                {
                    if (stream == null)
                    {
                        LogManager.WriteLog($"Null resource stream", _resourceName);
                        return null;
                    }

                    // Read the lines from the resource stream
                    List<string> lines = new List<string>();
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(ReplaceValues(line, _toReplace));
                        }
                    }
                    return lines;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Load resource error", ex.Message);
            }
            return null;
        }

        /**
         * Copies a compiled resource to a file, renaming in the process.
         * Also replaces given values in the file
         * 
         * used for writing values to compiled files eg registry scripts
         */
        internal static void CopyResourceToFileAndReplaceValues(string _resourceName, string _outputDir, string _outputFile, Dictionary<string, string> _toReplace)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_resourceName))
            {
                if (stream == null)
                {
                    LogManager.WriteLog($"Null resource stream", _resourceName);
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContent = ReplaceValues(reader.ReadToEnd(), _toReplace);

                    // Create the destination directory if it doesn't exist
                    Directory.CreateDirectory(_outputDir);

                    // Combine the destination directory and the file name
                    string destinationFilePath = Path.Combine(_outputDir, _outputFile);

                    // Write the modified content to the destination file`
                    System.IO.File.WriteAllText(destinationFilePath, fileContent);
                }
                stream.Close();
            }
        }

        /**
         * Object class for copy/replace function
         */
        internal class CopyReplaceResourcesData
        {
            public string outputDir { get; set; }
            public Dictionary<string, string> resourceInOutNames { get; set; }
            public Dictionary<string, string> toReplace { get; set; }
        }

        /**
         * Converts RGB to Hex Code 
         */
        internal static string RgbToHex(Color _rgb, bool _includeHash = true)
        {
            string baseString = _includeHash ? "#" : "";
            return $"{baseString}{_rgb.R:X2}{_rgb.G:X2}{_rgb.B:X2}";
        }
        internal static string RgbToHex(int _r, int _g, int _b, bool _includeHash = true)
        {
            string baseString = _includeHash ? "#" : "";
            return $"{baseString}{_r:X2}{_g:X2}{_b:X2}";
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
         * Converts HSL to RGB
         * Used after boosting saturation
         * 
         * ASSUMES HSL VALUES ARE IN RANGE 0-1
         */
        internal static void HslToRgb(float _h, float _s, float _l, out int _r, out int _g, out int _b)
        {
            float r, g, b;

            if (_s == 0)
            {
                r = g = b = _l;
            } else
            {
                float q = _l < 0.5 ? _l * (1 + _s) : _l + _s - (_l * _s);
                float p = 2 * _l - q;
                r = HueToRgb(p, q, _h + 1f / 3f);
                g = HueToRgb(p, q, _h);
                b = HueToRgb(p, q, _h - 1f / 3f);
            }

            _r = (int)(r * 255);
            _g = (int)(g * 255);
            _b = (int)(b * 255);
        }

        /**
         * Helper method for HslToRgb
         */
        private static float HueToRgb(float _p, float _q, float _t)
        {
            if (_t < 0f) _t += 1f;
            if (_t > 1f) _t -= 1f;
            if (_t < 1f / 6f) return _p + (_q - _p) * 6f * _t;
            if (_t < 1f / 2f) return _q;
            if (_t < 2f / 3f) return _p + (_q - _p) * (2f / 3f - _t) * 6f;
            return _p;
        }

        /**
         * Methods for clamping floats
         */
        internal static float Clamp(float _value, float _min, float _max)
        {
            if (_value < _min)
                return _min;
            if (_value > _max)
                return _max;
            return _value;
        }
        internal static float Clamp01(float _value)
        {
            return Clamp(_value, 0f, 1f);
        }

        /**
         * Map values to a new range
         */
        internal static float Map(float _value, float _oldMin, float _oldMax, float _newMin, float _newMax)
        {
            _value = Clamp(_value, _oldMin, _oldMax);
            return _newMin + (_value - _oldMin) * (_newMax - _newMin) / (_oldMax - _oldMin);
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
                    LogManager.WriteLog("File deletion success", _filePath);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("File deletion errpr", $"{_filePath} - {ex.Message}");
            }
        }

        /**
         * Replaces values in a string
         */
        internal static string ReplaceValues(string _input, Dictionary<string, string> _toReplace)
        {
            string output = _input;
            // Replace <variables> with the new value
            foreach (var variable in _toReplace)
            {
                output = output.Replace(variable.Key, variable.Value);
            }
            return output;
        }

        /**
         * 
         * 
         * BELOW THIS POINT
         * 
         * Are things that aren't currently used
         * 
         * but I'm holding on to for the moment just in case
         * 
         * 
         */

        /**
         * Performs bulk copy/replace functionality on a list of files and outputs
         */
        internal static void CopyFilesAndReplaceValues(CopyReplaceFilesData _data)
        {
            if (_data == null) return;
            foreach (var fileInOutNames in _data.fileInOutNames)
            {
                CopyFileAndReplaceValues(_data.inputDir, fileInOutNames.Key, _data.outputDir, fileInOutNames.Value, _data.toReplace);
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
                string fileContent = ReplaceValues(System.IO.File.ReadAllText(_inputDir + _inputFile), _toReplace);

                // Create the destination directory if it doesn't exist
                Directory.CreateDirectory(_outputDir);

                // Combine the destination directory and the file name
                string destinationFilePath = Path.Combine(_outputDir, _outputFile);

                // Write the modified content to the destination file
                System.IO.File.WriteAllText(destinationFilePath, fileContent);

                Console.WriteLine($"Processed: {_inputFile}");
                LogManager.WriteLog("Copy and replace success", _inputFile);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Copy and replace error", $"{_inputFile} - {ex.Message}");
            }
        }

        /**
         * Object class for copy/replace function
         */
        internal class CopyReplaceFilesData
        {
            public string inputDir { get; set; }
            public string outputDir { get; set; }
            public Dictionary<string, string> fileInOutNames { get; set; }
            public Dictionary<string, string> toReplace { get; set; }
        }
    }
}
