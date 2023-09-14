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

    internal static class PathManager
    {
        // file names
        static string configFileName = "config.json";
        static string wallpaperFileName = "wallpaper.jpg";
        static string paletteFileName = "palette.json";

        // input paths
        static string templatesPath = "Scripts\\Templates\\";
        static string nanoleafEffectTemplateDir = "effect_payloads\\";

        // output paths
        static string dataOutputPath = "Data\\";
        static string scriptOutputSubPath = "Scripts\\";
        static string batScriptOutputSubPath = "bat\\";
        static string regScriptOutputSubPath = "reg\\";

        // files
        internal static string ConfigFile
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, configFileName);
            }
        }
        internal static string WallpaperFile
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, dataOutputPath, wallpaperFileName);
            }
        }
        internal static string PaletteFile
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, dataOutputPath, paletteFileName);
            }
        }

        // paths
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
                return Path.Combine(Environment.CurrentDirectory, dataOutputPath, scriptOutputSubPath, batScriptOutputSubPath);
            }
        }
        internal static string RegScriptOutputDir
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, dataOutputPath, scriptOutputSubPath, regScriptOutputSubPath);
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
