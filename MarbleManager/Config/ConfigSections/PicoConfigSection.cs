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
        private static string checkboxPicoJuiceColours = "checkboxPicoJuiceColours";

        public PicoConfigSection() {
            sectionName = "Raspberry Pi Pico";
        }

        protected override Control CreateUIControls(ConfigSectionObject _config)
        {
            PicoConfig picoConfig = (PicoConfig)_config;

            List<Control> controlsCol1 = new List<Control>
            {
                GetGlobalLightControls(picoConfig)
            };

            // ip adresses label
            Label ipsLabel = new Label();
            ipsLabel.AutoSize = true;
            ipsLabel.Name = "labelPicoIpAddresses";
            ipsLabel.Text = "IP Addresses (comma separated)";
            controlsCol1.Add(ipsLabel);

            // ip adresses text box
            TextBox ipsBox = new TextBox();
            ipsBox.Name = textBoxPicoIpAddresses;
            ipsBox.Size = new Size(textBoxWidth, 20);
            ipsBox.Text = picoConfig != null ? picoConfig.ipAddresses : string.Empty;
            controlsCol1.Add(ipsBox);

            // column 2
            List<Control> controlsCol2 = new List<Control>();

            // Brightness label
            Label brightnessLabel = new Label();
            brightnessLabel.AutoSize = true;
            brightnessLabel.Name = "labelPicoBrightness";
            brightnessLabel.Text = "Brightness (1-255)";
            controlsCol2.Add(brightnessLabel);

            // highlight override value selector
            NumericUpDown brightnessNumericUpDown = new NumericUpDown();
            brightnessNumericUpDown.Name = numericUpDownPicoBrightness;
            brightnessNumericUpDown.Size = new Size(120, 20);
            brightnessNumericUpDown.Minimum = 1;
            brightnessNumericUpDown.Maximum = 255;
            brightnessNumericUpDown.Value = picoConfig != null ? picoConfig.brightness : 15;
            controlsCol2.Add(brightnessNumericUpDown);

            // Juice colours checkbox
            CheckBox checkboxJuiceColours = new CheckBox();
            checkboxJuiceColours.AutoSize = true;
            checkboxJuiceColours.Name = checkboxPicoJuiceColours;
            checkboxJuiceColours.Text = "Juice Colours";
            checkboxJuiceColours.UseVisualStyleBackColor = true;
            checkboxJuiceColours.Checked = (picoConfig != null && picoConfig.juiceColours);
            controlsCol2.Add(checkboxJuiceColours);

            return WrapIn2ColumnTable(controlsCol1, controlsCol2);
        }

        protected override LightConfig BuildLightConfig()
        {
            TextBox ipsTextBox = FindControl<TextBox>(textBoxPicoIpAddresses);
            NumericUpDown brightnessNumericUpDown = FindControl<NumericUpDown>(numericUpDownPicoBrightness);
            CheckBox juiceCheckbox = FindControl<CheckBox>(checkboxPicoJuiceColours);
            return new PicoConfig()
            {
                ipAddresses = ipsTextBox != null ? ipsTextBox.Text : string.Empty,
                brightness = brightnessNumericUpDown != null ? (int)brightnessNumericUpDown.Value : 15,
                juiceColours = juiceCheckbox != null && juiceCheckbox.Checked
            };
        }
    }
}
