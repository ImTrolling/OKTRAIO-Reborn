using System.Collections.Generic;
using EloBuddy;
using OKTR.Plugin; 
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace OKTR.AIO.Zyra
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new Zyra().RegisterPlugin();
        }
    }

    public class Zyra : ChampionPluginBase<Spell.Skillshot, Spell.SpellBase, Spell.SpellBase, Spell.SpellBase>
    {

        public override void InitializeSpells()
        {

        }

        public override void InitializeEvents()
        {

        }

        public override void InitializeMenu()
        {
            AddMultipleCheckBox(Q, new List<MenuCheckBox> {new MenuCheckBox(ComboMenu), new MenuCheckBox(HarassMenu), new MenuCheckBox(LaneClearMenu, false)});
        }

        public override void Combo()
        {
            
        }

        public override void Harass()
        {
            
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
