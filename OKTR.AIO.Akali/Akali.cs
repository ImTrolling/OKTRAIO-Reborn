using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using OKTR.Plugin;
using Extensions = OKTR.Plugin.Extensions;

namespace OKTR.AIO.Akali
{
    public class Akali : ChampionPluginBase<Spell.Targeted, Spell.Skillshot, Spell.Active, Spell.Targeted>
    {
        public override void InitializeSpells()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 600);
            W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Circular, 250, int.MaxValue, 400)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Active(SpellSlot.E, 325);
            R = new Spell.Targeted(SpellSlot.R, 700);
            DamageType = DamageType.Magical;
        }

        public override void InitializeEvents()
        {
            Gapcloser.OnGapcloser += OnGapCloser;
        }

        private void OnGapCloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsMe && sender.IsAlly && !MiscMenu.GetCheckBoxValue(MenuIds.MiscUseGapcloser)) return;
            if (MiscMenu.GetCheckBoxValue(52)) W.TryCast(Player.Instance);
            var ally = Extensions.CloseAllies(R.Range).FirstOrDefault();
            if (MiscMenu.GetCheckBoxValue(53)) RJump("Flee", ally);
        }

        public override void InitializeMenu()
        {
            AddMultipleCheckBox(Q,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(HarassMenu),
                    new MenuCheckBox(AutoHarassMenu),
                    new MenuCheckBox(LaneClearMenu),
                    new MenuCheckBox(JungleClearMenu),
                    new MenuCheckBox(LastHitMenu, false),
                    new MenuCheckBox(KillStealMenu),
                    new MenuCheckBox(DrawMenu)
                });
            AddMultipleCheckBox(W,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(FleeMenu),
                    new MenuCheckBox(DrawMenu)
                });
            AddMultipleCheckBox(E,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(HarassMenu),
                    new MenuCheckBox(AutoHarassMenu),
                    new MenuCheckBox(LaneClearMenu),
                    new MenuCheckBox(JungleClearMenu),
                    new MenuCheckBox(LastHitMenu, false),
                    new MenuCheckBox(KillStealMenu),
                    new MenuCheckBox(DrawMenu)
                });
            AddMultipleCheckBox(R,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(JungleClearMenu),
                    new MenuCheckBox(LastHitMenu, false),
                    new MenuCheckBox(FleeMenu, false),
                    new MenuCheckBox(KillStealMenu),
                    new MenuCheckBox(DrawMenu)
                });
            PlayerHasMana = false;

            FirstMenu.AddGroupLabel("Official plugin of OKTR- One Key To Report");

            ComboMenu.CreateCheckBox("Use R as GapCloser", 48, false);

            LaneClearMenu.CreateCheckBox("Prioritize Harass over Mode", 49, false);

            LastHitMenu.CreateCheckBox("Prioritize Harass over Mode", 50, false);

            KillStealMenu.CreateCheckBox("Use R for Jump & Kill", 51);


            MiscMenu.AddSeparator();
            MiscMenu.AddGroupLabel("AntiGapcloser - Interrupter settings:");
            MiscMenu.CreateCheckBox("Use AntiGapcloser", MenuIds.MiscUseGapcloser);
            MiscMenu.CreateCheckBox("Use W", 52);
            MiscMenu.CreateCheckBox("Use R", 53);

            MiscMenu.CreateCheckBox("AutoSpells on CC", 54);
            MiscMenu.CreateCheckBox("AutoSpells on Events", 55);
            MiscMenu.CreateCheckBox("Use Auto Q", 56, false);
            MiscMenu.CreateCheckBox("Use Q on Stunned Enemies", 57, false);
            MiscMenu.CreateCheckBox("Use Q on Snared Enemies", 58, false);
            MiscMenu.CreateCheckBox("Use Q on Feared Enemies", 59, false);
            MiscMenu.CreateCheckBox("Use Q on Taunted Enemy", 60, false);
            MiscMenu.CreateCheckBox("Use Q on Suppressed Enemy", 61, false);
            MiscMenu.CreateCheckBox("Use Q on Charmed Enemies", 62, false);

            MiscMenu.CreateCheckBox("Use Auto E", 63, false);
            MiscMenu.CreateCheckBox("Use E on Stunned Enemies", 64, false);
            MiscMenu.CreateCheckBox("Use E on Snared Enemies", 65, false);
            MiscMenu.CreateCheckBox("Use E on Feared Enemies", 66, false);
            MiscMenu.CreateCheckBox("Use E on Taunted Enemy", 67, false);
            MiscMenu.CreateCheckBox("Use E on Suppressed Enemy", 68, false);
            MiscMenu.CreateCheckBox("Use E on Charmed Enemies", 69, false);
            MiscMenu.CreateSlider("Energy Manager", 70, 60);
        }

        public override void Combo()
        {
            var target = GetTarget(1200);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "Q")) Q.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "W")) W.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "E")) E.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "R"))
            {
                RJump(ComboMenu.GetCheckBoxValue(48) ? "GapcloserCombo" : "Combo", target);
            }
        }

        public override void Harass()
        {
            var target = GetTarget(Q);
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "E")) E.TryCast(target);
        }

        public override void Laneclear()
        {
            var target = GetTarget(Q);
            var minion = E.GetLaneMinion();
            if (LaneClearMenu.GetCheckBoxValue(49))
            {
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "E")) E.TryCast(target);
            }
            else
            {
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "Q")) Q.TryCast(minion);
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "E")) E.TryCast(minion);
            }
        }

        public override void Lasthit()
        {
            var target = GetTarget(Q);
            var minion = R.GetLasthitLaneMinion();
            if (LastHitMenu.GetCheckBoxValue(50))
            {
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "E")) E.TryCast(target);
            }
            else
            {
                if (LastHitMenu.GetCheckBoxValue(LastMenuID + "Q")) Q.TryCast(minion);
                if (LastHitMenu.GetCheckBoxValue(LastMenuID + "E")) E.TryCast(minion);
            }
        }

        public override void Jungleclear()
        {
            var target = R.GetJungleMinion();
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "Q")) Q.TryCast(target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "W")) W.TryCast(target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "E")) E.TryCast(target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "R")) RJump("Lasthit", target);
        }

        public override void Flee()
        {
            var target = GetTarget(R);
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "W")) W.TryCast(Player.Instance);
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "R")) RJump("Flee", target);
        }

        public override void PermaActive()
        {
            var ks = R.GetKillableTarget();
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "Q")) Q.TryCast(ks);
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "E")) E.TryCast(ks);
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "R"))
            {
                RJump(ComboMenu.GetCheckBoxValue(51) ? "KillstealGapcloser" : "Killsteal", ks);
            }

            var target = GetTarget(1200);
            if (HarassMenu.GetCheckBoxValue(AutoHarassMenuID + "Q")) Q.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(AutoHarassMenuID + "E")) E.TryCast(target);

            if (MiscMenu.GetCheckBoxValue(54))
            {
                if (MiscMenu.GetCheckBoxValue(56))
                {
                    if (MiscMenu.GetCheckBoxValue(57) && target.IsStunned) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(58) && target.IsRooted) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(59) && target.IsFeared) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(60) && target.IsTaunted) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(61) && target.IsSuppressCallForHelp) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(62) && target.IsCharmed) Q.TryCast(target);
                }
                if (MiscMenu.GetCheckBoxValue(63))
                {
                    if (MiscMenu.GetCheckBoxValue(64) && target.IsStunned) E.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(65) && target.IsRooted) E.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(66) && target.IsFeared) E.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(67) && target.IsTaunted) E.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(68) && target.IsSuppressCallForHelp) E.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(69) && target.IsCharmed) E.TryCast(target);
                }
            }
        }

        public override void Draw()
        {
            DrawSpell(Q, Color.Red);
            DrawSpell(W, Color.Blue);
            DrawSpell(E, Color.Fuchsia);
            DrawSpell(R, Color.Green);
        }

        public void RJump(string mode, Obj_AI_Base target)
        {
            var near = R.GetNearestMinion(target);
            var furthest = target.GetTheFurthestMinionInRange(R);
            switch (mode)
            {
                case "Combo":
                    R.TryCast(target);
                    break;
                case "Lasthit":
                    R.TryCast(target);
                    break;
                case "GapcloserCombo":
                    if (target.IsValidTarget(R.Range))
                    {
                        goto case "Combo";
                    }
                    R.TryCast(near);
                    break;
                case "GapcloserKillsteal":
                    if (target.IsValidTarget(R.Range))
                    {
                        goto case "Killsteal";
                    }
                    R.TryCast(near);
                    break;
                case "Flee":
                    R.TryCast(furthest);
                    break;
                case "Killsteal":
                    R.TryCast(target);
                    break;
            }
        }
    }
}