using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using OKTR.Plugin;
using Color = System.Drawing.Color;

namespace OKTR.AIO.Ahri
{
    [Plugin("Ahri", "iRaxe", Champion.Ahri)]
    public class Ahri : ChampionPluginBase<Spell.Skillshot, Spell.Active, Spell.Skillshot, Spell.Skillshot>
    {
        public override void InitializeSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 880, SkillShotType.Linear, 250, 1500, 100)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Active(SpellSlot.W, 700);
            E = new Spell.Skillshot(SpellSlot.E, 975, SkillShotType.Linear, 250, 1550, 60)
            {
                AllowedCollisionCount = 0
            };
            R = new Spell.Skillshot(SpellSlot.R, 800, SkillShotType.Circular, 0, 1400, 300)
            {
                AllowedCollisionCount = int.MaxValue
            };
            DamageType = DamageType.Magical;
        }

        public override void InitializeEvents()
        {
            Gapcloser.OnGapcloser += OnGapCloser;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
        }

        private void OnGapCloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsMe && sender.IsAlly && !MiscMenu.GetCheckBoxValue(MenuIds.MiscUseGapcloser)) return;
            if (MiscMenu.GetCheckBoxValue(50)) Q.TryCast(sender);
            if (MiscMenu.GetCheckBoxValue(51)) E.TryCast(sender);
            if (MiscMenu.GetCheckBoxValue(52)) R.Cast(e.End + 5 * (Player.Instance.Position - e.End));
        }
        
        private void OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsMe && sender.IsAlly && !MiscMenu.GetCheckBoxValue(MenuIds.MiscUseInterrupt)) return;
            if (MiscMenu.GetCheckBoxValue(53)) E.TryCast(sender);
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
                    new MenuCheckBox(LastHitMenu, false),
                    new MenuCheckBox(FleeMenu),
                    new MenuCheckBox(KillStealMenu),
                    new MenuCheckBox(DrawMenu)
                });
            AddMultipleCheckBox(R,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(LaneClearMenu, false),
                    new MenuCheckBox(JungleClearMenu, false),
                    new MenuCheckBox(LastHitMenu),
                    new MenuCheckBox(FleeMenu, false),
                    new MenuCheckBox(KillStealMenu),
                    new MenuCheckBox(DrawMenu)
                });

            FirstMenu.AddGroupLabel("Official plugin of OKTR- One Key To Report");

            ComboMenu.AddGroupLabel("Mode Switcher");
            ComboMenu.AddLabel("Burst Mode = R -> E -> W -> Q");
            ComboMenu.AddLabel("Normal Mode = Q -> W -> E -> R");
            ComboMenu.CreateComboBox("Switch your Combo Mode", "combo.Switcher", new[] {"Burst Mode", "Normal Mode"});

            LaneClearMenu.CreateCheckBox("Prioritize Harass over Mode", 48, false);

            LastHitMenu.CreateCheckBox("Prioritize Harass over Mode", 49, false);

            MiscMenu.AddSeparator();
            MiscMenu.AddGroupLabel("AntiGapcloser - Interrupter settings:");
            MiscMenu.CreateCheckBox("Use AntiGapcloser", MenuIds.MiscUseGapcloser);
            MiscMenu.CreateCheckBox("Use Q", 50);
            MiscMenu.CreateCheckBox("Use E", 51);
            MiscMenu.CreateCheckBox("Use R", 52, false);

            MiscMenu.AddSeparator();
            MiscMenu.CreateCheckBox("Use Interrupter", MenuIds.MiscUseInterrupt);
            MiscMenu.CreateCheckBox("Use E", 53);

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
        }

        public override void Combo()
        {
            var target = GetTarget(1200);
            if (ComboMenu.GetComboboxIndex("combo.Switcher") == 0) BurstCombo(target);
            if (ComboMenu.GetComboboxIndex("combo.Switcher") == 1) NormalCombo(target);
        }

        private void BurstCombo(Obj_AI_Base target)
        {
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "R")) R.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "E")) E.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "W")) W.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "Q")) Q.TryCast(target);
        }

        private void NormalCombo(Obj_AI_Base target)
        {
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "Q")) Q.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "W")) W.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "E")) E.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "R")) R.TryCast(target);
        }

        public override void Harass()
        {
            var target = GetTarget(E);
            if (HarassMenu.GetSliderValue(HarassMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "W")) W.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "E")) E.TryCast(target);
        }

        public override void Laneclear()
        {
            var target = GetTarget(E);
            var minion = E.GetLaneMinion();
            if (LaneClearMenu.GetSliderValue(LaneMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (LaneClearMenu.GetCheckBoxValue(48))
            {
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "W")) W.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "E")) E.TryCast(target);
            }
            else
            {
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "Q")) Q.TryCast(minion);
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "W")) W.TryCast(minion);
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "E")) E.TryCast(minion);
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "R")) R.TryCast(minion);
            }
        }

        public override void Lasthit()
        {
            var target = GetTarget(E);
            var minion = E.GetLasthitLaneMinion();
            if (LastHitMenu.GetSliderValue(LastMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (LastHitMenu.GetCheckBoxValue(49))
            {
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "W")) W.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "E")) E.TryCast(target);
            }
            else
            {
                if (LastHitMenu.GetCheckBoxValue(LastMenuID + "Q")) Q.TryCast(minion);
                if (LastHitMenu.GetCheckBoxValue(LastMenuID + "W")) W.TryCast(minion);
                if (LastHitMenu.GetCheckBoxValue(LastMenuID + "E")) E.TryCast(minion);
            }
        }

        public override void Jungleclear()
        {
            var target = E.GetJungleMinion();
            if (JungleClearMenu.GetSliderValue(JunglesMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "Q")) Q.TryCast(target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "W")) W.TryCast(target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "E")) E.TryCast(target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "R")) R.TryCast(target);
        }

        public override void Flee()
        {
            var target = GetTarget(1200);
            var dash = Player.Instance.ServerPosition.Extend(Game.CursorPos, R.Range).To3D();
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "Q")) Q.TryCast(target);
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "W")) E.TryCast(target);
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "R")) R.Cast(dash);
        }

        public override void PermaActive()
        {
            var ks = R.GetKillableTarget();
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "Q")) Q.TryCast(ks);
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "W")) W.TryCast(ks);
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "E")) E.TryCast(ks);
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "R")) R.TryCast(ks);

            var target = GetTarget(1200);
            if (HarassMenu.GetSliderValue(AutoHarassMenuID + "mana") <= Player.Instance.ManaPercent)
            {
                if (HarassMenu.GetCheckBoxValue(AutoHarassMenuID + "Q")) Q.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(AutoHarassMenuID + "W")) W.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(AutoHarassMenuID + "E")) E.TryCast(target);
            }

            if (MiscMenu.GetSliderValue(MiscMenuID + "mana") <= Player.Instance.ManaPercent && MiscMenu.GetCheckBoxValue(54))
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
    }
}
