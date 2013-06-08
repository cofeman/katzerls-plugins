using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FCBot.UI
{
    public partial class UcProgressBar : UserControl
    {
        private string _toolTipText;
        private int _oldValue;
        private string _settingName;
        private bool _additionalsVisible;

        // Event stuff when value is changed
        public delegate void LineEventHandler(object sender, pbEventArgs e);
        public event LineEventHandler ValueChanged;
        protected virtual void OnValueChanged(pbEventArgs e){ValueChanged(this, e);}

        public delegate void Additionals_EventHandler(object sender, AddingNewEventArgs e);
        public event Additionals_EventHandler AdditionalsButtonPressed;
        protected virtual void OnAdditionalsButtonPressed(AddingNewEventArgs e) { try { AdditionalsButtonPressed(this, e); } catch { } }

        public UcProgressBar(string label, string settingName, int value)
        {
            Label = label;
            SettingName = settingName;
            Value = value;
        }

        public UcProgressBar(string label, string settingName, int value, string tooltipTitle, string tooltipText)
        {
            Label = label;
            SettingName = settingName;
            Value = value;
            TooltipTitle = tooltipTitle;
            TooltipText = tooltipText;
        }

        public UcProgressBar()
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

        [Category("Appearance")]
        [Description("Show or hide the 'additionals' button")]
        [DefaultValue(false)]
        public bool AdditionalsEnabled
        {
            get { return _additionalsVisible; }
            set
            {
                //bool visible = value;
                _additionalsVisible = value;

                if (_additionalsVisible)
                {
                    btnAdditionals.Visible = true;
                    pbValue.Width = ((Width - pbValue.Left - 3) - 37);
                }
                else
                {
                    btnAdditionals.Visible = false;
                    pbValue.Width = ((Width - pbValue.Left - 3));
                }
            }
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
        [DefaultValue(50)]
        public int Value
        {
            get { return pbValue.Value; }
            set { pbValue.Value= value; }
        }

        [Category("Behavior")]
        [Description("Gets or sets the maximum value allowed")]
        [DefaultValue(100)]
        public int MaxValue
        {
            get { return pbValue.Maximum; }
            set { pbValue.Maximum = value; }
        }

        [Category("Behavior")]
        [Description("Gets or sets the minimum value allowed")]
        [DefaultValue(0)]
        public int MinValue
        {
            get { return pbValue.Minimum; }
            set { pbValue.Minimum = value; }
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
                toolTip1.SetToolTip(pbValue, _toolTipText);
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
            get { return pbValue.Enabled; }
            set
            {
                pbValue.Enabled = value;
                lblText.Enabled = value;
            }
        }

        [Category("Misc")]
        [Description("Sets the width of the label, thus reducing the size of the progress bar")]
        [DefaultValue(110)]
        public int LabelSize
        {
            get { return lblText.Width; }
            set
            {
                lblText.Width = value;
                pbValue.Left = lblText.Width + 9;
                pbValue.Width = (Width - pbValue.Left - 3);
            }
        }


        private void CommonProgressBar_MouseAction(object sender, MouseEventArgs e)
        {
            if (_oldValue != pbValue.Value)
            {
                _oldValue = pbValue.Value;
                toolTip1.SetToolTip(pbValue, _toolTipText + "\r\nCurrent Value: " + +pbValue.Value);
            }

            if (e.Button != MouseButtons.Left) return;
            
            pbValue.Value = MousePosValue(e.Location.X, pbValue.Width);
            Value = pbValue.Value;
            
            // Trigger the 'value changed' event
            OnValueChanged(new pbEventArgs(Value,SettingName));

            
        }

        private int MousePosValue(double mousePosition, double progressBarWidth)
        {
            if (mousePosition < 0) mousePosition = 0;
            if (mousePosition > progressBarWidth) mousePosition = progressBarWidth;

            double ratio = mousePosition / progressBarWidth;
            double value = ratio * pbValue.Maximum;

            if (value >= pbValue.Maximum) value = pbValue.Maximum;
            if (value <= pbValue.Minimum) value = pbValue.Minimum;

            return (int)Math.Ceiling(value);
        }

        private void btnAdditionals_Click(object sender, EventArgs e)
        {
            OnAdditionalsButtonPressed(new AddingNewEventArgs(SettingName));
        }

        private void pbValue_Click(object sender, EventArgs e)
        {

        }
        /*
        private int MousePosValue(double mousePosition, double progressBarWidth)
        {
            if (mousePosition < 0) mousePosition = 0;
            if (mousePosition > progressBarWidth) mousePosition = progressBarWidth;

            double ratio = mousePosition / progressBarWidth;
            double value = ratio * 100;

            if (value > 100) value = 100;
            if (value < 0) value = 0;

            return (int)Math.Ceiling(value);
        }
         */



    }

    public class pbEventArgs : EventArgs
    {
        public pbEventArgs(int value, string settingName)
        {
            SettingName = settingName;
            Value = value;
        }

        public int Value { get; private set; }
        public string SettingName { get; private set; }
    }
}
