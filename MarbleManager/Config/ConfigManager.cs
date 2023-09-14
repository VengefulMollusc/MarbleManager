﻿using MarbleManager.Scripts;
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

            // create script files with new values
            ScriptBuilder.BuildScriptFiles(_config);

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
            // run relevant .reg files
        }
    }
}
