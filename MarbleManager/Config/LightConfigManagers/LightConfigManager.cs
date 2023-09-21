using System.Collections.Generic;
using System.Windows.Forms;

namespace MarbleManager.Config.LightConfigManagers
{
    internal abstract class LightConfigManager
    {
        protected string lightType = "LightConfigDefault";

        protected static int textBoxWidth = 270;

        private FlowLayoutPanel configWrapper;
        private CheckBox checkboxIsEnabled;

        /**
         * Creates all UI controls needed for this light config
         */
        protected abstract List<Control> CreateUIControls(LightConfig _config);

        /**
         * Builds the config object
         */
        protected abstract LightConfig BuildConfigObject();

        /**
         * Wraps and returns the created list of controls for this light type
         */
        internal GroupBox GetLightConfigUI(LightConfig _config)
        {
            // create flow layout panel
            configWrapper = new FlowLayoutPanel();
            configWrapper.AutoSize = true;
            configWrapper.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configWrapper.Dock = DockStyle.Fill;
            configWrapper.FlowDirection = FlowDirection.TopDown;
            configWrapper.WrapContents = false;
            configWrapper.Name = $"flowLayoutPanel_{lightType}Config";

            // create group box
            GroupBox groupBox = new GroupBox();
            groupBox.AutoSize = true;
            groupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox.Dock = DockStyle.Top;
            groupBox.Padding = new Padding(3, 3, 3, 9);
            groupBox.Name = $"groupBox_{lightType}Config";
            groupBox.Text = lightType;

            // create enabled checkbox
            checkboxIsEnabled = new CheckBox();
            checkboxIsEnabled.AutoSize = true;
            checkboxIsEnabled.Name = $"checkboxIsEnabled{lightType}";
            checkboxIsEnabled.Text = "Enabled";
            checkboxIsEnabled.UseVisualStyleBackColor = true;
            checkboxIsEnabled.Checked = (_config != null && _config.enabled);

            // add controls to panel
            configWrapper.Controls.Add(checkboxIsEnabled);
            foreach (Control control in CreateUIControls(_config))
            {
                configWrapper.Controls.Add(control);
            }

            // add panel to box
            groupBox.Controls.Add(configWrapper);

            return groupBox;
        }

        /**
         * Returns the config object created
         */
        internal T GetConfigObject<T>() where T : LightConfig
        {
            return BuildConfigObject() as T;
        }

        internal bool IsEnabled()
        {
            return checkboxIsEnabled.Checked;
        }

        /**
         * finds a control with the given type and name
         */
        protected T FindControl<T>(string _controlName) where T : Control
        {
            Control[] controls = configWrapper.Controls.Find(_controlName, true);
            if (controls != null && controls.Length > 0)
            {
                return controls[0] as T;
            }
            return null;
        }
    }
}
