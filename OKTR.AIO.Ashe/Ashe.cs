using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using OKTR.Plugin;
using Color = System.Drawing.Color;

namespace OKTR.AIO.Ashe
{
    [Plugin("Ashe", "iRaxe", Champion.Ashe)]
    public class Ashe : ChampionPluginBase<Spell.Active, Spell.Skillshot, Spell.Skillshot, Spell.Skillshot>
    {
        private int _mode;

        public override void InitializeSpells()
        {
            Q = new Spell.Active(SpellSlot.Q, 600);
            W = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Cone, 250, 2000, 20)
            {
                AllowedCollisionCount = 0,
                ConeAngleDegrees = 90,
                MinimumHitChance = HitChance.Medium
            };
            E = new Spell.Skillshot(SpellSlot.E, 2500, SkillShotType.Linear, 250, 1400, 300)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear, 250, 1600, 130)
            {
                AllowedCollisionCount = 0
            };
            DamageType = DamageType.Physical;
        }

        public override void InitializeEvents()
        {
            Gapcloser.OnGapcloser += OnGapcloser;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
        }

        public override void InitializeMenu()
        {
            AddMultipleCheckBox(Q,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(HarassMenu, false),
                    new MenuCheckBox(LaneClearMenu, false),
                    new MenuCheckBox(JungleClearMenu),
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
                    new MenuCheckBox(DrawMenu, false)
                });
            AddMultipleCheckBox(R,
                new List<MenuCheckBox>
                {
                    new MenuCheckBox(ComboMenu),
                    new MenuCheckBox(FleeMenu, false),
                    new MenuCheckBox(KillStealMenu),
                    new MenuCheckBox(DrawMenu, false)
                });

            ComboMenu.CreateSlider("Use R if the target leave {1} range", 48, 400, 100, 2000);
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("Mode Switcher");
            ComboMenu.AddLabel("Burst Mode = R -> W -> Q");
            ComboMenu.AddLabel("Normal Mode = Q -> W -> (??) R");
            ComboMenu.CreateKeybind("Change mode by key", 49, 'T').OnValueChange += ModeSwitch;

            LaneClearMenu.CreateCheckBox("Prioritize Harass over Mode", 50, false);

            LastHitMenu.CreateCheckBox("Prioritize Harass over Mode", 51, false);

            FleeMenu.CreateSlider("Use R if enemy is near {1} range", 52, 1200, 100, 2000);

            MiscMenu.AddSeparator();
            MiscMenu.AddGroupLabel("AntiGapcloser - Interrupter settings:");
            MiscMenu.CreateCheckBox("Use AntiGapcloser", MenuIds.MiscUseGapcloser);
            MiscMenu.CreateCheckBox("Use W", 53);
            MiscMenu.CreateCheckBox("Use R", 54);

            MiscMenu.AddSeparator();
            MiscMenu.CreateCheckBox("Use Interrupter", MenuIds.MiscUseInterrupt);
            MiscMenu.CreateCheckBox("Use R", 55);

            MiscMenu.AddSeparator();
            MiscMenu.CreateCheckBox("AutoSpells on CC", 56);
            MiscMenu.CreateCheckBox("AutoSpells on Events", 57);
            MiscMenu.CreateCheckBox("Use Auto W", 58);
            MiscMenu.CreateCheckBox("Use W on Stunned Enemies", 59);
            MiscMenu.CreateCheckBox("Use W on Snared Enemies", 60);
            MiscMenu.CreateCheckBox("Use W on Feared Enemies", 61);
            MiscMenu.CreateCheckBox("Use W on Taunted Enemy", 62);
            MiscMenu.CreateCheckBox("Use W on Suppressed Enemy", 63);
            MiscMenu.CreateCheckBox("Use W on Charmed Enemies", 64);
            MiscMenu.CreateCheckBox("Use Auto R", 65);
            MiscMenu.CreateCheckBox("Use R on Stunned Enemies", 66, false);
            MiscMenu.CreateCheckBox("Use R on Snared Enemies", 67, false);
            MiscMenu.CreateCheckBox("Use R on Feared Enemies", 68, false);
            MiscMenu.CreateCheckBox("Use R on Taunted Enemy", 69, false);
            MiscMenu.CreateCheckBox("Use R on Suppressed Enemy", 70, false);
            MiscMenu.CreateCheckBox("Use R on Charmed Enemies", 71, false);
        }

        public override void Combo()
        {
            var target = GetTarget(2000);
            if (_mode == 0) NormalCombo(target);
            if (_mode == 1) BurstCombo(target);
        }

        private void BurstCombo(Obj_AI_Base target)
        {
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "R")) R.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "W")) W.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "Q")) Q.TryCast(target);
        }

        private void NormalCombo(Obj_AI_Base target)
        {
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "Q")) Q.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(ComboMenuID + "W")) W.TryCast(target);


            if (!KillStealMenu.GetCheckBoxValue(KillstealMenuID + "R") &&
                ComboMenu.GetCheckBoxValue(ComboMenuID + "R") &&
                target.Distance(Player.Instance) >= ComboMenu.GetSliderValue(48))
            {
                R.TryCast(target);
            }
        }

        public override void Harass()
        {
            var target = GetTarget(W);
            if (HarassMenu.GetSliderValue(HarassMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "W")) W.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
        }

        public override void Laneclear()
        {
            var target = GetTarget(W);
            var minion = W.GetLaneMinion();
            if (LaneClearMenu.GetSliderValue(LaneMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (LaneClearMenu.GetCheckBoxValue(50))
            {
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "Q")) Q.TryCast(target);
                if (HarassMenu.GetCheckBoxValue(HarassMenuID + "W")) W.TryCast(target);
            }
            else
            {
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "Q")) Q.TryCast(minion);
                if (LaneClearMenu.GetCheckBoxValue(LaneMenuID + "W")) W.TryCast(minion);
            }
        }

        public override void Lasthit()
        {
            var target = GetTarget(W);
            var minion = W.GetLasthitLaneMinion();
            if (LastHitMenu.GetSliderValue(LastMenuID + "mana") >= Player.Instance.ManaPercent) return;
            if (LastHitMenu.GetCheckBoxValue(LastMenuID + "W"))
            {
                if (LastHitMenu.GetCheckBoxValue(51)) W.TryCast(target);
                else W.TryCast(minion);
            }
        }

        public override void Jungleclear()
        {
            var target = W.GetJungleMinion();
            if (JungleClearMenu.GetSliderValue(JunglesMenuID +"mana") >= Player.Instance.ManaPercent) return;
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "Q")) Q.TryCast(target);
            if (JungleClearMenu.GetCheckBoxValue(JunglesMenuID + "W")) W.TryCast(target);
        }

        public override void Flee()
        {
            var target = GetTarget(R);
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "W")) W.TryCast(target);
            if (FleeMenu.GetCheckBoxValue(FleeMenuID + "R") &&
                FleeMenu.GetSliderValue(52) <= target.Distance(Player.Instance))
            {
                R.TryCast(target);
            }
        }

        public override void PermaActive()
        {
            var ks = R.GetKillableTarget();
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "W")) W.TryCast(ks);
            if (KillStealMenu.GetCheckBoxValue(KillstealMenuID + "R")) R.TryCast(ks);

            var target = GetTarget(W);
            if (HarassMenu.GetCheckBoxValue(AutoHarassMenuID + "W") &&
                HarassMenu.GetSliderValue(AutoHarassMenuID +"mana") <= Player.Instance.ManaPercent)
            {
                W.TryCast(target);
            }

            if (MiscMenu.GetSliderValue(MiscMenuID + "mana") <= Player.Instance.ManaPercent && MiscMenu.GetCheckBoxValue(56))
            {
                if (MiscMenu.GetCheckBoxValue(52))
                {
                    if (MiscMenu.GetCheckBoxValue(53) && target.IsStunned) W.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(54) && target.IsRooted) W.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(55) && target.IsFeared) W.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(56) && target.IsTaunted) W.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(57) && target.IsSuppressCallForHelp) W.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(58) && target.IsCharmed) W.TryCast(target);
                }
                if (MiscMenu.GetCheckBoxValue(59))
                {
                    if (MiscMenu.GetCheckBoxValue(60) && target.IsStunned) R.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(61) && target.IsRooted) R.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(62) && target.IsFeared) R.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(63) && target.IsTaunted) R.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(64) && target.IsSuppressCallForHelp) R.TryCast(target);
                    if (MiscMenu.GetCheckBoxValue(65) && target.IsCharmed) R.TryCast(target);
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

        private void OnGapcloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsMe && sender.IsAlly && !MiscMenu.GetCheckBoxValue(MenuIds.MiscUseGapcloser)) return;
            if (MiscMenu.GetCheckBoxValue(53)) W.TryCast(sender);
            if (MiscMenu.GetCheckBoxValue(54)) R.TryCast(sender);
        }

        private void OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsMe && sender.IsAlly && !MiscMenu.GetCheckBoxValue(MenuIds.MiscUseInterrupt)) return;
            if (MiscMenu.GetCheckBoxValue(55)) R.TryCast(sender);
        }

        private void ModeSwitch(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (args.NewValue)
            {
                switch (_mode)
                {
                    case 0:
                        _mode = 1;
                        Chat.Print("Combo mode changed to: Burst Combo");
                        break;
                    case 1:
                        _mode = 0;
                        Chat.Print("Combo mode changed to: Normal Combo");
                        break;
                }
            }
        }
    }
}