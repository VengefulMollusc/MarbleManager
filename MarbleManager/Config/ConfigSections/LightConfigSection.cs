using System.Windows.Forms;

namespace MarbleManager.Config
{
    internal abstract class LightConfigSection : ConfigSection
    {
        private CheckBox checkboxIsEnabled;
        private CheckBox checkboxApplyPalette;

        protected override ConfigSectionObject BuildConfigObject()
        {
            LightConfig config = BuildLightConfig();
            config.enabled = checkboxIsEnabled.Checked;
            config.applyPalette = checkboxApplyPalette.Checked;
            return config;
        }

        protected abstract LightConfig BuildLightConfig();

        protected abstract override Control CreateUIControls(ConfigSectionObject _config);

        /**
         * Gets the global light controls for a light config section
         */
        protected FlowLayoutPanel GetGlobalLightControls(LightConfig _config)
        {
            // create flow layout panel
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.AutoSize = true;
            flowLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel.Dock = DockStyle.None;
            flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel.WrapContents = false;
            flowLayoutPanel.Name = $"globalLightControls{sectionName}";

            // create enabled checkbox
            checkboxIsEnabled = new CheckBox();
            checkboxIsEnabled.AutoSize = true;
            checkboxIsEnabled.Name = $"checkboxIsEnabled{sectionName}";
            checkboxIsEnabled.Text = "Enabled";
            checkboxIsEnabled.UseVisualStyleBackColor = true;
            checkboxIsEnabled.Checked = (_config != null && _config.enabled);

            // create apply palette checkbox
            checkboxApplyPalette = new CheckBox();
            checkboxApplyPalette.AutoSize = true;
            checkboxApplyPalette.Name = $"checkboxApplyPalette{sectionName}";
            checkboxApplyPalette.Text = "Apply Palette";
            checkboxApplyPalette.UseVisualStyleBackColor = true;
            checkboxApplyPalette.Checked = (_config != null && _config.applyPalette);

            flowLayoutPanel.Controls.Add(checkboxIsEnabled);
            flowLayoutPanel.Controls.Add(checkboxApplyPalette);

            return flowLayoutPanel;
        }
    }
}
