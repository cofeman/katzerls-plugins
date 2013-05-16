using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Windows.Media;
using System.Net;
using System.Globalization;


using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins;
using Styx.Pathing;

namespace ProfileChanger
{
    public partial class ProfileChangerUI : Form
    {
        public ProfileChangerUI()
        {
            InitializeComponent();
            CB1.Checked = ProfileChanger.Settings.Active1;
            CB2.Checked = ProfileChanger.Settings.Active2;
            CB3.Checked = ProfileChanger.Settings.Active3;
            CB4.Checked = ProfileChanger.Settings.Active4;
            CB5.Checked = ProfileChanger.Settings.Active5;
            CB6.Checked = ProfileChanger.Settings.Active6;
            TBMinutes1.Text = ProfileChanger.Settings.Minutes1;
            TBMinutes2.Text = ProfileChanger.Settings.Minutes2;
            TBMinutes3.Text = ProfileChanger.Settings.Minutes3;
            TBMinutes4.Text = ProfileChanger.Settings.Minutes4;
            TBMinutes5.Text = ProfileChanger.Settings.Minutes5;
            TBMinutes6.Text = ProfileChanger.Settings.Minutes6;
            TBProfile1.Text = ProfileChanger.Settings.Profile1;
            TBProfile2.Text = ProfileChanger.Settings.Profile2;
            TBProfile3.Text = ProfileChanger.Settings.Profile3;
            TBProfile4.Text = ProfileChanger.Settings.Profile4;
            TBProfile5.Text = ProfileChanger.Settings.Profile5;
            TBProfile6.Text = ProfileChanger.Settings.Profile6;

            CB7.Checked = ProfileChanger.Settings.Active7;
            CB8.Checked = ProfileChanger.Settings.Active8;
            CB9.Checked = ProfileChanger.Settings.Active9;
            CB10.Checked = ProfileChanger.Settings.Active10;
            CB11.Checked = ProfileChanger.Settings.Active11;
            CB12.Checked = ProfileChanger.Settings.Active12;
            TBMinutes7.Text = ProfileChanger.Settings.Minutes7;
            TBMinutes8.Text = ProfileChanger.Settings.Minutes8;
            TBMinutes9.Text = ProfileChanger.Settings.Minutes9;
            TBMinutes10.Text = ProfileChanger.Settings.Minutes10;
            TBMinutes11.Text = ProfileChanger.Settings.Minutes11;
            TBMinutes12.Text = ProfileChanger.Settings.Minutes12;
            TBProfile7.Text = ProfileChanger.Settings.Profile7;
            TBProfile8.Text = ProfileChanger.Settings.Profile8;
            TBProfile9.Text = ProfileChanger.Settings.Profile9;
            TBProfile10.Text = ProfileChanger.Settings.Profile10;
            TBProfile11.Text = ProfileChanger.Settings.Profile11;
            TBProfile12.Text = ProfileChanger.Settings.Profile12;

            CBStopBot.Checked = ProfileChanger.Settings.StopBot;

        }

        private void BSearchFile1_Click(object sender, EventArgs e)
        {
            var loadProfile1 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile1.ShowDialog() == DialogResult.OK)
            {
                string fileName1 = loadProfile1.FileName;
                if (!string.IsNullOrEmpty(fileName1))
                {
                    TBProfile1.Text = fileName1;
                }
            }
        }

        private void BSearchFile2_Click(object sender, EventArgs e)
        {
            var loadProfile2 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile2.ShowDialog() == DialogResult.OK)
            {
                string fileName2 = loadProfile2.FileName;
                if (!string.IsNullOrEmpty(fileName2))
                {
                    TBProfile2.Text = fileName2;
                }
            }

        }

        private void BSearchFile3_Click(object sender, EventArgs e)
        {
            var loadProfile3 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile3.ShowDialog() == DialogResult.OK)
            {
                string fileName3 = loadProfile3.FileName;
                if (!string.IsNullOrEmpty(fileName3))
                {
                    TBProfile3.Text = fileName3;
                }
            }
        }

        private void BSearchFile4_Click(object sender, EventArgs e)
        {
            var loadProfile4 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile4.ShowDialog() == DialogResult.OK)
            {
                string fileName4 = loadProfile4.FileName;
                if (!string.IsNullOrEmpty(fileName4))
                {
                    TBProfile4.Text = fileName4;
                }
            }
        }

        private void BSearchFile5_Click(object sender, EventArgs e)
        {
            var loadProfile5 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile5.ShowDialog() == DialogResult.OK)
            {
                string fileName5 = loadProfile5.FileName;
                if (!string.IsNullOrEmpty(fileName5))
                {
                    TBProfile5.Text = fileName5;
                }
            }
        }

        private void BSearchFile6_Click_1(object sender, EventArgs e)
        {
            var loadProfile6 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile6.ShowDialog() == DialogResult.OK)
            {
                string fileName6 = loadProfile6.FileName;
                if (!string.IsNullOrEmpty(fileName6))
                {
                    TBProfile6.Text = fileName6;
                }
            }
        }

        private void BSearchFile7_Click(object sender, EventArgs e)
        {
            var loadProfile7 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile7.ShowDialog() == DialogResult.OK)
            {
                string fileName7 = loadProfile7.FileName;
                if (!string.IsNullOrEmpty(fileName7))
                {
                    TBProfile7.Text = fileName7;
                }
            }
        }

        private void BSearchFile8_Click(object sender, EventArgs e)
        {
            var loadProfile8 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile8.ShowDialog() == DialogResult.OK)
            {
                string fileName8 = loadProfile8.FileName;
                if (!string.IsNullOrEmpty(fileName8))
                {
                    TBProfile6.Text = fileName8;
                }
            }
        }

        private void BSearchFile9_Click(object sender, EventArgs e)
        {
            var loadProfile9 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile9.ShowDialog() == DialogResult.OK)
            {
                string fileName9 = loadProfile9.FileName;
                if (!string.IsNullOrEmpty(fileName9))
                {
                    TBProfile6.Text = fileName9;
                }
            }
        }

        private void BSearchFile10_Click(object sender, EventArgs e)
        {
            var loadProfile10 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile10.ShowDialog() == DialogResult.OK)
            {
                string fileName10 = loadProfile10.FileName;
                if (!string.IsNullOrEmpty(fileName10))
                {
                    TBProfile6.Text = fileName10;
                }
            }
        }

        private void BSearchFile11_Click(object sender, EventArgs e)
        {
            var loadProfile11 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile11.ShowDialog() == DialogResult.OK)
            {
                string fileName11 = loadProfile11.FileName;
                if (!string.IsNullOrEmpty(fileName11))
                {
                    TBProfile6.Text = fileName11;
                }
            }
        }

        private void BSearchFile12_Click(object sender, EventArgs e)
        {
            var loadProfile12 = new OpenFileDialog
            {
                Filter = "Xml files | *.xml",
                Title = "Select xml file to load"
            };

            if (loadProfile12.ShowDialog() == DialogResult.OK)
            {
                string fileName12 = loadProfile12.FileName;
                if (!string.IsNullOrEmpty(fileName12))
                {
                    TBProfile6.Text = fileName12;
                }
            }
        }

        private void CB1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (CB1.Checked == true)
            {
                TBProfile1.Text = ProfileChanger.Settings.Profile1;
                TBProfile1.Enabled = true;
                TBMinutes1.Text = ProfileChanger.Settings.Minutes1;
                TBMinutes1.Enabled = true;
                CB2.Enabled = true;
            }
            else
            {
                TBProfile1.Text = "";
                TBProfile1.Enabled = false;
                TBMinutes1.Text = "0";
                TBMinutes1.Enabled = false;
                CB2.Checked = false;
                CB2.Enabled = false;
            }
        }
        private void CB2_CheckedChanged(object sender, EventArgs e)
        {
            if (CB2.Checked == true)
            {
                TBProfile2.Text = ProfileChanger.Settings.Profile2;
                TBProfile2.Enabled = true;
                TBMinutes2.Text = ProfileChanger.Settings.Minutes2;
                TBMinutes2.Enabled = true;
                CB3.Enabled = true;
            }
            else
            {
                TBProfile2.Text = "";
                TBProfile2.Enabled = false;
                TBMinutes2.Text = "0";
                TBMinutes2.Enabled = false;
                CB3.Checked = false;
                CB3.Enabled = false;
            }
        }
        private void CB3_CheckedChanged_1(object sender, EventArgs e)
        {
            if (CB3.Checked == true)
            {
                TBProfile3.Text = ProfileChanger.Settings.Profile3;
                TBProfile3.Enabled = true;
                TBMinutes3.Text = ProfileChanger.Settings.Minutes3;
                TBMinutes3.Enabled = true;
                CB4.Enabled = true;
            }
            else
            {
                TBProfile3.Text = "";
                TBProfile3.Enabled = false;
                TBMinutes3.Text = "0";
                TBMinutes3.Enabled = false;
                CB4.Checked = false;
                CB4.Enabled = false;
            }
        }

        private void CB4_CheckedChanged_1(object sender, EventArgs e)
        {
            if (CB4.Checked == true)
            {
                TBProfile4.Text = ProfileChanger.Settings.Profile4;
                TBProfile4.Enabled = true;
                TBMinutes4.Text = ProfileChanger.Settings.Minutes4;
                TBMinutes4.Enabled = true;
                CB5.Enabled = true;
            }
            else
            {
                TBProfile4.Text = "";
                TBProfile4.Enabled = false;
                TBMinutes4.Text = "0";
                TBMinutes4.Enabled = false;
                CB5.Checked = false;
                CB5.Enabled = false;
            }
        }
        private void CB5_CheckedChanged_1(object sender, EventArgs e)
        {
            if (CB5.Checked == true)
            {
                TBProfile5.Text = ProfileChanger.Settings.Profile5;
                TBProfile5.Enabled = true;
                TBMinutes5.Text = ProfileChanger.Settings.Minutes5;
                TBMinutes5.Enabled = true;
                CB6.Enabled = true;
            }
            else
            {
                TBProfile5.Text = "";
                TBProfile5.Enabled = false;
                TBMinutes5.Text = "0";
                TBMinutes5.Enabled = false;
                CB6.Checked = false;
                CB6.Enabled = false;
            }
        }
        private void CB6_CheckedChanged_1(object sender, EventArgs e)
        {
            if (CB6.Checked == true)
            {
                TBProfile6.Text = ProfileChanger.Settings.Profile6;
                TBProfile6.Enabled = true;
                TBMinutes6.Text = ProfileChanger.Settings.Minutes6;
                TBMinutes6.Enabled = true;
                CB7.Enabled = true;
            }
            else
            {
                TBProfile6.Text = "";
                TBProfile6.Enabled = false;
                TBMinutes6.Text = "0";
                TBMinutes6.Enabled = false;
                CB7.Checked = false;
                CB7.Enabled = false;
            }
        }

        private void CB7_CheckedChanged(object sender, EventArgs e)
        {
            if (CB7.Checked == true)
            {
                TBProfile7.Text = ProfileChanger.Settings.Profile7;
                TBProfile7.Enabled = true;
                TBMinutes7.Text = ProfileChanger.Settings.Minutes7;
                TBMinutes7.Enabled = true;
                CB8.Enabled = true;
            }
            else
            {
                TBProfile7.Text = "";
                TBProfile7.Enabled = false;
                TBMinutes7.Text = "0";
                TBMinutes7.Enabled = false;
                CB8.Checked = false;
                CB8.Enabled = false;
            }
        }

        private void CB8_CheckedChanged(object sender, EventArgs e)
        {
            if (CB8.Checked == true)
            {
                TBProfile8.Text = ProfileChanger.Settings.Profile8;
                TBProfile8.Enabled = true;
                TBMinutes8.Text = ProfileChanger.Settings.Minutes8;
                TBMinutes8.Enabled = true;
                CB9.Enabled = true;
            }
            else
            {
                TBProfile8.Text = "";
                TBProfile8.Enabled = false;
                TBMinutes8.Text = "0";
                TBMinutes8.Enabled = false;
                CB9.Checked = false;
                CB9.Enabled = false;
            }
        }

        private void CB9_CheckedChanged(object sender, EventArgs e)
        {
            if (CB9.Checked == true)
            {
                TBProfile9.Text = ProfileChanger.Settings.Profile9;
                TBProfile9.Enabled = true;
                TBMinutes9.Text = ProfileChanger.Settings.Minutes9;
                TBMinutes9.Enabled = true;
                CB10.Enabled = true;
            }
            else
            {
                TBProfile9.Text = "";
                TBProfile9.Enabled = false;
                TBMinutes9.Text = "0";
                TBMinutes9.Enabled = false;
                CB10.Checked = false;
                CB10.Enabled = false;
            }
        }

        private void CB10_CheckedChanged(object sender, EventArgs e)
        {
            if (CB10.Checked == true)
            {
                TBProfile10.Text = ProfileChanger.Settings.Profile10;
                TBProfile10.Enabled = true;
                TBMinutes10.Text = ProfileChanger.Settings.Minutes10;
                TBMinutes10.Enabled = true;
                CB11.Enabled = true;
            }
            else
            {
                TBProfile10.Text = "";
                TBProfile10.Enabled = false;
                TBMinutes10.Text = "0";
                TBMinutes10.Enabled = false;
                CB11.Checked = false;
                CB11.Enabled = false;
            }
        }

        private void CB11_CheckedChanged(object sender, EventArgs e)
        {
            if (CB11.Checked == true)
            {
                TBProfile11.Text = ProfileChanger.Settings.Profile11;
                TBProfile11.Enabled = true;
                TBMinutes11.Text = ProfileChanger.Settings.Minutes11;
                TBMinutes11.Enabled = true;
                CB12.Enabled = true;
            }
            else
            {
                TBProfile11.Text = "";
                TBProfile11.Enabled = false;
                TBMinutes11.Text = "0";
                TBMinutes11.Enabled = false;
                CB12.Checked = false;
                CB12.Enabled = false;
            }

        }

        private void CB12_CheckedChanged(object sender, EventArgs e)
        {
            if (CB12.Checked == true)
            {
                TBProfile12.Text = ProfileChanger.Settings.Profile12;
                TBProfile12.Enabled = true;
                TBMinutes12.Text = ProfileChanger.Settings.Minutes12;
                TBMinutes12.Enabled = true;
                //CB13.Enabled = true;
            }
            else
            {
                TBProfile12.Text = "";
                TBProfile12.Enabled = false;
                TBMinutes12.Text = "0";
                TBMinutes12.Enabled = false;
                //CB13.Checked = false;
                //CB13.Enabled = false;
            }
        }

        private void BSave_Click(object sender, EventArgs e)
        {
            //----------------- Save Configfile and set Settings ---------------- 
            string Folder = "Settings\\";

            XmlDocument xml;
            XmlElement root;
            XmlElement element;
            XmlText text;
            XmlComment xmlComment;

            string sPath = Process.GetCurrentProcess().MainModule.FileName;
            sPath = Path.GetDirectoryName(sPath);

            // ---------- Check Plausibility of the Config ----------------------------         
            if (CB1.Checked && (TBProfile1.Text == "" || TBMinutes1.Text == "0" || TBMinutes1.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 1 deactivated - no valid Settings.");
                CB1.Checked = false;
            }
            if (CB2.Checked && (TBProfile2.Text == "" || TBMinutes2.Text == "0" || TBMinutes2.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 2 deactivated - no valid Settings.");
                CB2.Checked = false;
            }
            if (CB3.Checked && (TBProfile3.Text == "" || TBMinutes3.Text == "0" || TBMinutes3.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 3 deactivated - no valid Settings.");
                CB3.Checked = false;
            }
            if (CB4.Checked && (TBProfile4.Text == "" || TBMinutes4.Text == "0" || TBMinutes4.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 4 deactivated - no valid Settings.");
                CB4.Checked = false;
            }
            if (CB5.Checked && (TBProfile5.Text == "" || TBMinutes5.Text == "0" || TBMinutes5.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 5 deactivated - no valid Settings.");
                CB5.Checked = false;
            }
            if (CB6.Checked && (TBProfile6.Text == "" || TBMinutes6.Text == "0" || TBMinutes6.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 6 deactivated - no valid Settings.");
                CB6.Checked = false;
            }

            if (CB7.Checked && (TBProfile7.Text == "" || TBMinutes7.Text == "0" || TBMinutes7.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 7 deactivated - no valid Settings.");
                CB7.Checked = false;
            }
            if (CB8.Checked && (TBProfile8.Text == "" || TBMinutes8.Text == "0" || TBMinutes8.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 8 deactivated - no valid Settings.");
                CB8.Checked = false;
            }
            if (CB9.Checked && (TBProfile9.Text == "" || TBMinutes9.Text == "0" || TBMinutes9.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 9 deactivated - no valid Settings.");
                CB9.Checked = false;
            }
            if (CB10.Checked && (TBProfile10.Text == "" || TBMinutes10.Text == "0" || TBMinutes10.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 10 deactivated - no valid Settings.");
                CB10.Checked = false;
            }
            if (CB11.Checked && (TBProfile11.Text == "" || TBMinutes11.Text == "0" || TBMinutes11.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 11 deactivated - no valid Settings.");
                CB11.Checked = false;
            }

            if (CB12.Checked && (TBProfile12.Text == "" || TBMinutes12.Text == "0" || TBMinutes12.Text == ""))
            {
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: Line 12 deactivated - no valid Settings.");
                CB12.Checked = false;
            }


            ProfileChanger.Settings.Active1 = CB1.Checked;
            ProfileChanger.Settings.Active2 = CB2.Checked;
            ProfileChanger.Settings.Active3 = CB3.Checked;
            ProfileChanger.Settings.Active4 = CB4.Checked;
            ProfileChanger.Settings.Active5 = CB5.Checked;
            ProfileChanger.Settings.Active6 = CB6.Checked;
            ProfileChanger.Settings.Minutes1 = TBMinutes1.Text;
            ProfileChanger.Settings.Minutes2 = TBMinutes2.Text;
            ProfileChanger.Settings.Minutes3 = TBMinutes3.Text;
            ProfileChanger.Settings.Minutes4 = TBMinutes4.Text;
            ProfileChanger.Settings.Minutes5 = TBMinutes5.Text;
            ProfileChanger.Settings.Minutes6 = TBMinutes6.Text;
            ProfileChanger.Settings.Profile1 = TBProfile1.Text;
            ProfileChanger.Settings.Profile2 = TBProfile2.Text;
            ProfileChanger.Settings.Profile3 = TBProfile3.Text;
            ProfileChanger.Settings.Profile4 = TBProfile4.Text;
            ProfileChanger.Settings.Profile5 = TBProfile5.Text;
            ProfileChanger.Settings.Profile6 = TBProfile6.Text;

            ProfileChanger.Settings.Active7 = CB7.Checked;
            ProfileChanger.Settings.Active8 = CB8.Checked;
            ProfileChanger.Settings.Active9 = CB9.Checked;
            ProfileChanger.Settings.Active10 = CB10.Checked;
            ProfileChanger.Settings.Active11 = CB11.Checked;
            ProfileChanger.Settings.Active12 = CB12.Checked;
            ProfileChanger.Settings.Minutes7 = TBMinutes7.Text;
            ProfileChanger.Settings.Minutes8 = TBMinutes8.Text;
            ProfileChanger.Settings.Minutes9 = TBMinutes9.Text;
            ProfileChanger.Settings.Minutes10 = TBMinutes10.Text;
            ProfileChanger.Settings.Minutes11 = TBMinutes11.Text;
            ProfileChanger.Settings.Minutes12 = TBMinutes12.Text;
            ProfileChanger.Settings.Profile7 = TBProfile7.Text;
            ProfileChanger.Settings.Profile8 = TBProfile8.Text;
            ProfileChanger.Settings.Profile9 = TBProfile9.Text;
            ProfileChanger.Settings.Profile10 = TBProfile10.Text;
            ProfileChanger.Settings.Profile11 = TBProfile11.Text;
            ProfileChanger.Settings.Profile12 = TBProfile12.Text;

            ProfileChanger.Settings.StopBot = CBStopBot.Checked;

            // ---------- Save XML-Config-File ---------------------------- 
            sPath = Path.Combine(sPath, Folder);

            if (!Directory.Exists(sPath))
            {
                Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Creating config directory");
                Directory.CreateDirectory(sPath);
            }

            sPath = Path.Combine(sPath, StyxWoW.Me.RealmName, StyxWoW.Me.Name, "ProfileChanger.config");

            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Settings Saved");
            xml = new XmlDocument();
            XmlDeclaration dc = xml.CreateXmlDeclaration("1.0", "utf-8", null);
            xml.AppendChild(dc);

            xmlComment = xml.CreateComment(
                "=======================================================================\n" +
                ".CONFIG  -  This is the Config File For ProfileChanger\n\n" +
                "XML file containing settings to customize in the ProfileChanger Plugin\n" +
                "It is STRONGLY recommended you use the Configuration UI to change this\n" +
                "file instead of direct changein it here.\n" +
                "========================================================================");

            //let's add the root element
            root = xml.CreateElement("ProfileChanger");
            root.AppendChild(xmlComment);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active1");
            text = xml.CreateTextNode(CB1.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile1");
            text = xml.CreateTextNode(TBProfile1.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes1");
            text = xml.CreateTextNode(TBMinutes1.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active2");
            text = xml.CreateTextNode(CB2.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile2");
            text = xml.CreateTextNode(TBProfile2.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes2");
            text = xml.CreateTextNode(TBMinutes2.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active3");
            text = xml.CreateTextNode(CB3.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile3");
            text = xml.CreateTextNode(TBProfile3.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes3");
            text = xml.CreateTextNode(TBMinutes3.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active4");
            text = xml.CreateTextNode(CB4.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile4");
            text = xml.CreateTextNode(TBProfile4.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes4");
            text = xml.CreateTextNode(TBMinutes4.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active5");
            text = xml.CreateTextNode(CB5.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile5");
            text = xml.CreateTextNode(TBProfile5.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes5");
            text = xml.CreateTextNode(TBMinutes5.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active6");
            text = xml.CreateTextNode(CB6.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile6");
            text = xml.CreateTextNode(TBProfile6.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes6");
            text = xml.CreateTextNode(TBMinutes6.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active7");
            text = xml.CreateTextNode(CB7.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile7");
            text = xml.CreateTextNode(TBProfile7.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes7");
            text = xml.CreateTextNode(TBMinutes7.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active8");
            text = xml.CreateTextNode(CB8.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile8");
            text = xml.CreateTextNode(TBProfile8.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes8");
            text = xml.CreateTextNode(TBMinutes8.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active9");
            text = xml.CreateTextNode(CB9.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile9");
            text = xml.CreateTextNode(TBProfile9.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes9");
            text = xml.CreateTextNode(TBMinutes9.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active10");
            text = xml.CreateTextNode(CB10.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile10");
            text = xml.CreateTextNode(TBProfile10.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes10");
            text = xml.CreateTextNode(TBMinutes10.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active11");
            text = xml.CreateTextNode(CB11.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile11");
            text = xml.CreateTextNode(TBProfile11.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes11");
            text = xml.CreateTextNode(TBMinutes11.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Active12");
            text = xml.CreateTextNode(CB12.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Profile12");
            text = xml.CreateTextNode(TBProfile12.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Minutes12");
            text = xml.CreateTextNode(TBMinutes12.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("StopBot");
            text = xml.CreateTextNode(CBStopBot.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            xml.AppendChild(root);

            System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.Create,
                                                               System.IO.FileAccess.Write);
            try
            {
                xml.Save(fs);
                fs.Close();
            }
            catch (Exception np)
            {
                Logging.WriteDiagnostic(Colors.Red, np.Message);
            }

        }
    }
}
