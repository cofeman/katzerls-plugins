using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Styx;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace FCBot.UI
{
    public partial class UIForm : Form
    {
        private bool _isLoading = true;

        public UIForm() { InitializeComponent(); }

        private void uiForm_Load(object sender, EventArgs e)
        {
            PopulateUIControls();
            PopulateDynamicLeader();
            
            
            _isLoading = false;
        }

        private void PopulateDynamicLeader()
        {
            // populate our dynamic leader dropdown list
            ucDropdown1.cbDropdown.Items.Clear();
            ucDropdown1.cbDropdown.Items.Add("[None]");
            ucDropdown1.cbDropdown.Items.Add("[Automatic]");

            try
            {
                IOrderedEnumerable<WoWPlayer> units = ObjectManager.GetObjectsOfTypeFast<WoWPlayer>().Where(u => u.IsAlive && u.IsFriendly && u.Distance < 100 && u.IsPlayer).OrderBy(u => u.Name);
                foreach (WoWPlayer unit in units.Where(unit => unit.Guid != StyxWoW.Me.Guid))
                {
                    ucDropdown1.cbDropdown.Items.Add(unit.Name);
                }

                ucDropdown1.cbDropdown.SelectedItem = Setting.Instance.FollowLeader;
                if (ucDropdown1.cbDropdown.SelectedItem == null || (string)ucDropdown1.cbDropdown.SelectedItem == "")
                {
                    ucDropdown1.Value = "[Automatic]";
                }
            }
            catch (Exception e) { }

        }

        #region Standard UI population stuff
        public void PopulateUIControls()
        {
            SuspendLayout();

            Type type = typeof(Setting); PropertyInfo[] settingsProperties = type.GetProperties();
            foreach (PropertyInfo setting in settingsProperties)
            {
                PropertyInfo pInfo = type.GetProperty(setting.Name);
                object propValue = pInfo.GetValue(Setting.Instance, null);

                foreach (TabPage tab in tabControl1.Controls)
                    //foreach (GroupBox gb in tab.Controls)
                    foreach (object group in tab.Controls)
                        if (group is GroupBox)
                        {
                            GroupBox gb = (GroupBox)group;
                            foreach (object obj in gb.Controls)
                            {
                                // Populate all combo boxes, this will be the most common
                                UcDropdown dropdown = obj as UcDropdown;
                                if (dropdown != null)
                                {
                                    //UcDropdown cbox = dropdown;

                                    if (!string.IsNullOrEmpty(dropdown.SpellName))
                                    {
                                        dropdown.IsSpellValid = SpellManager.HasSpell(dropdown.SpellName);
                                    }

                                    dropdown.ValueChanged += ucControl_ValueChanged;
                                    if (dropdown.SettingName == setting.Name && dropdown.Value != propValue.ToString())
                                    {
                                        dropdown.Value = propValue.ToString();
                                        break;
                                    }
                                }

                                // Populate all progress bars, mostly for health or mana 
                                UcProgressBar bar = obj as UcProgressBar;
                                if (bar != null)
                                {
                                    //ucProgressBar pbar = bar;

                                    if (!string.IsNullOrEmpty(bar.SpellName))
                                    {
                                        bar.IsSpellValid = SpellManager.HasSpell(bar.SpellName);
                                    }

                                    bar.ValueChanged += ucControl_ValueChanged;

                                    if (bar.SettingName == pInfo.Name)
                                    {
                                        bar.Value = (int) propValue;
                                        break;
                                    }
                                }

                                // Numeric up / down
                                UcNumericUpdown num = obj as UcNumericUpdown;
                                if (num != null)
                                {
                                    //ucNumericUpdown nuCount = num;

                                    if (!string.IsNullOrEmpty(num.SpellName))
                                    {
                                        num.IsSpellValid = SpellManager.HasSpell(num.SpellName);
                                    }

                                    num.ValueChanged += ucControl_ValueChanged;

                                    if (num.SettingName == pInfo.Name)
                                    {
                                        num.Value = (int) propValue;
                                        break;
                                    }
                                }

                                // Text box
                                UcTextBox box = obj as UcTextBox;
                                if (box != null)
                                {
                                    if (!string.IsNullOrEmpty(box.SpellName))
                                    {
                                        box.IsSpellValid = SpellManager.HasSpell(box.SpellName);
                                    }

                                    box.ValueChanged += ucControl_ValueChanged;

                                    if (box.SettingName == pInfo.Name)
                                    {
                                        box.Value = (string) propValue;
                                        break;
                                    }
                                }
                            }
                        }
            }

            _isLoading = false;
            ResumeLayout();
        }

        private void ucControl_ValueChanged(object sender, pbEventArgs e)
        {
            // Basically enumerate the setting class and find the Property that matches the name of 'e.SettingName' and assign the value of 'e.Value'
            // This saves us creating code for every item / setting and adds reusability

            if (_isLoading) return;

            Type type = typeof(Setting); PropertyInfo[] settingsProperties = type.GetProperties();
            foreach (PropertyInfo setting in settingsProperties.Where(setting => e.SettingName == setting.Name))
            {
                setting.SetValue(Setting.Instance, e.Value, null);
                //Log.Info(string.Format("===== settings value has changed. now {0} value: {1}", e.SettingName, e.Value));
            }
        }

        private void ucControl_ValueChanged(object sender, NudEventArgs e)
        {
            // Basically enumerate the setting class and find the Property that matches the name of 'e.SettingName' and assign the value of 'e.Value'
            // This saves us creating code for every item / setting and adds reusability

            if (_isLoading) return;

            Type type = typeof(Setting); PropertyInfo[] settingsProperties = type.GetProperties();
            foreach (PropertyInfo setting in settingsProperties.Where(setting => e.SettingName == setting.Name))
            {
                setting.SetValue(Setting.Instance, e.Value, null);
            }
        }

        private void ucControl_ValueChanged(object sender, cbEventArgs e)
        {
            // Basically enumerate thesetting class and find the Property that matches the name of 'e.SettingName' and assign the value of 'e.Value'
            // This saves us creating code for every item / setting and adds reusability

            if (_isLoading) return;

            Type type = typeof(Setting); PropertyInfo[] settingsProperties = type.GetProperties();
            foreach (PropertyInfo setting in settingsProperties.Where(setting => e.SettingName == setting.Name))
            {
                setting.SetValue(Setting.Instance, e.Value, null);

            }
        }

        private void ucControl_ValueChanged(object sender, TbEventArgs e)
        {
            // Basically enumerate thesetting class and find the Property that matches the name of 'e.SettingName' and assign the value of 'e.Value'
            // This saves us creating code for every item / setting and adds reusability

            if (_isLoading) return;

            Type type = typeof(Setting); PropertyInfo[] settingsProperties = type.GetProperties();
            foreach (PropertyInfo setting in settingsProperties.Where(setting => e.SettingName == setting.Name))
            {
                setting.SetValue(Setting.Instance, e.Value, null);

            }
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            Setting.Instance.Save();
            Close();
        }

    }

    
}
