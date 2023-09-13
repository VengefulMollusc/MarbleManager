using Microsoft.Win32;
using MarbleManager.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Colours
{
    internal static class WallpaperManager
    {
        static string wallpaperFileName = "wallpaper.jpg";

        static string WallpaperPath
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, ConfigManager.DataDirectory, wallpaperFileName);
            }
        }

        /**
         * Copies the current transcodedWallpaper to a local dir then returns as a bitmap
         * This avoids 'locking' the transcoded file by holding it in memory (eg: when previewed in UI)
         */
        internal static Bitmap GetWallpaperJpg()
        {
            CopyWallpaperToJpg();

            return new Bitmap(WallpaperPath);
        }

        /**
         * Deletes the local wallpaper copy if exists
         */
        internal static void DeleteCopiedWallpaper()
        {
            try
            {
                // Check if the file exists before attempting to delete it
                if (File.Exists(WallpaperPath))
                {
                    // Delete the file
                    File.Delete(WallpaperPath);
                    Console.WriteLine("File deleted successfully.");
                }
                else
                {
                    Console.WriteLine("File does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        /**
         * Returns the bitmap of the current transcodedWallpaper file
         * This is fine for quickly applying a palette etc.
         * but don't hold it in memory as it prevents wallpaper changes
         */
        internal static Bitmap GetWallpaperBitmap ()
        {
            string transcodedWallpaperPath = GetWallpaperPath();
            if (transcodedWallpaperPath != null)
            {
                // return transcoded wallpaper as a bitmap
                return new Bitmap(transcodedWallpaperPath);
            }
            return null;
        }

        /**
         * Returns the path to the transcodedWallpaper file
         */
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

        /**
         * Copies the transcodedWallpaper to a local directory
         */
        private static void CopyWallpaperToJpg()
        {
            string transcodedWallpaperPath = GetWallpaperPath();
            if (transcodedWallpaperPath != null)
            {
                // Copy and rename
                File.Copy(transcodedWallpaperPath, WallpaperPath, true);
            }
        }
    }
}
