using System.Linq;
using EloBuddy;
using OKTR.Plugin; 
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace OKTR.AIO.Graves
{
    internal class Program
    {
        public static void Main()
        {
            new Graves().RegisterPlugin();
        }
    }

    [Plugin("Graves", "KarmaPanda", Champion.Graves)]
    public class Graves : ChampionPluginBase<Spell.Skillshot, Spell.Skillshot, Spell.Skillshot, Spell.Skillshot>
    {
        public static Spell.Skillshot R2 { get; set; }

        public static Vector3 GravesQStartPoint { get; set; }

        public static Vector3 GravesQEndPoint { get; set; }

        public static bool HasGravesQExploded { get; set; }

        public override void InitializeSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 0, 3000, 40);
            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 500, 1500, 120);
            E = new Spell.Skillshot(SpellSlot.E, 425, SkillShotType.Linear, 500, 0, 50);
            R = new Spell.Skillshot(SpellSlot.R, 1000, SkillShotType.Linear, 500, 2100, 100)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R2 = new Spell.Skillshot(SpellSlot.R, 1650, SkillShotType.Linear, 500, 2000, 20)
            {
                AllowedCollisionCount = int.MaxValue
            };
        }

        public override void InitializeEvents()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public override void InitializeMenu()
        {
            ComboMenu.CreateCheckBox("Use Q", MenuIds.ComboUseQ);
            ComboMenu.CreateCheckBox("Use W", MenuIds.ComboUseW);
            ComboMenu.CreateCheckBox("Use E", MenuIds.ComboUseE);
            ComboMenu.CreateCheckBox("Use R", MenuIds.ComboUseR);
            ComboMenu.CreateComboBox("E Logic", "combo.e.mode", new[] {"E to Mouse"});
            ComboMenu.CreateCheckBox("Use R for Kill Steal only", "combo.r.ks");
            ComboMenu.CreateSlider("Minimum targets before casting R: {1}", "combo.r.aoe", 3, 1, 5);

            HarassMenu.CreateCheckBox("Use Q", MenuIds.HarassUseQ);
            HarassMenu.CreateCheckBox("Use W", MenuIds.HarassUseW);
            HarassMenu.CreateCheckBox("Use E", MenuIds.HarassUseE);
            HarassMenu.CreateComboBox("E Logic", "harass.e.mode", new[] {"E to Mouse"});

            LaneClearMenu.CreateCheckBox("Use Q", MenuIds.LaneclearUseQ);
            LaneClearMenu.Add("lane.mana", new Slider("Minimum {0}% mana to laneclear with Q", 65));
            LaneClearMenu.AddSeparator();
            LaneClearMenu.AddGroupLabel("Jungle Clear Settings");
            LaneClearMenu.Add("jungle.q", new CheckBox("Use Q"));
            LaneClearMenu.Add("jungle.w", new CheckBox("Use W"));
            LaneClearMenu.Add("jungle.e", new CheckBox("Use E"));
            LaneClearMenu.CreateComboBox("E Logic", "jungle.e.mode", new[] {"E to Mouse"});

            KillStealMenu.CreateCheckBox("Use Q", "killsteal.q");
            KillStealMenu.CreateCheckBox("Use W", "killsteal.w");
            KillStealMenu.CreateCheckBox("Use R", "killsteal.r");
            KillStealMenu.Add("killsteal.e.withr", new CheckBox("Use E with R to Kill Steal"));

            MiscMenu.Add("misc.advancedalgorithm", new Label("Advanced Algorithm Toggles"));
            MiscMenu.Add("misc.qreturn", new CheckBox("Calculate Intersection with Q Return"));
            MiscMenu.Add("misc.qcollision", new CheckBox("Add Wall Collision Check to Q"));

            DrawMenu.CreateCheckBox("Draw Q", "draw.q", false);
            DrawMenu.CreateCheckBox("Draw W", "draw.w", false);
            DrawMenu.CreateCheckBox("Draw E", "draw.e", false);
            DrawMenu.CreateCheckBox("Draw R", "draw.r", false);
            DrawMenu.Add("draw.qreturn", new CheckBox("Draw Q Return Position", false));
        }

        public override void Combo()
        {
            var enemies = EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget() && !t.HasUndyingBuff()).ToArray();

            if (Q.IsReady() && ComboMenu.GetCheckBoxValue(MenuIds.ComboUseQ))
            {
                var target = TargetSelector.GetTarget(enemies, DamageType.Physical);
                if (target == null || !Q.IsInRange(target)) return;
                LogicQ(target);
            }

            if (W.IsReady() && ComboMenu.GetCheckBoxValue(MenuIds.ComboUseW))
            {
                var target = TargetSelector.GetTarget(enemies, DamageType.Magical);
                if (target == null || !W.IsInRange(target)) return;
                LogicW(target);
            }

            if (R.IsReady() && ComboMenu.GetCheckBoxValue(MenuIds.ComboUseR))
            {
                var target = TargetSelector.GetTarget(enemies, DamageType.Physical);
                if (target == null || !R.IsInRange(target)) return;
                LogicR(target, false, false, Orbwalker.ActiveModes.Combo);
            }
        }

        public override void Harass()
        {
            var enemies = EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget() && !t.HasUndyingBuff()).ToArray();

            if (Q.IsReady() && HarassMenu.GetCheckBoxValue(MenuIds.HarassUseQ))
            {
                var target = TargetSelector.GetTarget(enemies, DamageType.Physical);
                if (target == null || !Q.IsInRange(target)) return;
                LogicQ(target);
            }

            if (W.IsReady() && HarassMenu.GetCheckBoxValue(MenuIds.HarassUseW))
            {
                var target = TargetSelector.GetTarget(enemies, DamageType.Magical);
                if (target == null || !W.IsInRange(target)) return;
                LogicW(target);
            }
        }

        public override void Laneclear()
        {
            if (!Q.IsReady() || !LaneClearMenu.GetCheckBoxValue(MenuIds.LaneclearUseQ) ||
                !(Player.Instance.ManaPercent >= LaneClearMenu.GetSliderValue("lane.mana"))) return;
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                Player.Instance.ServerPosition, Q.Range).Where(t => t.Health <= QDamage(t)).ToArray();
            var lineFarmLocation = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minion, Q.Width, (int) Q.Range);
            var lineFarmLocationReturn = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minion, Q.Width,
                (int) Q.Range, lineFarmLocation.CastPosition.Extend(Player.Instance, Q.Range));

            if (lineFarmLocation.HitNumber >= 2 || lineFarmLocationReturn.HitNumber >= 2)
            {
                Q.Cast(lineFarmLocation.CastPosition);
            }
        }

        public override void Lasthit()
        {

        }

        public override void Jungleclear()
        {
            if (Q.IsReady() && LaneClearMenu.GetCheckBoxValue("jungle.q"))
            {
                var jungleMobs =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).ToArray();
                var lineFarmLocation = EntityManager.MinionsAndMonsters.GetLineFarmLocation(jungleMobs, Q.Width,
                    (int) Q.Range);
                var lineFarmLocationReturn = EntityManager.MinionsAndMonsters.GetLineFarmLocation(jungleMobs, Q.Width,
                    (int) Q.Range, lineFarmLocation.CastPosition.Extend(Player.Instance, Q.Range));

                if (lineFarmLocation.HitNumber > 0 || lineFarmLocationReturn.HitNumber > 0)
                {
                    Q.Cast(lineFarmLocation.CastPosition);
                }
            }

            if (W.IsReady() && LaneClearMenu.GetCheckBoxValue("jungle.w"))
            {
                var jungleMobs =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, W.Range).ToArray();
                var circularFarmLocation = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(jungleMobs, Q.Width,
                    (int) Q.Range);

                if (circularFarmLocation.HitNumber > 1)
                {
                    W.Cast(circularFarmLocation.CastPosition);
                }
            }
        }

        public override void Flee()
        {

        }

        public override void PermaActive()
        {
            if (KillStealMenu.GetCheckBoxValue("killsteal.q") && Q.IsReady())
            {
                var killableEnemies =
                    EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(Q.Range) && QDamage(t) >= t.Health);
                var target = TargetSelector.GetTarget(killableEnemies, DamageType.Physical);
                if (target == null) return;
                LogicQ(target);
            }

            if (KillStealMenu.GetCheckBoxValue("killsteal.w") && W.IsReady())
            {
                var killableEnemies =
                    EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(W.Range) && WDamage(t) >= t.Health);
                var target = TargetSelector.GetTarget(killableEnemies, DamageType.Physical);
                if (target == null) return;
                LogicW(target);
            }

            if (KillStealMenu.GetCheckBoxValue("killsteal.r") && R.IsReady())
            {
                var useE = KillStealMenu.GetCheckBoxValue("killsteal.e.withr");
                var killableEnemies =
                    EntityManager.Heroes.Enemies.Where(
                        t => t.IsValidTarget(R.Range + (useE ? E.Range : 0)) && RDamage(t) >= t.Health);
                var target = TargetSelector.GetTarget(killableEnemies, DamageType.Physical);

                if (target == null)
                {
                    killableEnemies =
                        EntityManager.Heroes.Enemies.Where(
                            t => t.IsValidTarget(R2.Range + (useE ? E.Range : 0)) && RDamage(t, true) >= t.Health);
                    target = TargetSelector.GetTarget(killableEnemies, DamageType.Physical);

                    if (target != null)
                    {
                        LogicR(target, true, useE);
                    }
                }
                else
                {
                    LogicR(target, false, useE);
                }
            }
        }

        #region Methods

        private void LogicQ(Obj_AI_Base target)
        {
            if (!Q.IsInRange(target)) return;

            if (Q != null)
            {
                var qPrediction = Q.GetPrediction(target);

                if (qPrediction == null || qPrediction.HitChancePercent < 75) return;

                if (MiscMenu.GetCheckBoxValue("misc.qcollision"))
                {
                    var predictedLocation = qPrediction.CastPosition;
                    var playerLocation = Player.Instance.Position;

                    for (var i = 0; i < Player.Instance.Distance(predictedLocation); i++)
                    {
                        var newExtendedLocation = playerLocation.Extend(predictedLocation, i);
                        var collFlagPred = NavMesh.GetCollisionFlags(newExtendedLocation);

                        if (collFlagPred.HasFlag(CollisionFlags.Wall) && collFlagPred.HasFlag(CollisionFlags.Building))
                        {
                            return;
                        }

                        predictedLocation = new Vector3(newExtendedLocation,
                            NavMesh.GetHeightForPosition(newExtendedLocation.X, newExtendedLocation.Y));
                    }
                }

                if (MiscMenu.GetCheckBoxValue("misc.qreturn"))
                {
                    var returnQCollisionCheck = Prediction.Position.Collision.LinearMissileCollision(target,
                        qPrediction.CastPosition.To2D(), Player.Instance.Position.To2D(), Q.Speed, Q.Width, Q.CastDelay);

                    if (returnQCollisionCheck)
                    {
                        Q.Cast(qPrediction.CastPosition);
                    }
                    return;
                }

                Q.Cast(qPrediction.CastPosition);
            }
        }

        private void LogicW(Obj_AI_Base target)
        {
            if (W != null)
            {
                var wPrediction = W.GetPrediction(target);

                if (wPrediction == null || wPrediction.HitChancePercent < 75) return;

                W.Cast(wPrediction.CastPosition);
            }
        }

        private void LogicE(Obj_AI_Base target, Orbwalker.ActiveModes activeMode)
        {
            if (Player.Instance.CanAttack || target == null) return;

            if (activeMode == Orbwalker.ActiveModes.JungleClear && target.IsMonster)
            {
                var jungleBoxValue = LaneClearMenu.GetComboboxIndex("jungle.e.mode");

                if (jungleBoxValue == 0 && !Player.HasBuff("gravesbasicattackammo2"))
                {
                    var getMousePos = Player.Instance.Position.Extend(Game.ActiveCursorPos, E.Range);
                    E.Cast(new Vector3(getMousePos, NavMesh.GetHeightForPosition(getMousePos.X, getMousePos.Y)));
                    return;
                }
            }

            var t = target as AIHeroClient;

            if (t == null)
            {
                return;
            }

            if (activeMode == Orbwalker.ActiveModes.Combo)
            {
                var comboBoxValue = ComboMenu.GetComboboxIndex("combo.e.mode");

                if (comboBoxValue == 0 && !Player.HasBuff("gravesbasicattackammo2"))
                {
                    var getMousePos = Player.Instance.Position.Extend(Game.ActiveCursorPos, E.Range);
                    E.Cast(new Vector3(getMousePos, NavMesh.GetHeightForPosition(getMousePos.X, getMousePos.Y)));
                    return;
                }
            }

            if (activeMode == Orbwalker.ActiveModes.Harass)
            {
                var harassBoxValue = HarassMenu.GetComboboxIndex("harass.e.mode");

                if (harassBoxValue == 0 && !Player.HasBuff("gravesbasicattackammo2"))
                {
                    var getMousePos = Player.Instance.Position.Extend(Game.ActiveCursorPos, E.Range);
                    E.Cast(new Vector3(getMousePos, NavMesh.GetHeightForPosition(getMousePos.X, getMousePos.Y)));
                }
            }
        }

        private void LogicR(Obj_AI_Base target, bool extendedR = false, bool useE = false,
            Orbwalker.ActiveModes activeMode = Orbwalker.ActiveModes.None)
        {
            var t = target as AIHeroClient;

            if (t == null) return;

            if (extendedR)
            {
                if (!R2.IsInRange(t) && useE)
                {
                    var dist = Player.Instance.Distance(target) - R2.Range;

                    if (dist > E.Range) return;

                    var ePrediction = E.GetPrediction(target);

                    if (ePrediction.CastPosition.IsInRange(target, R2.Range))
                    {
                        E.Cast(ePrediction.CastPosition);
                    }

                    var rPrediction = R2.GetPrediction(t);

                    if (rPrediction != null && rPrediction.HitChancePercent >= 75)
                    {
                        R2.Cast(rPrediction.CastPosition);
                    }
                }
                else if (R2.IsInRange(t))
                {
                    var rPrediction = R2.GetPrediction(t);

                    if (rPrediction != null && rPrediction.HitChancePercent >= 75)
                    {
                        R2.Cast(rPrediction.CastPosition);
                    }
                }
            }
            else
            {
                if (!R.IsInRange(t) && useE)
                {
                    var dist = Player.Instance.Distance(target) - R.Range;

                    if (dist > E.Range) return;

                    var ePrediction = E.GetPrediction(target);

                    if (ePrediction.CastPosition.IsInRange(target, R.Range))
                    {
                        E.Cast(ePrediction.CastPosition);
                    }

                    var rPrediction = R.GetPrediction(t);

                    if (rPrediction != null && rPrediction.HitChancePercent >= 75)
                    {
                        R.Cast(rPrediction.CastPosition);
                    }
                }
                else if (activeMode == Orbwalker.ActiveModes.Combo)
                {
                    var enemiesAroundTarget = t.CountEnemiesInRange(R.Width);

                    if (enemiesAroundTarget < ComboMenu.GetSliderValue("combo.r.aoe")) return;

                    var rPrediction = R.GetPrediction(t);

                    if (rPrediction != null && rPrediction.HitChancePercent >= 75)
                    {
                        R.Cast(rPrediction.CastPosition);
                    }
                }
                else if (R.IsInRange(t))
                {
                    var rPrediction = R.GetPrediction(t);

                    if (rPrediction != null && rPrediction.HitChancePercent >= 75)
                    {
                        R.Cast(rPrediction.CastPosition);
                    }
                }
            }
        }

        private float QDamage(Obj_AI_Base target, bool calculateReturnDamage = true)
        {
            float damage = 0;
            if (calculateReturnDamage)
            {
                damage += Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                    new[] {0, 80, 125, 170, 215, 260}[Q.Level] +
                    (Player.Instance.FlatPhysicalDamageMod*new[] {0, 0.40f, 0.60f, 0.80f, 1f, 1.2f}[Q.Level]));
            }

            damage += Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                new[] {0, 55, 70, 85, 100, 115}[Q.Level] + (Player.Instance.FlatPhysicalDamageMod*0.75f));

            return damage;
        }

        private float WDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] {0, 60, 110, 160, 210, 260}[W.Level] + (Player.Instance.TotalMagicalDamage*0.6f));
        }

        private float RDamage(Obj_AI_Base target, bool calculateConeDamage = false)
        {
            if (calculateConeDamage)
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                    (new[] {0, 250, 400, 550}[R.Level] + (Player.Instance.FlatPhysicalDamageMod*1.5f)*0.8f));
            }
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                new[] {0, 250, 400, 550}[R.Level] + (Player.Instance.FlatPhysicalDamageMod*1.5f));
        }

        #endregion

        #region Events

        public override void Draw()
        {
            throw new System.NotImplementedException();
        }

        private void Drawing_OnDraw(System.EventArgs args)
        {
            if (DrawMenu.GetCheckBoxValue("draw.qreturn"))
            {
                new Circle
                {
                    Radius = 200,
                    Color = System.Drawing.Color.Red
                }.Draw(GravesQStartPoint);

                new Circle
                {
                    Radius = 200,
                    Color = System.Drawing.Color.Red
                }.Draw(GravesQEndPoint);
            }

            if (Q.IsReady() && DrawMenu.GetCheckBoxValue("draw.q"))
            {
                new Circle
                {
                    Radius = Q.Range
                }.Draw(Player.Instance.Position);
            }

            if (W.IsReady() && DrawMenu.GetCheckBoxValue("draw.w"))
            {
                new Circle
                {
                    Radius = W.Range
                }.Draw(Player.Instance.Position);
            }

            if (E.IsReady() && DrawMenu.GetCheckBoxValue("draw.e"))
            {
                new Circle
                {
                    Radius = E.Range
                }.Draw(Player.Instance.Position);
            }

            if (R.IsReady() && DrawMenu.GetCheckBoxValue("draw.r"))
            {
                new Circle
                {
                    Radius = R.Range
                }.Draw(Player.Instance.Position);
            }
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, System.EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                ComboMenu.GetCheckBoxValue(MenuIds.ComboUseE))
            {
                if (!E.IsReady()) return;

                LogicE(target as AIHeroClient, Orbwalker.ActiveModes.Combo);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                HarassMenu.GetCheckBoxValue(MenuIds.HarassUseE))
            {
                if (!E.IsReady()) return;

                LogicE(target as AIHeroClient, Orbwalker.ActiveModes.Harass);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) &&
                LaneClearMenu.GetCheckBoxValue("jungle.e"))
            {
                if (!E.IsReady()) return;

                LogicE(target as Obj_AI_Base, Orbwalker.ActiveModes.JungleClear);
            }
        }

        private static void GameObject_OnDelete(GameObject sender, System.EventArgs args)
        {
            var missileClient = sender as MissileClient;

            if (missileClient == null || !missileClient.SpellCaster.IsMe) return;

            // Called when Graves Q explodes.
            if (missileClient.SData.Name == "GravesClusterShotSoundMissile")
            {
                HasGravesQExploded = true;
            }

            // Delete Graves Starting and End Point after Graves Q Returns to original position
            if (missileClient.SData.Name == "GravesQReturn")
            {
                GravesQStartPoint = Vector3.Zero;
                GravesQEndPoint = Vector3.Zero;
                HasGravesQExploded = false;
            }
        }

        private static void GameObject_OnCreate(GameObject sender, System.EventArgs args)
        {
            var missileClient = sender as MissileClient;

            if (missileClient == null || !missileClient.SpellCaster.IsMe) return;

            // Called when Graves Q Return is called
            if (missileClient.SData.Name == "GravesQReturn")
            {
                HasGravesQExploded = false;
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name == "GravesQLineSpell")
            {
                GravesQStartPoint = args.Start;
                var endPoint = args.Start.Extend(args.End, Q.Range);
                GravesQEndPoint = new Vector3(endPoint, NavMesh.GetHeightForPosition(endPoint.X, endPoint.Y));
            }
        }

        #endregion
    }
}