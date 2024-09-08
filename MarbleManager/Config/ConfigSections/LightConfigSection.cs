using System.Windows.Forms;

namespace MarbleManager.Config
{
    /**
     * A subtype of ConfigSection specifically for Lights
     * 
     * includes some global controls that are used with all light types
     */
    internal abstract class LightConfigSection : ConfigSection
    {
        // is this light enabled
        private CheckBox checkboxIsEnabled;
        // apply colour/s to this light
        private CheckBox checkboxApplyPalette;

        protected override ConfigSectionObject BuildConfigObject()
        {
            // build light-specific config then add global fields
            LightConfig config = BuildLightConfig();
            config.enabled = checkboxIsEnabled.Checked;
            config.applyPalette = checkboxApplyPalette.Checked;
            return config;
        }

        /**
         * Build the config specific to the child light class type
         */
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

            // Toggle for if this light is enabled
            checkboxIsEnabled = new CheckBox();
            checkboxIsEnabled.AutoSize = true;
            checkboxIsEnabled.Name = $"checkboxIsEnabled{sectionName}";
            checkboxIsEnabled.Text = "Enabled";
            checkboxIsEnabled.UseVisualStyleBackColor = true;
            checkboxIsEnabled.Checked = (_config != null && _config.enabled);

            // Toggle for if this light should apply colours
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
