using System;
using System.IO;

namespace MarbleManager.Config
{
    internal static class StringExtensions
    {
        /**
         * Allows any string to be escaped via string.Escape()
         */
        public static string Escape(this string _path)
        {
            return _path.Replace("\\", "\\\\");
        }
    }

    /**
     * handles all relevant config and data paths, and file names
     */
    internal static class PathManager
    {
        // file names
        static string configFileName = "config.json";
        static string wallpaperFileName = "wallpaper.jpg";
        static string paletteFileName = "palette.json";
        static string logFileName = "log.txt";
        static string logSecondaryFileName = "log_sec.txt";

        // input paths
        static string templatesPath = "Scripts\\Templates\\";
        static string nanoleafEffectTemplateDir = "effect_payloads\\";

        // output paths
        static string dataOutputPath = "Data\\";
        static string scriptOutputSubPath = "Scripts\\";
        static string batScriptOutputSubPath = "bat\\";
        static string regScriptOutputSubPath = "reg\\";

        // files
        internal static string ConfigFilePath
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, configFileName);
            }
        }
        internal static string WallpaperFilePath
        {
            get
            {
                return Path.Combine(DataOutputDir, wallpaperFileName);
            }
        }
        internal static string PaletteFilePath
        {
            get
            {
                return Path.Combine(DataOutputDir, paletteFileName);
            }
        }
        internal static string MarbleManagerFilePath
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, AppDomain.CurrentDomain.FriendlyName);
            }
        }
        internal static string LogFilePath
        {
            get
            {
                return Path.Combine(DataOutputDir, logFileName);
            }
        }
        internal static string LogSecondaryFilePath
        {
            get
            {
                return Path.Combine(DataOutputDir, logSecondaryFileName);
            }
        }

        // directories
        internal static string DataOutputDir
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, dataOutputPath);
            }
        }
        internal static string BatScriptOutputDir
        {
            get
            {
                return Path.Combine(DataOutputDir, scriptOutputSubPath, batScriptOutputSubPath);
            }
        }
        internal static string RegScriptOutputDir
        {
            get
            {
                return Path.Combine(DataOutputDir, scriptOutputSubPath, regScriptOutputSubPath);
            }
        }
        internal static string NanoleafEffectTemplateDir
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, templatesPath, nanoleafEffectTemplateDir);
            }
        }
    }
}
