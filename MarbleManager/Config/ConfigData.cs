namespace MarbleManager.Config
{
    public class ConfigObject
    {
        public GeneralConfig generalConfig { get; set; }
        public NanoleafConfig nanoleafConfig { get; set; }
        public LifxConfig lifxConfig { get; set; }
    }

    public class GeneralConfig
    {
        // toggles updating light palette on wallpaper change
        public bool syncOnWallpaperChange { get; set; }
        // toggles activating scripts to turn on/off lights at logon/logoff
        public bool autoTurnOnOff { get; set; }
        // toggles between using MainSwatches and AllSwatches for applying palettes
        public bool onlyUseMainSwatches { get; set; }
        // whether this app runs on boot or not
        public bool runOnBoot { get; set; }
    }

    public class NanoleafConfig
    {
        public string ipAddress { get; set; }
        public string apiKey { get; set; }
        // The effect to use when applying palette to nanoleaf panels
        public NanoleafEffect effect { get; set; }
        
        // Highlight effect options
        // forces the main colour probability to a certain value
        public bool overrideMainColourProb { get; set; }
        // the value to set the main colour probability to
        public int mainColourProb { get; set; }
    }

    public class LifxConfig
    {
        public string selector { get; set; }
        public string authKey { get; set; }
    }

    public enum NanoleafEffect
    {
        Random,
        Highlight
    }
}
