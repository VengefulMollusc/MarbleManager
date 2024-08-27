using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace MarbleManager.Config
{
    internal class PicoConfigSection : LightConfigSection
    {
        private static string textBoxPicoIpAddresses = "textBoxPicoIpAddresses";
        private static string numericUpDownPicoBrightness = "numericUpDownPicoBrightness";

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
            ipsBox.Size = new Size(textBoxWidth, 20);
            ipsBox.Text = picoConfig != null ? picoConfig.ipAddresses : string.Empty;
            controls.Add(ipsBox);

            // Brightness label
            Label brightnessLabel = new Label();
            brightnessLabel.AutoSize = true;
            brightnessLabel.Name = "labelPicoBrightness";
            brightnessLabel.Text = "Brightness (1-255)";
            controls.Add(brightnessLabel);

            // highlight override value selector
            NumericUpDown brightnessNumericUpDown = new NumericUpDown();
            brightnessNumericUpDown.Name = numericUpDownPicoBrightness;
            brightnessNumericUpDown.Size = new Size(120, 20);
            brightnessNumericUpDown.Minimum = 1;
            brightnessNumericUpDown.Maximum = 255;
            brightnessNumericUpDown.Value = picoConfig != null ? picoConfig.brightness : 15;
            controls.Add(brightnessNumericUpDown);

            return WrapInFlowPanel(controls);
        }

        protected override LightConfig BuildLightConfig()
        {
            TextBox ipsTextBox = FindControl<TextBox>(textBoxPicoIpAddresses);
            NumericUpDown brightnessNumericUpDown = FindControl<NumericUpDown>(numericUpDownPicoBrightness);
            return new PicoConfig()
            {
                ipAddresses = ipsTextBox != null ? ipsTextBox.Text : string.Empty,
                brightness = brightnessNumericUpDown != null ? (int)brightnessNumericUpDown.Value : 15
            };
        }
    }
}
