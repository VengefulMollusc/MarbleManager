using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;

namespace MarbleManager.Config
{
    internal static class ConfigManager
    {
        /**
         * Loads config from file
         */
        internal static ConfigObject GetConfig()
        {
            try
            {
                // load file here if exists
                using (StreamReader r = new StreamReader(PathManager.ConfigFile))
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
            ConfigureAutoOnOff(_config.generalConfig.autoTurnLightsOnOff);

            Console.WriteLine("Applying changes done");
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
                using (StreamWriter outputFile = new StreamWriter(PathManager.ConfigFile))
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
        private static void CreateScriptFilesFromTemplates (ConfigObject _config)
        {
            CreateBatScripts(_config);

            CreateRegistryScripts();

            Console.WriteLine("Creating scripts done");
        }

        /**
         * Creates output Bat scripts for turning on and off lights
         */
        private static void CreateBatScripts(ConfigObject _config)
        {
            Utilities.CopyFilesAndReplaceValues(new Utilities.CopyReplaceFilesData()
            {
                inputDir = PathManager.BatScriptTemplateDir,
                outputDir = PathManager.BatScriptOutputDir,
                fileInOutNames = new Dictionary<string, string>
                    {
                        { "turnOnLights_template.bat", "turnOnLights.bat" },
                        { "turnOffLights_template.bat", "turnOffLights.bat" },
                    },
                toReplace = new Dictionary<string, string>
                    {
                        { "<nanoleafIp>", _config.nanoleafConfig.ipAddress },
                        { "<nanoleafApiKey>", _config.nanoleafConfig.apiKey },
                        { "<lifxSelector>", _config.lifxConfig.selector },
                        { "<lifxAuthKey>", _config.lifxConfig.authKey },
                    },
            });
            Console.WriteLine(".bat scripts done");
        }

        /**
         * Creates .reg scripts for enabling log on/off light controls in registry
         */
        private static void CreateRegistryScripts()
        {
            string userSid = GetUserSid();

            if (userSid == null)
            {
                Console.WriteLine("No user SID found");
                return;
            }

            Utilities.CopyResourcesAndReplaceValues(new Utilities.CopyReplaceResourcesData()
            {
                outputDir = PathManager.RegScriptOutputDir,
                resourceInOutNames = new Dictionary<string, string>
                {
                    { "MarbleManager.templates.reg_scripts.addLogOnOffScripts_template.reg", "addLogOnOffScripts.reg" },
                    { "MarbleManager.templates.reg_scripts.remLogOnOffScripts_template.reg", "remLogOnOffScripts.reg" },
                },
                toReplace = new Dictionary<string, string>
                    {
                        { "<userSID>", userSid },
                        { "<turnOnScriptPath>", Path.Combine(PathManager.BatScriptOutputDir, "turnOnLights.bat").Escape() },
                        { "<turnOffScriptPath>", Path.Combine(PathManager.BatScriptOutputDir, "turnOffLights.bat").Escape() },
                    },
            });
            Console.WriteLine(".reg scripts done");
        }

        /**
         * Retrieves the current user SID
         */
        private static string GetUserSid()
        {
            try
            {
                // Get the current Windows identity
                WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();

                // Get the user's SID
                string userSid = windowsIdentity.User.Value;

                // Display the user's SID
                Console.WriteLine("Current User SID: " + userSid);
                return userSid;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
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
            Utilities.DeleteFile(shortcutPath);

            if (_runOnBoot)
            {
                // add shortcut in startup folder
                Utilities.CreateShortcut(shortcutPath, appPath);
                Console.WriteLine("Created shortcut: " + shortcutPath);
            }
        }

        /**
         * Configures functionality to turn on/off lights with logon/logoff
         */
        private static void ConfigureAutoOnOff(bool _autoOnOff)
        {
            // insert or remove generated script files from group policy
        }
    }
}
