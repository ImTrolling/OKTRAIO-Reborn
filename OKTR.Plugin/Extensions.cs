using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using static OKTR.Plugin.OKTR_Core;

namespace OKTR.Plugin
{
    /// <summary>
    /// Spell Extensions
    /// </summary>
    public static partial class Extensions
    {
        #region IsNotNull
        /// <summary>
        /// Check if the object is not null
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsNotNull(this GameObject target)
        {
            return target != null;
        }
        #endregion 

        #region CanCast

        /// <summary>
        /// Check if can cast the spell on the target
        /// </summary>
        /// <param name="target">Target to check</param>
        /// <param name="spell">Any spell</param>
        /// <returns></returns>
        public static bool CanCast(this Obj_AI_Base target, Spell.SpellBase spell)
        {
            if (spell != null)
                return spell.IsReady() && target.IsValidTarget(spell.Range) && target.IsNotNull();
            return false;
        }
        /// <summary>
        /// Check if can cast the spell on the target
        /// </summary>
        /// <param name="target">Target to check</param>
        /// <param name="spell">Any spell</param>
        /// <returns></returns>
        public static bool CanCast(this Obj_AI_Base target, Spell.Ranged spell)
        {
            if (spell != null)
                return spell.IsReady() && target.IsValidTarget(spell.Range) && target.IsNotNull();
            return false;
        }

        /// <summary>
        /// Check if can cast the spell on the target
        /// </summary>
        /// <param name="target">Target to check</param>
        /// <param name="spell">Any spell</param>
        /// <returns></returns>
        public static bool CanCast(this Obj_AI_Base target, Spell.Active spell)
        {
            return CanCast(target, spell as Spell.SpellBase);
        }

        /// <summary>
        /// Check if can cast the spell on the target
        /// </summary>
        /// <param name="target">Target to check</param>
        /// <param name="spell">Any spell</param>
        /// <returns></returns>
        public static bool CanCast(this Obj_AI_Base target, Spell.Skillshot spell)
        {
            return CanCast(target, spell as Spell.Ranged);
        }

        /// <summary>
        /// Check if can cast the spell on the target
        /// </summary>
        /// <param name="target">Target to check</param>
        /// <param name="spell">Any spell</param>
        /// <returns></returns>
        public static bool CanCast(this Obj_AI_Base target, Spell.Chargeable spell)
        {
            return CanCast(target, spell as Spell.Ranged);
        }

        /// <summary>
        /// Check if can cast the spell on the target
        /// </summary>
        /// <param name="target">Target to check</param>
        /// <param name="spell">Any spell</param>
        /// <returns></returns>
        public static bool CanCast(this Obj_AI_Base target, Spell.Targeted spell)
        {
            return CanCast(target, spell as Spell.SpellBase);
        }

        #endregion

        #region TryCast

        /// <summary>
        /// It will only cast the spell if it can
        /// </summary>
        /// <param name="spell">Active Spell</param>
        /// <param name="target">Target to cast the spell</param>
        public static bool TryCast(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            if (spell != null)
            {
                var spellSlot = spell.Slot.ToString()[spell.Slot.ToString().Length - 1];
                if (target.CanCast(spell) &&
                    !OKTRSpellTargetSelectorMenu.GetCheckBoxValue(OKTRSpellTargetSelectorMenuID + spellSlot + target.BaseSkinName))
                {
                    return spell.Cast(target);
                }
            }   
            return false;
        }

        /// <summary>
        /// It will only cast the spell if it can
        /// </summary>
        /// <param name="spell">Active Spell</param>
        /// <param name="target">Target to cast the spell</param>
        public static bool TryCast(this Spell.Active spell, Obj_AI_Base target)
        {
            return TryCast(spell as Spell.SpellBase, target);
        }

        /// <summary>
        /// It will only cast the spell if it can
        /// </summary>
        /// <param name="spell">Active Spell</param>
        /// <param name="target">Target to cast the spell</param>
        public static bool TryCast(this Spell.Skillshot spell, Obj_AI_Base target)
        {
            return TryCast(spell as Spell.Ranged, target);
        }

        /// <summary>
        /// It will only cast the spell if it can
        /// </summary>
        /// <param name="spell">Active Spell</param>
        /// <param name="target">Target to cast the spell</param>
        public static bool TryCast(this Spell.Targeted spell, Obj_AI_Base target)
        {
            return TryCast(spell as Spell.SpellBase, target);
        }

        /// <summary>
        /// It will only cast the spell if it can
        /// </summary>
        /// <param name="spell">Active Spell</param>
        /// <param name="target">Target to cast the spell</param>
        public static bool TryCast(this Spell.Chargeable spell, Obj_AI_Base target)
        {
            return TryCast(spell as Spell.Ranged, target);
        }

        /// <summary>
        /// It will only cast the spell if it can
        /// </summary>
        /// <param name="spell">Active Spell</param>
        /// <param name="target">Target to cast the spell</param>
        public static bool TryCast(this Spell.Ranged spell, Obj_AI_Base target)
        {
            if (spell != null)
                if (target.CanCast(spell))
                    return spell.Cast(target);
            return false;
        }

        #endregion

        #region KillSteal

        /// <summary>
        /// It will cast the spell to steal a kill
        /// </summary>
        /// <param name="spell">Targeted spell</param>
        public static bool StealTargeted(this Spell.Targeted spell)
        {
            var target = GetKSTarget(spell);
            return target.CanCast(spell) && spell.Cast(target);
        }

        /// <summary>
        /// It will cast the spell to steal a kill
        /// </summary>
        /// <param name="spell">Skillshot spell</param>
        /// <param name="minHitChance">Minimun hitchance</param>
        public static void StealSkillShot(this Spell.Skillshot spell, HitChance minHitChance = HitChance.Medium)
        {
            var target = GetKSTarget(spell);
            var pred = spell.GetPrediction(target);
            if (spell.IsReady() && target.IsValidTarget(spell.Range) && target.IsNotNull() &&
                pred.HitChance >= minHitChance)
            {
                spell.Cast(pred.CastPosition);
            }
        }

        private static AIHeroClient GetKSTarget(Spell.SpellBase spell)
        {
            return
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    x =>
                        x.IsValidTarget(spell.Range) &&
                        Prediction.Health.GetPrediction(x, spell.CastDelay + Game.Ping) <=
                        Player.Instance.GetSpellDamage(x, spell.Slot)
                        && x.IsVisible
                        && !x.IsZombie
                        && !x.HasBuff("JudicatorIntervention") //kayle R
                        && !x.HasBuff("zhonyasringshield") //zhonya
                        && !x.HasBuff("VladimirSanguinePool") //vladimir W
                        && !x.HasBuff("ChronoShift") //zilean R
                        && !x.HasBuff("yorickrazombie") //yorick R
                        && !x.HasBuff("mordekaisercotgself") //mordekaiser R
                        && !x.HasBuff("UndyingRage") //tryndamere R
                        && !x.HasBuff("sionpassivezombie") //sion Passive
                        && !x.HasBuff("KarthusDeathDefiedBuff") //karthus passive
                        && !x.HasBuff("kogmawicathiansurprise") //kog'maw passive
                        && !x.HasBuff("zyrapqueenofthorns")); //zyra passive
        }

        #endregion
    }

    /// <summary>
    /// Menu Extensions
    /// </summary>
    public static partial class Extensions
    {

        #region Slider

        /// <summary>
        /// Creates a slider
        /// </summary>
        /// <param name="m">Menu</param>
        /// <param name="sliderName">The name of the slider</param>
        /// <param name="uniqueId">Unique id</param>
        /// <param name="defaulValueslider">The default value of the slider</param>
        /// <param name="minValue">The minimum value of the slider</param>
        /// <param name="maxValue">The maximum value of the slider</param>
        /// <returns></returns>
        public static Slider CreateSlider(this Menu m, string sliderName, int uniqueId, int defaulValueslider = 0,
            int minValue = 0, int maxValue = 100)
        {
            return m.Add("idInt" + uniqueId, new Slider(sliderName));
        }

        /// <summary>
        /// Creates a slider
        /// </summary>
        /// <param name="m">Menu</param>
        /// <param name="sliderName">The name of the slider</param>
        /// <param name="uniqueId">Unique id</param>
        /// <param name="defaulValueslider">The default value of the slider</param>
        /// <param name="minValue">The minimum value of the slider</param>
        /// <param name="maxValue">The maximum value of the slider</param>
        /// <returns></returns>
        public static Slider CreateSlider(this Menu m, string sliderName,
            ChampionPluginBase<Spell.SpellBase, Spell.SpellBase, Spell.SpellBase, Spell.SpellBase>.MenuIds uniqueId,
            int defaulValueslider = 0,
            int minValue = 0, int maxValue = 100)
        {
            return CreateSlider(m, sliderName, "idEnum" + uniqueId, defaulValueslider, minValue, maxValue);
        }

        /// <summary>
        /// Creates a slider
        /// </summary>
        /// <param name="m">Menu</param>
        /// <param name="sliderName">The name of the slider</param>
        /// <param name="uniqueId">Unique id</param>
        /// <param name="defaulValueslider">The default value of the slider</param>
        /// <param name="minValue">The minimum value of the slider</param>
        /// <param name="maxValue">The maximum value of the slider</param>
        /// <returns></returns>
        public static Slider CreateSlider(this Menu m, string sliderName, string uniqueId, int defaulValueslider = 0,
            int minValue = 0, int maxValue = 100)
        {
            return m.Add(uniqueId, new Slider(sliderName, defaulValueslider, minValue, maxValue));
        }

        /// <summary>
        /// Get the value of a slider
        /// </summary>
        /// <param name="menu">The menu that you want to get the value from</param>
        /// <param name="uniqueId">The unique id of the slider</param>
        /// <returns>The value of the slider</returns>
        public static int GetSliderValue(this Menu menu, string uniqueId)
        {
            try
            {
                return menu.Get<Slider>(uniqueId).CurrentValue;
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Error getting the slider value with the ID = ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(uniqueId);
                Console.ResetColor();
                Console.WriteLine();
            }
            return -1;
        }

        /// <summary>
        /// Get the value of a slider
        /// </summary>
        /// <param name="menu">The menu that you want to get the value from</param>
        /// <param name="uniqueId">The unique id of the slider</param>
        /// <returns>The value of the slider</returns>
        public static int GetSliderValue(this Menu menu, int uniqueId)
        {
            return GetSliderValue(menu, "idInt" + uniqueId);
        }

        /// <summary>
        /// Get the value of a slider
        /// </summary>
        /// <param name="menu">The menu that you want to get the value from</param>
        /// <param name="uniqueId">The unique id of the slider</param>
        /// <returns>The value of the slider</returns>
        public static int GetSliderValue<TQ, TW, TE, TR>(this Menu menu,
            ChampionPluginBase<TQ, TW, TE, TR>.MenuIds uniqueId)
        {
            return GetSliderValue(menu, "idEnum" + uniqueId);
        }

        #endregion

        #region KeyBind

        /// <summary>
        /// It creates a keybind on the menu
        /// </summary>
        /// <param name="m"> The menu that you want it to be created on</param>
        /// <param name="keyName">The title of the keybind</param>
        /// <param name="uniqueId">The unique id of the keybind</param>
        /// <param name="valueKey">What key to be used ex: 'Z','A','C','X'</param>
        /// <param name="defaultValue">The default value of the keybind On or Off</param>
        /// <param name="keyType">The type of the keybind</param>
        /// <returns></returns>
        public static KeyBind CreateKeybind(this Menu m, string keyName, int uniqueId, uint valueKey = 32,
            bool defaultValue = false, KeyBind.BindTypes keyType = KeyBind.BindTypes.PressToggle)
        {
            return m.Add("idInt" + uniqueId, new KeyBind(keyName, defaultValue, keyType, valueKey));
        }

        /// <summary>
        /// It creates a keybind on the menu
        /// </summary>
        /// <param name="m"> The menu that you want it to be created on</param>
        /// <param name="keyName">The title of the keybind</param>
        /// <param name="uniqueId">The unique id of the keybind</param>
        /// <param name="valueKey">What key to be used ex: 'Z','A','C','X'</param>
        /// <param name="defaultValue">The default value of the keybind On or Off</param>
        /// <param name="keyType">The type of the keybind</param>
        /// <returns></returns>
        public static KeyBind CreateKeybind<TQ, TW, TE, TR>(this Menu m, string keyName,
            ChampionPluginBase<TQ, TW, TE, TR>.MenuIds uniqueId,
            uint valueKey = 32,
            bool defaultValue = false, KeyBind.BindTypes keyType = KeyBind.BindTypes.PressToggle)
        {
            return m.Add("idEnum" + uniqueId, new KeyBind(keyName, defaultValue, keyType, valueKey));
        }

        /// <summary>
        /// It creates a keybind on the menu
        /// </summary>
        /// <param name="m"> The menu that you want it to be created on</param>
        /// <param name="keyName">The title of the keybind</param>
        /// <param name="uniqueId">The unique id of the keybind</param>
        /// <param name="valueKey">What key to be used ex: 'Z','A','C','X'</param>
        /// <param name="defaultValue">The default value of the keybind On or Off</param>
        /// <param name="keyType">The type of the keybind</param>
        /// <returns></returns>
        public static KeyBind CreateKeybind(this Menu m, string keyName, string uniqueId, uint valueKey = 32,
            bool defaultValue = false, KeyBind.BindTypes keyType = KeyBind.BindTypes.PressToggle)
        {
            return m.Add(uniqueId, new KeyBind(keyName, defaultValue, keyType, valueKey));
        }

        public static bool GetKeybindValue(this Menu menu, string uniqueId)
        {
            try
            {
                return menu.Get<KeyBind>(uniqueId).CurrentValue;
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Error getting the keybind value with the ID = ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(uniqueId);
                Console.ResetColor();
                Console.WriteLine();
            }
            return false;
        }

        public static bool GetKeybindValue(this Menu menu, int uniqueId)
        {
            return GetKeybindValue(menu, uniqueId.ToString());
        }

        public static bool GetKeybindValue<TQ, TW, TE, TR>(this Menu menu,
            ChampionPluginBase<TQ, TW, TE, TR>.MenuIds uniqueId)
        {
            return GetKeybindValue(menu, (int)uniqueId);
        }

        #endregion

        #region CreateKeys

        /// <summary>
        ///     Creates Q-W-E-R Keys for Normal Usage . 
        /// </summary>
        /// <param name="m">Menu</param>
        /// <param name="mode">The mode accept those strings: combo, harass, autoharass, laneclear, jungleclear, lasthit, flee, killsteal</param>
        /// <param name="useQ">Should it use Q? remember that the  option will be true if not changed</param>
        /// <param name="statusQ">
        ///     Should it be false instead of true? change it here & remember that the status will be true if not
        ///     changed
        /// </param>
        /// <param name="useW">Should it use W? remember that the option will be true if not changed</param>
        /// <param name="statusW">
        ///     Should it be false instead of true? change it here & remember that the status will be true if not
        ///     changed
        /// </param>
        /// <param name="useE">Should it use E? remember that the option will be true if not changed</param>
        /// <param name="statusE">
        ///     Should it be false instead of true? change it here & remember that the status will be true if not
        ///     changed
        /// </param>
        /// <param name="useR">Should it use R? remember that the option will be true if not changed</param>
        /// <param name="statusR">
        ///     Should it be false instead of true? change it here & remember that the status will be true if not
        ///     changed
        /// </param>
        public static void CreateKeys(this Menu m, string mode, bool useQ = true, bool statusQ = true, bool useW = true,
            bool statusW = true, bool useE = true, bool statusE = true,
            bool useR = true, bool statusR = true)
        {

            switch (mode)
            {
                case "combo":
                    ComboKeys(m, useQ, statusQ, useW, statusW, useE, statusE, useR, statusR);
                    break;
                case "harass":
                    HarassKeys(m, useQ, statusQ, useW, statusW, useE, statusE, useR, statusR);
                    break;
                case "autoharass":
                    AutoHarassKeys(m, useQ, statusQ, useW, statusW, useE, statusE, useR, statusR);
                    break;
                case "laneclear":
                    LaneclearKeys(m, useQ, statusQ, useW, statusW, useE, statusE, useR, statusR);
                    break;
                case "jungleclear":
                    JungleclearKeys(m, useQ, statusQ, useW, statusW, useE, statusE, useR, statusR);
                    break;
                case "lasthit":
                    LasthitKeys(m, useQ, statusQ, useW, statusW, useE, statusE, useR, statusR);
                    break;
                case "flee":
                    FleeKeys(m, useQ, statusQ, useW, statusW, useE, statusE, useR, statusR);
                    break;
                case "killsteal":
                    KillstealKeys(m, useQ, statusQ, useW, statusW, useE, statusE, useR, statusR);
                    break;
            }
        }
        
        private static void ComboKeys(this Menu m, bool useQ, bool statusQ, bool useW, bool statusW, bool useE,
            bool statusE, bool useR, bool statusR)
        {
            if (useQ) m.CreateCheckBox("Use Q", 1, statusQ);
            if (useW) m.CreateCheckBox("Use W", 2, statusW);
            if (useE) m.CreateCheckBox("Use E", 3, statusE);
            if (useR) m.CreateCheckBox("Use R", 4, statusR);
        }

        private static void HarassKeys(this Menu m, bool useQ, bool statusQ, bool useW, bool statusW, bool useE,
            bool statusE, bool useR, bool statusR)
        {
            if (useQ) m.CreateCheckBox("Use Q", 5, statusQ);
            if (useW) m.CreateCheckBox("Use W", 6, statusW);
            if (useE) m.CreateCheckBox("Use E", 7, statusE);
            if (useR) m.CreateCheckBox("Use R", 8, statusR);
        }

        private static void AutoHarassKeys(this Menu m, bool useQ, bool statusQ, bool useW, bool statusW, bool useE,
            bool statusE, bool useR, bool statusR)
        {
            if (useQ) m.CreateCheckBox("Use Q", 10, statusQ);
            if (useW) m.CreateCheckBox("Use W", 11, statusW);
            if (useE) m.CreateCheckBox("Use E", 12, statusE);
            if (useR) m.CreateCheckBox("Use R", 13, statusR);
        }

        private static void LaneclearKeys(this Menu m, bool useQ, bool statusQ, bool useW, bool statusW, bool useE,
            bool statusE, bool useR, bool statusR)
        {
            if (useQ) m.CreateCheckBox("Use Q", 15, statusQ);
            if (useW) m.CreateCheckBox("Use W", 16, statusW);
            if (useE) m.CreateCheckBox("Use E", 17, statusE);
            if (useR) m.CreateCheckBox("Use R", 18, statusR);
        }

        private static void JungleclearKeys(this Menu m, bool useQ, bool statusQ, bool useW, bool statusW, bool useE,
            bool statusE, bool useR, bool statusR)
        {
            if (useQ) m.CreateCheckBox("Use Q", 20, statusQ);
            if (useW) m.CreateCheckBox("Use W", 21, statusW);
            if (useE) m.CreateCheckBox("Use E", 22, statusE);
            if (useR) m.CreateCheckBox("Use R", 23, statusR);
        }

        private static void LasthitKeys(this Menu m, bool useQ, bool statusQ, bool useW, bool statusW, bool useE,
            bool statusE, bool useR, bool statusR)
        {
            if (useQ) m.CreateCheckBox("Use Q", 25, statusQ);
            if (useW) m.CreateCheckBox("Use W", 26, statusW);
            if (useE) m.CreateCheckBox("Use E", 27, statusE);
            if (useR) m.CreateCheckBox("Use R", 28, statusR);
        }

        private static void FleeKeys(this Menu m, bool useQ, bool statusQ, bool useW, bool statusW, bool useE,
            bool statusE, bool useR, bool statusR)
        {
            if (useQ) m.CreateCheckBox("Use Q", 30, statusQ);
            if (useW) m.CreateCheckBox("Use W", 31, statusW);
            if (useE) m.CreateCheckBox("Use E", 32, statusE);
            if (useR) m.CreateCheckBox("Use R", 33, statusR);
        }

        private static void KillstealKeys(this Menu m, bool useQ, bool statusQ, bool useW, bool statusW, bool useE,
            bool statusE, bool useR, bool statusR)
        {
            if (useQ) m.CreateCheckBox("Use Q", 35, statusQ);
            if (useW) m.CreateCheckBox("Use W", 36, statusW);
            if (useE) m.CreateCheckBox("Use E", 37, statusE);
            if (useR) m.CreateCheckBox("Use R", 38, statusR);
        }

        #endregion

        #region CreateManaSlider

        /// <summary>
        /// Creates a Mana Slider for that mode respecting your starting value, min value and max value that by default are 0 and 100%
        /// </summary>
        /// <param name="m">Menu</param>
        /// <param name="mode">The mode accept those strings: harass, autoharass, laneclear, jungleclear, lasthit, flee</param>
        /// <param name="startingvalue">Here you have to put the starting value you want the mana manager start</param>
        public static void CreateManaSlider(this Menu m, string mode, int startingvalue)
        {
            switch (mode)
            {
                case "harass":
                    ManaManagerTitle(m);
                    ManaManagerSlider(m, 9, startingvalue);
                    break;
                case "autoharass":
                    ManaManagerTitle(m);
                    ManaManagerSlider(m, 14, startingvalue);
                    break;
                case "laneclear":
                    ManaManagerTitle(m);
                    ManaManagerSlider(m, 19, startingvalue);
                    break;
                case "jungleclear":
                    ManaManagerTitle(m);
                    ManaManagerSlider(m, 24, startingvalue);
                    break;
                case "lasthit":
                    ManaManagerTitle(m);
                    ManaManagerSlider(m, 29, startingvalue);
                    break;
                case "flee":
                    ManaManagerTitle(m);
                    ManaManagerSlider(m, 34, startingvalue);
                    break;
            }
        }

        private static void ManaManagerTitle(this Menu m)
        {
            m.AddSeparator();
            m.AddGroupLabel("Mana Manager:");
        }

        private static void ManaManagerSlider(this Menu m, int uid, int startingvalue)
        {
            m.CreateSlider("Mana Manager", uid, startingvalue);
        }

        #endregion 

        #region CheckBox

        /// <summary>
        /// Creates a checkbox
        /// </summary>
        /// <param name="m">Menu</param>
        /// <param name="checkboxName">The name of the check box</param>
        /// <param name="uniqueId">Unique id</param>
        /// <param name="defaultValuecheck">The default value</param>
        /// <returns></returns>
        public static CheckBox CreateCheckBox(this Menu m, string checkboxName, int uniqueId,
            bool defaultValuecheck = true)
        {
            return m.Add("idInt" + uniqueId, new CheckBox(checkboxName, defaultValuecheck));
        }

        /// <summary>
        /// Creates a checkbox
        /// </summary>
        /// <param name="m">Menu</param>
        /// <param name="checkboxName">The name of the check box</param>
        /// <param name="uniqueId">Unique id</param>
        /// <param name="defaultValuecheck">The default value</param>
        /// <returns></returns>
        public static CheckBox CreateCheckBox<TQ, TW, TE, TR>(this Menu m, string checkboxName,
            ChampionPluginBase<TQ, TW, TE, TR>.MenuIds uniqueId,
            bool defaultValuecheck = true)
        {
            return CreateCheckBox(m, checkboxName, "idEnum" + uniqueId, defaultValuecheck);
        }

        /// <summary>
        /// Creates a checkbox
        /// </summary>
        /// <param name="m">Menu</param>
        /// <param name="checkboxName">The name of the check box</param>
        /// <param name="uniqueId">Unique id</param>
        /// <param name="defaultValuecheck">The default value</param>
        /// <returns></returns>
        public static CheckBox CreateCheckBox(this Menu m, string checkboxName, string uniqueId,
            bool defaultValuecheck = true)
        {
            return m.Add(uniqueId, new CheckBox(checkboxName, defaultValuecheck));
        }

        public static bool GetCheckBoxValue(this Menu menu, string uniqueId)
        {
            try
            {
                return menu.Get<CheckBox>(uniqueId).CurrentValue;
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Error getting the checkbox value with the ID = ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(uniqueId);
                Console.ResetColor();
                Console.WriteLine();
            }
            return false;
        }

        public static bool GetCheckBoxValue(this Menu menu, int uniqueId)
        {
            return GetCheckBoxValue(menu, "idInt" + uniqueId);
        }

        public static bool GetCheckBoxValue<TQ, TW, TE, TR>(this Menu menu,
            ChampionPluginBase<TQ, TW, TE, TR>.MenuIds uniqueId)
        {
            return GetCheckBoxValue(menu, "idEnum" + uniqueId);
        }

        #endregion

        #region ComboBox
        /// <summary>
        /// Create a Combobox
        /// </summary>
        /// <param name="m">The menu, dont take care, this thing isnt needed</param>
        /// <param name="comboName">The display name like Switch Spaghetti</param>
        /// <param name="uniqueId">A number for classify the combobox like 666</param>
        /// <param name="items">Here goes the list of the voices like new[] {"spaghetti", "pizza"}</param>
        /// <param name="defaaultValue">If you want to start from another value that is not the first one write here the int like we want to start with pizza write 1</param>
        /// <returns></returns>
        public static ComboBox CreateComboBox(this Menu m, string comboName, int uniqueId, IEnumerable<string> items,
            int defaaultValue = 0)
        {
            return m.Add("idInt" + uniqueId, new ComboBox(comboName, items, defaaultValue));
        }

        public static ComboBox CreateComboBox<TQ, TW, TE, TR>(this Menu m, string comboName,
            ChampionPluginBase<TQ, TW, TE, TR>.MenuIds uniqueId,
            IEnumerable<string> items, int defaaultValue = 0)
        {
            return m.Add("idEnum" + uniqueId, new ComboBox(comboName, items, defaaultValue));
        }
        /// <summary>
        /// Create a Combobox
        /// </summary>
        /// <param name="m">The menu, dont take care, this thing isnt needed</param>
        /// <param name="comboName">The display name like Switch Spaghetti</param>
        /// <param name="uniqueId">A word for classify the combobox like "imopasfack"</param>
        /// <param name="items">Here goes the list of the voices like new[] {"spaghetti", "pizza"}</param>
        /// <param name="defaaultValue">If you want to start from another value that is not the first one write here the int like we want to start with pizza write 1</param>
        /// <returns></returns>
        public static ComboBox CreateComboBox(this Menu m, string comboName, string uniqueId, IEnumerable<string> items,
            int defaaultValue = 0)
        {
            return m.Add(uniqueId, new ComboBox(comboName, items, defaaultValue));
        }

        /// <summary>
        /// Return the int value of the combobox chosen, like ComboMenu.GetComboboxIndex("imopasfack") == current value 
        /// </summary>
        /// <param name="menu">Ignore this, is already grabbed by the first call like ComboMenu.blablabla</param>
        /// <param name="uniqueId">Here you have to insert the string u assigned first in the menu</param>
        /// <returns></returns>
        public static int GetComboboxIndex(this Menu menu, string uniqueId)
        {
            try
            {
                return menu.Get<ComboBox>(uniqueId).CurrentValue;
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Error getting the combobox value with the ID = ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(uniqueId);
                Console.ResetColor();
                Console.WriteLine();
            }
            return -1;
        }

        /// <summary>
        /// Return the int value of the combobox chosen, like ComboMenu.GetComboboxIndex(666) == current value 
        /// </summary>
        /// <param name="menu">Ignore this, is already grabbed by the first call like ComboMenu.blablabla</param>
        /// <param name="uniqueId">Here you have to insert the int u assigned first in the menu</param>
        /// <returns></returns>
        public static int GetComboboxIndex(this Menu menu, int uniqueId)
        {
            return GetComboboxIndex(menu, "idInt" + uniqueId);
        }

        public static int GetComboboxIndex<TQ, TW, TE, TR>(this Menu menu,
            ChampionPluginBase<TQ, TW, TE, TR>.MenuIds uniqueId)
        {
            return GetComboboxIndex(menu, "idEnum" + uniqueId);
        }

        #endregion
    }

    /// <summary>
    /// Vector Extensions
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Check if the vector is under the enemy tower
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool IsUnderEnemyTurret(this Vector3 position)
        {
            return EntityManager.Turrets.Enemies.Any(t => t.IsValidTarget(950));
        }

        /// <summary>
        /// Check if the postion is dangerous
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsDangPos(this Vector3 pos, uint range)
        {
            return
                EntityManager.Heroes.Enemies.Any(
                    e => e.IsValidTarget() &&
                         e.Distance(pos) < range) ||
                (pos.IsUnderEnemyTurret() && !Player.Instance.IsUnderEnemyturret()) ||
                (pos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) &&
                 pos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building));
        }

        /// <summary>
        /// Get the direction that the target is looking at
        /// </summary>
        /// <param name="target">The minion/hero or etc to get the direction</param>
        /// <returns></returns>
        public static Vector3 GetTargetDirection(this Obj_AI_Base target)
        {
            return target.Direction.To2D().Perpendicular().To3D();
        }

        #region LaneClear

        /// <summary>
        /// It will get the best possible position for linear skillshot
        /// </summary>
        /// <param name="spell">Linear skillshot spell</param>
        /// <param name="minMinionsToHit">Minimun minions to hit</param>
        /// <returns></returns>
        public static Vector3 GetBestFarmPositionLinear(this Spell.Skillshot spell, int minMinionsToHit = 3)
        {
            var minions =
                EntityManager.MinionsAndMonsters.GetLaneMinions().Where(m => m.IsValidTarget(spell.Range)).ToArray();

            var bestPos = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, spell.Width,
                (int)spell.Range, Player.Instance.Position.To2D());

            return minions.Length != 0 ? bestPos.CastPosition : Vector3.Zero;
        }

        /// <summary>
        /// It will get the best possible position for circular skillshot
        /// </summary>
        /// <param name="spell">Circular skillshot spell</param>
        /// <param name="minMinionsToHit">Minimun minions to hit</param>
        /// <returns></returns>
        public static Vector3 GetBestFarmPositionCircular(this Spell.Skillshot spell, int minMinionsToHit = 3)
        {
            var minions =
                EntityManager.MinionsAndMonsters.GetLaneMinions().Where(m => m.IsValidTarget(spell.Range)).ToArray();

            var bestPos = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, spell.Width,
                (int)spell.Range, Player.Instance.Position.To2D());

            return minions.Length != 0 ? bestPos.CastPosition : Vector3.Zero;
        }

        #endregion LaneClear

        #region BestCastPosition
        /// <summary>
        /// Will get the best position to cast the spell
        /// </summary>
        /// <param name="entities">Heroes</param>
        /// <param name="width">Width of the spells</param>
        /// <param name="range">Range</param>
        /// <param name="delay">Delay</param>
        /// <param name="speed">Speed</param>
        /// <param name="sourcePosition">The position of the source for example Player`s pos</param>
        /// <returns></returns>
        public static BestCastPosition GetBestCircularCastPosition(IEnumerable<AIHeroClient> entities, float width, int range, int delay, float speed, Vector2? sourcePosition = null)
        {
            var targets = entities.Cast<Obj_AI_Base>().ToArray();

            Vector3? source = null;
            if (sourcePosition.HasValue)
            {
                source = sourcePosition.Value.To3DWorld();
            }

            var startPos = sourcePosition ?? Player.Instance.ServerPosition.To2D();
            var minionCount = 0;
            var result = Vector2.Zero;

            var validTargets =
                targets.Select(o => Prediction.Position.PredictCircularMissile(o, range, (int)(width / 2f), delay, speed, source))
                    .Where(o => o.UnitPosition.IsInRange(startPos, range + width / 2))
                    .ToArray();
            foreach (var pos in validTargets)
            {
                var count = validTargets.Count(o => o.UnitPosition.IsInRange(pos.UnitPosition, width / 2));

                if (count >= minionCount)
                {
                    result = pos.UnitPosition.To2D();
                    minionCount = count;
                }
            }

            return new BestCastPosition { CastPosition = result.To3DWorld(), HitNumber = minionCount };
        }

        /// <summary>
        /// Will get the best position to cast the spell
        /// </summary>
        /// <param name="entities">Heroes</param>
        /// <param name="width">Width of the spells</param>
        /// <param name="range">Range</param>
        /// <param name="sourcePosition">The position of the source for example Player`s pos</param>
        /// <returns></returns>
        public static BestCastPosition GetBestLinearCastPosition(IEnumerable<AIHeroClient> entities, float width, int range, Vector2? sourcePosition = null)
        {
            var targets = entities.ToArray();
            switch (targets.Length)
            {
                case 0:
                    return new BestCastPosition();
                case 1:
                    return new BestCastPosition { CastPosition = targets[0].ServerPosition, HitNumber = 1 };
            }

            var posiblePositions = new List<Vector2>(targets.Select(o => o.ServerPosition.To2D()));
            foreach (var target in targets)
            {
                posiblePositions.AddRange(from t in targets where t.NetworkId != target.NetworkId select (t.ServerPosition.To2D() + target.ServerPosition.To2D()) / 2);
            }

            var startPos = sourcePosition ?? Player.Instance.ServerPosition.To2D();
            var minionCount = 0;
            var result = Vector2.Zero;

            foreach (var pos in posiblePositions.Where(o => o.IsInRange(startPos, range)))
            {
                var endPos = startPos + range * (pos - startPos).Normalized();
                var count = targets.Count(o => o.ServerPosition.To2D().Distance(startPos, endPos, true, true) <= width * width);

                if (count >= minionCount)
                {
                    result = endPos;
                    minionCount = count;
                }
            }

            return new BestCastPosition { CastPosition = result.To3DWorld(), HitNumber = minionCount };
        }

        public struct BestCastPosition
        {
            public int HitNumber;
            public Vector3 CastPosition;
        }
        #endregion BestCastPosition
    }

    /// <summary>
    /// Gettings target
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// It will get a target that`s is killable with the spell
        /// </summary>
        /// <param name="spell">Any spell</param>
        /// <returns></returns>
        public static AIHeroClient GetKillableTarget(this Spell.SpellBase spell)
        {
            return
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    e =>
                        e.IsValidTarget(spell.Range) &&
                        Prediction.Health.GetPrediction(e, spell.CastDelay + Game.Ping) <=
                        Player.Instance.GetSpellDamage(e, spell.Slot));
        }

        /// <summary>
        /// It will get a target that`s is killable with the spell
        /// </summary>
        /// <param name="spell">Any spell</param>
        /// <param name="range"></param>
        /// <returns></returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static AIHeroClient GetKillableTarget(this Spell.SpellBase spell, uint range = 1000)
        {
            return
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    e =>
                        e.IsValidTarget(range) &&
                        Prediction.Health.GetPrediction(e, spell.CastDelay + Game.Ping) <=
                        Player.Instance.GetSpellDamage(e, spell.Slot));
        }

        /// <summary>
        ///     This function will list all the closest enemies
        /// </summary>
        /// <param name="range">Limit the list to only range value</param>
        /// <returns></returns>
        public static List<AIHeroClient> CloseEnemies(float range = 1200)
        {
            return EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(range)).ToList();
        }

        /// <summary>
        ///     This function will list all the closest allies
        /// </summary>
        /// <param name="range">Limit the list to only range value</param>
        /// <returns></returns>
        public static List<AIHeroClient> CloseAllies(float range = 1200)
        {
            return EntityManager.Heroes.Allies.Where(a => a.IsValidTarget(range) && !a.IsMe).ToList();
        }

        #region LaneMinions 

        /// <summary>
        /// It will get the best lane minion
        /// </summary>
        /// <param name="spell"></param>
        /// <returns></returns>
        public static Obj_AI_Minion GetLaneMinion(this Spell.SpellBase spell)
        {
            return
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderBy(mH => mH.Health)
                    .ThenBy(mD => mD.Distance(Player.Instance))
                    .FirstOrDefault(
                        m =>
                            m.IsValidTarget(spell.Range));
        }

        /// <summary>
        /// It will get the best minion to last hit on Lane
        /// </summary>
        /// <param name="spell">Any spell</param>
        /// <returns></returns>
        public static Obj_AI_Minion GetLasthitLaneMinion(this Spell.SpellBase spell)
        {
            return
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderBy(mH => mH.Health)
                    .ThenBy(mD => mD.Distance(Player.Instance))
                    .FirstOrDefault(
                        m =>
                            m.IsValidTarget(spell.Range) &&
                            Prediction.Health.GetPrediction(m, spell.CastDelay + Game.Ping) <=
                            Player.Instance.GetSpellDamage(m, spell.Slot) &&
                            Prediction.Health.GetPrediction(m, spell.CastDelay + Game.Ping) >=
                            Player.Instance.GetAutoAttackDamage(m));
        }

        /// <summary>
        /// It will get the best minion to jump on
        /// </summary>
        /// <param name="spell">Any spell</param>
        /// <param name="target">Check the distance from that target</param>
        /// <returns></returns>
        public static Obj_AI_Minion GetNearestMinion(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            return
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderBy(mH => mH.Health)
                    .ThenBy(mD => mD.Distance(target))
                    .FirstOrDefault(
                        m =>
                            m.IsValidTarget(spell.Range));
        }

        #endregion LaneMinions

        #region JungleMinions

        public static Obj_AI_Minion GetJungleMinion(this Spell.SpellBase spell)
        {
            return
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .OrderBy(mH => mH.Health)
                    .ThenBy(mD => mD.Distance(Player.Instance))
                    .FirstOrDefault(
                        m =>
                            m.IsValidTarget(spell.Range));
        }

        /// <summary>
        /// It will get the best minion to last hit on Jungle
        /// </summary>
        /// <param name="spell">Any spell</param>
        /// <returns></returns>
        public static Obj_AI_Minion GetLasthitJungleMinion(this Spell.SpellBase spell)
        {
            return
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .OrderBy(mH => mH.Health)
                    .ThenBy(mD => mD.Distance(Player.Instance))
                    .FirstOrDefault(
                        m =>
                            m.IsValidTarget(spell.Range) &&
                            Prediction.Health.GetPrediction(m, spell.CastDelay) <=
                            Player.Instance.GetSpellDamage(m, spell.Slot) &&
                            Prediction.Health.GetPrediction(m, spell.CastDelay) >=
                            Player.Instance.GetAutoAttackDamage(m));
        }

        #endregion JungleMinions

        #region Minions in general
        public static Obj_AI_Minion GetTheNearestMinion(this Obj_AI_Base target, uint range)
        {
            return
                EntityManager.MinionsAndMonsters.Minions.OrderBy(m => m.Distance(target)).FirstOrDefault(m => m.IsEnemy && m.IsInRange(target, range) && m.IsValidTarget());
        }

        /// <summary>
        /// Get the nearest minion in the specified spell range
        /// </summary>
        /// <param name="target"></param>
        /// <param name="spell"></param>
        /// <returns></returns>
        public static Obj_AI_Minion GetTheNearestMinion(this Obj_AI_Base target, Spell.SpellBase spell)
        {
            return
                EntityManager.MinionsAndMonsters.Minions.OrderBy(m => m.Distance(target)).FirstOrDefault(m => m.IsEnemy && m.IsInRange(target, spell.Range) && m.IsValidTarget());
        }

        /// <summary>
        /// Get the furthest minion in the specified range
        /// </summary>
        /// <param name="target"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Obj_AI_Minion GetTheFurthestMinionInRange(this Obj_AI_Base target, uint range)
        {
            return
                EntityManager.MinionsAndMonsters.Minions.OrderByDescending(m => m.Distance(target))
                    .FirstOrDefault(m => m.IsEnemy && m.IsInRange(target, range) && m.IsValidTarget());
        }

        /// <summary>
        /// Get the furthest minion in the specified spell range
        /// </summary>
        /// <param name="target"></param>
        /// <param name="spell"></param>
        /// <returns></returns>
        public static Obj_AI_Minion GetTheFurthestMinionInRange(this Obj_AI_Base target, Spell.SpellBase spell)
        {
            return
                EntityManager.MinionsAndMonsters.Minions.OrderByDescending(m => m.Distance(target))
                    .FirstOrDefault(m => m.IsEnemy && m.IsInRange(target, spell.Range) && m.IsValidTarget());
        }
        #endregion Minions in general
    }
}
