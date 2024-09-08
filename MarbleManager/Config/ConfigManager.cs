using MarbleManager.Scripts;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace MarbleManager.Config
{
    /**
     * Handles loading and saving of config objects
     * Also applies changes around running on boot, auto on/off etc
     */
    internal static class ConfigManager
    {
        /**
         * Loads config from file
         */
        internal static GlobalConfigObject GetConfig()
        {
            try
            {
                // load file here if exists
                using (StreamReader r = new StreamReader(PathManager.ConfigFilePath))
                {
                    string json = r.ReadToEnd();
                    GlobalConfigObject config = JsonConvert.DeserializeObject<GlobalConfigObject>(json);
                    return config;
                }
            } catch (Exception ex)
            {
                // file not found etc.
                LogManager.WriteLog("Error loading config", ex.Message);
            }
            return new GlobalConfigObject();
        }

        /**
         * Saves a config object to file then applies changes by generating scripts etc.
         */
        internal static void ApplyConfig(GlobalConfigObject _config, bool _forceApply)
        {
            GlobalConfigObject oldConfig = GetConfig();
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

            LogManager.WriteLog("Config changes applied");
        }

        /**
         * Saves a config object to file
         */
        private static void SaveConfigToFile(GlobalConfigObject _config)
        {
            // save to file
            try
            {
                string jsonString = JsonConvert.SerializeObject(_config, Formatting.Indented);
                using (StreamWriter outputFile = new StreamWriter(PathManager.ConfigFilePath))
                {
                    outputFile.WriteLine(jsonString);
                }
            } catch(Exception ex)
            {
                LogManager.WriteLog("Error saving config", ex.Message);
            }
        }

        /**
         * Handles adding/removing this app from windows startup apps
         */
        private static void ConfigureRunOnBoot(bool _createShortcut)
        {
            LogManager.WriteLog("Configuring run on boot");

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
                LogManager.WriteLog("Created app shortcut", shortcutPath);
            }
        }

        /**
         * Configures functionality to turn on/off lights with logon/logoff
         * uses registry script to apply changes
         */
        private static void ConfigureAutoOnOff(bool _autoOnOff)
        {
            // build .reg file to apply change
            string regFilePath = ScriptBuilder.BuildAutoOnOffRegistryScript(_autoOnOff);

            if (regFilePath == null)
            {
                LogManager.WriteLog("Null .reg file path");
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

            LogManager.WriteLog("Applied .reg file", $"{(_autoOnOff ? "enable" : "disable")} auto on/off functionality");
        }
    }
}
