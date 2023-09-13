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
        static string configFilePath = "config.json";
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
                using (StreamReader r = new StreamReader(configFilePath))
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
        internal static void ApplyConfig(ConfigObject newConfig)
        {
            SaveConfigToFile(newConfig);

            // write script files with new values using template files
            CreateScriptFilesFromTemplates(newConfig);

            // apply changes to add startup scripts etc.

            Console.WriteLine("Changes successfully applied");
        }

        /**
         * Saves a config object to file
         */
        private static void SaveConfigToFile(ConfigObject newConfig)
        {
            // save to file
            try
            {
                string jsonString = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
                using (StreamWriter outputFile = new StreamWriter(configFilePath))
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
         * Creates script files from templates with new values from config
         */
        private static void CreateScriptFilesFromTemplates (ConfigObject config)
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
                { "<nanoleafIp>", config.nanoleafConfig.ipAddress },
                { "<nanoleafApiKey>", config.nanoleafConfig.apiKey },
                { "<lifxSelector>", config.lifxConfig.selector },
                { "<lifxAuthKey>", config.lifxConfig.authKey },
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
