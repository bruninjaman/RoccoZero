namespace O9K.AIO.Heroes.Weaver.Modes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Modes.Permanent;

    using Base;

    using Core.Entities.Abilities.Heroes.Weaver;
    using Core.Extensions;
    using Core.Managers.Entity;

    using Divine;

    using KillStealer;

    using SharpDX;

    internal class HealthTrackerMode : PermanentMode
    {
        private readonly Dictionary<float, float> healthTime = new Dictionary<float, float>();

        private readonly KillSteal killSteal;

        private TimeLapse timeLapse;

        public HealthTrackerMode(BaseHero baseHero, PermanentModeMenu menu)
            : base(baseHero, menu)
        {
            this.Handler.SetUpdateRate(100);
            this.killSteal = baseHero.KillSteal;
        }

        private TimeLapse TimeLapse
        {
            get
            {
                if (this.timeLapse == null)
                {
                    this.timeLapse = EntityManager9.GetAbility<TimeLapse>(this.Owner.Hero);
                }

                return this.timeLapse;
            }
        }

        public override void Disable()
        {
            base.Disable();
            RendererManager.Draw -= this.OnDraw;
        }

        public override void Dispose()
        {
            base.Disable();
            RendererManager.Draw -= this.OnDraw;
        }

        public override void Enable()
        {
            base.Enable();
            RendererManager.Draw += this.OnDraw;
        }

        protected override void Execute()
        {
            var hero = this.Owner.Hero;
            if (hero?.IsValid != true)
            {
                return;
            }

            var time = GameManager.RawGameTime;

            this.healthTime[time] = hero.Health;

            foreach (var unitPosition in this.healthTime.ToList())
            {
                var key = unitPosition.Key;
                if (key + 6 < time)
                {
                    this.healthTime.Remove(key);
                }
            }
        }

        private void OnDraw()
        {
            if (this.TimeLapse?.CanBeCasted() != true)
            {
                return;
            }

            var hero = this.Owner.Hero;
            var hpPosition = hero.HealthBarPosition;
            if (hpPosition.IsZero)
            {
                return;
            }

            var time = GameManager.RawGameTime;
            var health = hero.Health;
            var values = this.healthTime.OrderBy(x => x.Key).ToList();

            var restore = values.Find(x => x.Key + 5f > time).Value;
            if (restore <= health)
            {
                return;
            }

            var restorePercentage = restore / hero.MaximumHealth;
            var healthBarSize = hero.HealthBarSize;
            var start = hpPosition + new Vector2(0, healthBarSize.Y * 0.7f) + this.killSteal.AdditionalOverlayPosition;
            var size = (healthBarSize * new Vector2(restorePercentage, 0.3f)) + this.killSteal.AdditionalOverlayPosition;

            RendererManager.DrawFilledRectangle(new RectangleF(start.X, start.Y, size.X, size.Y), Color.DarkOliveGreen);
            RendererManager.DrawRectangle(new RectangleF(start.X - 1, start.Y - 1, size.X + 1, size.Y + 1), Color.Black);

            var restoreEarly = values.Find(x => x.Key + 4f > time).Value;
            if (restoreEarly < restore)
            {
                var losePercentage = (restore - restoreEarly) / hero.MaximumHealth;
                var size2 = (healthBarSize * new Vector2(losePercentage, 0.3f)) + this.killSteal.AdditionalOverlayPosition;
                var start2 = hpPosition + new Vector2(Math.Max(size.X - size2.X, 0), healthBarSize.Y * 0.7f)
                                        + this.killSteal.AdditionalOverlayPosition;

                RendererManager.DrawFilledRectangle(new RectangleF(start2.X, start2.Y, size2.X, size2.Y), Color.LightGreen);
                RendererManager.DrawRectangle(new RectangleF(start2.X - 1, start2.Y - 1, size2.X + 1, size2.Y + 1), Color.Black);
            }
        }
    }
}