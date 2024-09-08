using Microsoft.Win32;
using MarbleManager.Config;
using System.Drawing;
using System.IO;

namespace MarbleManager.Colours
{
    /**
     * Handles functions related to retrieving the current desktop wallpaper
     */
    internal static class WallpaperManager
    {
        /**
         * Copies the current transcodedWallpaper to a local dir then returns as a bitmap
         * This avoids 'locking' the transcoded file by holding it in memory (eg: when previewed in UI)
         */
        internal static Bitmap GetWallpaperJpg()
        {
            CopyWallpaperToJpg();

            return new Bitmap(PathManager.WallpaperFilePath);
        }

        /**
         * Deletes the local wallpaper copy if exists
         */
        internal static void DeleteCopiedWallpaper()
        {
            Utilities.DeleteFile(PathManager.WallpaperFilePath);
        }

        /**
         * Returns the bitmap of the current transcodedWallpaper file
         * This is fine for quickly applying a palette etc.
         * but don't hold it in memory as it prevents wallpaper changes
         */
        internal static Bitmap GetWallpaperBitmap ()
        {
            string transcodedWallpaperPath = GetTranscodedWallpaperPath();
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
        internal static string GetTranscodedWallpaperPath ()
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
            string transcodedWallpaperPath = GetTranscodedWallpaperPath();
            if (transcodedWallpaperPath != null)
            {
                // Copy and rename
                Directory.CreateDirectory(PathManager.DataOutputDir);
                File.Copy(transcodedWallpaperPath, PathManager.WallpaperFilePath, true);
            }
        }
    }
}
