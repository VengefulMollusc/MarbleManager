using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MarbleManager.Colours
{
    /**
     * Class to hold a palette of SwatchObjects
     */
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

        /** 
         * Returns just the 'main' swatches, including dominant if it isn't already included
         */
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

        /**
         * returns a colour designated as the 'highlight' - for single colour lights etc.
         * weighted calculation is applied on palette creation
         */
        [JsonIgnore]
        public SwatchObject Highlight
        {
            get
            {
                return MainSwatches.FirstOrDefault(s => s.isHighlight) ?? MainSwatches[0];
            }
        }
    }

    /**
     * Represents a single colour swatch
     */
    internal class SwatchObject
    {
        // population of swatch in source image
        public int population { get; set; }
        // proportion of swatch compared to other colours in source image
        public float proportion {  get; set; }
        // is this swatch the calculated highlight colour
        public bool isHighlight { get; set; } = false;
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
