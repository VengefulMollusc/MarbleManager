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
        FileSystemWatcher watcher;

        public delegate void ChangeEventHandler(object source, FileSystemEventArgs e);

        public event ChangeEventHandler OnChange;

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
            // delay for 1/2 second before syncing to allow file to free up
            await Task.Delay(500);
            OnChange?.Invoke(source, e);
        }

        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}
