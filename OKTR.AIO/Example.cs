using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using OKTR.Plugin;

namespace OKTR.AIO
{
    internal class Example : ChampionPluginBase
    {
        /// <summary>
        /// Initializes the spells with the settings you set
        /// </summary>
        public override void InitializeSpells()
        {
            //Active spells requires only the range(if the spell has range if it doesnt just leave it blank). Active spells are for example : Kat`s W, Liss`s W, Rengar`s Q
            Q = new Spell.Active(SpellSlot.Q, 999);
            //Skillshots requires Range, Type,Delay,Speed,Width
            //Types: There are 3 types of skillshots Cone(Annie`s W),Linear(Ezreal`s Q),Circular(Lux`s E)
            //The information about the spells can be found on Wikia or RiotDecode
            W = new Spell.Skillshot(SpellSlot.W, 999, SkillShotType.Linear, 999, 5555, 11)
            {
                //In here we can put some addtional information about the spell like (Ex: AllowedCollisions, MinimunHitchance)
                //So below we set to only cast if the chance od the hit is high
                MinimumHitChance = HitChance.High,
                //Dont forget to put a , between the parameters
                //Below we set the allowed collision count for the spell in this example i put the maxvalue so it will ignore every collision object
                AllowedCollisionCount = int.MaxValue
            };
            //Targetted spells requires range some examples of targetted spells : Annie`s Q, Katarina`s Q, Ryze`s W
            E = new Spell.Targeted(SpellSlot.E, 999);
            //Chargeable spells requires MinimunRange,MaximunRange,Time to get fully charged, Delay,Speed,Width. Some spells examples of chargeables are Varus`s Q, Xerath`s Q, Vi`s Q
            //The information about the spells can be found on Wikia or RiotDecode
            R = new Spell.Chargeable(SpellSlot.R, 100, 1000, 30000, 250, 999, 55);

            //What damage does the champion cause ? Mixed(AD & AP), Physical(Only AD), Magical(Only AP), True(Only True Damage)
            DamageType = DamageType.Mixed;
        }

        /// <summary>
        /// Initializes events Ex: OnTick,PostAttack
        /// </summary>
        public override void InitializeEvents()
        {
        }

        /// <summary>
        /// Initializes the menu
        /// </summary>
        public override void InitializeMenu()
        {
            FirstMenuName = "Name of the addon";
            //There are a bunch of examples on how to create checkboxes and sliders really easy
            //Obs: You can use the MenuIds(Preset) or Ints as a uniqueID
            //ComboMenu
            ComboMenu.AddGroupLabel("-:Combo Spells:-");
            ComboMenu.CreateCheckBox("- Use Q", MenuIds.ComboUseQ);
            ComboMenu.CreateCheckBox("- Use W", MenuIds.ComboUseW);
            ComboMenu.CreateCheckBox("- Use E", MenuIds.ComboUseE);
            ComboMenu.CreateCheckBox("- Use R", 8);
            //HarassMenu
            HarassMenu.AddGroupLabel("-:Harass Spells:-");
            HarassMenu.CreateCheckBox("- Use Q", MenuIds.HarassUseQ);
            HarassMenu.CreateCheckBox("- Use W", MenuIds.HarassUseW);
            HarassMenu.CreateCheckBox("- Use E", MenuIds.HarassUseE);
            HarassMenu.CreateCheckBox("- Use R", MenuIds.HarassUseR);
            HarassMenu.AddGroupLabel("-:Harass Settings:-");
            HarassMenu.CreateSlider("It will use harass spells only if mana is greater than ({0}%)", MenuIds.HarassMana);
            HarassMenu.AddGroupLabel("-:AutoHarass Spells:-");
            HarassMenu.CreateCheckBox("- Use Q", MenuIds.AutoHarassUseQ);
            HarassMenu.CreateCheckBox("- Use W", MenuIds.AutoHarassUseW);
            HarassMenu.CreateCheckBox("- Use E", MenuIds.AutoHarassUseE);
            HarassMenu.CreateCheckBox("- Use R", MenuIds.AutoHarassUseR);
            HarassMenu.AddGroupLabel("-:AutoHarass Settings:-");
            HarassMenu.CreateSlider("It will use harass spells only if mana is greater than ({0}%)", MenuIds.AutoHarassMana);
            //LaneClearMenu
            LaneClearMenu.AddGroupLabel("-:Laneclear Spells:-");
            LaneClearMenu.CreateCheckBox("- Use Q", 9);
            LaneClearMenu.CreateCheckBox("- Use W", 10);
            LaneClearMenu.CreateCheckBox("- Use E", 11);
            LaneClearMenu.CreateCheckBox("- Use R", 12);
            LaneClearMenu.AddGroupLabel("-:Laneclear Settings:-");
            LaneClearMenu.CreateSlider("It will use laneclear spells only if mana is greater than ({0}%)", 30);
            //LastHitMenu
            LastHitMenu.AddGroupLabel("-:Lasthit Spells:-");
            LastHitMenu.CreateCheckBox("- Use Q", 13);
            LastHitMenu.CreateCheckBox("- Use W", 14);
            LastHitMenu.CreateCheckBox("- Use E", 15);
            LastHitMenu.CreateCheckBox("- Use R", 16);
            LastHitMenu.AddGroupLabel("-:Lasthit Settings:-");
            LastHitMenu.CreateSlider("It will use lasthit spells only if mana is greater than ({0}%)", 30);
            //FleeMenu
            FleeMenu.AddGroupLabel("-:Flee Spells:-");
            FleeMenu.CreateCheckBox("- Use Q", MenuIds.FleeUseQ);
            FleeMenu.CreateCheckBox("- Use W", MenuIds.FleeUseW);
            FleeMenu.CreateCheckBox("- Use E", MenuIds.FleeUseE);
            FleeMenu.CreateCheckBox("- Use R", MenuIds.FleeUseR);
            FleeMenu.AddGroupLabel("-:Flee Settings:-");
            FleeMenu.CreateSlider("It will use flee spells only if mana is greater than ({0}%)", 30);
            //MiscMenu
            MiscMenu.AddGroupLabel("-:Misc Spells:-");
            MiscMenu.CreateCheckBox("- Use X on gapclosers spells", MenuIds.MiscUseGapcloser);
            MiscMenu.CreateCheckBox("- Use X on interruptables spells", MenuIds.MiscUseInterrupt);
            MiscMenu.AddGroupLabel("-:Misc Settings:-");
            MiscMenu.CreateSlider("It will use misc spells only if mana is greater than ({0}%)", 30);
        }

        /// <summary>
        /// Executes Combo when Orbwalker`s Combo key is pressed
        /// </summary>
        public override void Combo()
        {
            //The spell that has the highest range should be here.
            var target = GetTarget(Q);

            //It Checks if the menu item(in this case checkbox) is checked and then it will chack if can cast the spell
            //if both conditions are met Cast X Spell
            if (ComboMenu.GetCheckBoxValue(MenuIds.ComboUseQ) && target.CanCastSpell(Q))
            {
                Q.Cast(target);
            }

            if (ComboMenu.GetCheckBoxValue(MenuIds.ComboUseW) && target.CanCastSpell(W))
            {
                W.Cast(target);
            }

            if (ComboMenu.GetCheckBoxValue(MenuIds.ComboUseE) && target.CanCastSpell(E))
            {
                E.Cast(target);
            }

            if (ComboMenu.GetCheckBoxValue(MenuIds.ComboUseR) && target.CanCastSpell(R))
            {
                R.Cast(target);
            }
        }

        /// <summary>
        /// Executes Harass when Orbwalker`s Harass key is pressed
        /// </summary>
        public override void Harass()
        {
            //The spell that has the highest range should be here.
            var target = GetTarget(W);

            //First it gets the value of the slider on the designed menu and check if it`s less or equal Player Mana Percent
            if (HarassMenu.GetSliderValue(MenuIds.HarassMana) <= Player.Instance.ManaPercent)
            {
                //It Checks if the menu item(in this case checkbox) is checked and then it will chack if can cast the spell
                //if both conditions are met Cast X Spell
                if (ComboMenu.GetCheckBoxValue(MenuIds.HarassUseQ) && target.CanCastSpell(Q))
                {
                    Q.Cast(target);
                }

                if (ComboMenu.GetCheckBoxValue(MenuIds.HarassUseW) && target.CanCastSpell(W))
                {
                    W.Cast(target);
                }

                if (ComboMenu.GetCheckBoxValue(MenuIds.HarassUseE) && target.CanCastSpell(E))
                {
                    E.Cast(target);
                }

                if (ComboMenu.GetCheckBoxValue(MenuIds.HarassUseR) && target.CanCastSpell(R))
                {
                    R.Cast(target);
                }
            }
        }

        /// <summary>
        /// Executes Laneclear when Orbwalker`s Laneclear key is pressed
        /// </summary>
        public override void Laneclear()
        {
            Chat.Print("Lane");
        }

        /// <summary>
        /// Executes Lasthit when Orbwalker`s Lasthit key is pressed
        /// </summary>
        public override void Lasthit()
        {
            Chat.Print("Last");
        }

        /// <summary>
        /// Executes JungleClear when Orbwalker`s JungleClear key is pressed
        /// </summary>
        public override void Jungleclear()
        {
            Chat.Print("Jungle");
        }

        /// <summary>
        /// Executes Flee when Orbwalker`s Flee key is pressed
        /// </summary>
        public override void Flee()
        {
            Chat.Print("Flee");
        }

        /// <summary>
        /// Always running without any conditions
        /// </summary>
        public override void PermaActive()
        {
            //Perma active is always running so it`s here where wi will put the auto harass

            //The spell that has the highest range should be here.
            var target = GetTarget(W);

            //First it gets the value of the slider on the designed menu and check if it`s less or equal Player Mana Percent
            if (HarassMenu.GetSliderValue(MenuIds.HarassMana) <= Player.Instance.ManaPercent)
            {
                //It Checks if the menu item(in this case checkbox) is checked and then it will chack if can cast the spell
                //if both conditions are met Cast X Spell
                if (ComboMenu.GetCheckBoxValue(MenuIds.AutoHarassUseQ) && target.CanCastSpell(Q))
                {
                    Q.Cast(target);
                }

                if (ComboMenu.GetCheckBoxValue(MenuIds.AutoHarassUseW) && target.CanCastSpell(W))
                {
                    W.Cast(target);
                }

                if (ComboMenu.GetCheckBoxValue(MenuIds.AutoHarassUseE) && target.CanCastSpell(E))
                {
                    E.Cast(target);
                }

                if (ComboMenu.GetCheckBoxValue(MenuIds.AutoHarassUseR) && target.CanCastSpell(R))
                {
                    R.Cast(target);
                }
            }
        }
    }
}
