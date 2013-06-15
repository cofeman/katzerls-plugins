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
using System.Net;
using System.Globalization;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins;
using Styx.Pathing;
using ObjectManager = Styx.WoWInternals.ObjectManager;

namespace ProfileChanger
{
    class ProfileChangerSettings
    {

        public static LocalPlayer Me = StyxWoW.Me;
		string FileFolder = "Settings\\";
        public ProfileChangerSettings()
        {
            if (Me != null)
                try
                {
                    Load();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                }
        }

		public bool Active1 = false;
        public bool Active2 = false;
        public bool Active3 = false;
        public bool Active4 = false;
        public bool Active5 = false;
        public bool Active6 = false;
        public string Profile1 = "";
        public string Profile2 = "";
        public string Profile3 = "";
        public string Profile4 = "";
        public string Profile5 = "";
        public string Profile6 = "";
        public string Minutes1 = "0";
        public string Minutes2 = "0";
        public string Minutes3 = "0";
        public string Minutes4 = "0";
        public string Minutes5 = "0";
        public string Minutes6 = "0";
        
        public bool Active7 = false;
        public bool Active8 = false;
        public bool Active9 = false;
        public bool Active10 = false;
        public bool Active11 = false;
        public bool Active12 = false;
        public string Profile7 = "";
        public string Profile8 = "";
        public string Profile9 = "";
        public string Profile10 = "";
        public string Profile11 = "";
        public string Profile12 = "";
        public string Minutes7 = "0";
        public string Minutes8 = "0";
        public string Minutes9 = "0";
        public string Minutes10 = "0";
        public string Minutes11 = "0";
        public string Minutes12 = "0";

        public string MinutesMax1 = "0";
        public string MinutesMax2 = "0";
        public string MinutesMax3 = "0";
        public string MinutesMax4 = "0";
        public string MinutesMax5 = "0";
        public string MinutesMax6 = "0";
        public string MinutesMax7 = "0";
        public string MinutesMax8 = "0";
        public string MinutesMax9 = "0";
        public string MinutesMax10 = "0";
        public string MinutesMax11 = "0";
        public string MinutesMax12 = "0";

        public bool StopBot = false;
        public bool RandomProfile = false;
        public bool RandomTime = false;

		// -------------- Load ConfigFile ---------------
        public void Load()
        {
            //    XmlTextReader reader;
            XmlDocument xml = new XmlDocument();
            XmlNode xvar;

            string sPath = Process.GetCurrentProcess().MainModule.FileName;
            sPath = Path.GetDirectoryName(sPath);
            sPath = Path.Combine(sPath, FileFolder);

            if (!Directory.Exists(sPath))
            {
                Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Creating config directory");
                Directory.CreateDirectory(sPath);
            }

            sPath = Path.Combine(sPath, StyxWoW.Me.RealmName, StyxWoW.Me.Name, "ProfileChanger.config");

            Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Loading config file");
            if (!File.Exists(sPath))
            {
                Logging.WriteDiagnostic("Profile Changer: No Special Config - Continuing with Default Config Values");
                return;
            }
            System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            try
            {
                xml.Load(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                Logging.WriteDiagnostic(Colors.Red, e.Message);
                fs.Close();
                return;
            }

            //            xml = new XmlDocument();

            try
            {
                //                xml.Load(reader);
                if (xml == null)
                    return;
                // Load Variables
                xvar = xml.SelectSingleNode("//ProfileChanger/Active1");
                if (xvar != null)
                {
                    Active1 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active1.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Profile1");
                if (xvar != null)
                {
                    Profile1 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile1.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Minutes1");
                if (xvar != null)
                {
                    Minutes1 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes1.ToString());
                }
				
				xvar = xml.SelectSingleNode("//ProfileChanger/Active2");
                if (xvar != null)
                {
                    Active2 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active2.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Profile2");
                if (xvar != null)
                {
                    Profile2 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile2.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Minutes2");
                if (xvar != null)
                {
                    Minutes2 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes2.ToString());
                }
				
				xvar = xml.SelectSingleNode("//ProfileChanger/Active3");
                if (xvar != null)
                {
                    Active3 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active3.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Profile3");
                if (xvar != null)
                {
                    Profile3 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile3.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Minutes3");
                if (xvar != null)
                {
                    Minutes3 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes3.ToString());
                }
				
				xvar = xml.SelectSingleNode("//ProfileChanger/Active4");
                if (xvar != null)
                {
                    Active4 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active4.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Profile4");
                if (xvar != null)
                {
                    Profile4 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile4.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Minutes4");
                if (xvar != null)
                {
                    Minutes4 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes4.ToString());
                }
				
				xvar = xml.SelectSingleNode("//ProfileChanger/Active5");
                if (xvar != null)
                {
                    Active5 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active5.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Profile5");
                if (xvar != null)
                {
                    Profile5 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile5.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Minutes5");
                if (xvar != null)
                {
                    Minutes5 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes5.ToString());
                }

				
				xvar = xml.SelectSingleNode("//ProfileChanger/Active6");
                if (xvar != null)
                {
                    Active6 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active6.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Profile6");
                if (xvar != null)
                {
                    Profile6 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile6.ToString());
                }
				xvar = xml.SelectSingleNode("//ProfileChanger/Minutes6");
                if (xvar != null)
                {
                    Minutes6 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes6.ToString());
                }

                xvar = xml.SelectSingleNode("//ProfileChanger/Active7");
                if (xvar != null)
                {
                    Active7 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active7.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Profile7");
                if (xvar != null)
                {
                    Profile7 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile7.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Minutes7");
                if (xvar != null)
                {
                    Minutes7 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes7.ToString());
                }

                xvar = xml.SelectSingleNode("//ProfileChanger/Active8");
                if (xvar != null)
                {
                    Active8 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active8.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Profile8");
                if (xvar != null)
                {
                    Profile8 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile8.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Minutes8");
                if (xvar != null)
                {
                    Minutes8 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes8.ToString());
                }

                xvar = xml.SelectSingleNode("//ProfileChanger/Active9");
                if (xvar != null)
                {
                    Active9 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active9.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Profile9");
                if (xvar != null)
                {
                    Profile9 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile9.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Minutes9");
                if (xvar != null)
                {
                    Minutes9 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes9.ToString());
                }

                xvar = xml.SelectSingleNode("//ProfileChanger/Active10");
                if (xvar != null)
                {
                    Active10 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active10.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Profile10");
                if (xvar != null)
                {
                    Profile10 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile10.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Minutes10");
                if (xvar != null)
                {
                    Minutes10 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes10.ToString());
                }

                xvar = xml.SelectSingleNode("//ProfileChanger/Active11");
                if (xvar != null)
                {
                    Active11 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active11.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Profile11");
                if (xvar != null)
                {
                    Profile11 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile11.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Minutes11");
                if (xvar != null)
                {
                    Minutes11 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes11.ToString());
                }

                xvar = xml.SelectSingleNode("//ProfileChanger/Active12");
                if (xvar != null)
                {
                    Active12 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Active12.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Profile12");
                if (xvar != null)
                {
                    Profile12 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Profile12.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/Minutes12");
                if (xvar != null)
                {
                    Minutes12 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + Minutes12.ToString());
                }

                xvar = xml.SelectSingleNode("//ProfileChanger/StopBot");
                if (xvar != null)
                {
                    StopBot = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + StopBot.ToString());
                }

                xvar = xml.SelectSingleNode("//ProfileChanger/RandomProfile");
                if (xvar != null)
                {
                    RandomProfile = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + RandomProfile.ToString());
                }

                xvar = xml.SelectSingleNode("//ProfileChanger/RandomTime");
                if (xvar != null)
                {
                    RandomTime = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + RandomTime.ToString());
                }

                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax1");
                if (xvar != null)
                {
                    MinutesMax1 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax1.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax2");
                if (xvar != null)
                {
                    MinutesMax2 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax2.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax3");
                if (xvar != null)
                {
                    MinutesMax3 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax3.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax4");
                if (xvar != null)
                {
                    MinutesMax4 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax4.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax5");
                if (xvar != null)
                {
                    MinutesMax5 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax5.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax6");
                if (xvar != null)
                {
                    MinutesMax6 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax6.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax7");
                if (xvar != null)
                {
                    MinutesMax7 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax7.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax8");
                if (xvar != null)
                {
                    MinutesMax8 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax8.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax9");
                if (xvar != null)
                {
                    MinutesMax9 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax9.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax10");
                if (xvar != null)
                {
                    MinutesMax10 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax10.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax11");
                if (xvar != null)
                {
                    MinutesMax11 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax11.ToString());
                }
                xvar = xml.SelectSingleNode("//ProfileChanger/MinutesMax12");
                if (xvar != null)
                {
                    MinutesMax12 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Profile Changer Load: " + xvar.Name + "=" + MinutesMax12.ToString());
                }
			}
            catch (Exception e)
            {
                Logging.WriteDiagnostic(Colors.Red, e.Message);
            }
		}
    }
}
