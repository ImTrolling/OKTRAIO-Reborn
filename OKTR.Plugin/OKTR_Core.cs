using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace OKTR.Plugin
{
    internal class OKTR_Core
    {
        public static Menu OKTRCoreMenu;
        public static Menu OKTRSpellTargetSelectorMenu;

        public const string OKTRCoreMenuID = "OKTRcoremenuid";
        public const string OKTRSpellTargetSelectorMenuID = "OKTRstsmenuid";

        public static void InitCore()
        {
            OKTRCoreMenu = MainMenu.AddMenu("OKTR Core", OKTRCoreMenuID);
        }

        public static void TargetSelectorMenuInit()
        {
            OKTRSpellTargetSelectorMenu = OKTRCoreMenu.AddSubMenu("Spell Target Selector", OKTRSpellTargetSelectorMenuID + Player.Instance.Hero);
            OKTRSpellTargetSelectorMenu.AddGroupLabel("Q BlackList Settings");
            foreach (var e in EntityManager.Heroes.Enemies)
            {
                OKTRSpellTargetSelectorMenu.CreateCheckBox("Dont use Q on" + e.ChampionName + "("+ e.Name+")", OKTRSpellTargetSelectorMenuID + "Q" + e.BaseSkinName, false);
            }
            OKTRSpellTargetSelectorMenu.AddGroupLabel("W BlackList Settings");
            foreach (var e in EntityManager.Heroes.Enemies)
            {
                OKTRSpellTargetSelectorMenu.CreateCheckBox("Dont use W on" + e.ChampionName + "(" + e.Name + ")", OKTRSpellTargetSelectorMenuID + "W" + e.BaseSkinName, false);
            }
            OKTRSpellTargetSelectorMenu.AddGroupLabel("E BlackList Settings");
            foreach (var e in EntityManager.Heroes.Enemies)
            {
                OKTRSpellTargetSelectorMenu.CreateCheckBox("Dont use E on" + e.ChampionName + "(" + e.Name + ")", OKTRSpellTargetSelectorMenuID + "E" + e.BaseSkinName, false);
            }
            OKTRSpellTargetSelectorMenu.AddGroupLabel("R BlackList Settings");
            foreach (var e in EntityManager.Heroes.Enemies)
            {
                OKTRSpellTargetSelectorMenu.CreateCheckBox("Dont use R on" + e.ChampionName + "(" + e.Name + ")", OKTRSpellTargetSelectorMenuID + "R" + e.BaseSkinName, false);
            }
        }
    }
}
