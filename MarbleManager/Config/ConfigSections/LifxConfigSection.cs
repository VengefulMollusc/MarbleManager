using System.Collections.Generic;
using System.Windows.Forms;

namespace MarbleManager.Config
{
    /**
     * Config section for Lifx lights
     */
    internal class LifxConfigSection : LightConfigSection
    {
        // UI element identification strings
        private static string textBoxLifxSelector = "textBoxLifxSelector";
        private static string textBoxLifxAuthKey = "textBoxLifxAuthKey";

        public LifxConfigSection() {
            sectionName = "Lifx";
        }

        protected override Control CreateUIControls(ConfigSectionObject _config)
        {
            LifxConfig lifxConfig = (LifxConfig)_config;

            List<Control> controls = new List<Control>
            {
                GetGlobalLightControls(lifxConfig)
            };

            // selectors label
            Label selectorsLabel = new Label();
            selectorsLabel.AutoSize = true;
            selectorsLabel.Name = "labelLifxSelectors";
            selectorsLabel.Text = "Selectors (comma separated)";
            controls.Add(selectorsLabel);
            
            // text box for lifx light selectors input
            TextBox selectorsBox = new TextBox();
            selectorsBox.Name = textBoxLifxSelector;
            selectorsBox.Size = new System.Drawing.Size(textBoxWidth, 20);
            selectorsBox.Text = lifxConfig != null ? lifxConfig.selectors : string.Empty;
            controls.Add(selectorsBox);

            // auth key label
            Label authKeyLabel = new Label();
            authKeyLabel.AutoSize = true;
            authKeyLabel.Name = "labelLifxAuthKey";
            authKeyLabel.Text = "Auth key (Bearer)";
            controls.Add(authKeyLabel);

            // text box for lifx auth key
            TextBox authKeyBox = new TextBox();
            authKeyBox.Name = textBoxLifxAuthKey;
            authKeyBox.Size = new System.Drawing.Size(textBoxWidth, 20);
            authKeyBox.Text = lifxConfig != null ? lifxConfig.authKey : string.Empty;
            controls.Add(authKeyBox);

            return WrapInFlowPanel(controls);
        }

        protected override LightConfig BuildLightConfig()
        {
            TextBox selectorsTextBox = FindControl<TextBox>(textBoxLifxSelector);
            TextBox authKeyTextBox = FindControl<TextBox>(textBoxLifxAuthKey);
            return new LifxConfig()
            {
                selectors = selectorsTextBox != null ? selectorsTextBox.Text : string.Empty,
                authKey = authKeyTextBox != null ? authKeyTextBox.Text : string.Empty,
            };
        }
    }
}
