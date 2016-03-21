using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using OKTR.Plugin;
using SharpDX;

namespace OKTR.AIO.Vayne
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //new Vayne().RegisterPlugin();
        }
    }

    //[Plugin("Biks Vayne", "MarioGK", Champion.Vayne)]

    public class Vayne : ChampionPluginBase<Spell.SpellBase, Spell.SpellBase, Spell.SpellBase, Spell.SpellBase>
    {
        private AIHeroClient Target;
        private readonly int Range = (int)Player.Instance.GetAutoAttackRange() + 300;
        public override void InitializeSpells()
        {
            Q = new Spell.Active(SpellSlot.Q);
            E = new Spell.Targeted(SpellSlot.E, 555); //440
            R = new Spell.Active(SpellSlot.R, (uint) Player.Instance.GetAutoAttackRange());
        }

        public override void InitializeEvents()
        {
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            //Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
        }

        #region Events

        private void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsValidTarget(E.Range) && sender.IsEnemy)
            {
                E.Cast(sender);
            }
        }

        private void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsValidTarget(E.Range) && sender.IsEnemy)
            {
                E.Cast(sender);
            }
        }

       /* private void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (Q.IsReady() && Target.IsValidTarget(Range))
            {
                Player.CastSpell(SpellSlot.Q, GetQPos());
            }
        }*/

        #endregion Events

        public override void InitializeMenu()
        {
            ComboMenu.CreateComboBox("Q Mode", MenuIds.ComboBoxMode,
                new List<string> {"Auto", "Agresive(Auto)", "Safe(Auto)", "Always to mouse position(Manual)"});
        }

        public override void Combo()
        {
            /*if (Q.IsReady() && Target.IsValidTarget(Range) && !Target.IsInRange(Player.Instance, Player.Instance.GetAutoAttackRange()))
            {
                Player.CastSpell(SpellSlot.Q, GetQPos());
            }*/
        }

        public override void Harass()
        {
           /* if (Q.IsReady() && Target.IsValidTarget(Range) && !Target.IsInRange(Player.Instance, Player.Instance.GetAutoAttackRange()))
            {
                Player.CastSpell(SpellSlot.Q, GetQPos());
            }*/
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
            Target = GetTarget(Range);

            Stun();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        #region Functions

        public Vector3 GetQPos()
        {
            var extentedToMouse = Player.Instance.Position.Extend(Game.CursorPos, 300).To3D();
            var awayFromTarget = Player.Instance.Position.Extend(Target, -300).To3D();
            var toTarget = Player.Instance.Position.Extend(Target, 300).To3D();

            switch (ComboMenu.GetComboboxIndex(MenuIds.ComboBoxMode))
            {
                //Auto
                case 0:
                    if (extentedToMouse.Distance(Target) >= Player.Instance.GetAutoAttackRange() && toTarget.Distance(Target) <= Player.Instance.GetAutoAttackRange())
                    {
                        if (!toTarget.IsDangPos(350))
                        {
                            Chat.Print("1");
                            return toTarget;
                        }
                    }

                    if (extentedToMouse.IsDangPos(350))
                    {
                        Chat.Print("2");
                        if (!awayFromTarget.IsDangPos(350))
                        {
                            Chat.Print("3");
                            return awayFromTarget;
                        }
                    }
                    Chat.Print("4");
                    return extentedToMouse;
                //Agressive
                case 1:
                    if (extentedToMouse.Distance(Target) < Player.Instance.GetAutoAttackRange())
                    {
                        if (!toTarget.IsDangPos(350))
                        {
                            return toTarget;
                        }
                    }
                    else
                    {
                        return extentedToMouse;
                    }
                    break;
                    //Safe
                case 2:
                    if (extentedToMouse.IsDangPos(350))
                    {
                        if (!awayFromTarget.IsDangPos(350))
                        {
                            return awayFromTarget;
                        }
                    }
                    else
                    {
                        return extentedToMouse;
                    }
                    break;
                //To Mouse pos
                case 3:
                    if (!extentedToMouse.IsDangPos(350))
                    {
                        return extentedToMouse;
                    }
                    break;
            }
            return Vector3.Zero;
        }

        public void Stun()
        {
            if(Target == null || !E.IsReady())return;
            var pos = Target.ServerPosition.Extend(Player.Instance.ServerPosition, -440).ToNavMeshCell();
            var predPos = Prediction.Position.PredictUnitPosition(Target, Game.Ping + E.CastDelay).ToNavMeshCell();
            if ((pos.CollFlags.HasFlag(CollisionFlags.Wall) && predPos.CollFlags.HasFlag(CollisionFlags.Wall)) ||
                (pos.CollFlags.HasFlag(CollisionFlags.Building) && predPos.CollFlags.HasFlag(CollisionFlags.Building)))
            {
                E.Cast(Target);
            }
        }

        #endregion Functions
    }
}
