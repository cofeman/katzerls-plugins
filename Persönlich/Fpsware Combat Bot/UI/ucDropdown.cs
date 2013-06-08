using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FCBot.UI
{
    public partial class UcDropdown : UserControl
    {
        private string _toolTipText;
        private string _settingName;
        private bool _additionalsVisible;
        //private bool _expanded = false;
        //private Color _backColour;
        private Collection<string> _dropdownItems = new Collection<string>();
        //private List<string> _additionalsDisabledItems = new List<string>();
        private Collection<string> _additionalsEnabledItems = new Collection<string>();

        // Event stuff when value is changed
        public delegate void DropDownSelectedItem_EventHandler(object sender, cbEventArgs e);
        public event DropDownSelectedItem_EventHandler ValueChanged;
        protected virtual void OnValueChanged(cbEventArgs e) { try { ValueChanged(this, e);} catch{}}

        public delegate void Additionals_EventHandler(object sender, AddingNewEventArgs e);
        public event Additionals_EventHandler AdditionalsButtonPressed;
        protected virtual void OnAdditionalsButtonPressed(AddingNewEventArgs e) { try { AdditionalsButtonPressed(this, e); } catch{} }
        
        public UcDropdown()
        {
            InitializeComponent();

            
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
                    cbDropdown.Width = ((Width - cbDropdown.Left - 3) - 37);
                }
                else
                {
                    btnAdditionals.Visible = false;
                    cbDropdown.Width = ((Width - cbDropdown.Left - 3));
                }
            }
        }

        [Category("Data")]
        [Description("Set the list of items in the dropdown")]
        [DefaultValue("Label")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<string> DropDownItems
        {
            get { return _dropdownItems; }
        }

        
        [Category("Data")]
        [Description("Set the list of items from the dropdown that will enable the 'additionals' button")]
        [DefaultValue("Label")]
        //[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<string> AdditionalsEnabledItemsList
        {
            get { return _additionalsEnabledItems; }
            set { _additionalsEnabledItems = value; }
        }
         

        /*
        [Category("Data")]
        [Description("Set the list of items from the dropdown that will disable the 'additionals' button")]
        [DefaultValue("Label")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        public List<string> AdditionalsDisabledItemsList
        {
            get { return _additionalsDisabledItems; }
            set { _additionalsDisabledItems = value; }
        }
         */
        

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

        //private bool _spellExists = true;
        [Category("Data"), Description("Sets if the the spell exists in the spellmanager"), DefaultValue(true)]
        public bool IsSpellValid
        {
            //get { return _spellExists; }
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
            get
            {
                if (cbDropdown.Items.Count <= 0) return "";
                if (cbDropdown.SelectedItem == null) return "";

                return cbDropdown.SelectedItem.ToString();
            }
            set
            {
                if (cbDropdown.Items.Count <= 0)
                {
                    foreach (string s in _dropdownItems)
                    {
                        cbDropdown.Items.Add(s);
                    }
                }
                cbDropdown.SelectedItem = value;
            }
        }

        [Category("Data")]
        [Description("Gets or sets the associated setting")]
        public string SettingName
        {
            get { return _settingName ?? "No setting or null"; }
            set { _settingName = value; }
        }

        [Category("Data")]
        [Description("Sorts the items in alphabetical order")]
        public bool SortItems
        {
            get { return cbDropdown.Sorted; }
            set { cbDropdown.Sorted = value; }
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
                toolTip1.SetToolTip(cbDropdown, _toolTipText);
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
            get { return cbDropdown.Enabled; }
            set
            {
                cbDropdown.Enabled = value;
                btnAdditionals.Enabled = value;
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
                    cbDropdown.Left = lblText.Width + 9;
                    cbDropdown.Width = (Width - cbDropdown.Left - 3);
                }
                else
                {
                    cbDropdown.Left = lblText.Width + 0;
                    cbDropdown.Width = (Width - cbDropdown.Left - 0);
                }
            }
        }



        private void cbDropdown_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Trigger the 'value changed' event
            /*bool visibilityChanged = false;
            foreach (string s in AdditionalsDisabledItemsList.Where(s => cbDropdown.SelectedItem.ToString() == s))
            {
                AdditionalsEnabled = false;
                visibilityChanged = true;
                break;
            }

            if (!visibilityChanged)
             */
            {
                foreach (string s in AdditionalsEnabledItemsList)
                {
                    if (cbDropdown.SelectedItem.ToString() == s)
                    {
                        AdditionalsEnabled = true;
                        break;
                    }
                    else
                    {
                        AdditionalsEnabled = false;
                    }
                }
            }
            OnValueChanged(new cbEventArgs(Value, SettingName));
        }

        private void ucDropdown_Load(object sender, System.EventArgs e)
        {
            //cbDropdown.Items.Clear();
            if (cbDropdown.Items.Count == 0)
            foreach (string s in _dropdownItems)
            {
                cbDropdown.Items.Add(s);
            }

        }

        private void btnAdditionals_Click(object sender, System.EventArgs e)
        {
            OnAdditionalsButtonPressed(new AddingNewEventArgs(SettingName));

            /*
            if (!_expanded)
            {
                _backColour = this.BackColor;
                //BackColor = Color.FromArgb(64, 64, 64);
                _expanded = true;
            }
            else
            {
                this.BackColor = _backColour;
                this.Height = 27;
                _expanded = false;
            }
             */
        }
    }

    public class cbEventArgs : System.EventArgs
    {
        public cbEventArgs(string value, string settingName)
        {
            SettingName = settingName;
            Value = value;
        }

        public string Value { get; private set; }
        public string SettingName { get; private set; }
    }

    public class AdditionalsEventArgs : System.EventArgs
    {
        public AdditionalsEventArgs(string settingName)
        {
            SettingName = settingName;
            //Value = value;
        }

        ///public string Value { get; private set; }
        public string SettingName { get; private set; }
    }
}
