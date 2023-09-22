using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarbleManager.Config.LightConfigManagers
{
    internal class WizConfigManager : LightConfigManager
    {
        private static string textBoxWizIpAddresses = "textBoxWizIpAddresses";

        public WizConfigManager() {
            lightType = "Wiz";
        }

        protected override Control CreateUIControls(LightConfig _config)
        {
            WizConfig wizConfig = (WizConfig)_config;

            List<Control> controls = new List<Control>();

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

            return WrapInFlowPanel(controls, _config);
        }

        protected override LightConfig BuildConfigObject()
        {
            TextBox ipsTextBox = FindControl<TextBox>(textBoxWizIpAddresses);
            return new WizConfig()
            {
                enabled = IsEnabled(),
                ipAddresses = ipsTextBox != null ? ipsTextBox.Text : string.Empty,
            };
        }
    }
}
