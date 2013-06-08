using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FCBot.UI
{
    public partial class UcTextBox : UserControl
    {
        private string _toolTipText;
        private string _settingName;

        // Event stuff when value is changed
        public delegate void LineEventHandler(object sender, TbEventArgs e);
        public event LineEventHandler ValueChanged;
        protected virtual void OnValueChanged(TbEventArgs e) { try { ValueChanged(this, e); } catch (Exception) { } }
        
        public UcTextBox()
        {
            InitializeComponent();
        }
         
        [Category("Data")]
        [Description("Gets or sets the label of this control")]
        [DefaultValue("Label")]
        public string Label
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }

        [Category("Data"), Description("Gets or sets the spell associated with this control"), DefaultValue("")]
        public string SpellName { get; set; }

        [Category("Data"), Description("Sets if the the spell exists in the spellmanager"), DefaultValue(true)]
        public bool IsSpellValid
        {
            set
            {
                lblText.ForeColor = value ? Color.Black : Color.Silver;
            }
        }

        [Category("Data")]
        [DefaultValue("... always")]
        [Description("Gets or sets the selected item. The passed string must exist in the collection")]
        public string Value
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
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
                toolTip1.SetToolTip(textBox1, _toolTipText);
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
            get { return textBox1.Enabled; }
            set
            {
                textBox1.Enabled = value;
                lblText.Enabled = value;
            }
        }

        [Category("Misc")]
        [Description("Sets the width of the label, thus reducing the size of the dropdown box")]
        [DefaultValue(110)]
        public int LabelSize
        {
            get { return lblText.Width; }
            set
            {
                lblText.Width = value;

                if (value > 0)
                {
                    textBox1.Left = lblText.Width + 9;
                    textBox1.Width = (Width - textBox1.Left - 3);
                }
                else
                {
                    textBox1.Left = lblText.Width + 0;
                    textBox1.Width = (Width - textBox1.Left - 0);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            OnValueChanged(new TbEventArgs(Value, SettingName));
        }


    }

    public class TbEventArgs : EventArgs
    {
        public TbEventArgs(string value, string settingName)
        {
            SettingName = settingName;
            Value = value;
        }

        public string Value { get; private set; }
        public string SettingName { get; private set; }
    }

}
