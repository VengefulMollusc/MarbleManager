﻿namespace MarbleManager.Colours
{
    internal class PaletteObject
    {
        public SwatchObject dominant { get; set; }
        public SwatchObject vibrant { get; set; }
        public SwatchObject lightVibrant { get; set; }
        public SwatchObject darkVibrant { get; set; }
        public SwatchObject muted { get; set; }
        public SwatchObject lightMuted { get; set; }
        public SwatchObject darkMuted { get; set; }

        // this doesn't return dominant as it's included as one of the other swatches
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
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
        public float[] hsl { get; set; }
    }
}
