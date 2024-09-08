using System.Collections.Generic;
using System.Windows.Forms;

namespace MarbleManager.Config
{
    /**
     * Config section for Wiz lights
     */
    internal class WizConfigSection : LightConfigSection
    {
        // UI element identification string
        private static string textBoxWizIpAddresses = "textBoxWizIpAddresses";

        public WizConfigSection() {
            sectionName = "Wiz";
        }

        protected override Control CreateUIControls(ConfigSectionObject _config)
        {
            WizConfig wizConfig = (WizConfig)_config;

            List<Control> controls = new List<Control>
            {
                GetGlobalLightControls(wizConfig)
            };

            // ip adresses label
            Label ipsLabel = new Label();
            ipsLabel.AutoSize = true;
            ipsLabel.Name = "labelWizIpAddresses";
            ipsLabel.Text = "IP Addresses (comma separated)";
            controls.Add(ipsLabel);

            // ip adresses text box
            TextBox ipsBox = new TextBox();
            ipsBox.Name = textBoxWizIpAddresses;
            ipsBox.Size = new System.Drawing.Size(textBoxWidth, 20);
            ipsBox.Text = wizConfig != null ? wizConfig.ipAddresses : string.Empty;
            controls.Add(ipsBox);

            return WrapInFlowPanel(controls);
        }

        protected override LightConfig BuildLightConfig()
        {
            TextBox ipsTextBox = FindControl<TextBox>(textBoxWizIpAddresses);
            return new WizConfig()
            {
                ipAddresses = ipsTextBox != null ? ipsTextBox.Text : string.Empty,
            };
        }
    }
}
