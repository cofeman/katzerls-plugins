using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FCBot.UI
{
    public partial class UcNumericUpdown : UserControl
    {
        private string _toolTipText;
        private string _settingName;

        // Event stuff when value is changed
        public delegate void LineEventHandler(object sender, NudEventArgs e);
        public event LineEventHandler ValueChanged;
        protected virtual void OnValueChanged(NudEventArgs e) { try { ValueChanged(this, e); } catch (Exception) { } }
        
        public UcNumericUpdown(string label, string settingName, int value)
        {
            Label = label;
            SettingName = settingName;
            Value = value;
        }

        public UcNumericUpdown(string label, string settingName, int value, string tooltipTitle, string tooltipText)
        {
            Label = label;
            SettingName = settingName;
            Value = value;
            TooltipTitle = tooltipTitle;
            TooltipText = tooltipText;
        }

        public UcNumericUpdown()
        {
            InitializeComponent();
        }

        [Category("Data"), Description("Gets or sets the spell associated with this control"), DefaultValue("")]
        public string SpellName { get; set; }

        [Category("Data"), Description("Sets if the the spell exists in the spellmanager"), DefaultValue(true)]
        public bool IsSpellValid
        {
            set { lblText.ForeColor = value ? Color.Black : Color.Silver; }
        }

        [Category("Data")]
        [Description("Gets or sets the label of this control")]
        [DefaultValue("Label")]
        public string Label
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }

        [Category("Data")]
        [Description("Gets or sets the value of this control")]
        [DefaultValue(10)]
        public int Value
        {
            get { return (int)numericUpDown1.Value; }
            set { numericUpDown1.Value = value; }
        }

        [Category("Behavior")]
        [Description("Gets or sets the maximum value allowed")]
        [DefaultValue(100)]
        public int MaxValue
        {
            get { return (int)numericUpDown1.Maximum; }
            set { numericUpDown1.Maximum = value; }
        }

        [Category("Behavior")]
        [Description("Gets or sets the minimum value allowed")]
        [DefaultValue(0)]
        public int MinValue
        {
            get { return (int)numericUpDown1.Minimum; }
            set { numericUpDown1.Minimum = value; }
        }

        [Category("Data")]
        [Description("Gets or sets the associated setting")]
        public string SettingName
        {
            get { return _settingName ?? "No setting or null"; }
            set { _settingName = value; }
        }

        [Category("ToolTip")]
        [Description("Gets or sets the tooltip text")]
        [DefaultValue("Fpsware CC")]
        public string TooltipText
        {
            get { return _toolTipText; }
            set
            {
                if (value == null) return;

                _toolTipText = value;
                toolTip1.SetToolTip(numericUpDown1, _toolTipText);
            }
        }

        [Category("ToolTip")]
        [Description("Gets or sets the title used in the tooltip")]
        public string TooltipTitle
        {
            get { return toolTip1.ToolTipTitle; }
            set
            {
                if (value == null) return;
                toolTip1.ToolTipTitle = value;
            }
        }

        [Category("Misc")]
        [Description("Enables or disables user changes")]
        public new bool Enabled
        {
            get { return numericUpDown1.Enabled; }
            set
            {
                numericUpDown1.Enabled = value;
                lblText.Enabled = value;
            }
        }

        [Category("Misc")]
        [Description("Sets the width of the label, thus reducing the size of the numeric bar")]
        [DefaultValue(110)]
        public int LabelSize
        {
            get { return lblText.Width; }
            set
            {
                lblText.Width = value;
                numericUpDown1.Left = lblText.Width + 9;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Value = (int)numericUpDown1.Value;
            OnValueChanged(new NudEventArgs(Value, SettingName));
        }
       
    }

    public class NudEventArgs : EventArgs
    {
        public NudEventArgs(int value, string settingName)
        {
            SettingName = settingName;
            Value = value;
        }

        public int Value { get; private set; }
        public string SettingName { get; private set; }
    }
}
