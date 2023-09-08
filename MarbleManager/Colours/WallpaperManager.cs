using Microsoft.Win32;
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
        static string wallpaperFilePath = "output\\wallpaper.jpg";

        // copy from transcoded file to local directory
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

        // delete copied wallpaper file
        public static void DeleteCopiedWallpaper()
        {
            try
            {
                // Check if the file exists before attempting to delete it
                if (File.Exists(wallpaperFilePath))
                {
                    // Delete the file
                    File.Delete(wallpaperFilePath);
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

        // get wallpaper directly from the transcoded image file
        // using this prevents wallpaper change as the image is in memory
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
