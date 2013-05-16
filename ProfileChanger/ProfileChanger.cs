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
using Styx.CommonBot.Profiles;

namespace ProfileChanger
{
    class ProfileChanger : HBPlugin
    {
        public override string Name { get { return "Profile Changer"; } }
        public override string Author { get { return "katzerle"; } }
        private readonly Version _version = new Version(2, 0);
        public override Version Version { get { return _version; } }
        public override string ButtonText { get { return "Settings"; } }
        public override bool WantButton { get { return true; } }

        public static ProfileChangerSettings Settings = new ProfileChangerSettings();
        public static LocalPlayer Me = StyxWoW.Me;
        bool hasItBeenInitialized = false;

        static Stopwatch SWProfile1 = new Stopwatch();
        static Stopwatch SWProfile2 = new Stopwatch();
        static Stopwatch SWProfile3 = new Stopwatch();
        static Stopwatch SWProfile4 = new Stopwatch();
        static Stopwatch SWProfile5 = new Stopwatch();
        static Stopwatch SWProfile6 = new Stopwatch();
        static Stopwatch SWProfile7 = new Stopwatch();
        static Stopwatch SWProfile8 = new Stopwatch();
        static Stopwatch SWProfile9 = new Stopwatch();
        static Stopwatch SWProfile10 = new Stopwatch();
        static Stopwatch SWProfile11 = new Stopwatch();
        static Stopwatch SWProfile12 = new Stopwatch();

        public ProfileChanger()
        {
			UpdatePlugin();
			Logging.Write(Colors.LightSkyBlue, "Profile Changer loaded");
        }

        public override void OnButtonPress()
        {
            ConfigForm.ShowDialog();
        }


        private Form MyForm;
        public Form ConfigForm
        {
            get
            {
                if (MyForm == null)
                    MyForm = new ProfileChangerUI();
                return MyForm;
            }
        }

        public override void Pulse()
        {
// ------------ Deactivate if not in Game etc
			if (Me == null || !StyxWoW.IsInGame)
				return;

// ------------ Deactivate Plugin in BGs, Inis, while Casting and on Transport
			if (Battlegrounds.IsInsideBattleground || Me.IsInInstance || Me.IsOnTransport)
				return;
				
// ------------ Deactivate Plugin if in Combat, Dead or Ghost			
			if (inCombat)
				return;
			
            if (!hasItBeenInitialized && Settings.Active1)
            {
                SWProfile1.Reset();
                SWProfile2.Reset();
                SWProfile3.Reset();
                SWProfile4.Reset();
                SWProfile5.Reset();
                SWProfile6.Reset();
                SWProfile7.Reset();
                SWProfile8.Reset();
                SWProfile9.Reset();
                SWProfile10.Reset();
                SWProfile11.Reset();
                SWProfile12.Reset();
                SWProfile1.Start();

                ChangeProfile(Settings.Profile1);
                hasItBeenInitialized = true;
				Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Init Done");
            }

            if (Settings.Active1)
            {
                if (SWProfile1.Elapsed.TotalMinutes > Convert.ToInt16(Settings.Minutes1))
                {						
					Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
					if (Settings.Active2)
                    {
                        ChangeProfile(Settings.Profile2);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }
						
					Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
					SWProfile1.Reset();
					if (Settings.Active2)
                    {
						SWProfile2.Start();
					}
					else
					{
						SWProfile1.Start();
					}
                }
            }
            if (Settings.Active2)
            {
                if (SWProfile2.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes2))
                {
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
                    if (Settings.Active3)
                    {
                        ChangeProfile(Settings.Profile3);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }
						
					Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
					SWProfile2.Reset();
					if (Settings.Active3)
                    {
						SWProfile3.Start();
					}
					else
					{
						SWProfile1.Start();
					}
                }
            }
            if (Settings.Active3)
            {
                if (SWProfile3.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes3))
                {
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
                    if (Settings.Active4)
                    {
                        ChangeProfile(Settings.Profile4);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }
						
					Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
					SWProfile3.Reset();
					if (Settings.Active4)
                    {
						SWProfile4.Start();
					}
					else
					{
						SWProfile1.Start();
					}						
                }
            }
            if (Settings.Active4)
            {
                if (SWProfile4.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes4))
                {                        
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
                    if (Settings.Active5)
                    {
                        ChangeProfile(Settings.Profile5);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }
						
					Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
					SWProfile4.Reset();
					if (Settings.Active5)
                    {
						SWProfile5.Start();
					}
					else
					{
						SWProfile1.Start();
					}
                }
            }
            if (Settings.Active5)
            {
                if (SWProfile5.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes5))
                {    
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
                    if (Settings.Active6)
                    {
                        ChangeProfile(Settings.Profile6);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }
						
					Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
					SWProfile5.Reset();
					if (Settings.Active6)
                    {
						SWProfile6.Start();
					}
					else
					{
						SWProfile1.Start();
					}
                }
            }
            if (Settings.Active6)
            {
                if (SWProfile6.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes6))
                {
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
                    if (Settings.Active7)
                    {
                        ChangeProfile(Settings.Profile7);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }

                    Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
                    SWProfile6.Reset();
                    if (Settings.Active7)
                    {
                        SWProfile7.Start();
                    }
                    else
                    {
                        SWProfile1.Start();
                    }
                }
            }
            if (Settings.Active7)
            {
                if (SWProfile7.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes7))
                {
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
                    if (Settings.Active8)
                    {
                        ChangeProfile(Settings.Profile8);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }

                    Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
                    SWProfile7.Reset();
                    if (Settings.Active8)
                    {
                        SWProfile8.Start();
                    }
                    else
                    {
                        SWProfile1.Start();
                    }
                }
            }
            if (Settings.Active8)
            {
                if (SWProfile8.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes8))
                {
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
                    if (Settings.Active9)
                    {
                        ChangeProfile(Settings.Profile9);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }

                    Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
                    SWProfile8.Reset();
                    if (Settings.Active9)
                    {
                        SWProfile9.Start();
                    }
                    else
                    {
                        SWProfile1.Start();
                    }
                }
            }
            if (Settings.Active9)
            {
                if (SWProfile9.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes9))
                {
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
                    if (Settings.Active10)
                    {
                        ChangeProfile(Settings.Profile10);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }

                    Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
                    SWProfile9.Reset();
                    if (Settings.Active10)
                    {
                        SWProfile10.Start();
                    }
                    else
                    {
                        SWProfile1.Start();
                    }
                }
            }
            if (Settings.Active10)
            {
                if (SWProfile10.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes10))
                {
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
                    if (Settings.Active11)
                    {
                        ChangeProfile(Settings.Profile11);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }

                    Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
                    SWProfile10.Reset();
                    if (Settings.Active11)
                    {
                        SWProfile11.Start();
                    }
                    else
                    {
                        SWProfile1.Start();
                    }
                }
            }
            if (Settings.Active11)
            {
                if (SWProfile11.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes11))
                {
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
                    if (Settings.Active12)
                    {
                        ChangeProfile(Settings.Profile12);
                    }
                    else
                    {
                        if (Settings.StopBot)
                        {
                            WoWMovement.MoveStop();
                            Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
                            Lua.DoString("ForceQuit()");
                            return;
                        }
                        else
                        {
                            ChangeProfile(Settings.Profile1);
                        }
                    }

                    Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
                    SWProfile11.Reset();
                    if (Settings.Active12)
                    {
                        SWProfile12.Start();
                    }
                    else
                    {
                        SWProfile1.Start();
                    }
                }
            }

            if (Settings.Active12)
            {
                if (SWProfile12.Elapsed.TotalMinutes > Convert.ToInt32(Settings.Minutes12))
                {
                    Logging.Write(Colors.LightSkyBlue, "Profile Changer: Change Profile");
					if (Settings.StopBot)
					{
						WoWMovement.MoveStop();
						Logging.Write(Colors.LightSkyBlue, "Profile Changer: Log Out Now, bb");
						Lua.DoString("ForceQuit()");
						return;
					}
					else
					{
						ChangeProfile(Settings.Profile1);
					}
						
					Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Reset Timer");
					SWProfile12.Reset();
					SWProfile1.Start();
                }
            }
        }
		
        static public void ChangeProfile(string Profile)
        {
            Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Load Profile: {0}", Profile);
			WoWMovement.MoveStop();
            Thread.Sleep(1000);
            ProfileManager.LoadNew(Profile);
            Thread.Sleep(1000);
			Logging.WriteDiagnostic(Colors.LightSkyBlue, "Profile Changer: Load Profile: {0} done", Profile);
        }
		
		static public string FolderPath
        {
            get
            {
                string sPath = Process.GetCurrentProcess().MainModule.FileName;
                sPath = Path.GetDirectoryName(sPath);
                sPath = Path.Combine(sPath, "Plugins\\ProfileChanger\\");
                return sPath;
            }
        }

//Update Function (deactivated
		static public void UpdatePlugin()
		{
			//Here you have to insert your Internetlocation for the SVN
			string Website = "http://katzerls-plugins.googlecode.com/svn/trunk";

            try
            {
                WebClient client = new WebClient();
                IFormatProvider culture = new CultureInfo("fr-FR", true);

                XDocument VersionLatest = XDocument.Load(Website + "/Updater.xml");
                XDocument VersionCurrent = XDocument.Load(FolderPath + "\\Updater.xml");
                DateTime latestTime = DateTime.Parse(VersionLatest.Element("ProfileChangerUpdater").Element("UpdateTime").Value, culture, DateTimeStyles.NoCurrentDateDefault);
                DateTime currentTime = DateTime.Parse(VersionCurrent.Element("Updater").Element("UpdateTime").Value, culture, DateTimeStyles.NoCurrentDateDefault);
                string Version = VersionLatest.Element("ProfileChangerUpdater").Element("Version").Value;
                //Compare if the new Updater XML has a newer Date as the current installed one and if yes: Update the Files
                if (latestTime <= currentTime)
                    return;
                Logging.Write(Colors.LightSkyBlue, "**********************************************");
                Logging.Write(Colors.LightSkyBlue, "Profile Changer: New Version available {0}", Version);
                Logging.Write(Colors.LightSkyBlue, "Download it in the Pluginsection of Honorbuddy", Version);
                Logging.Write(Colors.LightSkyBlue, "Link or via SVN");
                Logging.Write(Colors.LightSkyBlue, "**********************************************");
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception e)
            {
                Logging.WriteDiagnostic(Colors.Red, e.Message);
            }
			
		}
//Function In Combat
        static public bool inCombat
        {
            get
            {
                if (Me == null || !StyxWoW.IsInGame || Me.Combat || Me.IsDead || Me.IsGhost || InPetCombat()) return true;
                return false;
            }
        }

        static public bool InPetCombat()
        {
            List<string> cnt = Lua.GetReturnValues("dummy,reason=C_PetBattles.IsTrapAvailable() return dummy,reason");

            if (cnt != null) { if (cnt[1] != "0") return true; }
            return false;
        }
    }
}
