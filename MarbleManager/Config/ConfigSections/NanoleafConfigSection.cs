using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MarbleManager.Config
{
    internal class NanoleafConfigSection : LightConfigSection
    {
        private static string radioButtonEffectPrefix = "radioButtonNanoleafEffect";
        private static string checkBoxOverrideMainColourProb = "checkBoxOverrideMainColourProb";
        private static string numericUpDownColourProb = "numericUpDownColourProb";
        private static string flowLayoutPanelNanoleafLights = "flowLayoutPanelNanoleafLights";

        private int nanoleafLightIndex;

        public NanoleafConfigSection() {
            sectionName = "Nanoleaf";
        }
        protected override Control CreateUIControls(ConfigSectionObject _config)
        {
            NanoleafConfig nanoleafConfig = (NanoleafConfig)_config;

            // column 1
            List<Control> controlsCol1 = new List<Control>
            {
                GetGlobalLightControls(nanoleafConfig)
            };

            // effect label
            Label effectLabel = new Label();
            effectLabel.AutoSize = true;
            effectLabel.Name = "labelNanoleafEffect";
            effectLabel.Text = "Light effect";
            controlsCol1.Add(effectLabel);

            // effect radio boxes
            List<RadioButton> buttons = new List<RadioButton>();
            foreach (NanoleafEffect effect in Enum.GetValues(typeof(NanoleafEffect)))
            {
                RadioButton effectButton = new RadioButton();
                effectButton.AutoSize = true;
                effectButton.Name = $"{radioButtonEffectPrefix}{effect}";
                effectButton.RightToLeft = RightToLeft.No;
                effectButton.Text = $"{effect}";
                effectButton.UseVisualStyleBackColor = true;
                effectButton.TabIndex = (int)effect;
                effectButton.Checked = nanoleafConfig != null && nanoleafConfig.effect == effect;
                effectButton.CheckedChanged += new EventHandler(ChangeSelectedEffect);
                buttons.Add(effectButton);
            }

            // effects wrapper
            FlowLayoutPanel effectPanel = new FlowLayoutPanel();
            effectPanel.AutoSize = true;
            effectPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            effectPanel.Name = "flowLayoutPanelNanoleafEffectWrapper";
            foreach (RadioButton button in buttons)
            {
                effectPanel.Controls.Add(button);
            }
            controlsCol1.Add(effectPanel);

            // extra settings

            // highlight override value checkbox
            CheckBox highlightOverrideCheckBox = new CheckBox();
            highlightOverrideCheckBox.AutoSize = true;
            highlightOverrideCheckBox.Name = checkBoxOverrideMainColourProb;
            highlightOverrideCheckBox.Text = "Override main colour probability";
            highlightOverrideCheckBox.UseVisualStyleBackColor = true;
            highlightOverrideCheckBox.Checked = nanoleafConfig != null && nanoleafConfig.overrideMainColourProb;

            // highlight override value label
            Label highlightOverrideValueLabel = new Label();
            highlightOverrideValueLabel.AutoSize = true;
            highlightOverrideValueLabel.Name = "labelNanoleafHighlightOverride";
            highlightOverrideValueLabel.Text = "Value";

            // highlight override value selector
            NumericUpDown highlightOverrideNumericUpDown = new NumericUpDown();
            highlightOverrideNumericUpDown.Name = numericUpDownColourProb;
            highlightOverrideNumericUpDown.Size = new Size(120, 20);
            highlightOverrideNumericUpDown.Value = nanoleafConfig != null ? nanoleafConfig.mainColourProb : 0;

            // highlight flow panel
            FlowLayoutPanel highlightOptionsPanel = new FlowLayoutPanel();
            highlightOptionsPanel.AutoSize = true;
            highlightOptionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;;
            highlightOptionsPanel.Dock = DockStyle.Fill;
            highlightOptionsPanel.FlowDirection = FlowDirection.TopDown;
            highlightOptionsPanel.Name = "flowLayoutPanelHighlightOptions";
            highlightOptionsPanel.Controls.Add(highlightOverrideCheckBox);
            highlightOptionsPanel.Controls.Add(highlightOverrideValueLabel);
            highlightOptionsPanel.Controls.Add(highlightOverrideNumericUpDown);

            // highlight groupbox
            GroupBox highlightOptionsGroupBox = new GroupBox();
            highlightOptionsGroupBox.AutoSize = true;
            highlightOptionsGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            highlightOptionsGroupBox.Name = "groupBoxHighlightOptions";
            highlightOptionsGroupBox.Text = "Highlight options";
            highlightOptionsGroupBox.Controls.Add(highlightOptionsPanel);
            highlightOptionsGroupBox.Visible = nanoleafConfig != null && nanoleafConfig.effect == NanoleafEffect.Highlight;
            controlsCol1.Add(highlightOptionsGroupBox);

            // column 2
            List<Control> controlsCol2 = new List<Control>();

            // light list flow panel
            FlowLayoutPanel lightListPanel = new FlowLayoutPanel();
            lightListPanel.AutoSize = true;
            lightListPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            lightListPanel.FlowDirection = FlowDirection.TopDown;
            lightListPanel.Margin = new Padding(0);
            lightListPanel.MaximumSize = new Size(270, 0);
            lightListPanel.Name = flowLayoutPanelNanoleafLights;

            // create nanoleaf light ui elements
            nanoleafLightIndex = 0;
            if (nanoleafConfig.lights != null)
            {
                foreach (NanoleafConfig.Light light in nanoleafConfig.lights)
                {
                    lightListPanel.Controls.Add(CreateNanoleafLight(light));
                }
            }
            controlsCol2.Add(lightListPanel);

            // button to add new light
            Button addLightButton = new Button();
            addLightButton.Name = "buttonAddNanoleafLight";
            addLightButton.Text = "Add light";
            addLightButton.UseVisualStyleBackColor = true;
            addLightButton.Click += new EventHandler(AddNewLight);
            controlsCol2.Add(addLightButton);

            return WrapIn2ColumnTable(controlsCol1, controlsCol2);
        }

        protected override LightConfig BuildLightConfig()
        {
            CheckBox overrideCheckbox = FindControl<CheckBox>(checkBoxOverrideMainColourProb);
            NumericUpDown probValueNumericUpDown = FindControl<NumericUpDown>(numericUpDownColourProb);
            return new NanoleafConfig()
            {
                lights = GetNanoleafLights(),
                effect = GetSelectedNanoleafEffect(),
                overrideMainColourProb = overrideCheckbox != null && overrideCheckbox.Checked,
                mainColourProb = probValueNumericUpDown != null ? (int)probValueNumericUpDown.Value : 0,
            };
        }

        /**
         * Adds a new blank nanoleaf light
         */
        private void AddNewLight(object sender, EventArgs e)
        {
            FlowLayoutPanel flowLayoutPanel = FindControl<FlowLayoutPanel>(flowLayoutPanelNanoleafLights);
            if (flowLayoutPanel != null)
            {
                flowLayoutPanel.Controls.Add(CreateNanoleafLight());
            }
        }

        /**
         * Creates a Nanoleaf light config UI element
         */
        private GroupBox CreateNanoleafLight(NanoleafConfig.Light _light = null)
        {
            nanoleafLightIndex++;

            // wrapper group box
            GroupBox groupBox = new GroupBox();
            groupBox.Name = $"groupBoxNanoleaf{nanoleafLightIndex}";
            groupBox.Text = $"Light {nanoleafLightIndex}";
            groupBox.Size = new Size(260, 127);

            // flow layout panel
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.Name = $"flowLayoutPanelNanoleaf{nanoleafLightIndex}";
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.WrapContents = false;

            // Ip label
            Label ipLabel = new Label();
            ipLabel.Name = $"labelNanoleafIp{nanoleafLightIndex}";
            ipLabel.Text = "IP address";
            ipLabel.AutoSize = true;

            // ip textbox
            TextBox ipTextBox = new TextBox();
            ipTextBox.Name = $"textBoxNanoleafIp{nanoleafLightIndex}";
            ipTextBox.Size = new Size(248, 20);
            if (_light != null)
            {
                ipTextBox.Text = _light.ipAddress;
            }

            // api key label
            Label apiKeyLabel = new Label();
            apiKeyLabel.Name = $"labelNanoleafApiKey{nanoleafLightIndex}";
            apiKeyLabel.Text = "API Key";
            apiKeyLabel.AutoSize = true;

            // api key textbox
            TextBox apiKeyTextBox = new TextBox();
            apiKeyTextBox.Name = $"textBoxNanoleafApiKey{nanoleafLightIndex}";
            apiKeyTextBox.Size = new Size(248, 20);
            if (_light != null)
            {
                apiKeyTextBox.Text = _light.apiKey;
            }

            // remove light button
            Button buttonRemoveLight = new Button();
            buttonRemoveLight.Name = $"buttonRemoveNanoleafLight{nanoleafLightIndex}";
            buttonRemoveLight.Size = new Size(63, 23);
            buttonRemoveLight.Text = "Remove";
            buttonRemoveLight.Click += new EventHandler(RemoveNanoleafLight);

            // add controls
            flowLayoutPanel.Controls.Add(ipLabel);
            flowLayoutPanel.Controls.Add(ipTextBox);
            flowLayoutPanel.Controls.Add(apiKeyLabel);
            flowLayoutPanel.Controls.Add(apiKeyTextBox);
            flowLayoutPanel.Controls.Add(buttonRemoveLight);
            groupBox.Controls.Add(flowLayoutPanel);

            return groupBox;
        }

        /**
         * Removes a nanoleaf light config entry
         */
        private void RemoveNanoleafLight(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            // get parent wrapper control
            GroupBox parent = (GroupBox)button.Parent.Parent;
            FlowLayoutPanel flowLayoutPanel = FindControl<FlowLayoutPanel>(flowLayoutPanelNanoleafLights);
            if (flowLayoutPanel != null)
            {
                flowLayoutPanel.Controls.Remove(parent);
            }
        }

        /**
         * Creates a list of nanoleaf light objects from ui
         */
        private List<NanoleafConfig.Light> GetNanoleafLights()
        {
            List<NanoleafConfig.Light> lights = new List<NanoleafConfig.Light>();
            FlowLayoutPanel flowLayoutPanel = FindControl<FlowLayoutPanel>(flowLayoutPanelNanoleafLights);
            if (flowLayoutPanel == null)
            {
                return null;
            }
            foreach (Control control in flowLayoutPanel.Controls)
            {
                // ip address text
                string ip = control.Controls[0].Controls[1].Text;
                string apiKey = control.Controls[0].Controls[3].Text;
                if (ip != null || apiKey != null)
                {
                    lights.Add(new NanoleafConfig.Light()
                    {
                        ipAddress = ip,
                        apiKey = apiKey,
                    });
                }
            }
            return lights;
        }

        /**
         * Toggle visible extra settings
         */
        private void ChangeSelectedEffect(object sender, EventArgs e)
        {
            // this is called from each radio button, when checked or unchecked
            // so: show options when checked for this button
            // hide otherwise
            RadioButton checkBox = (RadioButton)sender;
            string effectName = checkBox.Text;
            GroupBox effectOptions = FindControl<GroupBox>($"groupBox{effectName}Options");
            if (effectOptions != null)
            {
                effectOptions.Visible = checkBox.Checked;
            }
        }

        /**
         * Fetches the currently selected nanoleaf effect enum
         */
        private NanoleafEffect GetSelectedNanoleafEffect()
        {
            foreach (NanoleafEffect effect in Enum.GetValues(typeof(NanoleafEffect)))
            {
                RadioButton effectCheckBox = FindControl<RadioButton>($"{radioButtonEffectPrefix}{effect}");
                if (effectCheckBox != null && effectCheckBox.Checked)
                {
                    return effect;
                }
            }
            // default return Random
            return NanoleafEffect.Random;
        }
    }
}
