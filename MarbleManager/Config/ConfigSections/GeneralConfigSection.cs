using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MarbleManager.Config
{
    internal class GeneralConfigSection : ConfigSection
    {
        private static string checkBoxUseMainSwatches = "checkBoxUseMainSwatches";
        private static string checkBoxSyncOnWallpaperChange = "checkBoxSyncOnWallpaperChange";
        private static string checkBoxAutoTurnOnOff = "checkBoxAutoTurnOnOff";
        private static string checkBoxRunOnBoot = "checkBoxRunOnBoot";
        private static string checkBoxUseLogs = "checkBoxUseLogs";
        private static string checkBoxShowAdvanced = "checkBoxShowAdvanced";
        private static string textBoxHighlightWeights = "textBoxHighlightWeights";

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
            CheckBox advanced = FindControl<CheckBox>(checkBoxShowAdvanced);
            
            TextBox weightsBox = FindControl<TextBox>(textBoxHighlightWeights);
            int[] weights = weightsBox.Text.Split(',').Select(int.Parse).ToArray();

            return new GeneralConfig()
            {
                onlyUseMainSwatches = useMain.Checked,
                syncOnWallpaperChange = syncOnChange.Checked,
                autoTurnLightsOnOff = autoOnOff.Checked,
                runOnBoot = runOnBoot.Checked,
                logUsage = useLogs.Checked,
                showAdvanced = advanced.Checked,
                highlightWeights = weights,
            };
        }

        protected override Control CreateUIControls(ConfigSectionObject _config)
        {
            GeneralConfig generalConfig = (GeneralConfig)_config;

            List<Control> controlsCol1 = new List<Control>();

            // checkBoxUseMainSwatches
            CheckBox checkBoxUseMain = new CheckBox();
            checkBoxUseMain.AutoSize = true;
            checkBoxUseMain.Name = checkBoxUseMainSwatches;
            checkBoxUseMain.Text = "Only use main swatches";
            checkBoxUseMain.Checked = generalConfig.onlyUseMainSwatches;
            controlsCol1.Add(checkBoxUseMain);

            // checkBoxSyncOnWallpaperChange
            CheckBox checkBoxSync = new CheckBox();
            checkBoxSync.AutoSize = true;
            checkBoxSync.Name = checkBoxSyncOnWallpaperChange;
            checkBoxSync.Text = "Sync colours on wallpaper change";
            checkBoxSync.Checked = generalConfig.syncOnWallpaperChange;
            controlsCol1.Add(checkBoxSync);

            // checkBoxAutoTurnOnOff
            CheckBox checkBoxAutoOnOff = new CheckBox();
            checkBoxAutoOnOff.AutoSize = true;
            checkBoxAutoOnOff.Name = checkBoxAutoTurnOnOff;
            checkBoxAutoOnOff.Text = "Turn lights on/off with logon state";
            checkBoxAutoOnOff.Checked = generalConfig.autoTurnLightsOnOff;
            controlsCol1.Add(checkBoxAutoOnOff);

            // checkBoxRunOnBoot
            CheckBox checkBoxBoot = new CheckBox();
            checkBoxBoot.AutoSize = true;
            checkBoxBoot.Name = checkBoxRunOnBoot;
            checkBoxBoot.Text = "Run Marble Manager on boot";
            checkBoxBoot.Checked = generalConfig.runOnBoot;
            controlsCol1.Add(checkBoxBoot);

            List<Control> controlsCol2 = new List<Control>();

            // ADVANCED

            // checkBoxShowAdvanced
            CheckBox checkBoxAdvanced = new CheckBox();
            checkBoxAdvanced.AutoSize = true;
            checkBoxAdvanced.Name = checkBoxShowAdvanced;
            checkBoxAdvanced.Text = "Show advanced settings";
            checkBoxAdvanced.Checked = generalConfig.showAdvanced;
            checkBoxAdvanced.CheckedChanged += new EventHandler(ChangeShowAdvanced);
            controlsCol2.Add(checkBoxAdvanced);

            // add advanced
            controlsCol2.Add(GetAdvancedControls(generalConfig));

            return WrapIn2ColumnTable(controlsCol1, controlsCol2);
        }

        private Control GetAdvancedControls(GeneralConfig _generalConfig)
        {
            // wrapper group box
            GroupBox groupBox = new GroupBox();
            groupBox.Name = "groupBoxAdvanced";
            groupBox.Text = $"Advanced options";
            groupBox.AutoSize = true;
            groupBox.Visible = _generalConfig.showAdvanced;

            // flow layout panel
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.Name = "flowLayoutPanelAdvanced";
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.AutoSize = true;
            flowLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.WrapContents = false;

            // checkBoxUseLogs
            CheckBox checkBoxLogs = new CheckBox();
            checkBoxLogs.AutoSize = true;
            checkBoxLogs.Name = checkBoxUseLogs;
            checkBoxLogs.Text = "Use log file";
            checkBoxLogs.Checked = _generalConfig.logUsage;
            flowLayoutPanel.Controls.Add(checkBoxLogs);

            // highlight weights label
            Label weightsLabel = new Label();
            weightsLabel.AutoSize = true;
            weightsLabel.Name = "labelHighlightWeights";
            weightsLabel.Text = "Highlight weights (sat, lum, prop, comma sep.)";
            flowLayoutPanel.Controls.Add(weightsLabel);

            // selectors text box
            TextBox weightsBox = new TextBox();
            weightsBox.Name = textBoxHighlightWeights;
            weightsBox.Size = new Size(textBoxWidthWrapped, 20);
            weightsBox.Text = (_generalConfig != null && _generalConfig.highlightWeights != null)
                ? string.Join(",", _generalConfig.highlightWeights.Select(f => f.ToString()))
                : string.Empty;
            flowLayoutPanel.Controls.Add(weightsBox);

            // add to controls
            groupBox.Controls.Add(flowLayoutPanel);
            return groupBox;
        }

        private void ChangeShowAdvanced(object sender, EventArgs e)
        {
            // show or hide advanced settings
            CheckBox checkbox = (CheckBox)sender;

            GroupBox advancedWrapper = FindControl<GroupBox>("groupBoxAdvanced");
            if (advancedWrapper != null)
            {
                advancedWrapper.Visible = checkbox.Checked;
            }
        }
    }
}
