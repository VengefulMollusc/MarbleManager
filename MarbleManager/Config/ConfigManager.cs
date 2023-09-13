using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarbleManager.Config
{
    internal static class ConfigManager
    {
        static string configFileName = "config.json";
        static string dataDirPath = "data\\";
        static string templatesDirPath = "templates\\";
        static string scriptOutputDirPath = "scripts\\";

        internal static string TemplatesDirectory { get { return templatesDirPath; } }
        internal static string DataDirectory { get { return dataDirPath; } }

        /**
         * Loads config from file
         */
        internal static ConfigObject GetConfig()
        {
            try
            {
                // load file here if exists
                using (StreamReader r = new StreamReader(Path.Combine(Environment.CurrentDirectory, configFileName)))
                {
                    string json = r.ReadToEnd();
                    ConfigObject config = JsonConvert.DeserializeObject<ConfigObject>(json);
                    Console.WriteLine("Loaded config");
                    return config;
                }
            } catch (Exception ex)
            {
                // file not found etc.
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Error loading config");
            }
            return null;
        }

        /**
         * Saves a config object to file then applies changes by generating scripts etc.
         */
        internal static void ApplyConfig(ConfigObject _config)
        {
            SaveConfigToFile(_config);

            // write script files with new values using template files
            // TODO remove if these aren't needed or arent configurable from code
            CreateScriptFilesFromTemplates(_config);

            // apply changes to add startup scripts etc.
            ConfigureRunOnBoot(_config.generalConfig.runOnBoot);

            Console.WriteLine("Changes successfully applied");
        }

        /**
         * Saves a config object to file
         */
        private static void SaveConfigToFile(ConfigObject _config)
        {
            // save to file
            try
            {
                string jsonString = JsonConvert.SerializeObject(_config, Formatting.Indented);
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(Environment.CurrentDirectory, configFileName)))
                {
                    outputFile.WriteLine(jsonString);
                }
                Console.WriteLine("Config successfully saved");
            } catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Error saving config");
            }
        }

        /**
         * Handles adding/removing this app from windows startup apps
         */
        private static void ConfigureRunOnBoot(bool _runOnBoot)
        {
            // Get relevant paths, names etc.
            string appPath = Path.Combine(Environment.CurrentDirectory, AppDomain.CurrentDomain.FriendlyName);
            string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutName = Path.GetFileNameWithoutExtension(appPath) + "_autoShortcut.lnk";
            string shortcutPath = Path.Combine(startupFolderPath, shortcutName);

            // remove shortcut if exists from startup folder
            try
            {
                // Check if the file exists before attempting to delete it
                if (File.Exists(shortcutPath))
                {
                    // Delete the file
                    File.Delete(shortcutPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            if (_runOnBoot) {
                // add shortcut in startup folder
                using (StreamWriter writer = new StreamWriter(shortcutPath))
                {
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=file:///" + appPath.Replace('\\', '/')); // Use file:/// for local file paths
                    writer.WriteLine("IconIndex=0");
                    writer.WriteLine("IconFile=" + appPath.Replace('\\', '/')); // Use the application's path as the icon source
                    writer.Flush();
                    Console.WriteLine("Created: " + shortcutPath);
                }
            }
        }

        /**
         * Creates script files from templates with new values from config
         */
        private static void CreateScriptFilesFromTemplates (ConfigObject _config)
        {
            string[] templates = {
                "turnOnLights_template.bat",
                "turnOffLights_template.bat",
                "getNanoleafUrl_template.bat",
            };

            string[] outputs =
            {
                "turnOnLights.bat",
                "turnOffLights.bat",
                "getNanoleafUrl.bat",
            };

            Dictionary<string, string> variables = new Dictionary<string, string>
            {
                { "<nanoleafIp>", _config.nanoleafConfig.ipAddress },
                { "<nanoleafApiKey>", _config.nanoleafConfig.apiKey },
                { "<lifxSelector>", _config.lifxConfig.selector },
                { "<lifxAuthKey>", _config.lifxConfig.authKey },
            };

            for (int i = 0; i < templates.Length; i++)
            {
                Utilities.CopyFileAndReplaceValues(
                    Path.Combine(Environment.CurrentDirectory, templatesDirPath), 
                    templates[i], 
                    Path.Combine(Environment.CurrentDirectory, dataDirPath, scriptOutputDirPath),
                    outputs[i], 
                    variables);
            }
        }
    }
}
