﻿using MarbleManager.Lights;
using MarbleManager.Properties;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MarbleManager
{
    /**
     * Controls notification tray context menu actions and program state when config UI is closed
     */
    internal class CustomApplicationContext : ApplicationContext 
    {
        NotifyIcon notifyIcon;
        Container components;
        string iconTooltip = "Marble Manager";

        ConfigForm configForm;

        GlobalLightController lightController;

        public CustomApplicationContext() {
            InitializeContext();

            lightController = GlobalLightController.GetInstance(true, true);
        }

        /**
         * setup the app context
         */
        private void InitializeContext()
        {
            components = new Container();
            // setup notification tray icon
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Resources.programIcon,
                Text = iconTooltip,
                Visible = true 
            };

            // open config on double-click
            notifyIcon.DoubleClick += openConfigFormItem_Click;
            
            //// setup a submenu
            //ToolStripMenuItem submenuItem = new ToolStripMenuItem()
            //{
            //    Text = "Extra options",
            //};
            //submenuItem.DropDownItems.Add("Config form 2", null, openConfigFormItem_Click);
            //notifyIcon.ContextMenuStrip.Items.Add(submenuItem);

            // Add items to context strip
            notifyIcon.ContextMenuStrip.Items.Add("Sync to wallpaper", null, syncFormItem_Click);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("Lights On", null, lightsOnFormItem_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Lights Off", null, lightsOffFormItem_Click);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("Config", null, openConfigFormItem_Click);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add("&Exit", null, exitItem_Click);
        }

        private async void lightsOnFormItem_Click(object sender, EventArgs e)
        {
            await lightController.TurnLightsOnOff(true);
        }

        private async void lightsOffFormItem_Click(object sender, EventArgs e)
        {
            await lightController.TurnLightsOnOff(false);
        }

        private async void syncFormItem_Click(object sender, EventArgs e)
        {
            await lightController.SyncToWallpaper();
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
                configForm.Icon = Resources.programIcon;
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
