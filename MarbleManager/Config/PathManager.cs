using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

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
        static string templatesPath = "templates\\";
        static string batTemplatesSubPath = "bat_scripts\\";
        static string regTemplatesSubPath = "reg_scripts\\";
        static string nanoleafEffectTemplateDir = "effect_payloads\\";

        // output paths
        static string dataOutputPath = "data\\";
        static string scriptOutputSubPath = "scripts\\";
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
        internal static string TemplateDir
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, templatesPath);
            }
        }
        internal static string DataOutputDir
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, dataOutputPath);
            }
        }
        internal static string BatScriptTemplateDir
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, templatesPath, batTemplatesSubPath);
            }
        }
        internal static string BatScriptOutputDir
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, dataOutputPath, scriptOutputSubPath, batScriptOutputSubPath);
            }
        }
        internal static string RegScriptTemplateDir
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, templatesPath, regTemplatesSubPath);
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
