using MarbleManager.Colours;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace MarbleManager.Config
{
    public class GlobalConfigObject
    {
        public GeneralConfig generalConfig { get; set; }
        public NanoleafConfig nanoleafConfig { get; set; }
        public LifxConfig lifxConfig { get; set; }
        public WizConfig wizConfig { get; set; }
    }

    public abstract class ConfigSectionObject { };

    public class GeneralConfig : ConfigSectionObject
    {
        // toggles updating light palette on wallpaper change
        public bool syncOnWallpaperChange { get; set; }
        // toggles activating scripts to turn on/off lights at logon/logoff
        public bool autoTurnLightsOnOff { get; set; }
        // toggles between using MainSwatches and AllSwatches for applying palettes
        public bool onlyUseMainSwatches { get; set; }
        // whether this app runs on boot or not
        public bool runOnBoot { get; set; }
        // whether the app creates log files
        public bool logUsage { get; set; }
        // advanced settings toggle
        public bool showAdvanced { get; set; }
        // highlight weight values
        public int[] highlightWeights { get; set; }
    }

    public abstract class LightConfig : ConfigSectionObject
    {
        // whether this light is enabled
        public bool enabled { get; set; }
        // whether this light should apply palettes
        public bool applyPalette { get; set; }
    }

    public class NanoleafConfig : LightConfig
    {
        // List of lights
        public List<Light> lights { get; set; }
        // The effect to use when applying palette to nanoleaf panels
        public NanoleafEffect effect { get; set; }
        
        // Highlight effect options
        // forces the main colour probability to a certain value
        public bool overrideMainColourProb { get; set; }
        // the value to set the main colour probability to
        public int mainColourProb { get; set; }

        public class Light
        {
            public string ipAddress { get; set; }
            public string apiKey { get; set; }
        }
    }

    public enum NanoleafEffect
    {
        Random = 0,
        Highlight = 1
    }

    public class LifxConfig : LightConfig
    {
        // light selectors seperated by commas
        public string selectors { get; set; }
        public string authKey { get; set; }

        // gets all selectors in a list
        [JsonIgnore]
        public List<string> SelectorList
        {
            get
            {
                return selectors != null ? new List<string>(selectors.Split(',')) : new List<string>();
            }
        }
    }

    public class WizConfig : LightConfig
    {
        // IP addresses seperated by commas
        public string ipAddresses { get; set; }

        // gets all ips in a list
        [JsonIgnore]
        public List<string> IpAddressList
        {
            get
            {
                return ipAddresses != null ? new List<string>(ipAddresses.Split(',')) : new List<string>();
            }
        }
    }
}
