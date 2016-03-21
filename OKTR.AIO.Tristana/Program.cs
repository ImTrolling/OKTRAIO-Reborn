using System.Diagnostics;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using OKTR.Plugin;
using Extensions = OKTR.Plugin.Extensions;


namespace OKTR.AIO.Tristana
{
    class Program
    {
        static void Main(string[] args)
        {
            new Tristana().RegisterPlugin();
        }
    }

    public class Tristana : ChampionPluginBase <Spell.Active, Spell.Skillshot, Spell.Targeted, Spell.Targeted>
    {
        public override void InitializeSpells()
        {
            Q = new Spell.Active(SpellSlot.Q, 550);
            W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 450, int.MaxValue, 180);
            E = new Spell.Targeted(SpellSlot.E, 550);
            R = new Spell.Targeted(SpellSlot.R, 550);
        }

        public override void InitializeEvents()
        {

        }

        public override void InitializeMenu()
        {
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.CreateCheckBox("Use Q", MenuIds.ComboUseQ);
            ComboMenu.CreateCheckBox("Use W", MenuIds.ComboUseW);
            ComboMenu.CreateCheckBox("Use E", MenuIds.ComboUseE);
            ComboMenu.CreateCheckBox("Use R", MenuIds.ComboUseR);
            HarassMenu.CreateCheckBox("Use Q", MenuIds.HarassUseQ);
            HarassMenu.CreateCheckBox("Use W", MenuIds.HarassUseW);
            HarassMenu.CreateCheckBox("Use E", MenuIds.HarassUseE);
            HarassMenu.CreateCheckBox("Use R", MenuIds.HarassUseR);

        }

        public override void Combo()
        {
            var target = GetTarget(Q);
            if (ComboMenu.GetCheckBoxValue(MenuIds.ComboUseQ)) Q.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(MenuIds.ComboUseW)) W.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(MenuIds.ComboUseE)) E.TryCast(target);
            if (ComboMenu.GetCheckBoxValue(MenuIds.ComboUseR)) R.TryCast(target);
        }

        public override void Harass()
        {
            var target = GetTarget(Q);
            if (HarassMenu.GetCheckBoxValue(MenuIds.HarassUseQ)) Q.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(MenuIds.HarassUseW)) W.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(MenuIds.HarassUseE)) E.TryCast(target);
            if (HarassMenu.GetCheckBoxValue(MenuIds.HarassUseR)) R.TryCast(target);
        }

        public override void Laneclear()
        {

        }

        public override void Lasthit()
        {

        }

        public override void Jungleclear()
        {

        }

        public override void Flee()
        {

        }

        public override void PermaActive()
        {
        }

        public override void Draw()
        {
            throw new System.NotImplementedException();
        }
    }
}
