using MarbleManager.Scripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                using (StreamReader r = new StreamReader(PathManager.ConfigFilePath))
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
        internal static void ApplyConfig(ConfigObject _config, bool _forceApply)
        {
            ConfigObject oldConfig = GetConfig();
            SaveConfigToFile(_config);

            // create script files with new values
            ScriptBuilder.BuildOnOffScriptFiles(_config);

            bool force = _forceApply || oldConfig == null;

            // add/remove startup scripts if changed 
            if (force || oldConfig.generalConfig.runOnBoot != _config.generalConfig.runOnBoot)
            {
                ConfigureRunOnBoot(
                    _config.generalConfig.runOnBoot 
                    && !_config.generalConfig.autoTurnLightsOnOff // auto scripts will boot app already
                    );
            }
            // turn on/off auto light state on logon etc.
            if (force || oldConfig.generalConfig.autoTurnLightsOnOff != _config.generalConfig.autoTurnLightsOnOff)
            {
                ConfigureAutoOnOff(_config.generalConfig.autoTurnLightsOnOff);
            }

            Console.WriteLine("Changes applied");
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
                using (StreamWriter outputFile = new StreamWriter(PathManager.ConfigFilePath))
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
        private static void ConfigureRunOnBoot(bool _createShortcut)
        {
            // Get relevant paths, names etc.
            string appPath = PathManager.MarbleManagerFilePath;
            string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutName = $"{Path.GetFileNameWithoutExtension(appPath)}_autoShortcut.lnk";
            string shortcutPath = Path.Combine(startupFolderPath, shortcutName);

            // remove shortcut if exists from startup folder
            Utilities.DeleteFile(shortcutPath);

            if (_createShortcut)
            {
                // add shortcut in startup folder
                Utilities.CreateShortcut(shortcutPath, appPath);
                Console.WriteLine($"Created shortcut: {shortcutPath}");
            }
        }

        /**
         * Configures functionality to turn on/off lights with logon/logoff
         */
        private static void ConfigureAutoOnOff(bool _autoOnOff)
        {
            // build .reg file to apply change
            string regFilePath = ScriptBuilder.BuildAutoOnOffRegistryScript(_autoOnOff);

            if (regFilePath == null)
            {
                Console.WriteLine("Null .reg file path");
                return;
            }

            // Run the regedit.exe command with the .reg file as an argument.
            Process regeditProcess = new Process();
            regeditProcess.StartInfo.FileName = "regedit.exe";
            regeditProcess.StartInfo.Arguments = $"/s {regFilePath}"; // /s is used to suppress dialogs
            regeditProcess.Start();
            regeditProcess.WaitForExit();

            // Check the exit code if needed.
            //int exitCode = regeditProcess.ExitCode;

            // Dispose of the process.
            regeditProcess.Dispose();

            // delete .reg file
            Utilities.DeleteFile(regFilePath);

            Console.WriteLine($"Applied .reg file to {(_autoOnOff ? "enable" : "disable")} auto on/off functionality");
        }
    }
}
