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

namespace katzerle
{
	class RareAlerter: HBPlugin
	{	
		public static string name { get { return "RareAlerter"; } }
		public override string Name { get { return name; } }
		public override string Author { get { return "katzerle"; } }
		private readonly static Version _version = new Version(2, 0);
		public override Version Version { get { return _version; } }
		public override string ButtonText { get { return "No Settings"; } }
		public override bool WantButton { get { return false; } }
		
		public static LocalPlayer Me = StyxWoW.Me;
		private static Stopwatch Checktimer = new Stopwatch();
		public static Random rnd = new Random();
        public static bool hasItBeenInitialized = false;
        Int32 MoveTimer = 100;
		
		public static bool LeftRight = true;
		public static bool AntiAFK;
		public static bool LowLevel;
		public static bool WOTLK;
		public static bool BC;
		public static bool CATA;
		public static bool MOP;
		public static bool Raptornest;	
		public static bool OnyxEgg;
		public static bool DarkSoil;
		public static bool StopBot;
		public static bool WaitBot;
		
		public static bool TomTom = false;	
		public static bool FlyThere = false;
		public static bool GroundMountMode = false;
		
		public static bool TEST;
		
		public static bool WisperAlert;
		public static bool GuildAlert;
		public static BlacklistFlags Flags = BlacklistFlags.All;
		
		// X and Y are inverted
        private static float x_start = -1002.084f;
        private static float y_start = -5531.25f;
        private static float x_weight = 46.54167f; // 3652.083 / -1002.084
        private static float y_weight = 69.83333f; // 1452.083 / -5531.25
		public static Dictionary<Int32, string> ObjectsList = new Dictionary<Int32, string>();


		public override void OnButtonPress()
		{
		}
		
		public RareAlerter()
        {

			UpdatePlugin();
			Logging.Write(Colors.MediumPurple, "Rare Alerter loaded");
        }

        static private void BotStopped(EventArgs args)
        {
			Chat.Whisper -= newWhisper;
            Lua.Events.DetachEvent("CHAT_MSG_BN_WHISPER", BNWhisper);
            Chat.Guild -= newGuild;
            Chat.Officer -= newOfficer;
			BotEvents.OnBotStopped -= BotStopped;
            hasItBeenInitialized = false;
			ObjectsList.Clear();
        }
		
		public override void Initialize()
        {
// Register the events of the Stop Button in HB
			BotEvents.OnBotStopped += BotStopped;
//Alerts Wisper and Guild
			Chat.Whisper += newWhisper;
            Lua.Events.AttachEvent("CHAT_MSG_BN_WHISPER", BNWhisper);
            Chat.Guild += newGuild;
            Chat.Officer += newOfficer;
			ConfigLoad();
			
//Objects to List
            XmlDocument ObjectsXML = new XmlDocument();
			string sPath = Path.Combine(FolderPath, "Objects.xml"); 
			System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
			try
            {
                ObjectsXML.Load(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                Logging.WriteDiagnostic(Colors.Red, e.Message);
                fs.Close();
                return;
            }
            XmlElement root = ObjectsXML.DocumentElement;
            foreach (XmlNode Node in root.ChildNodes)
			{
                Int32 Entry = Convert.ToInt32(Node.Attributes["Entry"].InnerText);
				string Name = Node.Attributes["Name"].InnerText;
                if (!ObjectsList.ContainsKey(Entry))
                {
                    ObjectsList.Add(Entry, Name);
                }
			}
			Logging.WriteDiagnostic(Colors.MediumPurple, "Objects.xml loaded");
		}
		
		public override void Pulse()
		{
			try
			{
// ------------ Deactivate if Rarekiller is Enabled			
				List<PluginContainer> _pluginList = PluginManager.Plugins;
				foreach (PluginContainer _plugin in _pluginList)
				{
					if (_plugin.Enabled && _plugin.Plugin.Name == "RareKiller")
					return;
				}
// ------------ Deactivate if not in Game etc
                if (Me == null || !StyxWoW.IsInGame)
                    return;

// ------------ Deactivate Plugin in BGs, Inis
                if (Battlegrounds.IsInsideBattleground || Me.IsInInstance)
				return;
				
// ------------ Deactivate if Rarekiller is Online
                if (Battlegrounds.IsInsideBattleground || Me.IsInInstance)
				return;
				
// ------------ Part Init
                if (!hasItBeenInitialized)
                {
                    Initialize();
					hasItBeenInitialized = true;
					Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter - Init done");
					
                }
				if (!inCombat && AntiAFK)
				{
					if (!Checktimer.IsRunning || Checktimer.Elapsed.TotalSeconds > MoveTimer)
					{
						Checktimer.Reset();
						Checktimer.Start();
						MoveTimer = rnd.Next(90, 200);
						if(!Me.IsMoving)
							Movearound();
					}
				}

                if (!inCombat)
					findMob();
                if (!inCombat)
					findObject();
				
			}
			catch (ThreadAbortException) { }
			catch (Exception e)
			{
				Logging.WriteDiagnostic(Colors.Red, e.Message);
			}
		}
		
		static public void findMob()
		{
			ObjectManager.Update();
			List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => ((o.Entry == 50005)  		//Poseidus Vashijr
					|| (o.Entry == 3868)				//Bloodseeker
					|| (o.Entry == 50062)				//Aeonaxx friendly					
					|| (o.Entry == 50410)				//Camel Dust					
					|| (o.Entry == 50409)				//Camel Port
					|| (TEST && ((o.Entry == 33687) || (o.Entry == 6579) || (o.Entry == 42730)))		// Test Mob
					|| ((o.CreatureRank == Styx.WoWUnitClassificationType.Rare) &&
					((MOP && o.Level > 85)						//MOP Rares
					|| (CATA && o.Level > 79 && o.Level < 86)  	//Cata Rares
					|| (WOTLK && o.Level > 69 && o.Level < 81) 	//WOTLK Rares
					|| (BC && o.Level > 59 && o.Level < 71)		//BC Rares
					|| (LowLevel && o.Level < 61))				//LowLevel Rares
					))).OrderBy(o => o.Distance).ToList();			

			foreach (WoWUnit o in objList)
			{
				if(!o.IsDead && !Blacklist.Contains(o.Guid, Flags) && !o.IsPet)
				{					
					if (File.Exists(RareAlerter.Soundfile))
						new SoundPlayer(RareAlerter.Soundfile).Play();
					else
						Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter playing Soundfile failes");
					Logging.Write(Colors.MediumPurple, "RareAlerter: Find a hunted Mob " + o.Name);
					WoWMovement.MoveStop();
					o.Face();
					Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter: Face Mob");
					if(FlyThere)
					{
						while (o.Location.Distance(Me.Location) > 10)
						{
							if (GroundMountMode)
								Navigator.MoveTo(o.Location);
							else
								Flightor.MoveTo(o.Location);
							Thread.Sleep(100);
							Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter: Fly");
						}
					}
					WoWMovement.MoveStop();
					
					if (!((o.Entry == 50410) || (o.Entry == 50409)))
					{
						o.Target();
						Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter: Target Mob");
					}
					
					if(StopBot)
					{
						Logging.Write(Colors.MediumPurple, "RareAlerter: Stop Bot");
						Logging.Write(Colors.MediumPurple, "RareAlerter: Blacklist Mob 2 Minutes");
						Blacklist.Add(o.Guid, Flags, TimeSpan.FromSeconds(120));
						Styx.CommonBot.TreeRoot.Stop();
					}
					else if (WaitBot)
					{
						Logging.Write(Colors.MediumPurple, "RareAlerter: Waiting 1 Minute");
						Thread.Sleep(60000);
						Logging.Write(Colors.MediumPurple, "RareAlerter: Blacklist Mob 2 Minutes");
						Blacklist.Add(o.Guid, Flags, TimeSpan.FromSeconds(120));
					}
					else
					{
						Logging.Write(Colors.MediumPurple, "RareAlerter: Blacklist Mob 2 Minutes");
						Blacklist.Add(o.Guid, Flags, TimeSpan.FromSeconds(120));
					}
				}
				else if(o.IsDead)
				{
					Logging.Write(Colors.MediumPurple, "RareAlerter: Find " + o.Name + ", but sadly he's dead");
					Blacklist.Add(o.Guid, Flags, TimeSpan.FromSeconds(3600));
				}
				else if(o.IsPet)
					Blacklist.Add(o.Guid, Flags, TimeSpan.FromSeconds(3600));
			}
		}
		
		static public void findObject()
		{

		    float x;
            float y;


			ObjectManager.Update();
            List<WoWGameObject> objList = ObjectManager.GetObjectsOfType<WoWGameObject>()
                .Where(o => (ObjectsList.ContainsKey(Convert.ToInt32(o.Entry))) //Objects.xml
                ).OrderBy(o => o.Distance).ToList();
			foreach (WoWGameObject o in objList)
			{
				if (!Blacklist.Contains(o.Guid, Flags))
				{
					if ((o.Entry == 214945) && !OnyxEgg)
						return;
					if ((o.Entry == 210565) && !DarkSoil)
						return;
					if ((o.Entry == 202080) && !Raptornest)
						return;
					if ((o.Entry == 202081) && !Raptornest)
						return;
					if ((o.Entry == 202082) && !Raptornest)
						return;
					if ((o.Entry == 202083) && !Raptornest)
						return;
						
					if (File.Exists(RareAlerter.Soundfile))
						new SoundPlayer(RareAlerter.Soundfile).Play();
					else
						Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter playing Soundfile failes");
					Logging.Write(Colors.MediumPurple, "RareAlerter: Find a hunted Object " + o.Name);
					
					if(TomTom)
					{
						x = (float)Math.Round(100 - ((o.Y - y_start) / y_weight), 2);
						y = (float)Math.Round(100 - ((o.X - x_start) / x_weight), 2);
						Logging.Write(Colors.MediumPurple, "RareAlerter: Find Object " + o.Name + " Object found at ["+x+";"+y+"]");

						Lua.DoString("TomTom:AddWaypoint("+x+", "+y+", \"Object!\")");
					}
					
					if(FlyThere)
					{
						while (o.Location.Distance(Me.Location) > 40)
						{
							if (GroundMountMode)
								Navigator.MoveTo(o.Location);
							else
								Flightor.MoveTo(o.Location);
							Thread.Sleep(100);
						}
					}
					WoWMovement.MoveStop();

					if(StopBot)
					{
						Logging.Write(Colors.MediumPurple, "RareAlerter: Stop Bot");
						Logging.Write(Colors.MediumPurple, "RareAlerter: Blacklist Object 2 Minutes");
						Blacklist.Add(o.Guid, Flags, TimeSpan.FromSeconds(120));
						Styx.CommonBot.TreeRoot.Stop();
					}
					else if (WaitBot)
					{
						Logging.Write(Colors.MediumPurple, "RareAlerter: Waiting 1 Minute");
						Thread.Sleep(60000);
						Logging.Write(Colors.MediumPurple, "RareAlerter: Blacklist Object 2 Minutes");
						Blacklist.Add(o.Guid, Flags, TimeSpan.FromSeconds(120));
					}
					else
					{
						Logging.Write(Colors.MediumPurple, "RareAlerter: Blacklist Object 2 Minutes");
						Blacklist.Add(o.Guid, Flags, TimeSpan.FromSeconds(120));
					}
						
				}
			}
		}
		
		//Keypresser
		static public void Movearound()
        {
            
            if (LeftRight)
            {
                WoWMovement.Move(WoWMovement.MovementDirection.TurnLeft);
				Logging.WriteDiagnostic(Colors.MediumPurple, "AntiAFK move around, Left");
				Thread.Sleep(100);
				WoWMovement.MoveStop();
                LeftRight = false;
            }
            else
            {
				WoWMovement.Move(WoWMovement.MovementDirection.TurnRight);
				Logging.WriteDiagnostic(Colors.MediumPurple, "AntiAFK move around, Right");
                Thread.Sleep(100);
				WoWMovement.MoveStop();
                LeftRight = true;
            }
        }
		
		

		static public string FolderPath
        {
            get
            {
                string sPath = Process.GetCurrentProcess().MainModule.FileName;
                sPath = Path.GetDirectoryName(sPath);
                sPath = Path.Combine(sPath, "Plugins\\RareAlerter\\");
                return sPath;
            }
        }

        static public string SettingsPath
        {
            get
            {
                string sPath = Process.GetCurrentProcess().MainModule.FileName;
                sPath = Path.GetDirectoryName(sPath);
                sPath = Path.Combine(sPath, "Settings\\");
                return sPath;
            }
        }
		static public string Soundfile
        {
            get
            {
				string sPath = Path.Combine(FolderPath, "siren.wav");
                return sPath;
            }
        }
		
//Load Config
		static public void ConfigLoad()
		{
			//    XmlTextReader reader;
            XmlDocument xml = new XmlDocument();
            XmlNode xvar;
			
            string sPath = SettingsPath;

            sPath = Path.Combine(sPath, "RareAlerter_", StyxWoW.Me.Name, ".config");
			
			if (!File.Exists(sPath))
            {
                Logging.WriteDiagnostic("RareAlerter: No Special Config - Continuing with Default Config Values");
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

            try
            {

                if (xml == null)
                    return;
                // Load Variables - Addons
				xvar = xml.SelectSingleNode("//RareAlerter/AntiAFK");
                if (xvar != null)
                {
                    AntiAFK = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + AntiAFK.ToString());
                }
				
				xvar = xml.SelectSingleNode("//RareAlerter/LowLevel");
                if (xvar != null)
                {
                    LowLevel = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + LowLevel.ToString());
                }
				xvar = xml.SelectSingleNode("//RareAlerter/MOP");
                if (xvar != null)
                {
                    MOP = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + MOP.ToString());
                }
                xvar = xml.SelectSingleNode("//RareAlerter/CATA");
                if (xvar != null)
                {
                    CATA = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + CATA.ToString());
                }
                xvar = xml.SelectSingleNode("//RareAlerter/WOTLK");
                if (xvar != null)
                {
                    WOTLK = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + WOTLK.ToString());
                }
                xvar = xml.SelectSingleNode("//RareAlerter/BC");
                if (xvar != null)
                {
                    BC = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + BC.ToString());
                }
                xvar = xml.SelectSingleNode("//RareAlerter/Raptornest");
                if (xvar != null)
                {
                    Raptornest = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + Raptornest.ToString());
                }
                xvar = xml.SelectSingleNode("//RareAlerter/OnyxEgg");
                if (xvar != null)
                {
                    OnyxEgg = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + OnyxEgg.ToString());
                }
                xvar = xml.SelectSingleNode("//RareAlerter/DarkSoil");
                if (xvar != null)
                {
                    DarkSoil = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + DarkSoil.ToString());
                }
                xvar = xml.SelectSingleNode("//RareAlerter/StopBot");
                if (xvar != null)
                {
                    StopBot = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + StopBot.ToString());
                }
                xvar = xml.SelectSingleNode("//RareAlerter/WaitBot");
                if (xvar != null)
                {
                    WaitBot = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + WaitBot.ToString());
                }
                xvar = xml.SelectSingleNode("//RareAlerter/WisperAlert");
                if (xvar != null)
                {
                    WisperAlert = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + WisperAlert.ToString());
                }
                xvar = xml.SelectSingleNode("//RareAlerter/GuildAlert");
                if (xvar != null)
                {
                    GuildAlert = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + GuildAlert.ToString());
                }
				
                xvar = xml.SelectSingleNode("//RareAlerter/TEST");
                if (xvar != null)
                {
                    TEST = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "RareAlerter Load: " + xvar.Name + "=" + TEST.ToString());
                }
				
				Logging.Write(Colors.MediumPurple, "RareAlerter: Loaded config file");				
            }
            catch (Exception e)
            {
                Logging.WriteDiagnostic(Colors.Red, e.Message);
            }
		}
		
		
//Wisperalert
		static public string SoundfileWisper
        {
            get
            {
				string sPath = Path.Combine(FolderPath, "attention.wav");
                return sPath;
            }
        }
		
		static public void newWhisper(Chat.ChatWhisperEventArgs arg)
        {
			if(WisperAlert)
			{
				if (File.Exists(SoundfileWisper))
					new SoundPlayer(SoundfileWisper).Play();
				else
					Logging.Write(Colors.Red, "Alert playing SoundfileWisper failes");
			}
			Logging.Write(Colors.Pink, "You got a Wisper: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message,  DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
        }
        static public void BNWhisper(object sender, LuaEventArgs args)
        {
            object[] Args = args.Args;
            string Message = Args[0].ToString();
            string presenceId = Args[12].ToString();
            string Author = Lua.GetReturnValues(String.Format("return BNGetFriendInfoByID({0})", presenceId))[3];

			if(WisperAlert)
			{
				if (File.Exists(SoundfileWisper))
					new SoundPlayer(SoundfileWisper).Play();
				else
					Logging.WriteDiagnostic(Colors.Red, "Alert playing SoundfileWisper failes");
			}
            Logging.Write(Colors.Aqua, "You got a BN Wisper: {0}: {1} - Timestamp: {2}: {3}", Author, Message,  DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
        }
        static public void newGuild(Chat.ChatGuildEventArgs arg)
        {
			if(GuildAlert)
			{
				if (File.Exists(SoundfileWisper))
					new SoundPlayer(SoundfileWisper).Play();
				else
					Logging.WriteDiagnostic(Colors.Red, "Alert playing SoundfileWisper failes");
			}
			Logging.Write(Colors.Lime, "Guildmessage: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
        }
        static public void newOfficer(Chat.ChatLanguageSpecificEventArgs arg)
        {
			if(GuildAlert)
			{
				if (File.Exists(SoundfileWisper))
					new SoundPlayer(SoundfileWisper).Play();
				else
					Logging.WriteDiagnostic(Colors.Red, "Alert playing SoundfileWisper failes");
			}
			Logging.Write(Colors.Lime, "Officermessage: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
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
                DateTime latestTime = DateTime.Parse(VersionLatest.Element("RareAlertUpdater").Element("UpdateTime").Value, culture, DateTimeStyles.NoCurrentDateDefault);
                DateTime currentTime = DateTime.Parse(VersionCurrent.Element("Updater").Element("UpdateTime").Value, culture, DateTimeStyles.NoCurrentDateDefault);
                string Version = VersionLatest.Element("RareAlertUpdater").Element("Version").Value;
                //Compare if the new Updater XML has a newer Date as the current installed one and if yes: Update the Files
                if (latestTime <= currentTime)
                    return;
                Logging.Write(Colors.MediumPurple, "**********************************************");
                Logging.Write(Colors.MediumPurple, "RareAlert: New Version available {0}", Version);
                Logging.Write(Colors.MediumPurple, "Download it in the Pluginsection of Honorbuddy", Version);
                Logging.Write(Colors.MediumPurple, "Link or via SVN");
                Logging.Write(Colors.MediumPurple, "**********************************************");
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception e)
            {
                Logging.WriteDiagnostic(Colors.Red, e.Message);
            }
			
		}


	}
}

