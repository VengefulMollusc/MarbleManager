using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MarbleManager.Colours
{
    internal class PaletteObject
    {
        // dominant colour
        public SwatchObject dominant { get; set; }
        // main swatches
        public SwatchObject vibrant { get; set; }
        public SwatchObject lightVibrant { get; set; }
        public SwatchObject darkVibrant { get; set; }
        public SwatchObject muted { get; set; }
        public SwatchObject lightMuted { get; set; }
        public SwatchObject darkMuted { get; set; }

        // all swatches (can be many more than just main)
        public List<SwatchObject> AllSwatches { get; set; }

        // gets just the main swatches, including dominant if it doesn't match one of them
        [JsonIgnore]
        public List<SwatchObject> MainSwatches { get {
                List<SwatchObject> swatches = new List<SwatchObject>();
                if (vibrant != null) swatches.Add(vibrant);
                if (lightVibrant != null) swatches.Add(lightVibrant);
                if (darkVibrant != null) swatches.Add(darkVibrant);
                if (muted != null) swatches.Add(muted);
                if (lightMuted != null) swatches.Add(lightMuted);
                if (darkMuted != null) swatches.Add(darkMuted);
                if (dominant != null && !swatches.Exists(x => 
                    x.population == dominant.population
                    && x.r == dominant.r
                    && x.g == dominant.g
                    && x.b == dominant.b)) {
                    // include dominant swatch if it is not already included
                    swatches.Prepend(dominant);
                }
                return swatches.OrderByDescending(x => x.population).ToList();
            } 
        }

        // gets the single brightest swatch - used for single colour palette applying
        // returns the swatch with the highest luminance
        [JsonIgnore]
        public SwatchObject Brightest { get {
                return MainSwatches.Aggregate((x, y) => x.l > y.l ? x : y);
        } }
    }

    internal class SwatchObject
    {
        // population of swatch in source image
        public int population { get; set; }
        // proportion of swatch compared to other colours in source image
        public float proportion {  get; set; }
        // RGB
        // 0-255
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
        // HSL
        // H 0-360
        // S 0-100
        // L 0-100
        public int h { get; set; }
        public int s { get; set; }
        public int l { get; set; }
    }
}
