//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//    Credits to highvoltz, bloodlove, SMcCloud, Lofi, ZapMan 
//                and all the brave Testers
//
//==================================================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Pathing;
using Styx.WoWInternals.World;


namespace katzerle
{
    class RarekillerMoPRares
    {

        #region Units
        public WoWUnit Hozen
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50828 || o.Entry == 50836 || o.Entry == 50840 || o.Entry == 50823 ||
                    o.Entry == 50831 || o.Entry == 50830 || o.Entry == 50832) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Jinyu
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50750 || o.Entry == 50768 || o.Entry == 50772 || o.Entry == 50766 ||
                    o.Entry == 50769 || o.Entry == 50780 || o.Entry == 50776) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Mantid
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50739 || o.Entry == 50749 || o.Entry == 50734 || o.Entry == 50364 ||
                    o.Entry == 50363 || o.Entry == 50733 || o.Entry == 50388) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit MoguSorcerer
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50341 || o.Entry == 50349 || o.Entry == 50340 || o.Entry == 50347 ||
                    o.Entry == 50338 || o.Entry == 50344 || o.Entry == 50339) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit MoguWarrior
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50354 || o.Entry == 50351 || o.Entry == 50355 || o.Entry == 50356 ||
                    o.Entry == 50350 || o.Entry == 50352 || o.Entry == 50359) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Pandaren
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50821 || o.Entry == 50817 || o.Entry == 50822 || o.Entry == 50816 ||
                    o.Entry == 50811 || o.Entry == 50808 || o.Entry == 50820) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Saurok
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50787 || o.Entry == 50806 || o.Entry == 50789 || o.Entry == 50805 ||
                    o.Entry == 50783 || o.Entry == 50782 || o.Entry == 50791) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Yaungol
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 51059 || o.Entry == 50334 || o.Entry == 51078 || o.Entry == 50331 ||
                    o.Entry == 50332 || o.Entry == 50333 || o.Entry == 50336) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Tornado
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (u.Entry == 64267) && u.Distance < 10 && !u.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Dormus
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (u.Entry == 50245) && !u.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
                }
        }
        #endregion

        #region Avoid Enemy AOE Behaviors - not yet tested!

        // Developer Thing (ToDo Remove)
        public void DumpAOEEffect()
        {
            ObjectManager.Update();
            List<WoWDynamicObject> AOEList = ObjectManager.GetObjectsOfType<WoWDynamicObject>()
                .Where(o => o.Distance < 10)
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWDynamicObject o in AOEList)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP - AOE Dump: Unit {0}, AOE {1} ID {2}", StyxWoW.Me.CurrentTarget.Name, o.Name, o.Entry);
            }
        }
        public void DumpJinyuThings()
        {
            ObjectManager.Update();
            List<WoWAreaTrigger> AreaTriggerList = ObjectManager.GetObjectsOfType<WoWAreaTrigger>()
                .Where(o => o.Distance < 10)
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWAreaTrigger o in AreaTriggerList)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP - AOE Dump: Unit {0}, WoWAreaTrigger {1} ID {2}", StyxWoW.Me.CurrentTarget.Name, o.Name, o.Entry);
            }

            List<WoWGameObject> GameObjectList = ObjectManager.GetObjectsOfType<WoWGameObject>()
                .Where(o => o.Distance < 10)
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWGameObject o in GameObjectList)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP - AOE Dump: Unit {0}, WoWGameObject {1} ID {2}", StyxWoW.Me.CurrentTarget.Name, o.Name, o.Entry);
            }

            List<WoWObject> WoWObjectList = ObjectManager.GetObjectsOfType<WoWObject>()
                .Where(o => o.Distance < 10)
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWObject o in WoWObjectList)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP - AOE Dump: Unit {0}, WoWObject {1} ID {2}", StyxWoW.Me.CurrentTarget.Name, o.Name, o.Entry);
            }

            List<WoWUnit> WoWUnitList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => o.Distance < 10 && o != Jinyu)
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in WoWUnitList)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP - AOE Dump: Unit {0}, Unit {1} ID {2}", StyxWoW.Me.CurrentTarget.Name, o.Name, o.Entry);
            }
        }

        public void AvoidEnemyAOE(WoWPoint location, List<WoWDynamicObject> Objects, string Aura, int TraceStep)
        {
            if (Objects == null)
            { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP: no Pools found .."); return; }
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP: found {0} {1}! start RayCast ..", Objects.Count, Aura);

            int MinDistToPools = (int)(Objects[0].Radius * 1.6f);
            int MaxDistToMove = MinDistToPools * 2;

            // get save location
            WoWPoint newP = getSaveLocation(location, Objects, MinDistToPools, MaxDistToMove, TraceStep);

            if (newP == WoWPoint.Empty)
            {
                // no save location found, move 2sec Strafe Left
                WoWMovement.Move(WoWMovement.MovementDirection.StrafeLeft, TimeSpan.FromSeconds(2));
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            else
            {
                // move to save location
                while (StyxWoW.Me.HasAura(Aura) && StyxWoW.Me.Location.Distance(newP) > 0.2f)
                {
                    Navigator.MoveTo(newP);
                    Thread.Sleep(80);
                }
            }

            WoWMovement.MoveStop();
            if (StyxWoW.Me.CurrentTargetGuid != 0)
                StyxWoW.Me.CurrentTarget.Face();
        }


        public void AvoidEnemyAOE(WoWPoint location, List<WoWUnit> Units, string Aura, int TraceStep)
        {
            if (Units == null)
            { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP: no Pools found .."); return; }
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP: found {0} {1}! start RayCast ..", Units.Count, Aura);

            int MinDistToPools = 15;
            int MaxDistToMove = MinDistToPools * 2;

            // get save location
            WoWPoint newP = getSaveLocation(location, Units, MinDistToPools, MaxDistToMove, TraceStep);

            if (newP == WoWPoint.Empty)
            {
                // no save location found, move 2sec Strafe Left
                WoWMovement.Move(WoWMovement.MovementDirection.StrafeLeft, TimeSpan.FromSeconds(2));
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            else
            {
                // move to save location
                while (StyxWoW.Me.HasAura(Aura) && StyxWoW.Me.Location.Distance(newP) > 0.2f)
                {
                    Navigator.MoveTo(newP);
                    Thread.Sleep(80);
                }
            }

            WoWMovement.MoveStop();
            if (StyxWoW.Me.CurrentTargetGuid != 0)
                StyxWoW.Me.CurrentTarget.Face();
        }



        private bool wlog(WoWDynamicObject obj)
        { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP: add pool - dis2D: {0}", obj.Distance2D); return true; }

        private bool wlog(WoWUnit unit)
        { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP: add pool - dis2D: {0}", unit.Distance2D); return true; }


        public List<WoWDynamicObject> getVoidcloudList
        {
            get
            {
                ObjectManager.Update();
                return (from lp in ObjectManager.GetObjectsOfType<WoWDynamicObject>()
                        orderby lp.Distance2D ascending
                        where lp.Entry == 125241
                        where wlog(lp)
                        select lp).ToList();
            }
        }

        public List<WoWDynamicObject> getHarshWindsList
        {
            get
            {
                ObjectManager.Update();
                return (from lp in ObjectManager.GetObjectsOfType<WoWDynamicObject>()
                        orderby lp.Distance2D ascending
                        //where lp.Entry == 99999 ToDo: Entry herausfinden !!!
                        where lp.Name == "Harsh Winds"
                        where wlog(lp)
                        select lp).ToList();
            }
        }

        public List<WoWUnit> getTornadoList
        {
            get
            {
                ObjectManager.Update();
                return (from lp in ObjectManager.GetObjectsOfType<WoWUnit>()
                        orderby lp.Distance2D ascending
                        where lp.Entry == 64267
                        //where lp.Name == "Tornado"
                        where wlog(lp)
                        select lp).ToList();
            }
        }

        #region RayCast

        private WoWPoint getSaveLocation(WoWPoint Location, List<WoWDynamicObject> badObjects, int minDist, int maxDist, int traceStep)
        {
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP Navigation: Looking for save Location around {0}.", Location);

            try
            {
                //float _PIx2 = 3.14159f * 2f;
                float _PIx2 = ((float)new Random().Next(1, 80) * (1.248349f + (float)new Random().NextDouble()));

                for (int i = 0, x = minDist; i < traceStep && x < maxDist; i++)
                {
                    WoWPoint p = Location.RayCast((i * _PIx2) / traceStep, x);

                    p.Z = getGroundZ(p);

                    if (p.Z != float.MinValue && StyxWoW.Me.Location.Distance2D(p) > 1 &&
                        (badObjects.FirstOrDefault(_obj => _obj.Location.Distance2D(p) <= minDist) == null) &&
                        //(ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Location.Distance2D(p) < 20 && u.IsAlive && !u.Combat) == null) &&
                        Navigator.GeneratePath(StyxWoW.Me.Location, p).Length != 0)
                    {
                        if (getHighestSurroundingSlope(p) < 1.2f)
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP Navigation: Moving to {0}. Distance: {1}", p, Location.Distance(p));
                            return p;
                        }
                    }

                    if (i == (traceStep - 1))
                    {
                        i = 0;
                        x++;
                    }
                }
            }
            catch (Exception ex)
            { Logging.WriteException(ex); }


            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP - No valid points returned by RayCast ...");
            return WoWPoint.Empty;

        }

        private WoWPoint getSaveLocation(WoWPoint Location, List<WoWUnit> badUnit, int minDist, int maxDist, int traceStep)
        {
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP Navigation: Looking for save Location around {0}.", Location);

            try
            {
                //float _PIx2 = 3.14159f * 2f;
                float _PIx2 = ((float)new Random().Next(1, 80) * (1.248349f + (float)new Random().NextDouble()));

                for (int i = 0, x = minDist; i < traceStep && x < maxDist; i++)
                {
                    WoWPoint p = Location.RayCast((i * _PIx2) / traceStep, x);

                    p.Z = getGroundZ(p);

                    if (p.Z != float.MinValue && StyxWoW.Me.Location.Distance2D(p) > 1 &&
                        (badUnit.FirstOrDefault(_obj => _obj.Location.Distance2D(p) <= minDist) == null) &&
                        //(ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Location.Distance2D(p) < 20 && u.IsAlive && !u.Combat) == null) &&
                        Navigator.GeneratePath(StyxWoW.Me.Location, p).Length != 0)
                    {
                        if (getHighestSurroundingSlope(p) < 1.2f)
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP Navigation: Moving to {0}. Distance: {1}", p, Location.Distance(p));
                            return p;
                        }
                    }

                    if (i == (traceStep - 1))
                    {
                        i = 0;
                        x++;
                    }
                }
            }
            catch (Exception ex)
            { Logging.WriteException(ex); }


            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP - No valid points returned by RayCast ...");
            return WoWPoint.Empty;

        }

        /// <summary>
        /// Credits to exemplar.
        /// </summary>
        /// <returns>Z-Coordinates for PoolPoints so we don't jump into the water.</returns>
        private float getGroundZ(WoWPoint p)
        {
            WoWPoint ground = WoWPoint.Empty;

            GameWorld.TraceLine(new WoWPoint(p.X, p.Y, (p.Z + 100)), new WoWPoint(p.X, p.Y, (p.Z - 5)), GameWorld.CGWorldFrameHitFlags.HitTestGroundAndStructures/* | GameWorld.CGWorldFrameHitFlags.HitTestBoundingModels | GameWorld.CGWorldFrameHitFlags.HitTestWMO*/, out ground);

            if (ground != WoWPoint.Empty)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP - Ground Z: {0}.", ground.Z);
                return ground.Z;
            }
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP - Ground Z returned float.MinValue.");
            return float.MinValue;
        }

        /// <summary>
        /// Credits to funkescott.
        /// </summary>
        /// <returns>Highest slope of surrounding terrain, returns 100 if the slope can't be determined</returns>
        private float getHighestSurroundingSlope(WoWPoint p)
        {
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP Navigation: Sloapcheck on Point: {0}", p);
            float _PIx2 = 3.14159f * 2f;
            float highestSlope = -100;
            float slope = 0;
            int traceStep = 15;
            float range = 0.5f;
            WoWPoint p2;
            for (int i = 0; i < traceStep; i++)
            {
                p2 = p.RayCast((i * _PIx2) / traceStep, range);
                p2.Z = getGroundZ(p2);
                slope = Math.Abs(getSlope(p, p2));
                if (slope > highestSlope)
                {
                    highestSlope = (float)slope;
                }
            }
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MOP - Highslope {0}", highestSlope);
            return Math.Abs(highestSlope);
        }

        /// <summary>
        /// Credits to funkescott.
        /// </summary>
        /// <param name="p1">from WoWPoint</param>
        /// <param name="p2">to WoWPoint</param>
        /// <returns>Return slope from WoWPoint to WoWPoint.</returns>
        private float getSlope(WoWPoint p1, WoWPoint p2)
        {
            float rise = p2.Z - p1.Z;
            float run = (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

            return rise / run;
        }
        #endregion

        #endregion

        #region Avoid Enemy Cast
        /// <summary>
        /// this behavior will move the bot StrafeRight/StrafeLeft only if enemy is casting and we needed to move!
        /// Credits to BarryDurex.
        /// </summary>
        /// <param name="EnemyAttackRadius">EnemyAttackRadius or 0 for move Behind</param>
        public void AvoidEnemyCast(WoWUnit Unit, float EnemyAttackRadius, float SaveDistance)
        {
            if (!StyxWoW.Me.IsFacing(Unit))
            { Unit.Face(); Thread.Sleep(300); }

            float BehemothRotation = getPositive(Unit.RotationDegrees);
            float invertEnemyRotation = getInvert(BehemothRotation);

            WoWMovement.MovementDirection move = WoWMovement.MovementDirection.None;

            if (getPositive(StyxWoW.Me.RotationDegrees) > invertEnemyRotation)
            { move = WoWMovement.MovementDirection.StrafeRight; }
            else
            { move = WoWMovement.MovementDirection.StrafeLeft; }

            while (Unit.Distance2D <= SaveDistance && Unit.IsCasting && ((EnemyAttackRadius == 0 && !StyxWoW.Me.IsSafelyBehind(Unit)) ||
                (EnemyAttackRadius != 0 && Unit.IsSafelyFacing(StyxWoW.Me, EnemyAttackRadius)) || Unit.Distance2D <= 2))
            {
                WoWMovement.Move(move);
                Unit.Face();
            }
            WoWMovement.MoveStop();
        }

        private float getInvert(float f)
        {
            if (f < 180)
                return (f + 180);
            //else if (f >= 180)
            return (f - 180);
        }

        private float getPositive(float f)
        {
            if (f < 0)
                return (f + 360);
            return f;
        }
        #endregion
    }
}
