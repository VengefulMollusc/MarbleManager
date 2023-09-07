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
    internal class ConfigHandler
    {
        private ConfigObject configObject;
        static string configFilePath = "config.json";

        public ConfigHandler() {
            LoadConfig();
        }

        public void LoadConfig()
        {
            try
            {
                // load file here if exists
                using (StreamReader r = new StreamReader(configFilePath))
                {
                    string json = r.ReadToEnd();
                    configObject = JsonConvert.DeserializeObject<ConfigObject>(json);
                }
            } catch (Exception ex)
            {
                // file not found etc.
                Console.WriteLine(ex.ToString());
            }
        }

        public void SaveConfig(ConfigObject newConfig)
        {
            // overwrite object
            configObject = newConfig;

            // save to file
            try
            {
                string jsonString = JsonConvert.SerializeObject(configObject, Formatting.Indented);
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

        public void ApplyChanges (ConfigObject newConfig)
        {
            SaveConfig(newConfig);

            // write script files with new values using template files

            // apply changes to add startup scripts etc.
        }

        public GeneralConfig GetGeneralConfig()
        {
            return configObject.generalConfig;
        }

        public NanoleafConfig GetNanoleafConfig()
        {
            return configObject.nanoleafConfig;
        }

        public LifxConfig GetLifxConfig()
        {
            return configObject.lifxConfig;
        }
    }
}
