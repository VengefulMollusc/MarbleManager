using System.Collections.Generic;
using System.Windows.Forms;

namespace MarbleManager.Config
{
    internal abstract class ConfigSection
    {
        protected string sectionName = "ConfigSectionDefault";

        protected static int textBoxWidth = 270;
        protected static int textBoxWidthWrapped = 248;

        private GroupBox configWrapper;

        /**
         * Creates all UI controls needed for this light config
         */
        protected abstract Control CreateUIControls(ConfigSectionObject _config);

        /**
         * Builds the config object
         */
        protected abstract ConfigSectionObject BuildConfigObject();

        /**
         * Wraps and returns the created list of controls for this light type
         */
        internal GroupBox GetConfigUI(ConfigSectionObject _config)
        {
            // create group box
            configWrapper = new GroupBox();
            configWrapper.AutoSize = true;
            configWrapper.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configWrapper.Dock = DockStyle.Top;
            configWrapper.Padding = new Padding(3, 3, 3, 9);
            configWrapper.Name = $"groupBox_{sectionName}Config";
            configWrapper.Text = sectionName;

            // add controls to box
            configWrapper.Controls.Add(CreateUIControls(_config));

            return configWrapper;
        }

        /**
         * Returns the config object created
         */
        internal T GetConfigObject<T>() where T : ConfigSectionObject
        {
            return BuildConfigObject() as T;
        }

        /**
         * Wraps controls in a single FlowLayoutPanel
         */
        protected FlowLayoutPanel WrapInFlowPanel(List<Control> _controls)
        {
            // create flow layout panel
            FlowLayoutPanel flowLayoutPanel = CreateFlowPanel($"flowLayoutPanel_{sectionName}Config");

            if (_controls != null)
            {
                foreach (Control control in _controls)
                {
                    flowLayoutPanel.Controls.Add(control);
                }
            }

            return flowLayoutPanel;
        }

        /**
         * Wraps controls in a table with 2 columns
         */
        protected TableLayoutPanel WrapIn2ColumnTable(List<Control> _col1, List<Control> _col2)
        {
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Name = $"tableLayoutPanel_{sectionName}Config";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // create column panels
            FlowLayoutPanel col1FlowPanel = CreateFlowPanel($"tableLayoutPanel_{sectionName}Config_Col1");
            FlowLayoutPanel col2FlowPanel = CreateFlowPanel($"tableLayoutPanel_{sectionName}Config_Col2");

            // add controls to column 1
            if (_col1 != null)
            {
                foreach (Control control in _col1)
                {
                    col1FlowPanel.Controls.Add(control);
                }
            }
            // add controls to column 2
            if (_col2 != null)
            {
                foreach (Control control in _col2)
                {
                    col2FlowPanel.Controls.Add(control);
                }
            }

            // add column flow panels to table
            tableLayoutPanel.Controls.Add(col1FlowPanel, 0, 0);
            tableLayoutPanel.Controls.Add(col2FlowPanel, 1, 0);
            return tableLayoutPanel;
        }

        private FlowLayoutPanel CreateFlowPanel(string _name)
        {
            // create flow layout panel
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.AutoSize = true;
            flowLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.WrapContents = false;
            flowLayoutPanel.Name = _name;
            return flowLayoutPanel;
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
