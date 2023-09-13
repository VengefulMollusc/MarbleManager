using MarbleManager.Config;
using MarbleManager.Colours;
using MarbleManager.Lights;
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

        GlobalLightController lightController;

        public CustomApplicationContext() {
            InitializeContext();

            lightController = new GlobalLightController();
        }

        /**
         * setup the app context
         */
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

        /**
         * creates the context menu strip items
         */
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
            notifyIcon.ContextMenuStrip.Items.Add("Sync to wallpaper", null, syncFormItem_Click);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("Lights On", null, lightsOnFormItem_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Lights Off", null, lightsOffFormItem_Click);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("Config", null, openConfigFormItem_Click);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("&Exit", null, exitItem_Click);
        }

        /**
         * Opens config form on double click
         */
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // open config form on double click
            openConfigFormItem_Click(sender, e);
        }

        private void lightsOnFormItem_Click(object sender, EventArgs e)
        {
            lightController.TurnLightsOnOff(true);
        }

        private void lightsOffFormItem_Click(object sender, EventArgs e)
        {
            lightController.TurnLightsOnOff(false);
        }

        private void syncFormItem_Click(object sender, EventArgs e)
        {
            lightController.SyncToWallpaper();
        }

        /**
         * opens the config form
         */
        private void openConfigFormItem_Click(object sender, EventArgs e)
        {
            if (configForm != null)
            {
                configForm.Activate();
            }
            else
            {
                configForm = new ConfigForm();
                configForm.FormClosed += ConfigForm_FormClosed;
                configForm.SetLightController(lightController);
                configForm.Show();
            }
        }

        /**
         * disposes of the config form when closed to free up memory
         */
        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            configForm?.Dispose();
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
