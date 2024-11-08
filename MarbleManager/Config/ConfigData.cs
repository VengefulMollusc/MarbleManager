﻿using Newtonsoft.Json;
using System.Collections.Generic;

/**
 * Data structures for config objects
 */
namespace MarbleManager.Config
{
    /**
     * Wrapper that holds all the individual config section objects
     */
    public class GlobalConfigObject
    {
        public GeneralConfig generalConfig { get; set; }
        public NanoleafConfig nanoleafConfig { get; set; }
        public LifxConfig lifxConfig { get; set; }
        public WizConfig wizConfig { get; set; }
        public PicoConfig picoConfig { get; set; }

        public GlobalConfigObject()
        {
            generalConfig = new GeneralConfig();
            nanoleafConfig = new NanoleafConfig();
            lifxConfig = new LifxConfig();
            wizConfig = new WizConfig();
            picoConfig = new PicoConfig();
        }
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
        // whether the app creates extended log files
        public bool logUsage { get; set; }
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
        public bool overrideDominantColourProb { get; set; }
        // the value to set the main colour probability to
        public int dominantColourProb { get; set; }
        // subclass to hold individual light settings
        public class Light
        {
            public bool enabled { get; set; }
            public string ipAddress { get; set; }
            public string apiKey { get; set; }
        }
    }

    /**
     * Which Nanoleaf palette effect to use
     */
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

    public class PicoConfig : LightConfig
    {
        // IP addresses seperated by commas
        public string ipAddresses { get; set; }
        // light brightness (1-255)
        public int brightness { get; set; }
        // whether to boost saturation
        public bool juiceColours { get; set; }

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
