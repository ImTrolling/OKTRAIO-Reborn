using System.Collections.Generic;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using OKTR.Plugin;

namespace OKTR.AIO.Aatrox
{
    [Plugin("Aatrox", "iRaxe", Champion.Aatrox)]
    public class Aatrox : ChampionPluginBase<Spell.Skillshot, Spell.Active, Spell.Skillshot, Spell.Active>
    {
        public override void InitializeSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 650, SkillShotType.Circular, 250, 2000, 600)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 1075, SkillShotType.Linear, 35, 1250, 250)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Active(SpellSlot.R, 550);
            DamageType = DamageType.Physical;
        }

        public override void InitializeEvents()
        {
            Gapcloser.OnGapcloser += OnGapCloser;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
            Orbwalker.OnPreAttack += OnPreAttack;
        }

        private void OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "W")) WCast(ComboMenu, 48, target);
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "W")) WCast(HarassMenu, 49, target);
            if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "W")) WCast(LaneClearMenu, 50, target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "W")) WCast(JungleClearMenu, 52, target);
        }

        private void OnGapCloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsMe && sender.IsAlly && !MiscMenu.GetCheckBoxValue(MenuIds.MiscUseGapcloser)) return;
            if (MiscMenu.GetCheckBoxValue(54)) Q.Cast(e.End + 5*(Player.Instance.Position - e.End));
            if (MiscMenu.GetCheckBoxValue(55)) E.TryCast(sender);
        }

        private void OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsMe && sender.IsAlly && !MiscMenu.GetCheckBoxValue(MenuIds.MiscUseInterrupt)) return;
            if (MiscMenu.GetCheckBoxValue(56)) Q.TryCast(sender);
        }

        public override void InitializeMenu()
        {
            AddMultipleCheckBox(Q,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(HarassMenu),
                    new MenuCheckBox(LaneClearMenu, false),
                    new MenuCheckBox(JungleClearMenu),
                    new MenuCheckBox(LastHitMenu, false),
                    new MenuCheckBox(FleeMenu),
                    new MenuCheckBox(KillStealMenu),
                    new MenuCheckBox(DrawMenu)
                });
            AddMultipleCheckBox(W,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(HarassMenu),
                    new MenuCheckBox(LaneClearMenu),
                    new MenuCheckBox(JungleClearMenu),
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
                    new MenuCheckBox(FleeMenu),
                    new MenuCheckBox(KillStealMenu),
                    new MenuCheckBox(DrawMenu)
                });
            AddMultipleCheckBox(R,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(DrawMenu, false)
                });
            PlayerHasMana = false;

            FirstMenu.AddGroupLabel("Official plugin of OKTR- One Key To Report");

            ComboMenu.CreateSlider("Change W when under {1} HP %", 48, 80);

            HarassMenu.CreateSlider("Change W when under {1} HP %", 49, 80);

            LaneClearMenu.CreateSlider("Change W when under {1} HP %", 50, 90);
            LaneClearMenu.CreateCheckBox("Prioritize Harass over Mode", 51, false);

            JungleClearMenu.CreateSlider("Change W when under {1} HP %", 52, 80);

            LastHitMenu.CreateCheckBox("Prioritize Harass over Mode", 53, false);

            MiscMenu.AddSeparator();
            MiscMenu.AddGroupLabel("AntiGapcloser - Interrupter settings:");
            MiscMenu.CreateCheckBox("Use AntiGapcloser", MenuIds.MiscUseGapcloser);
            MiscMenu.CreateCheckBox("Use Q", 54);
            MiscMenu.CreateCheckBox("Use E", 55);

            MiscMenu.AddSeparator();
            MiscMenu.CreateCheckBox("Use Interrupter", MenuIds.MiscUseInterrupt);
            MiscMenu.CreateCheckBox("Use Q", 56);
            MiscMenu.CreateCheckBox("AutoSpells on CC", 57);
            MiscMenu.CreateCheckBox("AutoSpells on Events", 58);

            MiscMenu.AddSeparator();
            MiscMenu.CreateCheckBox("Use Auto Q", 59, false);
            MiscMenu.CreateCheckBox("Use Q on Stunned Enemies", 60, false);
            MiscMenu.CreateCheckBox("Use Q on Snared Enemies", 61, false);
            MiscMenu.CreateCheckBox("Use Q on Feared Enemies", 62, false);
            MiscMenu.CreateCheckBox("Use Q on Taunted Enemy", 63, false);
            MiscMenu.CreateCheckBox("Use Q on Suppressed Enemy", 64, false);
            MiscMenu.CreateCheckBox("Use Q on Charmed Enemies", 65, false);

            MiscMenu.CreateCheckBox("Use Auto E", 66, false);
            MiscMenu.CreateCheckBox("Use E on Stunned Enemies", 67, false);
            MiscMenu.CreateCheckBox("Use E on Snared Enemies", 68, false);
            MiscMenu.CreateCheckBox("Use E on Feared Enemies", 69, false);
            MiscMenu.CreateCheckBox("Use E on Taunted Enemy", 70, false);
            MiscMenu.CreateCheckBox("Use E on Suppressed Enemy", 71, false);
            MiscMenu.CreateCheckBox("Use E on Charmed Enemies", 72, false);
        }

        public override void Combo()
        {
            var target = GetTarget(E);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "Q")) Q.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "E")) E.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "R")) R.TryCast(target);
        }

        public override void Harass()
        {
            var target = GetTarget(E);
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "E")) E.TryCast(target);
        }

        public override void Laneclear()
        {
            var target = GetTarget(W);
            var minion = W.GetLaneMinion();
            if (LaneClearMenu.GetCheckBoxValue(51))
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
            var target = GetTarget(W);
            var minion = W.GetLasthitLaneMinion();
            if (LastHitMenu.GetCheckBoxValue(44))
            {
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "W")) WCast(HarassMenu, 53, target);
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
            var target = W.GetJungleMinion();
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "Q")) Q.TryCast(target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "E")) E.TryCast(target);
        }

        public override void Flee()
        {
            var target = GetTarget(E);
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "Q")) Q.Cast(Game.CursorPos);
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "E")) E.TryCast(target);
        }

        public override void PermaActive()
        {
            var ks = R.GetKillableTarget();
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "Q")) Q.TryCast(ks);
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "E")) E.TryCast(ks);

            var target = GetTarget(E);
            if (HarassMenu.GetCheckBoxValue(AutoHarassMenuID + "E")) E.TryCast(target);

            if (MiscMenu.GetCheckBoxValue(57))
            {
                if (MiscMenu.GetCheckBoxValue(59))
                {
                    if (MiscMenu.GetCheckBoxValue(60) && target.IsStunned) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(61) && target.IsRooted) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(62) && target.IsFeared) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(63) && target.IsTaunted) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(64) && target.IsSuppressCallForHelp) Q.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(65) && target.IsCharmed) Q.TryCast(target);
                }
                if (MiscMenu.GetCheckBoxValue(66))
                {
                    if (MiscMenu.GetCheckBoxValue(67) && target.IsStunned) E.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(68) && target.IsRooted) E.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(69) && target.IsFeared) E.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(70) && target.IsTaunted) E.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(71) && target.IsSuppressCallForHelp) E.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(72) && target.IsCharmed) E.TryCast(target);
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

        private static bool WActive()
        {
            return Player.GetSpell(SpellSlot.W).ToggleState == 2;
        }

        private void WCast(Menu m, int uid, AttackableUnit target)
        {
            if (!W.IsReady() || !target.IsValidTarget(W.Range)) return;
            if (WActive() && Player.Instance.HealthPercent <= m.GetSliderValue(uid)) W.Cast();
            if (!WActive() && Player.Instance.HealthPercent >= m.GetSliderValue(uid)) W.Cast();
        }
    }
}