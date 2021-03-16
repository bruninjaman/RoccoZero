using System;

using Divine;

using Ensage;
using Ensage.Common;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Menu;
using Ensage.SDK.Renderer.Particle;
using SharpDX;

namespace TinkerCrappahilationPaid
{
    public class DrawingHelper
    {
        private readonly TinkerCrappahilationPaid _main;
        private Config Config => _main.Config;
        private Hero Me => _main.Me;

        public DrawingHelper(TinkerCrappahilationPaid main)
        {
            _main = main;
            Factory = Config.Factory.Menu("Drawing");
            EnableDamageDrawing = Factory.Item("Draw damage", true);
            DrawBlinkRangeOnTeleport = Factory.Item("Draw blink range while teleporting", true);

            if (EnableDamageDrawing)
            {
                Drawing.OnDraw += DrawingOnOnDraw;
            }

            EnableDamageDrawing.PropertyChanged += (sender, args) =>
            {
                if (EnableDamageDrawing)
                {
                    Drawing.OnDraw += DrawingOnOnDraw;
                }
                else
                {
                    Drawing.OnDraw -= DrawingOnOnDraw;
                }
            };

            if (DrawBlinkRangeOnTeleport)
            {
                Unit.OnModifierAdded += UnitOnOnModifierAdded;
                Unit.OnModifierRemoved += UnitOnOnModifierRemoved;
            }

            DrawBlinkRangeOnTeleport.PropertyChanged += (sender, args) =>
            {
                if (DrawBlinkRangeOnTeleport)
                {
                    Unit.OnModifierAdded += UnitOnOnModifierAdded;
                    Unit.OnModifierRemoved += UnitOnOnModifierRemoved;
                }
                else
                {
                    Unit.OnModifierAdded -= UnitOnOnModifierAdded;
                    Unit.OnModifierRemoved -= UnitOnOnModifierRemoved;
                    ParticleManager.Remove("blink_tp_range");
                }
            };
        }

        private void UnitOnOnModifierRemoved(Unit unit, ModifierChangedEventArgs args)
        {
            if (unit is Hero me && me.HeroId == HeroId.npc_dota_hero_tinker)
            {
                if (args.Modifier.Name == "modifier_teleporting")
                {
                    _teleportTime = 0f;
                    ParticleManager.Remove("blink_tp_range");
                }
            }
        }

        private float _teleportTime = 0f;
        private void UnitOnOnModifierAdded(Unit unit, ModifierChangedEventArgs args)
        {
            if (unit is Hero me && me.HeroId == HeroId.npc_dota_hero_tinker)
            {
                if (args.Modifier.Name == "modifier_teleporting")
                {
                    _teleportTime = GameManager.RawGameTime;
                }
            }
            else if (unit.IsAlly(Me) && args.Modifier.Name == "modifier_boots_of_travel_incoming")
            {
                UpdateManager.BeginInvoke(() =>
                {
                    if (_teleportTime <= 0 || _main.AbilitiesInCombo.Blink == null)
                        return;
                    if (Math.Abs(GameManager.RawGameTime - _teleportTime) <= 0.05)
                    {
                        //TinkerCrappahilationPaid.Log.Debug($"Tp Dif: {Math.Abs(GameManager.RawGameTime - _teleportTime)}");
                        ParticleManager.DrawRange(unit, "blink_tp_range", _main.AbilitiesInCombo.Blink.CastRange,
                            Color.White);
                    }
                }, 5);
            }
        }

        public MenuItem<bool> DrawBlinkRangeOnTeleport { get; set; }

        private void DrawingOnOnDraw(EventArgs eventArgs)
        {
            foreach (var targetClass in _main.DamageCalculator.DamageDict)
            {
                var health = (int)targetClass.Value.HealthWithoutRange;
                var health2 = (int)targetClass.Value.HealthAfterFirstCastWithoutRange;
                var target = targetClass.Key;
                var pos = HUDInfo.GetHPbarPosition(target);
                if (pos.IsZero)
                {
                    continue;
                }
                var text = $"{health2}({health})";
                var textSize = new Vector2(HUDInfo.HpBarY);
                var mesText = Drawing.MeasureText(text, "Airal", textSize,
                    FontFlags.None);
                Drawing.DrawText(text, "Airal", pos - new Vector2(mesText.X, 0), textSize, Color.White,
                    FontFlags.None);
            }
        }

        public MenuItem<bool> EnableDamageDrawing { get; set; }

        public MenuFactory Factory { get; set; }
    }
}