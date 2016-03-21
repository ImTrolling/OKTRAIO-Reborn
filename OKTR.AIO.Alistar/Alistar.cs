using System.Collections.Generic;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using OKTR.Plugin;

namespace OKTR.AIO.Alistar
{
    [Plugin("Alistar", "iRaxe", Champion.Alistar)]
    public class Alistar : ChampionPluginBase<Spell.Active, Spell.Targeted, Spell.Active, Spell.Active>
    {
        public override void InitializeSpells()
        {
            Q = new Spell.Active(SpellSlot.Q, 365);
            W = new Spell.Targeted(SpellSlot.W, 650);
            E = new Spell.Active(SpellSlot.E, 575);
            R = new Spell.Active(SpellSlot.R);
        }

        public override void InitializeEvents()
        {
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
                    new MenuCheckBox(LastHitMenu),
                    new MenuCheckBox(FleeMenu),
                    new MenuCheckBox(KillStealMenu),
                    new MenuCheckBox(DrawMenu)
                });
            AddMultipleCheckBox(W,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(HarassMenu),
                    new MenuCheckBox(AutoHarassMenu, false),
                    new MenuCheckBox(LaneClearMenu),
                    new MenuCheckBox(JungleClearMenu),
                    new MenuCheckBox(LastHitMenu),
                    new MenuCheckBox(FleeMenu),
                    new MenuCheckBox(KillStealMenu),
                    new MenuCheckBox(DrawMenu)
                });
            AddMultipleCheckBox(E,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(HarassMenu),
                    new MenuCheckBox(AutoHarassMenu, false),
                    new MenuCheckBox(LaneClearMenu, false),
                    new MenuCheckBox(JungleClearMenu),
                    new MenuCheckBox(DrawMenu)
                });
            AddMultipleCheckBox(R,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(DrawMenu, false)
                });

            FirstMenu.AddGroupLabel("Official plugin of OKTR- One Key To Report");

            ComboMenu.CreateSlider("Use E if under {1} HP %", 48, 40);

            HarassMenu.CreateSlider("Use E if under {1} HP %", 49, 40);
            AutoHarassMenu.CreateSlider("Use E if under {1} HP %", 50, 40);

            LaneClearMenu.CreateSlider("Use E if under {1} HP %", 51, 40);
            LaneClearMenu.CreateCheckBox("Prioritize Harass over Mode", 52, false);

            JungleClearMenu.CreateSlider("Use E if under {1} HP %", 53, 40);

            LastHitMenu.CreateCheckBox("Prioritize Harass over Mode", 53, false);

            MiscMenu.AddSeparator();
            MiscMenu.AddGroupLabel("AntiGapcloser - Interrupter settings:");
            MiscMenu.CreateCheckBox("Use AntiGapcloser", MenuIds.MiscUseGapcloser);
            MiscMenu.CreateCheckBox("Use Q", 54);
            MiscMenu.CreateCheckBox("Use W", 55);

            MiscMenu.AddSeparator();
            MiscMenu.CreateCheckBox("Use Interrupter", MenuIds.MiscUseInterrupt);
            MiscMenu.CreateCheckBox("Use Q", 56);
            MiscMenu.CreateCheckBox("Use W", 57);

            MiscMenu.CreateCheckBox("AutoSpells on CC", 58);
            MiscMenu.CreateCheckBox("AutoSpells on Events", 59);
            MiscMenu.CreateCheckBox("Use Auto Q", 60, false);
            MiscMenu.CreateCheckBox("Use Q on Stunned Enemies", 61, false);
            MiscMenu.CreateCheckBox("Use Q on Snared Enemies", 62, false);
            MiscMenu.CreateCheckBox("Use Q on Feared Enemies", 63, false);
            MiscMenu.CreateCheckBox("Use Q on Taunted Enemy", 64, false);
            MiscMenu.CreateCheckBox("Use Q on Suppressed Enemy", 65, false);
            MiscMenu.CreateCheckBox("Use Q on Charmed Enemies", 66, false);

            MiscMenu.CreateCheckBox("Use Auto W", 67, false);
            MiscMenu.CreateCheckBox("Use W on Stunned Enemies", 68, false);
            MiscMenu.CreateCheckBox("Use W on Snared Enemies", 69, false);
            MiscMenu.CreateCheckBox("Use W on Feared Enemies", 70, false);
            MiscMenu.CreateCheckBox("Use W on Taunted Enemy", 71, false);
            MiscMenu.CreateCheckBox("Use W on Suppressed Enemy", 72, false);
            MiscMenu.CreateCheckBox("Use W on Charmed Enemies", 73, false);
        }

        public override void Combo()
        {
            var target = GetTarget(1200);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "Q")) Q.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "W")) W.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "E") &&
                ComboMenu.GetSliderValue(48) <= Player.Instance.HealthPercent)
            {
                E.TryCast(target);
            }
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "R")) R.TryCast(target);
        }

        public override void Harass()
        {
            var target = GetTarget(W);
            if (HarassMenu.GetSliderValue(HarassMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "W")) W.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "E") &&
                HarassMenu.GetSliderValue(49) <= Player.Instance.HealthPercent)
            {
                E.TryCast(target);
            }
        }

        public override void Laneclear()
        {
            var target = GetTarget(E);
            var minion = E.GetLaneMinion();
            if (LaneClearMenu.GetSliderValue(LaneMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (LaneClearMenu.GetCheckBoxValue(52))
            {
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "W")) W.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "E") &&
                    HarassMenu.GetSliderValue(49) <= Player.Instance.HealthPercent)
                {
                    E.TryCast(target);
                }
            }
            else
            {
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "Q")) Q.TryCast(minion);
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "W")) W.TryCast(minion);
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "E") &&
                    LaneClearMenu.GetSliderValue(51) <= Player.Instance.HealthPercent)
                {
                    E.TryCast(minion);
                }
            }
        }

        public override void Lasthit()
        {
            var target = GetTarget(E);
            var minion = E.GetLasthitLaneMinion();
            if (LastHitMenu.GetSliderValue(LastMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (LastHitMenu.GetCheckBoxValue(53))
            {
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "W")) W.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "E") &&
                    HarassMenu.GetSliderValue(49) <= Player.Instance.HealthPercent)
                {
                    E.TryCast(target);
                }
            }
            else
            {
                if (LastHitMenu.GetCheckBoxValue(LastMenuID + "Q")) Q.TryCast(minion);
                if (LastHitMenu.GetCheckBoxValue(LastMenuID + "W")) W.TryCast(minion);
            }
        }

        public override void Jungleclear()
        {
            var target = E.GetJungleMinion();
            if (JungleClearMenu.GetSliderValue(JunglesMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "Q")) Q.TryCast(target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "W")) W.TryCast(target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "E") &&
                JungleClearMenu.GetSliderValue(53) <= Player.Instance.HealthPercent)
            {
                E.TryCast(target);
            }
        }

        public override void Flee()
        {
            var target = GetTarget(1200);
            var minion = target.GetTheFurthestMinionInRange(W);
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "Q")) Q.TryCast(target);
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "W"))
            {
                if (minion != null) W.TryCast(minion);
                else W.TryCast(target);
            }
        }

        public override void PermaActive()
        {
            var ks = W.GetKillableTarget();
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "Q")) Q.TryCast(ks);
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "W")) W.TryCast(ks);

            var target = GetTarget(1200);
            if (HarassMenu.GetSliderValue(AutoHarassMenuID + "mana") <= Player.Instance.ManaPercent)
            {
                if (HarassMenu.GetCheckBoxValue(AutoHarassMenuID + "Q")) Q.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(AutoHarassMenuID + "W")) W.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(AutoHarassMenuID + "E") &&
                    HarassMenu.GetSliderValue(50) <= Player.Instance.HealthPercent)
                {
                    E.TryCast(target);
                }
            }

            if (MiscMenu.GetSliderValue(MiscMenuID + "mana") <= Player.Instance.ManaPercent &&
                MiscMenu.GetCheckBoxValue(58))
            {
                if (MiscMenu.GetCheckBoxValue(60))
                {
                    if (MiscMenu.GetCheckBoxValue(61) && target.IsStunned) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(62) && target.IsRooted) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(63) && target.IsFeared) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(64) && target.IsTaunted) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(65) && target.IsSuppressCallForHelp) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(66) && target.IsCharmed) Q.TryCast(target);
                }
                if (MiscMenu.GetCheckBoxValue(67))
                {
                    if (MiscMenu.GetCheckBoxValue(68) && target.IsStunned) W.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(69) && target.IsRooted) W.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(70) && target.IsFeared) W.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(71) && target.IsTaunted) W.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(72) && target.IsSuppressCallForHelp) W.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(73) && target.IsCharmed) W.TryCast(target);
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
    }
}