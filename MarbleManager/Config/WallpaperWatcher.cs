using MarbleManager.Colours;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MarbleManager.Config
{
    /**
     * Watches the transcoded desktop wallpaper file for changes
     * Triggers given functions when wallpaper is changed
     */
    internal class WallpaperWatcher : IDisposable
    {
        // delay to allow file to settle and avoid duplicate calls
        private static int delay = 1000;

        public delegate void ChangeEventHandler(object source, FileSystemEventArgs e);
        public event ChangeEventHandler OnChange;

        FileSystemWatcher watcher;
        bool isProcessing = false;

        public WallpaperWatcher() { 
            watcher = new FileSystemWatcher();

            string transcodedWallpaperDir = Path.GetDirectoryName(WallpaperManager.GetTranscodedWallpaperPath());

            watcher.Path = transcodedWallpaperDir;
            watcher.Filter = "transcodedWallpaper";
            watcher.Changed += TriggerOnChanged;

            watcher.EnableRaisingEvents = true;
        }

        private async void TriggerOnChanged(object source, FileSystemEventArgs e)
        {
            // check if method is already processing
            if (isProcessing) return;

            isProcessing = true;

            try
            {
                // delay before syncing to allow file to free up
                await Task.Delay(delay);
                OnChange?.Invoke(source, e);
            }
            finally
            {
                isProcessing = false;
            }
        }

        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
