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
        public bool syncOnWallpaperChange { get; set; }
        public bool turnOnOffWithPc { get; set; }
        public bool onlyUseMainSwatches { get; set; }
    }

    public class NanoleafConfig
    {
        public string ipAddress { get; set; }
        public string apiKey { get; set; }
        public NanoleafEffect effect { get; set; }
        public bool overrideMainColourProb { get; set; }
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
