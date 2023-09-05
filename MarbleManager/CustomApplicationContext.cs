using MarbleManager.LightScripts;
using MarbleManager.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarbleManager
{
    internal class CustomApplicationContext : ApplicationContext 
    {
        NotifyIcon notifyIcon;
        Container components;
        string iconTooltip = "Marble Manager";

        ConfigForm configForm;

        // TODO swap this for a single script manager
        WallpaperManager wallpaperManager;

        public CustomApplicationContext() {
            InitializeContext();

            // init actual scripts etc.
            wallpaperManager = new WallpaperManager();
        }

        private void InitializeContext()
        {
            components = new Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Resources.programIcon,
                Text = iconTooltip,
                Visible = true 
            };

            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            // I assume could add menu items here if static
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // open config form on double click
            openConfigFormItem_Click(sender, e);
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            // clear items
            notifyIcon.ContextMenuStrip.Items.Clear();

            // setup a submenu
            ToolStripMenuItem submenuItem = new ToolStripMenuItem()
            {
                Text = "Extra options",
            };
            submenuItem.DropDownItems.Add("Config form 2", null, openConfigFormItem_Click);

            // dynamically add items to context strip
            notifyIcon.ContextMenuStrip.Items.Add(submenuItem);
            notifyIcon.ContextMenuStrip.Items.Add("Config", null, openConfigFormItem_Click);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("&Exit", null, exitItem_Click);
        }

        private void openConfigFormItem_Click(object sender, EventArgs e)
        {
            if (configForm != null)
            {
                configForm.Activate();
            } else
            {
                configForm = new ConfigForm();
                configForm.AddWallpaperManager(wallpaperManager);
                configForm.FormClosed += ConfigForm_FormClosed;
                configForm.Show();
            }
        }

        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            configForm = null;
        }

        private void exitItem_Click(object sender, EventArgs e)
        {
            ExitThread();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) { components.Dispose(); }
        }

        protected override void ExitThreadCore()
        {
            if (configForm != null) { configForm.Close(); }
            notifyIcon.Visible = false; // should remove lingering tray icon
            base.ExitThreadCore();
        }
    }
}
