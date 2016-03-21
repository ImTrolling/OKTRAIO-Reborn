using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

namespace OKTR.Plugin
{
    public abstract class ChampionPluginBase<TSpellQ, TSpellW, TSpellE, TSpellR> : PluginBase
    {
        #region Abstract
        protected Menu FirstMenu { get; private set; }
        public Menu ComboMenu { get; private set; }
        public Menu HarassMenu { get; private set; }
        public Menu AutoHarassMenu { get; private set; }
        public Menu LaneClearMenu { get; private set; }
        public Menu JungleClearMenu { get; private set; }
        public Menu LastHitMenu { get; private set; }
        public Menu FleeMenu { get; private set; }
        public Menu MiscMenu { get; private set; }
        public Menu KillStealMenu { get; private set; }
        public Menu DrawMenu { get; private set; }

        public const string ComboMenuID = "comboOKTR";
        public const string HarassMenuID = "harassOKTR";
        public const string AutoHarassMenuID = "autoharassOKTR";
        public const string LaneMenuID = "laneOKTR";
        public const string JunglesMenuID = "jungleOKTR";
        public const string LastMenuID = "lastOKTR";
        public const string FleeMenuID = "fleeOKTR";
        public const string KillstealMenuID = "killstealOKTR";
        public const string DrawMenuID = "drawOKTR";
        public const string MiscMenuID = "miscOKTR";

        //Spell
        public TSpellQ Q;
        public TSpellW W;
        public TSpellE E;
        public TSpellR R;
        public bool PlayerHasMana = true;
        //Damage type
        public DamageType DamageType;

        public abstract void InitializeSpells();
        public abstract void InitializeEvents();
        public abstract void InitializeMenu();

        public override void Initialize()
        {
            FirstMenu = MainMenu.AddMenu("OKTR " + GetAttribute().Name, "id" + GetAttribute().Name.ToLower());
            FirstMenu.AddGroupLabel("Introduction:");
            FirstMenu.AddSeparator();
            FirstMenu.AddGroupLabel("Author: " + GetAttribute().Author);
            FirstMenu.AddLabel("Plugin From: http://oktraio.com");
            ComboMenu = FirstMenu.AddSubMenu("Combo", ComboMenuID);
            ComboMenu.AddGroupLabel("Combo Settings:");
            HarassMenu = FirstMenu.AddSubMenu("Harass", HarassMenuID);
            HarassMenu.AddGroupLabel("Harass Settings:");
            AutoHarassMenu = FirstMenu.AddSubMenu("Auto Harass", AutoHarassMenuID);
            AutoHarassMenu.AddGroupLabel("Auto Harass Settings:");
            LaneClearMenu = FirstMenu.AddSubMenu("Laneclear", LaneMenuID);
            LaneClearMenu.AddGroupLabel("Laneclear Settings:");
            JungleClearMenu = FirstMenu.AddSubMenu("Jungleclear", JunglesMenuID);
            JungleClearMenu.AddGroupLabel("Jungleclear Settings:");
            LastHitMenu = FirstMenu.AddSubMenu("Lasthit", LastMenuID);
            LastHitMenu.AddGroupLabel("Lasthit Settings:");
            FleeMenu = FirstMenu.AddSubMenu("Flee", FleeMenuID);
            FleeMenu.AddGroupLabel("Flee Settings:");
            KillStealMenu = FirstMenu.AddSubMenu("Killsteal", KillstealMenuID);
            KillStealMenu.AddGroupLabel("Killsteal Settings:");
            DrawMenu = FirstMenu.AddSubMenu("Draw", DrawMenuID);
            DrawMenu.CreateCheckBox("Only draw spells if they are ready", DrawMenuID + "whenready");
            DrawMenu.AddGroupLabel("Damage Indicator settings: ");
            DrawMenu.CreateCheckBox("Draw damage indicator", DrawMenuID + "damageDraw");
            DrawMenu.CreateCheckBox("Draw statistics", DrawMenuID + "statDraw");
            DrawMenu.CreateCheckBox("Draw percentage", DrawMenuID + "perDraw");
            // ReSharper disable once ObjectCreationAsStatement
            new ColorSlider(DrawMenu, DrawMenuID + "damageHealthIndicator", Color.Yellow, "Damage Indicator Color");
            DrawMenu.AddSeparator();
            DrawMenu.CreateCheckBox("Calculate Q", DrawMenuID + "calculateQ");
            DrawMenu.CreateCheckBox("Calculate W", DrawMenuID + "calculateW");
            DrawMenu.CreateCheckBox("Calculate E", DrawMenuID + "calculateE");
            DrawMenu.CreateCheckBox("Calculate R", DrawMenuID + "calculateR");
            DrawMenu.AddGroupLabel("Draw Settings:");
            MiscMenu = FirstMenu.AddSubMenu("Misc", MiscMenuID);
            MiscMenu.AddGroupLabel("Miscellanous Settings:");

            InitializeSpells();
            InitializeEvents();
            InitializeMenu();

            if (PlayerHasMana)
            {
                HarassMenu.CreateSlider("Player mana must be less than [{0}%] to use harass spells", HarassMenuID + "mana", 40);
                AutoHarassMenu.CreateSlider("Player mana must be less than [{0}%] to use auto harass spells", AutoHarassMenuID + "mana", 40);
                LaneClearMenu.CreateSlider("Player mana must be less than [{0}%] to use lane clear spells", LaneClearMenu + "mana", 40);
                JungleClearMenu.CreateSlider("Player mana must be less than [{0}%] to use jungle clear spells", JunglesMenuID + "mana", 40);
                LastHitMenu.CreateSlider("Player mana must be less than [{0}%] to use lasthit spells", LastMenuID + "mana", 40);
                KillStealMenu.CreateSlider("Player mana must be less than [{0}%] to use Killsteal spells", KillstealMenuID + "mana", 10);
                MiscMenu.CreateSlider("Player mana must be less than [{0}%] to use misc spells", MiscMenuID + "mana", 30);
            }

            Game.OnTick += Game_OnTick;

            Drawing.OnDraw += Drawing_OnDraw;
        }

        public abstract void Combo();
        public abstract void Harass();
        public abstract void Laneclear();
        public abstract void Lasthit();
        public abstract void Jungleclear();
        public abstract void Flee();
        public abstract void PermaActive();

        #endregion Abstract

        public enum MenuIds
        {
            ComboUseQ = 1,
            ComboUseW = 2,
            ComboUseE = 3,
            ComboUseR = 4,
            HarassUseQ = 5,
            HarassUseW = 6,
            HarassUseE = 7,
            HarassUseR = 8,
            HarassMana = 9,
            AutoHarassUseQ = 10,
            AutoHarassUseW = 11,
            AutoHarassUseE = 12,
            AutoHarassUseR = 13,
            AutoHarassMana = 14,
            LaneclearUseQ = 15,
            LaneclearUseW = 16,
            LaneclearUseE = 17,
            LaneclearUseR = 18,
            LaneclearMana = 19,
            JungleclearUseQ = 20,
            JungleclearUseW = 21,
            JungleclearUseE = 22,
            JungleclearUseR = 23,
            JungleclearMana = 24,
            LasthitUseQ = 25,
            LasthitUseW = 26,
            LasthitUseE = 27,
            LasthitUseR = 28,
            LasthitMana = 29,
            FleeUseQ = 30,
            FleeUseW = 31,
            FleeUseE = 32,
            FleeUseR = 33,
            FleeMana = 34,
            KillstealUseQ = 35,
            KillstealUseW = 36,
            KillstealUseE = 37,
            KillstealUseR = 38,
            MiscUseInterrupt = 39,
            MiscUseGapcloser = 40,
            MiscMana = 41,
            ComboBoxMode = 42,
            DrawQ = 43,
            DrawW = 44,
            DrawE = 45,
            DrawR = 46,
        }

        #region ModesHandler

        private void Game_OnTick(EventArgs args)
        {
            PermaActive();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Laneclear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                Lasthit();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Jungleclear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
        }

        #endregion ModesHandler

        #region Draw
        public abstract void Draw();

        private void Drawing_OnDraw(EventArgs args)
        {
            Draw();
        }

        public void DrawSpell(Spell.SpellBase spell, Color spellColor)
        {
            if (spell != null)
            {
                var ready = DrawMenu.GetCheckBoxValue(DrawMenuID + "whenready");
                var spellSlot = spell.Slot.ToString()[spell.Slot.ToString().Length - 1];
                var check = DrawMenu.GetCheckBoxValue(DrawMenuID + spellSlot);
                var colorSharp = new SharpDX.Color(spellColor.R, spellColor.G, spellColor.B, spellColor.A);

                if (check && ready ? spell.IsReady() : check)
                {
                    Circle.Draw(colorSharp, spell.Range, Player.Instance);
                }
            }
        }
        #endregion Draw

        #region GettingTargetHelpers
        /// <summary>
        /// It auto gets a target using EB`s target selector
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public AIHeroClient GetTarget(Spell.SpellBase spell)
        {
            return TargetSelector.GetTarget(spell.Range, DamageType);
        }

        /// <summary>
        /// It auto gets a target using EB`s target selector
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public AIHeroClient GetTarget(int range)
        {
            return TargetSelector.GetTarget(range, DamageType);
        }
        #endregion GetTarget helpers

        #region Lazyness
        public void AddMultipleCheckBox(Spell.SpellBase spell, List<MenuCheckBox> menus)
        {
            var spellSlot = spell.Slot.ToString()[spell.Slot.ToString().Length - 1];

            foreach (var m in menus)
            {
                var lenghtChampName = Player.Instance.ChampionName.Length;
                switch (m.Menuu.UniqueMenuId.Remove(0, 6 + lenghtChampName))
                {
                    case ComboMenuID:
                        ComboMenu.CreateCheckBox("- Use " + spellSlot, ComboMenuID + spellSlot, m.DefaultValue);
                        break;
                    case HarassMenuID:
                        HarassMenu.CreateCheckBox("- Use " + spellSlot, HarassMenuID + spellSlot, m.DefaultValue);
                        break;
                    case AutoHarassMenuID:
                        AutoHarassMenu.CreateCheckBox("- Use " + spellSlot, AutoHarassMenuID + spellSlot, m.DefaultValue);
                        break;
                    case LaneMenuID:
                        LaneClearMenu.CreateCheckBox("- Use " + spellSlot, LaneMenuID + spellSlot, m.DefaultValue);
                        break;
                    case LastMenuID:
                        LastHitMenu.CreateCheckBox("- Use " + spellSlot, LastMenuID + spellSlot, m.DefaultValue);
                        break;
                    case FleeMenuID:
                        FleeMenu.CreateCheckBox("- Use " + spellSlot, FleeMenuID + spellSlot, m.DefaultValue);
                        break;
                    case KillstealMenuID:
                        KillStealMenu.CreateCheckBox("- Use " + spellSlot, KillstealMenuID + spellSlot, m.DefaultValue);
                        break;
                    case DrawMenuID:
                        DrawMenu.CreateCheckBox("- Draw " + spellSlot, DrawMenuID + spellSlot, m.DefaultValue);
                        // ReSharper disable once ObjectCreationAsStatement
                        new ColorSlider(DrawMenu, DrawMenuID + spellSlot + "colorid", Color.White, "- " + spellSlot + " Color");
                        break;
                }
            }
        }

        public class MenuCheckBox
        {
            public Menu Menuu;
            public bool DefaultValue;

            public MenuCheckBox(Menu menu, bool defaultValue = true)
            {
                Menuu = menu;
                DefaultValue = defaultValue;
            }
        }
        #endregion Lazyness

        /// <summary>
        /// Support mode
        /// </summary>
        /// <param name="turnOn">Turn on or off support mode</param>
        public void SupportMode(bool turnOn)
        {
            if (turnOn)
            {
                var menu = FirstMenu.AddSubMenu("• SupportMode", "id" + GetAttribute().Name.ToLower() + "support");
                menu.CreateCheckBox("Disable auto attack on minions", "SupportModeAADisable");
                Game.OnTick += SuppTick;
            }
            else
            {
                FirstMenu.Remove("id" + GetAttribute().Name.ToLower() + "support");
                Game.OnTick -= SuppTick;
                Orbwalker.ForcedTarget = null;
            }
        }


        private void SuppTick(EventArgs args)
        {
            var menu = MainMenu.GetMenu("id" + GetAttribute().Name.ToLower() + "support");
            Orbwalker.ForcedTarget = menu.GetCheckBoxValue("SupportModeAADisable")
                ? TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange(), DamageType.Mixed)
                : null;
        }

        #region DamageIndicator

        //Offsets
        private const float YOff = 9.8f;
        private const float XOff = 0;
        private const float Width = 107;
        private const float Thick = 9.82f;
        //Offsets
        private static Font _Font, _Font2;
        private static Color color = Color.Yellow;

        public void InitDamageIndicator()
        {
            Drawing.OnEndScene += Drawing_OnEndScene;

            _Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                    Height = 16,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType,
                });

            _Font2 = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Segoi UI",
                    Height = 11,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType,
                });
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(e => e.IsValid && e.IsHPBarRendered && e.TotalShieldHealth() > 10)
                )
            {
                var damage = GetTotalDamage(enemy);

                if (DrawMenu.GetCheckBoxValue(DrawMenuID + "damageDraw"))
                {
                    //Drawing Line Over Enemies Helth bar
                    var dmgPer = (enemy.TotalShieldHealth() - damage > 0 ? enemy.TotalShieldHealth() - damage : 0) /
                                 enemy.TotalShieldMaxHealth();
                    var currentHPPer = enemy.TotalShieldHealth() / enemy.TotalShieldMaxHealth();
                    var initPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + dmgPer * Width),
                        (int)enemy.HPBarPosition.Y + YOff);
                    var endPoint = new Vector2((int)(enemy.HPBarPosition.X + XOff + currentHPPer * Width) + 1,
                        (int)enemy.HPBarPosition.Y + YOff);

                    var colour = Color.FromArgb(180, color);
                    EloBuddy.SDK.Rendering.Line.DrawLine(colour, Thick, initPoint, endPoint);
                }

                if (DrawMenu.GetCheckBoxValue(DrawMenuID + "statDraw"))
                {
                    //Statistics
                    var posXStat = (int)enemy.HPBarPosition[0];
                    var posYStat = (int)enemy.HPBarPosition[1] - 7;
                    var mathStat = "-" + Math.Round(damage) + " / " +
                                   Math.Round(enemy.Health - damage);
                    _Font2.DrawText(null, mathStat, posXStat, posYStat, SharpDX.Color.Yellow);
                }

                if (DrawMenu.GetCheckBoxValue(DrawMenuID + "perDraw"))
                {
                    //Percent
                    var posXPer = (int)enemy.HPBarPosition[0] + 106;
                    var posYPer = (int)enemy.HPBarPosition[1] - 12;
                    _Font.DrawText(null, string.Concat(Math.Ceiling((int)damage / enemy.TotalShieldHealth() * 100), "%"),
                        posXPer, posYPer, SharpDX.Color.Yellow);
                }
            }
        }

        private float GetTotalDamage(Obj_AI_Base target)
        {
            var spells =
                Player.Spells.Where(
                    spell =>
                        (spell.Slot == SpellSlot.Q && DrawMenu.GetCheckBoxValue(DrawMenuID + "calculateQ")) ||
                          (spell.Slot == SpellSlot.W && DrawMenu.GetCheckBoxValue(DrawMenuID + "calculateW")) ||
                           (spell.Slot == SpellSlot.E && DrawMenu.GetCheckBoxValue(DrawMenuID + "calculateE")) ||
                            (spell.Slot == SpellSlot.R && DrawMenu.GetCheckBoxValue(DrawMenuID + "calculateR")));

            var damage = spells.Where(spe => spe.IsReady).Sum(s => Player.Instance.GetSpellDamage(target, s.Slot));

            return (damage + Player.Instance.GetAutoAttackDamage(target)) - 10;
        }
    }

    #endregion DamageIndicator
}