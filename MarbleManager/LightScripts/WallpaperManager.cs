using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.LightScripts
{
    internal static class WallpaperManager
    {

        public static void CopyWallpaperToJpg () {
            string transcodedWallpaperPath = GetWallpaperPath();
            if (transcodedWallpaperPath != null)
            {
                // Temp output file path and rename - current directory
                string newFileName = "output\\wallpaper.jpg";
                string destinationPath = Path.Combine(Environment.CurrentDirectory, newFileName);

                // Copy and rename
                File.Copy(transcodedWallpaperPath, destinationPath, true);
            }
        }

        public static Bitmap GetWallpaperBitmap ()
        {
            string transcodedWallpaperPath = GetWallpaperPath();
            if (transcodedWallpaperPath != null)
            {
                // return transcoded wallpaper as a bitmap
                return new Bitmap(transcodedWallpaperPath);
            }
            return null;
        }

        private static string GetWallpaperPath ()
        {
            // Get the original wallpaper image path
            using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop"))
            {
                if (key != null)
                {
                    string wallpaperPath = key.GetValue("Wallpaper") as string;

                    if (!string.IsNullOrEmpty(wallpaperPath))
                    {
                        return wallpaperPath;
                    }
                }
            }

            return null;
        }
    }
}
