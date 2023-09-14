using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Scripts
{
    internal static class ScriptBuilder
    {
        static string turnOnLightsFileName = "turnOnLights.bat";
        static string turnOffLightsFileName = "turnOffLights.bat";

        static string regTemplateNamespace = "MarbleManager.Scripts.Templates.reg_scripts.";

        /**
         * Creates script files from templates with new values from config
         */
        internal static void BuildOnOffScriptFiles(ConfigObject _config)
        {
            CreateOnOffBatScript(true, _config);
            CreateOnOffBatScript(false, _config);

            Console.WriteLine("Creating .bat scripts done");
        }

        /**
         * Creates .bat scripts for turning on/off lights
         */
        private static void CreateOnOffBatScript(bool _lightsOn, ConfigObject _config)
        {
            string outputFile = Path.Combine(PathManager.BatScriptOutputDir, _lightsOn ? turnOnLightsFileName : turnOffLightsFileName);

            List<LightScriptBuilder> builders = new List<LightScriptBuilder>()
            {
                new LifxLightScriptBuilder(),
                new NanoleafLightScriptBuilder(),
            };

            // Define the batch commands you want to include in the .bat file.
            List<string> batchCommands = new List<string>()
            {
                "@echo off",
                "setlocal",
            };

            // add commands for light scripts
            foreach (LightScriptBuilder builder in builders)
            {
                foreach (string command in builder.GetLightOnOffCommands(_lightsOn, _config))
                {
                    batchCommands.Add(command);
                }
            }

            // end the file
            batchCommands.Add("endlocal");

            try
            {
                // create output dir if not exists
                Directory.CreateDirectory(PathManager.BatScriptOutputDir);
                // Create the .bat file and write the batch commands to it.
                File.WriteAllLines(outputFile, batchCommands);

                Console.WriteLine("Batch file created successfully: " + outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        /**
         * Creates .reg scripts for enabling log on/off light controls in registry
         */
        internal static string BuildAutoOnOffRegistryScript(bool _enable)
        {
            string userSid = GetUserSid();

            if (userSid == null)
            {
                Console.WriteLine("Null user SID");
                return null;
            }

            Utilities.CopyResourceAndReplaceValues(
                regTemplateNamespace + (_enable ? "addLogOnOffScripts_template.reg" : "remLogOnOffScripts_template.reg"),
                PathManager.RegScriptOutputDir,
                _enable ? "addLogOnOffScripts.reg" : "remLogOnOffScripts.reg",
                new Dictionary<string, string>
                    {
                        { "<userSID>", userSid },
                        { "<turnOnScriptPath>", Path.Combine(PathManager.BatScriptOutputDir, turnOnLightsFileName).Escape() },
                        { "<turnOffScriptPath>", Path.Combine(PathManager.BatScriptOutputDir, turnOffLightsFileName).Escape() },
                    }
            );
            Console.WriteLine(".reg script done");

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
    }
}
