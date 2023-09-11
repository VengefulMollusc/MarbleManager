using Newtonsoft.Json;

namespace MarbleManager.Colours
{
    internal class PaletteObject
    {
        // dominant colour
        public SwatchObject dominant { get; set; }
        // swatches
        public SwatchObject vibrant { get; set; }
        public SwatchObject lightVibrant { get; set; }
        public SwatchObject darkVibrant { get; set; }
        public SwatchObject muted { get; set; }
        public SwatchObject lightMuted { get; set; }
        public SwatchObject darkMuted { get; set; }

        // this doesn't return dominant as it's included as one of the other swatches
        [JsonIgnore]
        public SwatchObject[] Swatches { get {
                return new SwatchObject[] { 
                    vibrant, 
                    lightVibrant, 
                    darkVibrant,
                    muted,
                    lightMuted,
                    darkMuted
                };
            } 
        }
    }

    internal class SwatchObject
    {
        public int population { get; set; }
        // RGB
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
        // HSL
        public float h { get; set; }
        public float s { get; set; }
        public float l { get; set; }
    }
}
