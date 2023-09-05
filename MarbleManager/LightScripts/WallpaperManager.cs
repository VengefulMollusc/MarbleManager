using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.LightScripts
{
    internal class WallpaperManager
    {

        public void FetchWallpaper () {
            // Get the original wallpaper image path
            using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop"))
            {
                if (key != null)
                {
                    string wallpaperPath = key.GetValue("Wallpaper") as string;

                    if (!string.IsNullOrEmpty(wallpaperPath))
                    {
                        // TranscodedWallpaper file path
                        string transcodedImagePath = wallpaperPath;

                        // Temp output file path and rename - current directory
                        string newFileName = "output\\wallpaper.jpg";
                        string destinationPath = Path.Combine(Environment.CurrentDirectory, newFileName);

                        // Copy and rename
                        File.Copy(transcodedImagePath, destinationPath, true);
                    }
                }
            }
        }
    }
}
