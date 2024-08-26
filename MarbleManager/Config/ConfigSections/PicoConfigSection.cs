using System.Collections.Generic;
using System.Windows.Forms;

namespace MarbleManager.Config
{
    internal class PicoConfigSection : LightConfigSection
    {
        private static string textBoxPicoIpAddresses = "textBoxPicoIpAddresses";

        public PicoConfigSection() {
            sectionName = "Raspberry Pi Pico";
        }

        protected override Control CreateUIControls(ConfigSectionObject _config)
        {
            PicoConfig picoConfig = (PicoConfig)_config;

            List<Control> controls = new List<Control>
            {
                GetGlobalLightControls(picoConfig)
            };

            // ip adresses label
            Label ipsLabel = new Label();
            ipsLabel.AutoSize = true;
            ipsLabel.Name = "labelPicoIpAddresses";
            ipsLabel.Text = "IP Addresses (comma separated)";
            controls.Add(ipsLabel);

            // ip adresses text box
            TextBox ipsBox = new TextBox();
            ipsBox.Name = textBoxPicoIpAddresses;
            ipsBox.Size = new System.Drawing.Size(textBoxWidth, 20);
            ipsBox.Text = picoConfig != null ? picoConfig.ipAddresses : string.Empty;
            controls.Add(ipsBox);

            return WrapInFlowPanel(controls);
        }

        protected override LightConfig BuildLightConfig()
        {
            TextBox ipsTextBox = FindControl<TextBox>(textBoxPicoIpAddresses);
            return new PicoConfig()
            {
                ipAddresses = ipsTextBox != null ? ipsTextBox.Text : string.Empty,
            };
        }
    }
}
