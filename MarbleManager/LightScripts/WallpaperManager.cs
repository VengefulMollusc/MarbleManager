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
        static string wallpaperFilePath = "output\\wallpaper.jpg";

        private static void CopyWallpaperToJpg () {
            string transcodedWallpaperPath = GetWallpaperPath();
            if (transcodedWallpaperPath != null)
            {
                // Temp output file path and rename - current directory
                string destinationPath = Path.Combine(Environment.CurrentDirectory, wallpaperFilePath);

                // Copy and rename
                File.Copy(transcodedWallpaperPath, destinationPath, true);
            }
        }
        public static Bitmap GetWallpaperJpg()
        {
            CopyWallpaperToJpg();

            return new Bitmap(wallpaperFilePath);
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
