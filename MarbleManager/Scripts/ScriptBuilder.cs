using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace MarbleManager.Scripts
{
    internal static class ScriptBuilder
    {
        static string turnOnLightsFileName = "turnOnLights.bat";
        static string turnOffLightsFileName = "turnOffLights.bat";

        static string batchLogTemplateFileName = "batchLog_template.bat";

        static string templateNamespace = "MarbleManager.Scripts.Templates";
        static string regNamespace = "reg_scripts";

        /**
         * Creates script files from templates with new values from config
         */
        internal static void BuildOnOffScriptFiles(GlobalConfigObject _config)
        {
            // on script
            string onCommand = _config.generalConfig.syncOnWallpaperChange ? "syncon" : "on";
            if (_config.generalConfig.runOnBoot)
            {
                // boot full app from script
                onCommand += " bootapp";
            }
            CreateAndSaveBatScript(
                _config,
                onCommand, 
                turnOnLightsFileName
            );

            // off script
            CreateAndSaveBatScript(
                _config, 
                "off",
                turnOffLightsFileName
            );

            LogManager.WriteLog("Creating .bat scripts done");
        }

        /**
         * Wraps a list of string commands and saves to a given filename
         */
        private static void CreateAndSaveBatScript(GlobalConfigObject _config, string _exeParams, string _fileName)
        {
            string outputFile = Path.Combine(PathManager.BatScriptOutputDir, _fileName);

            // Define commands
            List<string> batchCommands = new List<string>()
            {
                // wrapper commands
                "@echo off",
                $"setlocal{(_config.generalConfig.logUsage ? " enabledelayedexpansion" : " ")}", // only need enabledelayedexpansion if logging
            };

            // add logs if needed
            if (_config.generalConfig.logUsage)
            {
                batchCommands.AddRange(GetLogCommands(_exeParams));
            }

            // add light commands
            batchCommands.AddRange(new List<string>()
            {
                $"cd /d \"{Environment.CurrentDirectory}\"",
                $@".\{AppDomain.CurrentDomain.FriendlyName} {_exeParams}"
            });

            // end the file
            batchCommands.Add("endlocal");

            try
            {
                // create output dir if not exists
                Directory.CreateDirectory(PathManager.BatScriptOutputDir);
                // Create the .bat file and write the batch commands to it.
                File.WriteAllLines(outputFile, batchCommands);

                LogManager.WriteLog($".bat file created", outputFile);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(".bat creation error", ex.Message);
            }
        }

        /**
         * Returns the list of commands needed to generate logs
         */
        private static List<string> GetLogCommands(string _command)
        {
            return Utilities.LoadResourceAsListAndReplaceValues(
                $"{templateNamespace}.{batchLogTemplateFileName}",
                new Dictionary<string, string>
                    {
                        { "<command>", _command.ToUpper() },
                        { "<filePath>", PathManager.LogFilePath },
                    });
        }

        /**
         * Creates .reg scripts for enabling log on/off light controls in registry
         */
        internal static string BuildAutoOnOffRegistryScript(bool _enable)
        {
            string userSid = GetUserSid();

            if (userSid == null)
            {
                LogManager.WriteLog(".reg script error", "Null user SID");
                return null;
            }

            Utilities.CopyResourceToFileAndReplaceValues(
                $"{templateNamespace}.{regNamespace}.{(_enable ? "addLogOnOffScripts_template.reg" : "remLogOnOffScripts_template.reg")}",
                PathManager.RegScriptOutputDir,
                _enable ? "addLogOnOffScripts.reg" : "remLogOnOffScripts.reg",
                new Dictionary<string, string>
                    {
                        { "<userSID>", userSid },
                        { "<turnOnScriptPath>", Path.Combine(PathManager.BatScriptOutputDir, turnOnLightsFileName).Escape() },
                        { "<turnOffScriptPath>", Path.Combine(PathManager.BatScriptOutputDir, turnOffLightsFileName).Escape() },
                    }
            );
            LogManager.WriteLog(".reg script creation done");

            return Path.Combine(PathManager.RegScriptOutputDir, _enable ? "addLogOnOffScripts.reg" : "remLogOnOffScripts.reg");
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
                return windowsIdentity.User.Value;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Get user SID error", ex.Message);
            }
            return null;
        }
    }
}
