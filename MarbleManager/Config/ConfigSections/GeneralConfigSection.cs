﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MarbleManager.Config
{
    /**
     * Displays general settings not specific to any light type
     */
    internal class GeneralConfigSection : ConfigSection
    {
        // UI element identification strings
        private static string checkBoxUseMainSwatches = "checkBoxUseMainSwatches";
        private static string checkBoxSyncOnWallpaperChange = "checkBoxSyncOnWallpaperChange";
        private static string checkBoxAutoTurnOnOff = "checkBoxAutoTurnOnOff";
        private static string checkBoxRunOnBoot = "checkBoxRunOnBoot";
        private static string checkBoxUseLogs = "checkBoxUseLogs";

        public GeneralConfigSection() {
            sectionName = "General";
        }

        protected override ConfigSectionObject BuildConfigObject()
        {
            CheckBox useMain = FindControl<CheckBox>(checkBoxUseMainSwatches);
            CheckBox syncOnChange = FindControl<CheckBox>(checkBoxSyncOnWallpaperChange);
            CheckBox autoOnOff = FindControl<CheckBox>(checkBoxAutoTurnOnOff);
            CheckBox runOnBoot = FindControl<CheckBox>(checkBoxRunOnBoot);
            CheckBox useLogs = FindControl<CheckBox>(checkBoxUseLogs);

            return new GeneralConfig()
            {
                onlyUseMainSwatches = useMain.Checked,
                syncOnWallpaperChange = syncOnChange.Checked,
                autoTurnLightsOnOff = autoOnOff.Checked,
                runOnBoot = runOnBoot.Checked,
                logUsage = useLogs.Checked,
            };
        }

        protected override Control CreateUIControls(ConfigSectionObject _config)
        {
            GeneralConfig generalConfig = (GeneralConfig)_config;

            List<Control> controlsCol1 = new List<Control>();

            // Toggle to use only main swatches
            CheckBox checkBoxUseMain = new CheckBox();
            checkBoxUseMain.AutoSize = true;
            checkBoxUseMain.Name = checkBoxUseMainSwatches;
            checkBoxUseMain.Text = "Only use main swatches";
            checkBoxUseMain.Checked = generalConfig.onlyUseMainSwatches;
            controlsCol1.Add(checkBoxUseMain);

            // Toggle to auto-sync palette when wallpaper changes
            CheckBox checkBoxSync = new CheckBox();
            checkBoxSync.AutoSize = true;
            checkBoxSync.Name = checkBoxSyncOnWallpaperChange;
            checkBoxSync.Text = "Sync colours on wallpaper change";
            checkBoxSync.Checked = generalConfig.syncOnWallpaperChange;
            controlsCol1.Add(checkBoxSync);

            // Toggle to turn lights on and off with logon state
            CheckBox checkBoxAutoOnOff = new CheckBox();
            checkBoxAutoOnOff.AutoSize = true;
            checkBoxAutoOnOff.Name = checkBoxAutoTurnOnOff;
            checkBoxAutoOnOff.Text = "Turn lights on/off with logon state";
            checkBoxAutoOnOff.Checked = generalConfig.autoTurnLightsOnOff;
            controlsCol1.Add(checkBoxAutoOnOff);

            // Toggle to run this program when PC boots
            CheckBox checkBoxBoot = new CheckBox();
            checkBoxBoot.AutoSize = true;
            checkBoxBoot.Name = checkBoxRunOnBoot;
            checkBoxBoot.Text = "Run Marble Manager on boot";
            checkBoxBoot.Checked = generalConfig.runOnBoot;
            controlsCol1.Add(checkBoxBoot);

            List<Control> controlsCol2 = new List<Control>();

            // Toggle whether to use extended log file
            CheckBox checkBoxLogs = new CheckBox();
            checkBoxLogs.AutoSize = true;
            checkBoxLogs.Name = checkBoxUseLogs;
            checkBoxLogs.Text = "Use log file";
            checkBoxLogs.Checked = generalConfig.logUsage;
            controlsCol2.Add(checkBoxLogs);

            return WrapIn2ColumnTable(controlsCol1, controlsCol2);
        }

        /**
         * Shows or hides advanced settings when toggled
         */
        private void ChangeShowAdvanced(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;

            GroupBox advancedWrapper = FindControl<GroupBox>("groupBoxAdvanced");
            if (advancedWrapper != null)
            {
                advancedWrapper.Visible = checkbox.Checked;
            }
        }
    }
}
