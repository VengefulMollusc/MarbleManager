using MarbleManager.Colours;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleManager.Config
{
    internal class WallpaperWatcher : IDisposable
    {
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
                await Task.Delay(500);
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
